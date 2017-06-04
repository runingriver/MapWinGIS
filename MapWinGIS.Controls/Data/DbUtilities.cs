
#define DEBUG_VERSION

namespace MapWinGIS.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.Windows.Forms;
    using System.Data.Common;
    using System.IO;
    using System.Diagnostics;

    /// <summary>
    /// Provides minimal information about table 
    /// </summary>
    public class TableInfo
    {
        public string Name;
        public List<string> Fields = new List<string>();

        public TableInfo(string name)
        {
            Name = name;
        }
    }

    public class DbUtilities
    {
        #region Test connection
        /// <summary>
        /// Tests connection to the selected database
        /// </summary>
        public static bool TestConnection(string dbName, bool silentMode)
        {
            IDataProvider provider = DbUtilities.GetDataProvider(dbName);
            if (provider == null)
                return false;

            bool result = false;
            DbConnection conn = provider.CreateConnection(dbName);
            try
            {
                conn.Open();
                Application.DoEvents();

                if (conn.State == ConnectionState.Open)
                {
                    if (!silentMode)
                    {
                        MessageBox.Show("Connection is successful.", "MapWinGIS",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    result = true;
                }
                else
                {
                    if (!silentMode)
                    {
                        MessageBox.Show("Connection failed.", "MapWinGIS",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            finally
            {
                conn.Close();
            }
            return result;
        }
        #endregion

        #region Provider
        /// <summary>
        /// Gets data provider according to the selected database
        /// </summary>
        public static IDataProvider GetDataProvider(string dbName)
        {
            switch (Path.GetExtension(dbName).ToLower())
            {
                case ".db":
                case ".db3":
                    return new SQLiteProvider();
                case ".mdb":
                case ".accdb":
                    return new OleDbProvider();
                default:
                    MessageBox.Show("No valid database is selected.", "MapWinGIS",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
            }
        }
        #endregion

        #region Tables

        /// <summary>
        /// Checks whether table with specified name exists in the database
        /// </summary>
        public static bool TableExists(DbConnection connection, string tblName)
        {
            bool result = false;
            if (connection == null)
                return false;

            try
            {
                // TODO: implement alternative methods for SQL Server CE (don't support GetSchema until 4.0 version)
                string[] restrictions = new string[4];
                restrictions[2] = tblName;
                DataTable dt = connection.GetSchema("Tables", restrictions);
                result = !(dt.Rows.Count == 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("TableExists: " + ex.Message);
                result = true;    // assuming that it exists
            }
            return result;
        }

        /// <summary>
        /// Gets list of tables from the source
        /// </summary>
        public static List<TableInfo> GetTableList(DbConnection connection)
        {
            if (connection == null || connection.State != ConnectionState.Open)
                return null;

            List<TableInfo> list = new List<TableInfo>();

            try
            {
                DataTable dt = connection.GetSchema("Tables");
                if (dt != null)
                {
                    //DbUtilities.DumpDataTable(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string name = dt.Rows[i][2].ToString();
                        list.Add(new TableInfo(name));
                    }
                    dt.Clear();

                    // retrieving information about fields
                    dt = connection.GetSchema("Columns");
                    int indexColumnName = dt.Columns.IndexOf("COLUMN_NAME");
                    int indexTableName = dt.Columns.IndexOf("TABLE_NAME");

                    if (indexColumnName >= 0 && indexTableName >= 0)
                    {
                        foreach (TableInfo table in list)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                if (table.Name == row[indexTableName].ToString())
                                {
                                    table.Fields.Add(row[indexColumnName].ToString());
                                }
                            }
                        }
                    }
                    dt.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Error. GetTableList: " + ex.Message);
                return null;
            }
            return list;
        }

        /// <summary>
        /// Dumps the content of the given table to the output window
        /// </summary>
        public static void DumpDataTable(DataTable dt)
        {
            Debug.Print(Environment.NewLine + "Table contents: " + dt.TableName.ToUpper());
            string s = "";
            foreach (DataColumn column in dt.Columns)
            {
                s += column.ColumnName + ";\t";
            }
            Debug.Print("--------------------------------------------");
            Debug.Print(s);
            Debug.Print("--------------------------------------------");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                s = "";
                foreach (DataColumn column in dt.Columns)
                {
                    s += dt.Rows[i][column].ToString() + ";\t";
                }
                Debug.Print(s);
            }
            Debug.Print("--------------------------------------------");
        }
        #endregion

        #region Columns
        /// <summary>
        /// Checks whether a column with given name exists in the given table
        /// </summary>
        public static bool ColumnExists(DbConnection connection, string tableName, string columnName)
        {
            // extracting fields of the given table only
            string[] restrictions = new string[3];
            restrictions[2] = tableName;

            DataTable table = connection.GetSchema("Columns", restrictions);
            if (table != null)
            {
                DbUtilities.DumpDataTable(table);

                int index = table.Columns.IndexOf("COLUMN_NAME");
                if (index >= 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row[index].ToString().ToLower() == columnName.ToLower())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the list of tables which have a field with a given name and type
        /// </summary>
        /// <param name="connection">Opened database connection</param>
        /// <param name="filedName">The name of filed to restrict to</param>
        /// <param name="dataType">The name of filed to restrict to</param>
        /// <returns>List of table names</returns>
        public static List<string> GetTablesByField(DbConnection connection, string fieldName, string dataType)
        {
            DbUtilities.DumpDataSchema(connection);

            List<string> list = new List<string>();

            DataTable table = connection.GetSchema("Columns");
            if (table != null)
            {
                int indexColumnName = table.Columns.IndexOf("COLUMN_NAME");
                int indexDataType = table.Columns.IndexOf("DATA_TYPE");
                int indexTableName = table.Columns.IndexOf("TABLE_NAME");
                int size = table.Columns.Count;

                if (indexColumnName >= 0 && indexDataType >= 0 && indexTableName >= 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row[indexColumnName].ToString() == fieldName &&
                            row[indexDataType].ToString() == dataType)
                        {
                            list.Add(row[indexTableName].ToString());
                        }
                    }
                }
                table.Clear();
            }
            return list;
        }

        /// <summary>
        /// Dumps data schema to specified writer
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="writer"></param>
        public static void DumpDataSchema(DbConnection connection, StreamWriter writer)
        {
            string key = "debug_key";
            if (writer != null)
            {
                Debug.Listeners.Add(new TextWriterTraceListener(writer.BaseStream, key));
            }

            DbUtilities.DumpDataSchema(connection);

            if (writer != null)
            {
                Debug.Listeners.Remove(key);
            }

        }

        /// <summary>
        /// Dumps all the schemas in the datasource
        /// </summary>
        public static void DumpDataSchema(DbConnection connection)
        {
            //TODO: for metadatacollection first and extract everything else from it

            string[] list = new string[]{
                            "MetaDataCollections",
                            "DataSourceInformation",
                            "DataTypes",
                            "ReservedWords",
                            "Catalogs",
                            "Columns",
                            "Indexes",
                            "IndexColumns",
                            "Tables",
                            "Views",
                            "ViewColumns",
                            "ForeignKeys",
                            "Triggers"};

            foreach (string s in list)
            {
                DataTable table = connection.GetSchema(s);
                if (table != null)
                {
                    DbUtilities.DumpDataTable(table);
                }
            }
        }

        #endregion
    }
}
