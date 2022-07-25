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
            CONN = new SqlConnection("Data Source=127.0.0.1;Database=AGV;uid=sa;pwd=1q2w3e4r");
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
        #region Carriers
        public List<Carrier> GetAllCarriers()
        {
            List<Carrier> list = new List<Carrier>(0);
            string sql = "select * from Carriers";
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = null;
            try
            {
                reader = com.ExecuteReader();
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
            }
            catch (Exception)
            {
                list = null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            
            com.Dispose();
            return list;
        }

        public List<Carrier> GetCarriersByStatus(CarrierStatus status)
        {
            List<Carrier> list = new List<Carrier>(0);
            string sql = "select * from Carriers where status = " + (int)status;
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = null;
            try
            {
                reader = com.ExecuteReader();
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
            }
            catch (Exception)
            {
                list = null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            com.Dispose();
            return list;
        }

        public List<Material> GetMaterialsByCarrierCode(string code)
        {
            List<Material> list = new List<Material>();
            string sql = "select * from Materials where CarrierCode = '"+code+"' and Status = " + (int)MaterialStatus.Carrier;
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = null;
            try
            {
                reader = com.ExecuteReader();
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
                        list.Add(material);
                    }
                }
            }
            catch (Exception)
            {
                list = null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            
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
        #endregion
        #region Lifters
        public List<Lifter> GetAllLiftersWithType(LifterType type)
        {
            List<Lifter> lifters = new List<Lifter>(0);
            string sql = "select * from Lifters";
            if (type != LifterType.None)
            {
                sql += " where Type = " + (int)type;
            }
            
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                SqlDataReader reader = null;
                try
                {
                    reader = com.ExecuteReader();
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
                }
                catch (Exception)
                {
                    lifters = null;
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
                
                com.Dispose();
            }
            return lifters;
        }

        public List<Lifter> GetLiftersWithStatus(LifterType lt, int status)
        {
            List<Lifter> lifters = new List<Lifter>(0);
            string sql = "select * from Lifters where type = " + (int)lt + " and status = " + status;
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                SqlDataReader reader = null;
                try
                {
                    reader = com.ExecuteReader();
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
                }
                catch (Exception)
                {
                    lifters = null;
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
                com.Dispose();
            }
            return lifters;
        }

        public string GetFirstCarrierInLifterQueuebyCode(string liftCode, LifterType lt)
        {
            string carrierCode = string.Empty;
            string sql;
            if (lt == LifterType.Retrive)
            {
                sql = "select l.CarrierCode from LifterQueue l inner join Carriers c on l.Number = 0 and l.Code = '" +
                    liftCode + "' and c.Code = l.CarrierCode and c.Status = " + (int)CarrierStatus.Retrieving;
            }
            else
            {
                sql = "select l.CarrierCode from LifterQueue l inner join Carriers c on l.Number = 0 and l.Code = '" +
                    liftCode + "' and c.Code = l.CarrierCode and c.Status = " + (int)CarrierStatus.Initing;
            }
            
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                object obj = com.ExecuteScalar();
                carrierCode = obj != null ? obj.ToString() : null;
                com.Dispose();
            }
            else
            {
                carrierCode = null;
            }
            return carrierCode;
        }

        public bool SetRetriveLifterStatus(RetriveLifterStatus status, string code)
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

        public bool SetSupplyLifterStatus(SupplyLifterStatus status, string code)
        {
            bool res = false;
            string sql = "update lifters set status = " + (int)status + " where Code = '" + code + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }

        public List<CarrierInLifterQueue> GetLifterQueue(string lifterCode)
        {
            List<CarrierInLifterQueue> queue = new List<CarrierInLifterQueue>();
            string sql = "select * from LifterQueue where Code = '" + lifterCode + "'";
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = null;
            try
            {
                reader = com.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        CarrierInLifterQueue carrier = new CarrierInLifterQueue();
                        carrier.Code = reader["Code"].ToString();
                        carrier.Number = reader["Number"].ToString();
                        carrier.CarrierCode = reader["CarrierCode"].ToString();
                        queue.Add(carrier);
                    }
                }
            }
            catch (Exception)
            {
                queue = null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            com.Dispose();
            return queue;
        }

        public bool LifterQueueDeleteZero(string lifterCode)
        {
            bool result = false;
            try
            {
                string sql = "delete LifterQueue where Number = 0 and Code = '" + lifterCode + "'";
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    com.ExecuteNonQuery();
                    com.Dispose();
                    result = true;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool LifterQueueAutoCheck(string lifterCode)
        {
            bool result = false;
            try
            {
                string sql = "select top 1 Number from LifterQueue where Number = 0 and Code = '" + lifterCode + "'";
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    object carr = com.ExecuteScalar();
                    com.Dispose();
                    if (carr != null)
                    {
                    }
                    else
                    {
                        sql = "update LifterQueue set Number = Number - 1 where Code = '" + lifterCode + "'";
                        com = new SqlCommand(sql, CONN);
                        com.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool AddCarrierToShortestQueue(LifterType lt, string carrierCode)
        {
            bool result = false;
            try
            {
                string sql = "insert into LifterQueue(Code, Number, CarrierCode) values(" +
                    "(select top 1 q.Code from LifterQueue q inner join Lifters l on l.Code = q.Code and l.Type = " + (int)lt + " group by q.Code having count(*) > 0 order by count(*) asc), " +
                    "(select top 1 count(*) from LifterQueue q inner join Lifters l on l.Code = q.Code and l.Type = " + (int)lt + " group by q.Code order by count(*) asc)," +
                    "'" + carrierCode + "')";
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    com.ExecuteNonQuery();
                    com.Dispose();
                    result = true;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        #endregion
        #region Request
        public int ExistMaterialRequests()
        {
            int result = 0;
            string sql = "select count(*) from Requests where RequestCode = 1";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                object count = com.ExecuteScalar();
                if (count != null)
                {
                    result = Convert.ToInt32(count);
                }
                com.Dispose();
            }
            return result;
        }

        public bool SetRequestOnByDeviceID(string deviceID)
        {
            bool res = false;
            string sql = "update Requests set RequestCode = " + (int)RequestCodeType.Oncall + " where DeviceCode = '" + deviceID + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }
        #endregion

        public List<Robot> GetAllRobots()
        {
            List<Robot> list = new List<Robot>(0);
            string sql = "select * from Robots";
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = null;
            try
            {
                reader = com.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Robot robot = new Robot();
                        robot.Code = reader["Code"].ToString();
                        robot.Status = reader["Status"].ToString();
                        robot.Battery = reader["Battery"].ToString();
                        robot.Coordination = reader["Coordination"].ToString();
                        robot.Process = reader["Process"].ToString();
                        robot.DeviceIndex = reader["DeviceIndex"].ToString();
                        robot.CarrierIndex = reader["CarrierIndex"].ToString();
                        robot.Buffer1 = reader["Buffer1"].ToString();
                        robot.Buffer2 = reader["Buffer2"].ToString();
                        robot.Arm = reader["Arm"].ToString();
                        list.Add(robot);
                    }
                }
            }
            catch (Exception)
            {
                list = null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            com.Dispose();
            return list;
        }

        private SqlConnection CONN;
    }
}
