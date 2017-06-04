// Data provider
// Author: Sergei Leschinski
// Created: 06 september 2011

//#define SQL_SERVER_CE  // sql server ce provider is included (appropriate reference should be added to the project)
//#define SQL_SERVER     // sql server provider is included (appropriate reference should be added to the project)
#define SQLITE_PROVIDER     // sql server provider is included (appropriate reference should be added to the project)

namespace MapWinGIS.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Common;
    using System.Data.OleDb;

#if SQLITE_PROVIDER
    using System.Data.SQLite;
    using System.Data;
#endif

#if SQL_SERVER
        using System.Data.SqlClient;
#endif

#if SQL_SERVER_CE
        using System.Data.SqlServerCe;
#endif

    #region Interface
    /// <summary>
    /// Custom provider interface
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Returns connection object for specified provider. No setting of properties is expected.
        /// </summary>
        DbConnection CreateConnection();

        /// <summary>
        /// Returns connection object for specified provider. Sets the connection string based on the specified db name.
        /// </summary>
        DbConnection CreateConnection(string dbName);

        /// <summary>
        /// Returns command object for specified provider. No setting of properties is expected.
        /// </summary>
        DbCommand CreateCommand();

        /// <summary>
        /// Returns parameter for simple types: string, integer, double. No setting of properties is expected.
        /// </summary>
        DbParameter CreateParameter();

        /// <summary>
        /// Creates long binary parameter for geometry field. The type of parameter should be set according to provider.
        /// </summary>
        DbParameter CreateBinaryParameter();

        /// <summary>
        /// Returns data adapter object for specified provider. No setting of properties is expected.
        /// </summary>
        DbDataAdapter CreateDataAdapter();

        /// <summary>
        /// Name of the BLOB data type to use in SQL to store geometry
        /// </summary>
        string GetBinaryTypeName();

        /// <summary>
        /// Provides the name of the interger data type for SQL exression
        /// </summary>
        string GetIntergerTypeName();

        /// <summary>
        /// Provides the name of the double data type for SQL exression
        /// </summary>
        string GetDoubleTypeName();

        /// <summary>
        /// Provides the name of the string data type for SQL exression
        /// </summary>
        string GetStringTypeName(out bool fixedLength);

        /// <summary>
        /// Creates default connection string for specified database
        /// </summary>
        string CreateConnectionString(string dbName);
    }
    #endregion

    #region SqLite provider
#if SQLITE_PROVIDER
    /// <summary>
    /// SQLite provider for shapefile conveter
    /// </summary>
    public class SQLiteProvider : IDataProvider
    {
        public DbConnection CreateConnection()
        {
            return new SQLiteConnection();
        }
        public DbCommand CreateCommand()
        {
            return new SQLiteCommand();
        }
        public DbParameter CreateParameter()
        {
            return new SQLiteParameter();
        }
        public DbParameter CreateBinaryParameter()
        {
            SQLiteParameter param = new SQLiteParameter();
            param.DbType = DbType.Object;
            return param;
        }
        public DbDataAdapter CreateDataAdapter()
        {
            return new SQLiteDataAdapter();
        }
        public string GetBinaryTypeName()
        {
            return "BLOB";
        }
        public string GetIntergerTypeName()
        {
            return "INTEGER";
        }
        public string GetDoubleTypeName()
        {
            return "REAL";
        }
        public string GetStringTypeName(out bool fixedLength)
        {
            fixedLength = false;
            return "TEXT";
        }
        public string CreateConnectionString(string dbName)
        {
            return "Data Source = " + dbName + "; Version = 3;";
        }
        public DbConnection CreateConnection(string dbName)
        {
            DbConnection conn = this.CreateConnection();
            conn.ConnectionString = this.CreateConnectionString(dbName);
            return conn;
        }
    }
#endif
    #endregion

    #region OleDb provider
    /// <summary>
    /// OleDb provider for shapefile converter
    /// </summary>
    public class OleDbProvider : IDataProvider
    {
        public DbConnection CreateConnection()
        {
            return new OleDbConnection();
        }
        public DbCommand CreateCommand()
        {
            return new OleDbCommand();
        }
        public DbParameter CreateParameter()
        {
            return new OleDbParameter();
        }
        public DbDataAdapter CreateDataAdapter()
        {
            return new OleDbDataAdapter();
        }
        public DbParameter CreateBinaryParameter()
        {
            OleDbParameter param = new OleDbParameter();
            param.OleDbType = OleDbType.LongVarBinary;
            return param;
        }
        public string GetBinaryTypeName()
        {
            return "longbinary";
        }
        public string GetIntergerTypeName()
        {
            return "integer";
        }
        public string GetDoubleTypeName()
        {
            return "double";
        }
        public string GetStringTypeName(out bool fixedLength)
        {
            fixedLength = true;
            return "char";
        }
        public string CreateConnectionString(string dbName)
        {
            return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbName + ";User Id=admin;Password=;";
        }
        public DbConnection CreateConnection(string dbName)
        {
            DbConnection conn = this.CreateConnection();
            conn.ConnectionString = this.CreateConnectionString(dbName);
            return conn;
        }
    }
    #endregion

    #region Sql Server CE provider
#if SQL_SERVER_CE
    /// <summary>
    /// Imlementation of IDataProvider interface for SQL Server compact edition
    /// </summary>
    public class SqlCeProvider : MapWinGIS.Data.IDataProvider
    {
        public DbParameter CreateBinaryParameter()
        {
            SqlCeParameter param = new SqlCeParameter();
            param.SqlDbType = SqlDbType.VarBinary;
            return param;
        }
        public DbCommand CreateCommand()
        {
            return new SqlCeCommand();
        }
        public DbConnection CreateConnection()
        {
            return new SqlCeConnection();
        }
        public DbDataAdapter CreateDataAdapter()
        {
            return new SqlCeDataAdapter();
        }
        public DbParameter CreateParameter()
        {
            return new SqlCeParameter();
        }
        public string GetBinaryTypeName()
        {
            return "image";
        }
        public string GetIntergerTypeName()
        {
            return "integer";
        }
        public string GetDoubleTypeName()
        {
            return "float";
        }
        public string GetStringTypeName(out bool fixedLength)
        {
            fixedLength = true;
            return "nvarchar";
        }
        public string CreateConnectionString(string dbName)
        {
            return "Data Source=" + dbName + ";Persist Security Info=False;";
        }
    }
#endif
    #endregion

    #region SQL Server provider
#if SQL_SERVER
    /// <summary>
    /// Sql provider for shapefile converter
    /// </summary>
    public class SqlProvider : IDataProvider
    {
        public DbConnection CreateConnection()
        {
            return new SqlConnection();
        }
        public DbCommand CreateCommand()
        {
            return new SqlCommand();
        }
        public DbParameter CreateParameter()
        {
            return new SqlParameter();
        }
        public DbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }
        public DbParameter CreateBinaryParameter()
        {
            SqlParameter param = new SqlParameter();
            param.SqlDbType = SqlDbType.VarBinary;
            return param;
        }
        public string GetBinaryTypeName()
        {
            return "varbinary(max)";
        }
        public string GetIntergerTypeName()
        {
            return "integer";
        }
        public string GetDoubleTypeName()
        {
            return "float";
        }
        public string GetStringTypeName(out bool fixedLength)
        {
            fixedLength = true;
            return "nvarchar";
        }
        public string CreateConnectionString(string dbName)
        {
            return "Data Source=" + dbName + ";Persist Security Info=False;";
        }
    }
#endif
    #endregion
}
