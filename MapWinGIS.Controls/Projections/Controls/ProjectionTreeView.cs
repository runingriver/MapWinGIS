// ----------------------------------------------------------------------------
// MapWinGIS.Controls.Projections: store controls to work with EPSG projections
// database
// Author: Sergei Leschinski
// ----------------------------------------------------------------------------

namespace MapWinGIS.Controls.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Data;
    using System.Data.OleDb;
    using System.Collections;
    using System.Drawing;
    using System.IO;
    using System.ComponentModel;

    #region Subclasses
    /// <summary>
    /// Holds coordinates of bounding box for selected coordinate system
    /// </summary>
    public struct BoundingBox
    {
        /// <summary>
        /// Minimal value by X axis
        /// </summary>
        public double xMin;
        
        /// <summary>
        /// Maximum value by X axis
        /// </summary>
        public double xMax;
        
        /// <summary>
        /// Minimal value by Y axis
        /// </summary>
        public double yMin;
        
        /// <summary>
        /// Maximumm value by Y axis
        /// </summary>
        public double yMax;

        /// <summary>
        /// Creates a new instance of BoundingBox class with specified bounds
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        public BoundingBox(double minX, double maxX, double minY, double maxY)
        {
            xMin = minX;
            yMin = minY;
            xMax = maxX;
            yMax = maxY;
        }
    }
    #endregion

    /// <summary>
    /// A class derived from TreeView, which shows the list of projection of EPSG projection database
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TreeView))]
    public class ProjectionTreeView : TreeView
    {
        #region Declarations

        /// <summary>
        /// Selection mode for set extents operation
        /// </summary>
        public enum SelectionMode
        {
            /// <summary>
            /// Selects those objects that are completely within extents
            /// </summary>
            Include = 0,

            /// <summary>
            /// Selects those objects that are completely within bounding box or intersects it
            /// </summary>
            Intersection = 1,

            /// <summary>
            /// Selects those objects that completely covers specified extents
            /// </summary>
            IsIncluded = 2,
        }
       
        // database to take data from
        ProjectionDatabase m_database = null;

        // icons of the image list
        const int ICON_FOLDER = 0;
        const int ICON_FOLDER_OPEN = 1;
        const int ICON_GLOBE = 2;
        const int ICON_MAP = 3;
        const int ICON_PLUS = 4;
        const int ICON_MINUS = 5;

        private string SCOPE_NOT_RECOMMENDED = "Not recommended.";

        // GCS that should be classified by type (by default the option is off, to ensure performance)
        Hashtable m_dctLocalWithClassification = null;

        // USA GCS that should be classified by state (NAD)
        Hashtable m_dctUsaStates = null;

        // sets behavior of the set extents tool
        SelectionMode m_selectionMode = SelectionMode.Intersection;

        // notifies about double click event to suppress collasing/expanding of nodes
        private bool m_doubleClickWasDone = false;

        // the tab page of properties window that was opened last time
        private int m_propertiesTab = 0;

        // context menu for nodes
        private ContextMenuStrip m_contextMenu = null;

        // Reference to MapWinGIS to load the list of favorite projections
        private MapWinGIS.Interfaces.IMapWin m_mapWin = null;

        // preserving selected node when context menu is showing
        private TreeNode m_selectedNode = null;

        // top nodes keys
        private const string NODE_UNSPECIFIED_DATUMS = "Unspecified datums";
        private const string NODE_WORLD = "WORLD";
        private const string NODE_FAVORITE = "Favorite";
        private const string NODE_GEOGRAPHICAL = "Geographical";
        private const string NODE_PROJECTED = "Projected";

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new isntance of ProjectionTreeView class
        /// </summary>
        public ProjectionTreeView()
        {
            this.ImageList = this.CreateImageList();

            CreateContextMenu();

            m_dctLocalWithClassification = new Hashtable();
            int[] codes = { 4617, 4214, 4490, 4555, 4610, 4612, 4301, 4755, 4283, 4200 };
            foreach (int code in codes)
                m_dctLocalWithClassification.Add(code, null);

            m_dctUsaStates = new Hashtable();
            int[] codes2 = { 4267, 4269, 4152, 4759 };
            foreach (int code in codes2)
                m_dctUsaStates.Add(code, null);

            this.SetEventHandlers();
        }

        /// <summary>
        /// Sets event handling for the control
        /// </summary>
        private void SetEventHandlers()
        {
            this.BeforeCollapse += new TreeViewCancelEventHandler(ProjectionTreeView_BeforeCollapse);
            this.BeforeExpand += new TreeViewCancelEventHandler(ProjectionTreeView_BeforeExpand);
            this.AfterSelect += new TreeViewEventHandler(ProjectionTreeView_AfterSelect);
            this.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ProjectionTreeView_NodeMouseDoubleClick);
            this.NodeMouseClick += new TreeNodeMouseClickEventHandler(ProjectionTreeView_NodeMouseClick);
        }

        /// <summary>
        /// Creates image list associated with tree view
        /// </summary>
        private ImageList CreateImageList()
        {
            ImageList list = new ImageList();
            list.ColorDepth = ColorDepth.Depth24Bit;

            Bitmap bmp = new Bitmap(MapWinGIS.Controls.Properties.Resources.folder, new Size(16, 16));
            list.Images.Add(bmp);

            bmp = new Bitmap(MapWinGIS.Controls.Properties.Resources.folder_open, new Size(16, 16));
            list.Images.Add(bmp);

            bmp = new Bitmap(MapWinGIS.Controls.Properties.Resources.globe, new Size(16, 16));
            list.Images.Add(bmp);

            bmp = new Bitmap(MapWinGIS.Controls.Properties.Resources.map, new Size(16, 16));
            list.Images.Add(bmp);

            bmp = new Bitmap(MapWinGIS.Controls.Properties.Resources.map_add, new Size(16, 16));
            list.Images.Add(bmp);

            bmp = new Bitmap(MapWinGIS.Controls.Properties.Resources.map_delete, new Size(16, 16));
            list.Images.Add(bmp);

            return list;
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes by the pass to the executable file. \Projection folder is assumed.
        /// </summary>
        public bool InitializeByExePath(string executablePath, MapWinGIS.Interfaces.IMapWin mapWin)
        {
            string path = System.IO.Path.GetDirectoryName(executablePath) + @"\Projections\";
            if (!Directory.Exists(path))
            {
                MessageBox.Show("Projections folder isn't found: " + path);
                return false;
            }

            string[] files = Directory.GetFiles(path, "*.mdb");
            if (files.Length != 1)
            {
                MessageBox.Show("A single database is expected. " + files.Length.ToString() + " databases are found." + Environment.NewLine +
                                "Path : " + path + Environment.NewLine);
                return false;
            }
            else
            {
                this.Initialize(files[0], mapWin);
                return true;
            }
        }

        /// <summary>
        /// Initializes tree view
        /// </summary>
        /// <param name="databaseName">The name of MS Access database with projections</param>
        /// <returns></returns>
        public bool Initialize(string databaseName, MapWinGIS.Interfaces.IMapWin mapWin)
        {
            m_mapWin = mapWin;  // null is acceptable as well
            try
            {
                m_database = new ProjectionDatabase(databaseName, new MapWinGIS.Data.SQLiteProvider());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Initilizes tree view with the existing in-memory version of database
        /// </summary>
        public bool Initialize(ProjectionDatabase database, MapWinGIS.Interfaces.IMapWin mapWin)
        {
            m_mapWin = mapWin;  // null is acceptable as well
            
            if (database.Name == "")
                return false;

            this.m_database = database;
            return true;
        }
        #endregion

        #region Context Menu
        // Context menu commands
        private const string CONTEXT_ADD_TO_FAVORITE = "Add to Favorite";
        private const string CONTEXT_REMOVE_FROM_FAVORITE = "Remove from Favorite";
        private const string CONTEXT_SHOW_PROPERTIES = "Properties";
        private const string CONTEXT_NODE_EXPAND = "Node Expand";
        private const string CONTEXT_NODE_COLLAPSE = "Node Collapse";
        
        /// <summary>
        /// Creates a context menu associated with nodes
        /// </summary>
        private void CreateContextMenu()
        {
            m_contextMenu = new ContextMenuStrip();
            m_contextMenu.ImageList = this.ImageList;
            m_contextMenu.Items.Add(CONTEXT_ADD_TO_FAVORITE).ImageIndex = ICON_PLUS;
            m_contextMenu.Items.Add(CONTEXT_REMOVE_FROM_FAVORITE).ImageIndex = ICON_MINUS;
            m_contextMenu.Items.Add(new ToolStripSeparator());
            m_contextMenu.Items.Add(CONTEXT_SHOW_PROPERTIES);
            foreach (ToolStripItem item in m_contextMenu.Items)
                item.Name = item.Text;
            
            m_contextMenu.ItemClicked +=new ToolStripItemClickedEventHandler(m_contextMenu_ItemClicked);
        }

        /// <summary>
        /// Showing context menu;
        /// </summary>
        void ProjectionTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.Node != null)
                    this.SelectedNode = e.Node;
                
                if (e.Node.ImageIndex == ICON_GLOBE || e.Node.ImageIndex == ICON_MAP)
                {
                    TreeNode nodeFavorite = this.Nodes[NODE_FAVORITE];
                    TreeNode nodeProjected = nodeFavorite.Nodes[NODE_PROJECTED];
                    TreeNode nodeGeograpical = nodeFavorite.Nodes[NODE_GEOGRAPHICAL];
                    
                    bool favorite = e.Node.Parent == nodeProjected || e.Node.Parent == nodeGeograpical;
                    m_contextMenu.Items[CONTEXT_ADD_TO_FAVORITE].Visible = !favorite;
                    m_contextMenu.Items[CONTEXT_REMOVE_FROM_FAVORITE].Visible = favorite;
                    
                    m_selectedNode = e.Node;
                    //this.SelectedNode = e.Node;
                    Rectangle r = this.RectangleToScreen(this.ClientRectangle);
                    m_contextMenu.Show(r.Left + e.X, r.Top + e.Y);
                }
            }
        }

        /// <summary>
        /// Executes context menu commands
        /// </summary>
        void m_contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            m_contextMenu.Hide();
            Application.DoEvents();
            switch (e.ClickedItem.Text)
            {
                case CONTEXT_ADD_TO_FAVORITE:
                    AddProjectionToFavorite(m_selectedNode.Tag as CoordinateSystem);
                    break;
                case CONTEXT_SHOW_PROPERTIES:
                    ShowProjectionProperties(m_selectedNode.Tag as CoordinateSystem);
                    break;
                case CONTEXT_REMOVE_FROM_FAVORITE:
                    RemoveFromFavorite(m_selectedNode.Tag as CoordinateSystem);
                    break;
            }
        }
        #endregion

        #region Favorite
        /// <summary>
        /// Removes coordinate system from favorite list
        /// </summary>
        private void RemoveFromFavorite(CoordinateSystem cs)
        {
            if (cs != null && m_mapWin != null)
            {
                IList<int> list = m_mapWin.ApplicationInfo.FavoriteProjections;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == cs.Code)
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }

                UpdateFavoriteList();
            }
        }

        /// <summary>
        /// Adds projection to the favorite list
        /// </summary>
        private void AddProjectionToFavorite(CoordinateSystem cs)
        {
            if (cs == null)
            {
                MessageBox.Show("cs is null");
                return;
            }

            if (m_mapWin != null)
            {
                IList<int> list = m_mapWin.ApplicationInfo.FavoriteProjections;
                if (list.Where(prj => prj == cs.Code).Count() == 0)
                {
                    list.Add(cs.Code);

                    UpdateFavoriteList();
                }
                else
                {
                    
                    MessageBox.Show("Projection is added to the list already", "Projection exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Adds nodes with favorite projections
        /// </summary>
        private void UpdateFavoriteList()
        {
            this.SuspendLayout();

            TreeNode nodeFavorite = this.Nodes[NODE_FAVORITE];
            if (nodeFavorite != null)
            {
                this.Nodes.Remove(nodeFavorite);
            }
            
            nodeFavorite = this.Nodes.Add(NODE_FAVORITE, NODE_FAVORITE, ICON_FOLDER);

            if (m_mapWin != null)
            {
                TreeNode nodeGeographical = nodeFavorite.Nodes.Add(NODE_GEOGRAPHICAL, NODE_GEOGRAPHICAL, ICON_FOLDER);
                TreeNode nodeProjected = nodeFavorite.Nodes.Add(NODE_PROJECTED, NODE_PROJECTED, ICON_FOLDER);

                IList<int> list  = m_mapWin.ApplicationInfo.FavoriteProjections;
                if (list != null)
                {
                    int count = 0;
                    // geographical
                    IEnumerable<GeographicCS> results = from gcs in m_database.GeographicCS
                                                        from code in list
                                                        orderby gcs.Name
                                                        where gcs.Code == code
                                                        select(gcs);
                    foreach (GeographicCS gcs in results)
                    {
                        TreeNode nodeGcs = nodeGeographical.Nodes.Add(gcs.Code.ToString(), gcs.Name, ICON_GLOBE);
                        nodeGcs.Tag = gcs;
                        count++;
                    }

                    // projected
                    IEnumerable<ProjectedCS> results2 = from pcs in m_database.ProjectedCS
                                                        from code in list
                                                        orderby pcs.Name
                                                        where pcs.Code == code
                                                        select(pcs);
                    foreach (ProjectedCS pcs in results2)
                    {
                        TreeNode nodePcs = nodeProjected.Nodes.Add(pcs.Code.ToString(), pcs.Name, ICON_MAP);
                        nodePcs.Tag = pcs;
                        count++;
                    }

                    nodeFavorite.ExpandAll();
                    TreeNode nodeWorld = this.Nodes[NODE_WORLD];
                    if (nodeWorld != null && count < 5)
                        nodeWorld.Expand();
                }
            }

            this.ResumeLayout();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the underlying projection database 
        /// </summary>
        public ProjectionDatabase Database
        {
            get { return m_database; }
        }
        
        /// <summary>
        /// Gets the selected item in tree view, only coordinate systems will be returned
        /// </summary>
        public CoordinateSystem SelectedCoordinateSystem
        {
            get
            {
                if (this.SelectedNode == null)
                {
                    return null;
                }
                else
                {
                    if (this.SelectedNode.ImageIndex == ICON_GLOBE || this.SelectedNode.ImageIndex == ICON_MAP)
                    {
                        return this.SelectedNode.Tag as CoordinateSystem;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Returns geoprojection initialized with selected coordinate system
        /// </summary>
        public MapWinGIS.GeoProjection SelectedProjection
        {
            get
            {
                CoordinateSystem cs = this.SelectedCoordinateSystem;
                if (cs == null)
                {
                    return null;
                }
                else
                {
                    MapWinGIS.GeoProjection proj = new MapWinGIS.GeoProjection();
                    if (proj.ImportFromEPSG(cs.Code))
                    {
                        return proj;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Returns list of regions
        /// </summary>
        public List<GeographicCS> CoordinateSystems
        {
            get { return m_database.GeographicCS; }
        }

        /// <summary>
        /// Returns list of regions
        /// </summary>
        public List<ProjectedCS> Projections
        {
            get { return m_database.ProjectedCS; }
        }
        #endregion

        #region Tree view filling
        /// <summary>
        /// Fills treeview with all CS from list
        /// </summary>
        public bool RefreshList()
        {
            int gcsCount, pcsCount;
            return this.RefreshList(new BoundingBox(-180.0, 180.0, -90.0, 90.0), out gcsCount, out pcsCount);
        }
        
        /// <summary>
        /// Fills treeview with all CS from list
        /// </summary>
        /// <param name="gcsCount">Number of geographic CS found</param>
        /// <param name="projCount">Number of projected CS found</param>
        public bool RefreshList(out int gcsCount, out int projCount)
        {
            return this.RefreshList(new BoundingBox(-180.0, 180.0, -90.0, 90.0), out gcsCount, out projCount);
        }

        /// <summary>
        /// Fills treeview with CS which fall into specified bounds
        /// </summary>
        public bool RefreshList(BoundingBox extents, out int gcsCount, out int projCount)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            gcsCount = 0;
            projCount = 0;

            if (m_database == null)
                throw new Exception("No database was specified to populate tree view");

            this.SuspendLayout();
            this.Nodes.Clear();

            // limits coordinate systems to those which fall into extents
            bool showFullExtents = extents.xMin == -180.0 && extents.xMax == 180.0 && extents.yMin == -90.0 && extents.yMax == 90.0;
            this.ApplyExtents(extents, out gcsCount, out projCount);

            // adding top-most nodes
            TreeNode nodeUnspecified = this.Nodes.Add(NODE_UNSPECIFIED_DATUMS, NODE_UNSPECIFIED_DATUMS, ICON_FOLDER);
            TreeNode nodeWorld = this.Nodes.Add(NODE_WORLD, NODE_WORLD, ICON_FOLDER);

            // adding regions
            Hashtable dctRegions = new Hashtable();
            var listRegions = m_database.Regions.Where(r => r.ParentCode == 1);
            foreach (var region in listRegions)
            {
                TreeNode nodeRegion = nodeWorld.Nodes.Add(region.Code.ToString(), region.Name.ToString(), ICON_FOLDER);
                dctRegions.Add(region.Code, nodeRegion);
            }

            // local GCS
            var listRegions2 = m_database.Regions.Where(r => r.ParentCode > 1 && dctRegions.ContainsKey(r.ParentCode));
            foreach (var region in listRegions2)
            {
                TreeNode nodeRegion = dctRegions[region.ParentCode] as TreeNode;
                TreeNode nodeSubregion = nodeRegion.Nodes.Add(region.Code.ToString(), region.Name.ToString(), ICON_FOLDER);
                dctRegions.Add(region.Code, nodeSubregion);

                var listCountries = region.Countries.Where(cn => cn.IsActive).Where(c => c.GeographicCS.Where(cs => cs.IsActive).Count() > 0);
                foreach (var country in listCountries)
                {
                    TreeNode nodeCountry = null;   // it's to difficult to determine whether the country should be shown

                    foreach (GeographicCS gcs in country.GeographicCS.Where(cs => cs.IsActive).OrderBy(gcs => gcs.Name))
                    {
                        // when extents are limited, no need to show global systems for each country
                        if (!showFullExtents && gcs.Type != GeographicalCSType.Local)
                            continue;

                        if (gcs.Type != GeographicalCSType.Local || m_dctLocalWithClassification.ContainsKey(gcs.Code))
                        {
                            List<ProjectedCS> projections = new List<ProjectedCS>();
                            foreach (int code in country.ProjectedCS)
                            {
                                ProjectedCS pcs = gcs.ProjectionByCode(code);
                                if (pcs != null && pcs.IsActive)
                                    projections.Add(pcs);
                            }

                            if (projections.Count() > 0)
                            {
                                // country node will be added only here
                                if (nodeCountry == null)
                                    nodeCountry = nodeSubregion.Nodes.Add(country.Code.ToString(), country.Name.ToString(), ICON_FOLDER);

                                TreeNode nodeGcs = nodeCountry.Nodes.Add(gcs.Code.ToString(), gcs.Name.ToString(), ICON_GLOBE);
                                nodeGcs.Tag = gcs;
                                this.AddProjections(nodeGcs, gcs, projections);
                            }
                        }
                        else
                        {
                            if (nodeCountry == null)
                                nodeCountry = nodeSubregion.Nodes.Add(country.Code.ToString(), country.Name.ToString(), ICON_FOLDER);

                            // local GCS should be added to country even if there is no projection specified
                            TreeNode nodeGcs = nodeCountry.Nodes.Add(gcs.Code.ToString(), gcs.Name.ToString(), ICON_GLOBE);
                            nodeGcs.Tag = gcs;

                            foreach (ProjectedCS pcs in gcs.Projections.OrderBy(cs => cs.Name))
                            {
                                TreeNode nodePcs = nodeGcs.Nodes.Add(pcs.Code.ToString(), pcs.Name.ToString(), ICON_MAP);
                                nodePcs.Tag = pcs;
                            }
                        }
                    }
                }
            }

            // regional GCS
            var list1 = m_database.GeographicCS.Where(cs => cs.Type == GeographicalCSType.Regional && cs.IsActive).OrderBy(cs => cs.Name);
            foreach (var gcs in list1)
            {
                TreeNode nodeRegion = dctRegions[gcs.RegionCode] as TreeNode;
                TreeNode nodeGcs = nodeRegion.Nodes.Add(gcs.Code.ToString(), gcs.Name.ToString(), ICON_GLOBE);
                nodeGcs.Tag = gcs;
                this.AddProjections(nodeGcs, gcs, gcs.Projections.Where(cs => !cs.Local));
            }

            // global GCS
            var list2 = m_database.GeographicCS.Where(cs => cs.Type == GeographicalCSType.Global && cs.IsActive).OrderBy(cs => cs.Name);
            foreach (var gcs in list2)
            {
                TreeNode nodeParent = gcs.Scope == SCOPE_NOT_RECOMMENDED || gcs.Name.ToLower().StartsWith("unspecified") ? nodeUnspecified : nodeWorld;
                TreeNode nodeGcs = nodeParent.Nodes.Add(gcs.Code.ToString(), gcs.Name.ToString(), ICON_GLOBE);
                nodeGcs.Tag = gcs;
                this.AddProjections(nodeGcs, gcs, gcs.Projections.Where(cs => !cs.Local));
            }

            this.RemoveEmptyChilds(nodeWorld);

            this.UpdateFavoriteList();

            return true;
        }

        /// <summary>
        /// Adding projections for a gcs node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="gcs"></param>
        /// <param name="projections"></param>
        private void AddProjections(TreeNode parentNode, GeographicCS gcs, IEnumerable<ProjectedCS> projections)
        {
            if (m_dctUsaStates.Contains(gcs.Code))
            {
                string[] states = { "Alaska", "Alabama", "Arkansas", "Arizona", "California", "Colorado", "Connecticut", "District Columbia",
                                    "Delaware", "Florida", "Georgia", "Hawaii", "Iowa", "Idaho", "Illinois", "Indiana", "Kansas", "Kentucky",
                                    "Louisiana", "Massachusetts", "Maryland", "Maine", "Michigan", "Minnesota", "Missouri", "Mississippi",
                                    "Montana", "North Carolina", "North Dakota", "Nebraska", "New Hampshire", "New Jersey", "New Mexico",
                                    "Nevada", "New York", "Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Rhode Island", "South Carolina",
                                    "South Dakota", "Tennessee", "Texas", "Utah", "Virginia", "Vermont", "Washington", "Wisconsin", 
                                    "West Virginia", "Wyoming"};

                Hashtable dctStatePCS = new Hashtable();
                TreeNode nodeStates = null;
                foreach (string state in states)
                {
                    var list = projections.Where(p => p.Name.Contains(state));
                    if (list.Count() > 0)
                    {
                        if (nodeStates == null)
                        {
                            nodeStates = parentNode.Nodes.Add("States", "States", ICON_FOLDER);
                        }

                        TreeNode nodeState = nodeStates.Nodes.Add(state, state, ICON_FOLDER);
                        foreach (ProjectedCS pcs in list)
                        {
                            TreeNode node = nodeState.Nodes.Add(pcs.Code.ToString(), pcs.Name, ICON_MAP);
                            node.Tag = pcs;
                            if (!dctStatePCS.ContainsKey(pcs.Code))
                                dctStatePCS.Add(pcs.Code, null);
                        }
                    }
                }

                // now process the rest as usual
                projections = projections.Where(p => !dctStatePCS.ContainsKey(p.Code));
            }

            if (gcs.Type != GeographicalCSType.Local || m_dctLocalWithClassification.ContainsKey(gcs.Code))
            {
                IEnumerable<string> uniqueTypes = projections.Select(cs => cs.ProjectionType).Distinct().OrderBy(t => t);
                if (uniqueTypes.Count() > 1)
                {
                    foreach (string type in uniqueTypes)
                    {
                        if (type != "")
                        {
                            TreeNode nodeType = parentNode.Nodes.Add(type.ToString(), type.ToString(), ICON_FOLDER);
                            var projList = projections.Select(cs => cs).Where(c => c.ProjectionType == type);
                            foreach (var val in projList)
                            {
                                TreeNode nodePcs = nodeType.Nodes.Add(val.Code.ToString(), val.Name.ToString(), ICON_MAP);
                                nodePcs.Tag = val;
                            }
                        }
                    }
                }

                // adding projections with undefined type
                IEnumerable<ProjectedCS> list = uniqueTypes.Count() > 1 ?
                                                projections.Where(c => c.ProjectionType == "").OrderBy(c => c.Name) : projections.OrderBy(c => c.Name);
                if (list.Count() > 0)
                {
                    foreach (ProjectedCS pcs in list)
                    {
                        TreeNode nodePcs = parentNode.Nodes.Add(pcs.Code.ToString(), pcs.Name.ToString(), ICON_MAP);
                        nodePcs.Tag = pcs;
                    }
                }
            }
            else
            {
                foreach (ProjectedCS pcs in projections)
                {
                    TreeNode nodePcs = parentNode.Nodes.Add(pcs.Code.ToString(), pcs.Name.ToString(), ICON_MAP);
                    nodePcs.Tag = pcs;
                }
            }
        }
        #endregion

        #region Tree view events

        /// <summary>
        /// Event fired when user selects a node with coordinate system
        /// </summary>
        public event CoordinateSystemSelectedDelegate CoordinateSystemSelected;
        
        /// <summary>
        /// A delegate for CoordinateSystemSelected event
        /// </summary>
        /// <param name="cs">Reference to the selected territory (country or coordinate system)</param>
        public delegate void CoordinateSystemSelectedDelegate(Territory cs);
        internal void FireCoordinateSystemSelected(Territory cs)
        {
            if (CoordinateSystemSelected != null)
                CoordinateSystemSelected(cs);
        }

        /// <summary>
        /// Event fired when user selects a node with geographic CS
        /// </summary>
        public event GeographicCSSelectedDelegate GeographicCSSelected;
        
        /// <summary>
        /// A delegate for FireGeographicCSSelected event
        /// </summary>
        /// <param name="gcs">Reference to the selected geographical coordinate system</param>
        public delegate void GeographicCSSelectedDelegate(GeographicCS gcs);
        internal void FireGeographicCSSelected(GeographicCS gcs)
        {
            if (GeographicCSSelected != null)
                GeographicCSSelected(gcs);
        }

        /// <summary>
        /// Event fired when user selects a node with projected CS
        /// </summary>
        public event ProjectedCSSelectedDelegate ProjectedCSSelected;
        
        /// <summary>
        /// A delegate for ProjectedCSSelected event
        /// </summary>
        /// <param name="pcs">Reference to the selected projected coordinate system</param>
        public delegate void ProjectedCSSelectedDelegate(ProjectedCS pcs);
        internal void FireProjectedCSSelected(ProjectedCS pcs)
        {
            if (ProjectedCSSelected != null)
                ProjectedCSSelected(pcs);
        }

        /// <summary>
        /// Fires selection event for coordinate systems
        /// </summary>
        private void ProjectionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                e.Node.SelectedImageIndex = e.Node.ImageIndex;
                switch (e.Node.ImageIndex)
                {
                    case ICON_GLOBE:
                        {
                            GeographicCS gcs = e.Node.Tag as GeographicCS;
                            FireGeographicCSSelected(gcs);
                            FireCoordinateSystemSelected((Territory)gcs);
                            break;
                        }
                    case ICON_MAP:
                        {
                            ProjectedCS pcs = e.Node.Tag as ProjectedCS;
                            FireProjectedCSSelected(pcs);
                            FireCoordinateSystemSelected((Territory)pcs);
                            break;
                        }
                }
            }
        }


        /// <summary>
        /// Changes the icons for the folder
        /// </summary>
        void ProjectionTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (m_doubleClickWasDone)
            {
                e.Cancel = true;
                m_doubleClickWasDone = false;
                return;
            }
            
            if (e.Node != null)
            {
                if (e.Node.ImageIndex == ICON_FOLDER)
                {
                    e.Node.ImageIndex = ICON_FOLDER_OPEN;
                    e.Node.SelectedImageIndex = ICON_FOLDER_OPEN;
                }
            }
        }

        /// <summary>
        /// Changes the icons for the folder
        /// </summary>
        void ProjectionTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (m_doubleClickWasDone)
            {
                e.Cancel = true;
                m_doubleClickWasDone = false;
                return;
            }
            
            if (e.Node != null)
            {
                if (e.Node.ImageIndex == ICON_FOLDER_OPEN)
                {
                    e.Node.ImageIndex = ICON_FOLDER;
                    e.Node.SelectedImageIndex = ICON_FOLDER;
                }
            }
        }
        #endregion

        #region Utilities

        /// <summary>
        /// Opens all nodes to show GCS
        /// </summary>
        private void ShowGCS(TreeNode node)
        {
            if (node.ImageIndex == ICON_FOLDER || node.ImageIndex == ICON_FOLDER_OPEN)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    ShowGCS(child);
                }
                node.Expand();
            }
        }

        /// <summary>
        /// Browses through list of CS and marks them as active or not active depending on their bounds
        /// </summary>
        private void ApplyExtents(BoundingBox extents, out int gcsCount, out int projCount)
        {
            gcsCount = 0;
            projCount = 0;

            foreach (Territory cs in m_database.GeographicCS)
            {
                cs.IsActive = this.WithinExtents(extents, cs);
                if (cs.IsActive) gcsCount++;
            }

            foreach (Territory cs in m_database.ProjectedCS)
            {
                cs.IsActive = this.WithinExtents(extents, cs);
                if (cs.IsActive) projCount++;
            }

            foreach (Country country in m_database.Countries)
            {
                country.IsActive = this.WithinExtents(extents, country);
            }
        }

        /// <summary>
        /// Checks whether coordinate system is located within specified extents (taking into account SelectionMode)
        /// </summary>
        private bool WithinExtents(BoundingBox extents, Territory cs)
        {
            switch (m_selectionMode)
            {
                case SelectionMode.Include:
                    // extents should cover coordinate system
                    return extents.xMin <= cs.Left && extents.xMax >= cs.Right && extents.yMin <= cs.Bottom && extents.yMax >= cs.Top;
                case SelectionMode.IsIncluded:
                    // coordinate system should cover extents
                    return cs.Left <= extents.xMin && cs.Right >= extents.xMax && cs.Bottom <= extents.yMin && cs.Top >= extents.yMax;
                case SelectionMode.Intersection:
                    // the case of intersection ( the non-intersection hypothesis is checked )
                    return !(cs.Left > extents.xMax || cs.Right < extents.xMin || cs.Bottom > extents.yMax || cs.Top < extents.yMin);
                default:
                    return true;
            }
        }

        /// <summary>
        /// Recusive function which removes all empty folders childs of given node
        /// </summary>
        /// <param name="node"></param>
        private void RemoveEmptyChilds(TreeNode node)
        {
            if (node == null)
                return;

            for (int i = node.Nodes.Count - 1; i >= 0; i--)
            {
                TreeNode child = node.Nodes[i];
                // the end of branch; delete it if it is a folder
                if (child.Nodes.Count == 0)
                {
                    if (child.ImageIndex == ICON_FOLDER || child.ImageIndex == ICON_FOLDER_OPEN)
                    {
                        child.Remove();
                    }
                }
                else
                {
                    RemoveEmptyChilds(child);

                    // if all the branch was removed, delete the node as well
                    if (child.Nodes.Count == 0)
                    {
                        if (child.ImageIndex == ICON_FOLDER || child.ImageIndex == ICON_FOLDER_OPEN)
                        {
                            child.Remove();
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Suppresses standard double click behavior - expanding/collapsing
        /// </summary>
        /// <param name="m"></param>
        protected override void DefWndProc(ref Message m)
        {
            m_doubleClickWasDone = (m.Msg == 515);  /* WM_LBUTTONDBLCLK */
            base.DefWndProc(ref m);
        }

        /// <summary>
        /// Shows projection view for selected projection
        /// </summary>
        private void ProjectionTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.ShowProjectionProperties(this.SelectedNode.Tag as CoordinateSystem);
        }

        /// <summary>
        /// Shows properties for projection with given code
        /// </summary>
        /// <param name="Code"></param>
        public void ShowProjectionProperties(int Code)
        {
            foreach (GeographicCS gcs in m_database.GeographicCS)
            {
                if (gcs.Code == Code)
                {
                    ShowProjectionProperties((CoordinateSystem)gcs);
                    return;
                }
            }

            foreach (ProjectedCS pcs in m_database.ProjectedCS)
            {
                if (pcs.Code == Code)
                {
                    ShowProjectionProperties((CoordinateSystem)pcs);
                    return;
                }
            }
        }

        /// <summary>
        /// Shows property window for projection
        /// </summary>
        private void ShowProjectionProperties(CoordinateSystem proj)
        {
            if (proj != null)
            {
                frmProjectionProperties form = new frmProjectionProperties(proj, m_database);
                form.tabControl1.SelectedIndex = m_propertiesTab;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    m_propertiesTab = form.tabControl1.SelectedIndex;
                }
                form.Dispose();
            }
        }

        /// <summary>
        /// Selects node with the given EPSG code
        /// </summary>
        /// <returns></returns>
        public void SelectNodeByCode(int code)
        {
            TreeNode node = new TreeNode();
            this.Seek(this.Nodes, code, node.Nodes);

            MessageBox.Show("Found: " + node.Nodes.Count.ToString());
        }

        private void Seek(TreeNodeCollection nodes, int code, TreeNodeCollection results)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.ImageIndex == ICON_GLOBE || node.ImageIndex == ICON_MAP)
                {
                    //if (Convert.ToInt32(node.Tag) == code)
                    //{
                    //    results.Add(node);
                    //}
                }
                else
                { 
                    this.Seek(node.Nodes, code, results);
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}
