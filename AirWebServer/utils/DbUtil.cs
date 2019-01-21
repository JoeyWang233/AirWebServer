using System;
using System.Data;
using System.Data.SqlClient;

namespace AirWebServer.utils {
    public class DbUtil {
        private static string strConn = "Data Source=localhost;Initial Catalog=AirServer2;Persist Security Info=True;User ID=sa;Password=wgf3864486994";
        private static SqlConnection conn;
        private static SqlCommand cmd;
        private static DataSet dataSet;
        private static SqlDataAdapter adapter;
        private static DataTable dataTable;

        public static DataTable ExecuteQuery(string sqlStr) {
            conn = new SqlConnection(strConn);
            adapter = new SqlDataAdapter(sqlStr, strConn);
            dataTable = new DataTable();

            try {
                conn.Open();
                adapter.Fill(dataTable);
            } catch (Exception) {
                throw;
            } finally {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dataTable;
        }
    }
}
