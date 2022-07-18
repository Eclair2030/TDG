using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class DbAccess
    {
        public DbAccess()
        {
            CONN = new SqlConnection("Data Source=127.0.0.1;Database=AGV;Trusted_Connection = SSPI");
        }

        public bool Open()
        {
            bool res = false;
            CONN.Open();
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                res = true;
            }
            return res;
        }

        public void Close()
        {
            CONN.Close();
        }

        public string Query(string targetColumn, string tableName, string param)
        {
            string res = string.Empty;
            string sql = "select " + targetColumn + " from " + tableName + " where " + param;
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = com.ExecuteReader();
            if (reader != null && reader.HasRows && reader.Read())
            {
                reader[0].ToString();
            }

            return res;
        }

        public string[] GetAllRetriveLiftersWithType(LifterType type)
        {
            string sql = "select code from Lifters where Type = " + (int)type;
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                SqlDataReader reader = com.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    string[] retriveLifters = new string[0];
                    while (reader.Read())
                    {
                        retriveLifters.Append(reader[0]);
                    }
                    reader.Close();
                    return retriveLifters;
                }
                else
                {
                    reader.Close();
                    return null;
                }
            }
            else
            {
                return null;
            }
            
        }

        public string QueryCarriersAtLifter(CarrierStatus status, string liftCode)
        {
            string sql = "select c.Code from Carriers as c inner join LifterQueue as l on c.Status = " + (int)status  + " and l.CarrierCode = c.Code and l.Code = '"+ liftCode  + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                object obj = com.ExecuteScalar();
                string res = obj != null ? obj.ToString() : string.Empty;
                return res;
            }
            else
            {
                return string.Empty;
            }
        }


        private SqlConnection CONN;
    }
}
