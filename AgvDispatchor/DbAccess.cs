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
            CONN = new SqlConnection("Data Source=192.168.1.159;Database=AGV;uid=sa;pwd=1q2w3e4r");
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

        public List<Carrier> GetAllCarriers()
        {
            List<Carrier> list = new List<Carrier>(0);
            string sql = "select * from Carriers";
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = com.ExecuteReader();
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    Carrier carrier = new Carrier();
                    carrier.Code = reader["Code"].ToString();
                    carrier.Status = reader["Status"].ToString();
                    carrier.Battery = reader["Battery"].ToString();
                    carrier.Robot_1 = reader["Robot_1"].ToString();
                    carrier.Robot_2 = reader["Robot_2"].ToString();
                    list.Add(carrier);
                }
            }
            reader.Close();
            com.Dispose();
            return list;
        }

        public List<Material> GetMaterialsByCarrierCode(string code)
        {
            List<Material> list = new List<Material>();
            string sql = "select * from Materials where CarrierCode = '"+code+"' and Status = " + (int)MaterialStatus.Carrier;
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = com.ExecuteReader();
            try
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Material material = new Material();
                        material.Code = reader["Code"].ToString();
                        material.Status = reader["Status"].ToString();
                        material.LifterCode = reader["LifterCode"].ToString();
                        material.CarrierCode = reader["CarrierCode"].ToString();
                        material.CarrierIndex = reader["CarrierIndex"].ToString();
                        material.TargetDeviceCode = reader["TargetDeviceCode"].ToString();
                        material.TargetDeviceIndex = reader["TargetDeviceIndex"].ToString();
                        material.RobotCode = reader["RobotCode"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                list = null;
            }
            reader.Close();
            com.Dispose();
            return list;
        }

        public bool SetCarrierStatus(string code, CarrierStatus status)
        {
            bool result = false;
            string sql = "update Carriers set status = " + (int)status + " where Code = '" + code + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                result = true;
            }
            return result;
        }

        public List<Material> QueryMaterialByCarrierCode(string code)
        {
            List<Material> list = new List<Material>();
            string sql = "select * from Materials where CarrierCode = '" + code + "'";
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = com.ExecuteReader();
            if (reader != null && reader.HasRows)
            {
                try
                {
                    while (reader.Read())
                    {
                        Material m = new Material();
                        m.Code = reader["Code"].ToString();
                        m.LifterCode = reader["LifterCode"].ToString();
                        m.CarrierCode = reader["CarrierCode"].ToString();
                        m.CarrierIndex = reader["CarrierIndex"].ToString();
                        m.TargetDeviceCode = reader["TargetDeviceCode"].ToString();
                        m.TargetDeviceIndex = reader["TargetDeviceIndex"].ToString();
                        m.RobotCode = reader["RobotCode"].ToString();
                        m.Status = reader["Status"].ToString();
                        list.Add(m);
                    }
                }
                catch (Exception)
                {
                    list = null;
                }
            }
            reader.Close();
            com.Dispose();

            return list;
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
