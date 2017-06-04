
namespace MapWinGIS.Controls.Tiles
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Diagnostics;
    using MapWinGIS;
    using System.Drawing;
    #endregion

    #if OCX_VERSION49
    /// <summary>
    /// A wrapper class to access settings from TileProviders.xml file
    /// </summary>
    public class TileSettings
    {
        // marks whether an error has occured
        private static int errorCount = 0;

        #region LINQ to XML
        /// <summary>
        /// Reads settings from the file
        /// </summary>
        /// <param name="tiles">Reference to the tiles class</param>
        public static void Read(Tiles tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException("Reference to the tiles wasn't passed");

            XElement root = XElement.Load(GetFilename());
            string ns = root.GetDefaultNamespace().NamespaceName;

            var list = from p in root.Descendants(XName.Get("DefaultProvider", ns))
                            where p.Attribute("Selected").Value != "0" select p;

            // setting versions
            foreach (var item in list)
            {
                tkTileProvider p = (tkTileProvider)Convert.ToInt32(item.Attribute("Id").Value);
                int index = tiles.Providers.get_IndexByProvider(p);
                tiles.Providers.set_Version(index, item.Attribute("Version").Value);
            }

            // updating custom providers
            tiles.Providers.Clear(false);
            var list2 = from p in root.Descendants(XName.Get("CustomProvider", ns))
                       where p.Attribute("Selected").Value != "0" 
                       select new {
                           Id = Convert.ToInt32(p.Attribute("Id").Value),
                           Name = p.Attribute("Name").Value,
                           Url = p.Attribute("UrlPattern").Value,
                           Projection = Convert.ToInt32(p.Attribute("Projection").Value)
                       };

            foreach (var item in list2)
            {
                tiles.Providers.Add(item.Id, item.Name, item.Url, (tkTileProjection)item.Projection, 0, 17);
            }
        }
        
        /// <summary>
        /// Saves the state of nodes of providers tree view
        /// </summary>
        public static void SaveTreeState(TreeView tree)
        {
            if (tree.Nodes.Count == 0)
                return;

            try
            {
                Dictionary<string, bool> state = new Dictionary<string, bool>();

                TreeNode node = tree.Nodes[0];
                foreach (TreeNode g in node.Nodes)
                {
                    state.Add(g.Text, g.IsExpanded);
                }

                XElement el = XElement.Load(GetFilename());
                string ns = el.GetDefaultNamespace().NamespaceName;

                var list = from g in el.Descendants(XName.Get("TileGroup", ns)) select g;

                foreach (var item in list)
                {
                    string name = item.Attribute("Name").Value;
                    if (state.ContainsKey(name))
                    {
                        item.Attribute("Expanded").Value = state[name] ? "1" : "0";
                    }
                }
                el.Save(GetFilename());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Reads providers from the settings
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        public static void FillProviderTree(TreeView tree, int selectedProvider)
        {
            tree.Nodes.Clear();
            TreeNode node = tree.Nodes.Add("Providers");
            node.Expand();
            
            XElement root = XElement.Load(GetFilename());
            string ns = root.GetDefaultNamespace().NamespaceName;

            var list = from p in root.Descendants(XName.Get("TileGroup", ns)) select p;
            foreach (var g in list)
            {
                string id = g.Attribute("Id").Value;
                var providers = from p in root.Descendants(XName.Get("DefaultProvider", ns)) 
                                where p.Attribute("GroupId").Value == id &&
                                      p.Attribute("Selected").Value != "0" select p;

                var custom = from p in root.Descendants(XName.Get("CustomProvider", ns))
                             where p.Attribute("GroupId").Value == id && 
                                   p.Attribute("Selected").Value != "0" 
                             select p;

                if (providers.Count() > 0 || custom.Count() > 0)
                {
                    TreeNode group = node.Nodes.Add(g.Attribute("Name").Value);

                    foreach (var item in providers)
                    {
                        TreeNode n = new TreeNode(item.Attribute("Name").Value);
                        n.Tag = Convert.ToInt32(item.Attribute("Id").Value);
                        group.Nodes.Add(n);

                        if ((int)n.Tag == selectedProvider)
                            tree.SelectedNode = n;
                    }

                    foreach (var item in custom)
                    {
                        TreeNode n = new TreeNode(item.Attribute("Name").Value);
                        n.Tag = Convert.ToInt32(item.Attribute("Id").Value);
                        group.Nodes.Add(n);

                        if ((int)n.Tag == selectedProvider)
                            tree.SelectedNode = n;
                    }

                    if (g.Attribute("Expanded").Value != "0")
                        group.Expand();
                }
            }
        }

        /// <summary>
        /// Creates image list associated with tree view
        /// </summary>
        private static void CreateImageList(TreeView tree)
        {
            if (tree.ImageList == null)
            {
                ImageList list = new ImageList();
                list.ColorDepth = ColorDepth.Depth24Bit;
                list.Images.Add(new Bitmap(MapWinGIS.Controls.Properties.Resources.map, new Size(16, 16)));
                list.Images.Add(new Bitmap(MapWinGIS.Controls.Properties.Resources.folder, new Size(16, 16)));
                list.Images.Add(new Bitmap(MapWinGIS.Controls.Properties.Resources.folder_open, new Size(16, 16)));
                tree.ImageList = list;
            }
        }

        /// <summary>
        /// Validates XML file with settings against a schema
        /// </summary>
        /// <returns>True if the file is valid and false otherwise</returns>
        public static bool Validate()
        {
            XDocument doc = XDocument.Load(GetFilename());
            XmlSchemaSet set = new XmlSchemaSet();
            set.Add(null, GetSchemaName());

            ValidationEventHandler del = new ValidationEventHandler(ValidationHandler);
            errorCount = 0;
            doc.Validate(set, del);
            return errorCount == 0;
        }

        static void ValidationHandler(object o, ValidationEventArgs e)
        {
            Debug.WriteLine(e.Message);
            errorCount++;
        }
        #endregion

        #region XML DOM Validation
        /// <summary>
        /// Tries to open XML document with settings
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        private static bool ValidateByDom()
        {
            string path = GetSchemaName();
            XmlReaderSettings settings = null;
            if (File.Exists(path))
            {
                settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas.Add(null, path);
                settings.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(settings_ValidationEventHandler);
            }

            errorCount = 0;
            XmlReader reader = XmlReader.Create(GetFilename(), settings);
            while (reader.Read()) { };  // first run the validation; 
            // it protects from situation when the process is broken half way through

            return errorCount == 0;
        }

        /// <summary>
        /// Logs information on XML schema errors
        /// </summary>
        static void settings_ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            Debug.WriteLine(e.Message);
            errorCount++;
        }
        #endregion

        #region Utilities
        private static string GetFilename()
        {
            string filename = Application.StartupPath + @"\tileproviders.xml";
            if (!File.Exists(filename))
                throw new FileNotFoundException("Tile settings were not found: " + filename);
            return filename;
        }

        private static string GetSchemaName()
        {
            string filename = Application.StartupPath + @"\tileproviders.xsd";
            if (!File.Exists(filename))
                throw new FileNotFoundException("Validation schema wasn't found: " + filename);
            return filename;
        }
        #endregion
    }
    #endif
}
