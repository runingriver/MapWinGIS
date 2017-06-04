// Shapefile data client
// Author: Sergei Leschinski
// Created: 14 august 2011

namespace MapWinGIS.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.OleDb;
    using System.Data;
    using System.IO;
    using System.Windows.Forms;
    using System.Data.Common;
    using System.Diagnostics;

    /// <summary>
    /// Database types with predefined connection strings supported by converter
    /// </summary>
    public enum DatabaseType
    {
        MsAccess = 0,
        SqlServerCE = 1,
        SQLite = 2,
    }

    public enum InitStringType
    {
        DatabaseName = 0,
        ConnectionString = 1,
    }

    /// <summary>
    /// Provides means to save and load shapefiles to databases using ADO.NET
    /// </summary>
    public class ShapefileDataClient
    {
        #region Declarations
        // index of geometry field
        private const int FIELD_GEOMETRY = 0;

        // connection string
        private string m_connectionString = "";

        // custom provider
        private IDataProvider m_provider = null;

        // connection object used by class
        private DbConnection m_connection = null;


        // callback to report progress
        private MapWinGIS.ICallback m_callback = null;

        // callback for individual operations        
        private bool m_showCallback = false;

        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the DataConverter class based on database name
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="databaseName"></param>
        public ShapefileDataClient(string databaseName)
        {
            m_provider = DbUtilities.GetDataProvider(databaseName);
            if (m_provider == null)
                throw new DataException("Invalid database name. Coundn't find data provider.");

            m_connectionString = m_provider.CreateConnectionString(databaseName);
        }


        /// <summary>
        /// Creates a new instance of the DataConverter class based on database name
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="databaseName"></param>
        public ShapefileDataClient(IDataProvider provider, string connectionString, InitStringType stringType)
        {
            if (provider == null)
                throw new NullReferenceException("Reference to data provider wasn't passed");

            m_provider = provider;

            if (stringType == InitStringType.ConnectionString)
            {
                m_connectionString = connectionString;
            }
            else
            {
                m_connectionString = m_provider.CreateConnectionString(connectionString);
            }
        }

        ~ShapefileDataClient()
        {
            if (m_connection != null)
                m_connection.Close();
        }
        #endregion

        #region Properties
        /// <summary>
        ///  Gets or sets callback to report the process of the operation where it is applicable
        /// </summary>
        public MapWinGIS.ICallback Callback
        {
            get { return m_callback; }
            set { m_callback = value; }
        }

        /// <summary>
        /// Whether callback for individual operations is needed
        /// </summary>
        public bool ShowCallback
        {
            get { return m_showCallback; }
            set { m_showCallback = value; }
        }

        /// <summary>
        /// Gets connection associated with the object
        /// </summary>
        public DbConnection Connection
        {
            get { return m_connection; }
        }

        #endregion

        #region Connection
        /// <summary>
        /// Opens connection. Use this methods if you won't to perform several operations
        /// without closing connection. Without it new connection will be opened for each operation.
        /// </summary>
        /// <returns>Returns true on success.</returns>
        public bool OpenConnection()
        {
            if (m_connection != null)
            {
                this.CloseConnection();
            }

            DbConnection connection = m_provider.CreateConnection();
            if (connection == null)
                throw new NullReferenceException("Connection object wasn't created");

            connection.ConnectionString = m_connectionString;
            connection.Open();

            m_connection = connection;

            Application.DoEvents();
            return m_connection.State == ConnectionState.Open;
        }

        /// <summary>
        /// Closes underlying connection
        /// </summary>
        public void CloseConnection()
        {
            if (m_connection != null)
            {
                m_connection.Close();
                m_connection = null;
            }
        }

        /// <summary>
        /// Checks whether connection was created
        /// </summary>
        private void CheckConnection()
        {
            if (m_connection == null)
                throw new DataException("Connection wasn't opened.");
        }
        #endregion

        #region Save Shapefile
        /// <summary>
        /// Save shapefile in the specified database. It calls CreateTable and InsertShapes internally.
        /// </summary>
        /// <param name="sf">Shapfile object to save into database</param>
        /// <param name="tableName">The table name to save shapefile into. The new table will be created. 
        /// If table with such name exists it will be rewritten.</param>
        public bool SaveShapefile(MapWinGIS.Shapefile sf, string tableName, bool overwrite)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            bool result = false;

            // deleting the table
            if (DbUtilities.TableExists(m_connection, tableName))
            {
                if (overwrite)
                {
                    this.DeleteTable(tableName);
                }
                else
                {
                    return false;
                }
            }

            //creating new one
            if (!this.CreateTable(sf, tableName))
                return false;

            //inserting
            int count = this.InsertShapes(sf, tableName, false);
            result = count == sf.NumShapes;

            watch.Stop();
            Debug.Print(watch.Elapsed.ToString());
            return result;
        }
        #endregion

        #region Create and Delete table
        /// <summary>
        /// Creates a table based upon fileds of specified shapefile
        /// </summary>
        public bool CreateTable(MapWinGIS.Shapefile sf, string tableName)
        {
            this.CheckConnection();

            DbCommand command = m_connection.CreateCommand();

            string sql = " ([Geometry] " + m_provider.GetBinaryTypeName() + ", ";

            int count = sf.NumFields;
            for (int i = 0; i < count; i++)
            {
                MapWinGIS.Field field = sf.get_Field(i);
                string s = "[" + field.Name + "] ";
                switch (field.Type)
                {
                    case MapWinGIS.FieldType.DOUBLE_FIELD:
                        s += m_provider.GetDoubleTypeName();
                        break;
                    case MapWinGIS.FieldType.INTEGER_FIELD:
                        s += m_provider.GetIntergerTypeName();
                        break;
                    case MapWinGIS.FieldType.STRING_FIELD:
                        bool fixedLength = true;
                        string name = m_provider.GetStringTypeName(out fixedLength);
                        s += fixedLength ? name + "(" + field.Width.ToString() + ")" : name;
                        break;
                    default:
                        continue;
                }
                sql += s;
                sql += i < count - 1 ? ", " : ")";
            }

            command.CommandText = "CREATE TABLE [" + tableName + "]" + sql;
            command.ExecuteNonQuery();
            return DbUtilities.TableExists(m_connection, tableName);
        }

        /// <summary>
        /// Deletes table with the given name
        /// </summary>
        public bool DeleteTable(string tableName)
        {
            this.CheckConnection();

            bool result = false;
            try
            {
                DbCommand command = m_connection.CreateCommand();
                command.CommandText = "DROP TABLE " + "[" + tableName + "]";
                result = command.ExecuteNonQuery() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("DeleteTable: " + ex.Message);
                return true;    // ignore for now
            }
            return result;
        }
        #endregion

        #region Insert shapes
        /// <summary>
        /// Inserts shapes in the specified table.
        /// </summary>
        /// <returns>Number of inserted shapes. -1 if the table doesn't exist.</returns>
        public int InsertShapes(MapWinGIS.Shapefile sf, string tableName, bool selectedOnly)
        {
            return this.InsertShapes(sf, tableName, this.GetShapefileIndices(sf, selectedOnly));
        }

        /// <summary>
        /// Inserts shapes with the given indices in the specified table.
        /// </summary>
        /// <returns>Number of inserted shapes. -1 if the table doesn't exist.</returns>
        public int InsertShapes(MapWinGIS.Shapefile sf, string tableName, int[] indices)
        {
            this.CheckConnection();

            int count = 0;
            using (DbTransaction dbTrans = m_connection.BeginTransaction())
            {
                using (DbCommand cmd = this.CreateCommand())
                {
                    DbParameter param = this.CreateBinaryParameter();
                    param.SourceColumn = "Geometry";
                    param.ParameterName = "@Geometry";
                    cmd.Parameters.Add(param);

                    // generating sql and parameters
                    int fieldCount = sf.NumFields;
                    string sql = "INSERT INTO [" + tableName + "] ([Geometry], ";
                    string values = "VALUES (?, ";

                    for (int j = 0; j < fieldCount; j++)
                    {
                        param = this.CreateParameter();

                        MapWinGIS.Field fld = sf.get_Field(j);
                        switch (fld.Type)
                        {
                            case MapWinGIS.FieldType.STRING_FIELD:

                                param.DbType = DbType.StringFixedLength;
                                param.Size = fld.Width;
                                break;
                            case MapWinGIS.FieldType.INTEGER_FIELD:
                                param.DbType = DbType.Int32;
                                break;
                            case MapWinGIS.FieldType.DOUBLE_FIELD:
                                param.DbType = DbType.Double;
                                break;
                        }

                        param.ParameterName = "@p" + j.ToString();
                        param.SourceColumn = fld.Name;
                        cmd.Parameters.Add(param);

                        sql += "[" + fld.Name + "]";
                        sql += (j == fieldCount - 1) ? ") " : ", ";
                        values += (j == fieldCount - 1) ? "?)" : "?, ";
                    }
                    cmd.CommandText = sql + values;
                    cmd.Transaction = dbTrans;
                    cmd.Connection = m_connection;
                    System.Diagnostics.Debug.Print(cmd.CommandText);

                    // adding new records
                    DbParameterCollection paramters = cmd.Parameters;
                    MapWinGIS.Table table = sf.Table;
                    int maxSize = 0;
                    int percent = 0;

                    for (int i = 0; i < sf.NumShapes; i++)
                    {
                        if (m_callback != null && m_showCallback)
                        {
                            int newPercent = (int)((double)i / (double)(sf.NumShapes - 1) * 100.0);
                            if (newPercent != percent)
                            {
                                m_callback.Progress("", newPercent, "Exporting shapes...");
                                percent = newPercent;
                            }
                        }

                        object data = null;
                        MapWinGIS.Shape shape = sf.get_Shape(i);

                        if (shape.ExportToBinary(ref data))
                        {
                            if ((data as byte[]).Length > maxSize)
                                maxSize = (data as byte[]).Length;

                            paramters[0].Value = data as byte[];
                            for (int j = 0; j < fieldCount; j++)
                            {
                                paramters[j + 1].Value = table.get_CellValue(j, i);
                            }
                            if (cmd.ExecuteNonQuery() != 0)
                                count++;
                        }
                    }

                    if (m_callback != null && m_showCallback)
                        m_callback.Progress("", 100, "");
                }
                dbTrans.Commit();
            }
            return count;
        }
        #endregion

        #region Update shapes
        /// <summary>
        /// Updates data in the table. 
        /// For successful update mwShapeId field must be present in both shapefile and table.
        /// It's expected that this field ha unique values.
        /// This function won't add shapes that are missing in the table and won't delete any shapes.
        /// </summary>
        /// <returns>Number of updated shapes</returns>
        public int UpdateShapes(MapWinGIS.Shapefile sf, string tableName, bool selectedOnly)
        {
            return this.UpdateShapes(sf, tableName, this.GetShapefileIndices(sf, selectedOnly));
        }

        /// <summary>
        /// Updates data in the table for shapes with specific indices. 
        /// For successful update mwShapeId field must be present in both shapefile and table.
        /// It's expected that this field ha unique values.
        /// This function won't add shapes that are missing in the table and won't delete any shapes.
        /// </summary>
        /// <returns>Number of updated shapes</returns>
        public int UpdateShapes(MapWinGIS.Shapefile sf, string tableName, int[] indices)
        {
            if (indices.Length == 0)
                return 0;

            this.CheckConnection();

            return 0;
        }
        #endregion

        #region Load shapefile
        /// <summary>
        /// Creates shapefile and fill it with data from the specified table or sql query
        /// </summary>
        /// <param name="sql">Table name, stored procedure name or sql selection query</param>
        /// <param name="commandType">Command type. Specifies the content of sql parameter.</param>
        /// <returns>Reference of the created shapefile on success and null otherwise.</returns>
        public MapWinGIS.Shapefile LoadShapefile(string sql, System.Data.CommandType commandType)
        {
            if (commandType == CommandType.TableDirect)
            {
                string query = "SELECT * FROM [" + sql + "]";
                return this.LoadShapefile(query);
            }
            else
            {
                return this.LoadShapefile(sql);
            }
        }

        /// <summary>
        /// Creates shapefile from specified database table.
        /// Geometry or GeometryText columns are expected
        /// </summary>
        private MapWinGIS.Shapefile LoadShapefile(string sql)
        {
            this.CheckConnection();

            MapWinGIS.Shapefile sf = null;

            if (m_callback != null)
                m_callback.Progress("", 0, "Extracting data...");

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            DbDataAdapter adapter = this.CreateAdapter();
            adapter.SelectCommand = this.CreateCommand();
            adapter.SelectCommand.Connection = m_connection;
            adapter.SelectCommand.CommandText = sql;

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            int index = dt.Columns.IndexOf("Geometry");
            if (index >= 0 && dt.Rows.Count > 0)
            {
                // get shapefile type by the first shape
                MapWinGIS.ShpfileType shpType;
                MapWinGIS.Shape shape = new MapWinGIS.Shape();
                if (shape.ImportFromBinary(dt.Rows[0][index]))
                {
                    shpType = shape.ShapeType;
                }
                else
                {
                    return null;
                }

                sf = new MapWinGIS.Shapefile();
                sf.CreateNew("", shpType);

                foreach (DataColumn column in dt.Columns)
                {
                    MapWinGIS.Field field = new MapWinGIS.Field();
                    field.Name = column.ColumnName;
                    switch (column.DataType.Name.ToLower())
                    {
                        case "int32":
                        case "int64":
                            field.Type = MapWinGIS.FieldType.INTEGER_FIELD;
                            break;
                        case "double":
                            field.Type = MapWinGIS.FieldType.DOUBLE_FIELD;
                            break;
                        case "string":
                            field.Type = MapWinGIS.FieldType.STRING_FIELD;
                            break;
                        case "byte[]":
                            continue;
                        default:
                            continue;
                    }
                    int fieldIndex = sf.NumFields;
                    bool result = sf.EditInsertField(field, ref fieldIndex, null);
                    System.Diagnostics.Debug.Print(result.ToString());
                }

                int shapeIndex = dt.Rows.Count;
                shape = null;
                int percent = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (m_callback != null && dt.Rows.Count > 1)
                    {
                        int newPercent = Convert.ToInt32((double)i / (double)(dt.Rows.Count - 1) * 100.0);
                        if (newPercent > percent)
                        {
                            m_callback.Progress("", newPercent, "Creating shapefile...");
                            percent = newPercent;
                        }
                    }

                    DataRow row = dt.Rows[i];

                    shape = new MapWinGIS.Shape();
                    if (shape.ImportFromBinary(row[index]))
                    {
                        shapeIndex = sf.NumShapes;
                        if (sf.EditInsertShape(shape, ref shapeIndex))
                        {
                            for (int j = 0; j < sf.NumFields; j++)
                            {
                                sf.EditCellValue(j, shapeIndex, row[j + 1]);
                            }
                        }
                    }
                }

                if (m_callback != null)
                    m_callback.Progress("", 100, "");
            }

            watch.Stop();
            Debug.Print("Finished: " + watch.Elapsed.ToString());

            return sf;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Returns the list of tables which hold geometry
        /// </summary>
        /// <returns>List of table names</returns>
        public List<string> GetSpatialTables()
        {
            this.CheckConnection();
            return DbUtilities.GetTablesByField(m_connection, "Geometry", "blob");
        }

        /// <summary>
        /// Returns indices of shapes to be passed to insert or update routine
        /// </summary>
        private int[] GetShapefileIndices(MapWinGIS.Shapefile sf, bool selectedOnly)
        {
            int size = selectedOnly ? sf.NumSelected : sf.NumShapes;
            if (size == 0)
                return null;

            int[] indices = new int[size];

            if (selectedOnly)
            {
                int count = 0;
                for (int i = 0; i < size; i++)
                {
                    if (sf.get_ShapeSelected(i))
                        indices[count++] = i;
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                    indices[i] = i;
            }
            return indices;
        }



        /// <summary>
        /// Creates connection for default database types
        /// </summary>
        public static string CreateConnectionString(DatabaseType dbType, string filename)
        {
            switch (dbType)
            {
                case DatabaseType.MsAccess:
                    return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename + ";User Id=admin;Password=;";
                case DatabaseType.SqlServerCE:
                    return "Data Source=" + filename + ";Persist Security Info=False;";
                case DatabaseType.SQLite:
                    return "Data Source = " + filename + "; Version = 3;";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Creates command of the specified type
        /// </summary>
        private DbCommand CreateCommand()
        {
            DbCommand command = m_provider.CreateCommand();
            if (command == null)
                throw new NullReferenceException("Invalid DataProvider. Command object wasn't created");

            return command;
        }

        /// <summary>
        /// Create parameter
        /// </summary>
        private DbParameter CreateParameter()
        {
            DbParameter parameter = m_provider.CreateParameter();
            if (parameter == null)
                throw new NullReferenceException("Invalid DataProvider. Parameter object wasn't created");

            return parameter;
        }

        /// <summary>
        /// Creates adapter
        /// </summary>
        private DbDataAdapter CreateAdapter()
        {
            DbDataAdapter adapter = m_provider.CreateDataAdapter();
            if (adapter == null)
                throw new NullReferenceException("Invalid DataProvider. Adapter object wasn't created");

            return adapter;
        }

        /// <summary>
        /// Create binary parameter for geometry field
        /// </summary>
        private DbParameter CreateBinaryParameter()
        {
            DbParameter parameter = m_provider.CreateBinaryParameter();
            if (parameter == null)
                throw new NullReferenceException("Invalid DataProvider. Parameter object wasn't created");

            return parameter;
        }
        #endregion
    }
}
