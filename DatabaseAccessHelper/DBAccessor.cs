using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DatabaseAccessHelper
{
    public class DatabaseAccessor
    {
        string connectionString = "";

        string finalValue = string.Empty;
        public string FinalValue { get { return finalValue; } }
        SqlConnection connection = null;

        public DatabaseAccessor()
        {
            System.Data.DataTable dt = Read("SELECT * FROM TestTable");
            object o = dt.Rows[0][0];
            o.ToString();
        }

        public DatabaseAccessor(string databaseName)
        {
        }


        //public void Write(string table, DateTime dt, decimal d1, decimal d2)
        //{
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        // Pool A is created.

        //        using (SqlCommand cmd = connection.CreateCommand())
        //        {
        //            cmd.CommandText = "INSERT INTO " + table + "(date, value, slope) VALUES ( \'" + dt.ToString() + "\', \'" + Decimal.Round(d1, 2) + "\', \'" + Decimal.Round(d2, 2) + "\')";
        //            int newId = cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        public void TestConnection()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Pool A is created.

                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM TestTable";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        string s = reader.GetString(0);
                        finalValue = s;

                        if (finalValue == string.Empty)
                        {
                            throw new Exception( "Final value should not be empty" );
                        }
                    }

                }
            }
        }

        private void Open()
        {
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Pool A is created.
            }
        }

        public System.Data.DataTable Read(string sqlCommand)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;

                    dt = GetTableFromCommand(cmd);

                    connection.Close();
                }
            }

            return dt;
        }

        public System.Data.DataTable GetTableFromCommand(SqlCommand cmd)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (dt.Rows.Count == 0 && dt.Columns.Count == 0)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dt.Columns.Add(reader.GetName(i));
                        }
                    }

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        object[] values = new object[reader.FieldCount];
                        reader.GetValues( values );

                        dt.Rows.Add(values);
                    }
                }
            }

            return dt;
        }

        private int GetIntFromInsert(SqlCommand cmd)
        {
            int newId = 0;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    newId = (int)reader.GetValue(0);
                }
            }

            return newId;
        }

        public int Write(string sqlCommand)
        {
            int newId = 0;
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    newId = (int)cmd.ExecuteNonQuery();

                    connection.Close();
                }
            }


            return newId;
        }
    }
}
