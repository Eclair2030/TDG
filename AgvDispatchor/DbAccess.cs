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
            reader.Close();
            com.Dispose();
            return res;
        }

        public List<Lifter> GetAllRetriveLiftersWithType(LifterType type)
        {
            List<Lifter> lifters = new List<Lifter>(0);
            string sql = "select code from Lifters";
            if (type != LifterType.None)
            {
                sql += " where Type = " + (int)type;
            }
            
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                SqlDataReader reader = com.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Lifter lifter = new Lifter();
                        lifter.Code = reader["Code"].ToString();
                        lifter.Type = reader["Type"].ToString();
                        lifter.Status = reader["Status"].ToString();
                        lifter.Parking = reader["Parking"].ToString();
                        lifters.Add(lifter);
                    }
                }
                reader.Close();
                com.Dispose();
            }
            return lifters;
        }

        public string ExistCarriersAtLifter(string liftCode)
        {
            string sql = "select count(*) from Lifters where Parking = 2 and Code = '" + liftCode + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                object obj = com.ExecuteScalar();
                string res = ( obj != null && obj.ToString() != string.Empty ) ? obj.ToString() : string.Empty;
                com.Dispose();
                return res;
            }
            else
            {
                return null;
            }
        }

        public bool SetLifterStatus(LifterStatus status, string code)
        {
            bool res = false;
            string sql = "update lifters set status = "+ (int)status + " where Code = '"+ code + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }


        private SqlConnection CONN;
    }
}
