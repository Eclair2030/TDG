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
                        material.TargetDeviceCode = Convert.ToInt32(reader["TargetDeviceCode"]);
                        material.TargetDeviceArea = Convert.ToInt32(reader["TargetDeviceArea"]);
                        material.TargetDeviceIndex = Convert.ToInt32(reader["TargetDeviceIndex"]);
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
            try
            {
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    object obj = com.ExecuteScalar();
                    carrierCode = obj != null ? obj.ToString() : null;
                    com.Dispose();
                }
            }
            catch (Exception)
            {
                carrierCode = null;
            }
            return carrierCode;
        }

        public int GetCarrierTargetPosition(string carrCode)
        {
            int pos = -1;
            string sql = "select * from Materials where Status = " + (int)MaterialStatus.Carrier + " and CarrierCode = '" + carrCode + "' order by " +
                    "TargetDeviceCode asc, TargetDeviceArea asc, TargetDeviceIndex asc";
            try
            {
                SqlCommand scom = new SqlCommand(sql, CONN);
                SqlDataReader r = scom.ExecuteReader();
                if (r != null && r.HasRows)
                {
                    while (r.Read())
                    {
                        int dCode = Convert.ToInt32(r["TargetDeviceCode"]);
                        int dArea = Convert.ToInt32(r["TargetDeviceArea"]);
                        int dIndex = Convert.ToInt32(r["TargetDeviceIndex"]);
                        pos = dCode * Material.TOTAL_MATERIAL_ONE_DEVICE / Material.TOTAL_MATERIAL_ONE_POSITION
                            + dArea * Material.TOTAL_MATERIAL_ONE_AREA / Material.TOTAL_MATERIAL_ONE_POSITION
                            + dIndex / Material.TOTAL_MATERIAL_ONE_POSITION;
                    }
                }
            }
            catch (Exception)
            {
            }
            return pos;
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
                            lifter.Type = Convert.ToInt32(reader["Type"]);
                            lifter.Status = Convert.ToInt32(reader["Status"]);
                            lifter.Parking = reader["Parking"].ToString();
                            lifter.QueuePosition = Convert.ToInt32(reader["QueuePosition"]);
                            lifter.Position = Convert.ToInt32(reader["Position"]);
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
                            lifter.Type = Convert.ToInt32(reader["Type"]);
                            lifter.Status = Convert.ToInt32(reader["Status"]);
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

        public string LifterQueueAutoCheck(string lifterCode)
        {
            string carrierCode = string.Empty;
            try
            {
                string sql = "select top 1 CarrierCode from LifterQueue where Number = 0 and Code = '" + lifterCode + "'";
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    object carr = com.ExecuteScalar();
                    com.Dispose();
                    if (carr != null)
                    {
                        carrierCode = carr.ToString();
                    }
                    else
                    {
                        sql = "update LifterQueue set Number = Number - 1 where Code = '" + lifterCode + "'";
                        com = new SqlCommand(sql, CONN);
                        com.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                carrierCode = null;
            }
            return carrierCode;
        }

        public bool AddCarrierToShortestQueue(LifterType lt, string carrierCode)
        {
            bool result = false;
            try
            {
                string sql = "insert into LifterQueue(Code, Number, CarrierCode) values(" +
                    "(select top 1 q.Code from LifterQueue q inner join Lifters l on l.Code = q.Code and l.Type = " + (int)lt + " group by q.Code having count(*) > 0 order by count(*) asc)," +
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

        public bool AddCarrierToShortestQueue(LifterType lt, string carrierCode, out string lifterCode)
        {
            bool result = false;
            try
            {
                string sql = "select top 1 q.Code from LifterQueue q inner join Lifters l on l.Code = q.Code and l.Type = " + (int)lt + " group by q.Code having count(*) > 0 order by count(*) asc";
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    object code = com.ExecuteScalar();
                    com.Dispose();
                    if (code != null)
                    {
                        lifterCode = code.ToString();
                        sql = "insert into LifterQueue(Code, Number, CarrierCode) values('" + lifterCode + "'," + 
                            "(select top 1 count(*) from LifterQueue q inner join Lifters l on l.Code = q.Code and l.Type = " + (int)lt + " group by q.Code order by count(*) asc)," +
                            "'" + carrierCode + "')";
                        com = new SqlCommand(sql, CONN);
                        com.ExecuteNonQuery();
                        com.Dispose();
                        result = true;
                    }
                    else
                    {
                        lifterCode = string.Empty;
                    }
                }
                else
                {
                    lifterCode = null;
                }
            }
            catch (Exception)
            {
                lifterCode = null;
            }
            return result;
        }
        #endregion
        #region Request
        public int ExistMaterialRequests()
        {
            int result = 0;
            //Carrier.RESOURCE_ONE_SIDE
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

        public bool MakeResponse()
        {
            bool res = false;
            string sql = "update Requests set RequestCode = " + (int)RequestCodeType.Transporting +
                " where DeviceCode = (select MIN(DeviceCode) from Requests) " +
                "and DeviceArea = (select MIN(DeviceArea) from Requests where Device = (select MIN(DeviceCode) from Requests)) " +
                "and DeviceIndex = (select MIN(DeviceIndex) from Requests where DeviceArea = (select MIN(DeviceArea) from Requests where Device = (select MIN(DeviceCode) from Requests)))";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }

        public bool InitDevicebyDeviceID(int deviceID)
        {
            bool res = false;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < Material.TOTAL_MATERIAL_ONE_AREA; j++)
                {
                    string sql = "insert into Requests(DeviceCode, DeviceArea, DeviceIndex, RequestCode) values(" + deviceID + ", " + i + ", " + j + ", " + (int)RequestCodeType.Oncall + ")";
                    if (CONN.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand com = new SqlCommand(sql, CONN);
                        com.ExecuteNonQuery();
                        com.Dispose();
                        res = true;
                    }
                }
            }
            
            return res;
        }
        #endregion
        #region Robots
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
                        robot.Status = Convert.ToInt32(reader["Status"]);
                        robot.Battery = reader["Battery"].ToString();
                        robot.Coordination = Convert.ToInt32(reader["Coordination"]);
                        robot.Process = Convert.ToInt32(reader["Process"]);
                        robot.DeviceIndex = Convert.ToInt32(reader["DeviceIndex"]);
                        robot.CarrierIndex = Convert.ToInt32(reader["CarrierIndex"]);
                        robot.Buffer1 = Convert.ToInt32(reader["Buffer1"]);
                        robot.Buffer2 = Convert.ToInt32(reader["Buffer2"]);
                        robot.Arm = Convert.ToInt32(reader["Arm"]);
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
        #endregion
        #region Materials
        public bool AssignMaterialsToLifter(string lifterCode)
        {
            bool res = false;
            for (int i = 0; i < Material.TOTAL_MATERIAL_ONE_CAR; i++)
            {
                string sql = "insert into Materials(Code,LifterCode,CarrierIndex,TargetDeviceCode,TargetDeviceIndex,Status) values(''," +
                    "'" + lifterCode + "'," + i + "," +
                    "(select top 1 DeviceCode from Requests where RequestCode = " + (int)RequestCodeType.Oncall + " order by DeviceCode, DeviceIndex asc)," +
                    "(select top 1 DeviceIndex from Requests where RequestCode = " + (int)RequestCodeType.Oncall + " order by DeviceCode, DeviceIndex asc)," +
                    (int)MaterialStatus.Lifter + ")";
                try
                {
                    if (CONN.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand com = new SqlCommand(sql, CONN);
                        com.ExecuteNonQuery();
                        com.Dispose();
                        res = true;
                    }
                }
                catch (Exception)
                {
                }
            }
            return res;
        }

        public bool AssignMaterialsToCarrier(string lifterCode, string carrierCode)
        {
            bool res = false;
            string sql = "Update Materials set CarrierCode = '"+ carrierCode + 
                "',Status = " + (int)MaterialStatus.Carrier + " where LifterCode = '"+ lifterCode + "' and Status = " + (int)MaterialStatus.Lifter;
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }

        public bool AssignMaterialsOnCarrierToDevice(string carrCode)
        {
            bool res = false;
            try
            {
                int total_materials = Material.TOTAL_MATERIAL_ONE_CAR;
                int circle = 0;
                string sql = string.Empty;
                while (total_materials > 0)
                {
                    sql = "select count(*) from Carriers where WorkDevice = ((select top 1 DeviceCode from Requests where RequestCode = 1 order by DeviceCode asc) + " + circle + ")";
                    if (CONN.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand com = new SqlCommand(sql, CONN);
                        object obj = com.ExecuteScalar();
                        com.Dispose();
                        if (obj != null)
                        {
                            int currDev_CarrierCount = Convert.ToInt32(obj);
                            string order = string.Empty;
                            if (currDev_CarrierCount == 0)
                            {
                                order = "asc";
                            }
                            else if (currDev_CarrierCount == 1)
                            {
                                order = "desc";
                            }
                            else
                            {
                                continue;
                            }
                            if (order != string.Empty)
                            {
                                sql = "select count(*) from Requests where DeviceCode = ((select top 1 DeviceCode from Requests where RequestCode = 1 order by DeviceCode asc) + " +
                                    circle + ") and RequestCode = 1";
                                com = new SqlCommand(sql, CONN);
                                obj = com.ExecuteScalar();
                                com.Dispose();
                                if (obj != null)
                                {
                                    int currDev_RequestCount = Convert.ToInt32(obj);
                                    sql = "select top " + currDev_RequestCount / 2 + " * from Requests where RequestCode = 1 and " + 
                                        "DeviceCode = ((select top 1 DeviceCode from Requests where RequestCode = 1 order by DeviceCode asc) + " + circle + ") and " + 
                                        "DeviceArea % 2 = 1 order by DeviceCode asc, DeviceArea " + order + ", DeviceIndex asc";
                                    com = new SqlCommand(sql, CONN);
                                    SqlDataReader reader = com.ExecuteReader();
                                    if (reader != null && reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            int devCode = Convert.ToInt32(reader["DeviceCode"]);
                                            int devArea = Convert.ToInt32(reader["DeviceArea"]);
                                            int devIndex = Convert.ToInt32(reader["DeviceIndex"]);
                                            sql = "update Materials set TargetDeviceCode = " + devCode + ", TargetDeviceArea = " + devArea + ", TargetDeviceIndex = " + devIndex +
                                                " where CarrierCode = '" + carrCode + "' and Status = " + (int)MaterialStatus.Carrier;
                                            SqlCommand com1 = new SqlCommand(sql, CONN);
                                            com1.ExecuteNonQuery();
                                            com1.Dispose();
                                            sql = "update Materials set TargetDeviceCode = " + devCode + ", TargetDeviceArea = " + (devArea + 1) + ", TargetDeviceIndex = " + devIndex +
                                                " where CarrierCode = '" + carrCode + "' and Status = " + (int)MaterialStatus.Carrier;
                                            SqlCommand com2 = new SqlCommand(sql, CONN);
                                            com2.ExecuteNonQuery();
                                            com2.Dispose();
                                        }
                                    }
                                    reader.Close();
                                    com.Dispose();
                                    total_materials -= currDev_RequestCount;
                                    circle++;
                                    if (circle == 16)
                                    {
                                        circle = 0;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                sql = "select top 1 * from Materials where Status = " + (int)MaterialStatus.Carrier + " and CarrierCode = '" + carrCode + "' order by " + 
                    "TargetDeviceCode asc, TargetDeviceArea asc, TargetDeviceIndex asc";
                SqlCommand scom = new SqlCommand(sql, CONN);
                SqlDataReader r = scom.ExecuteReader();
                if (r != null && r.HasRows)
                {
                    while (r.Read())
                    {
                        int dCode = Convert.ToInt32(r["TargetDeviceCode"]);
                        int dArea = Convert.ToInt32(r["TargetDeviceArea"]);
                        int dIndex = Convert.ToInt32(r["TargetDeviceIndex"]);
                        sql = "update Carriers set WorkCode = " + dCode + ", WorkArea = " + dArea + ", WorkIndex = " + dIndex +
                            " where CarrierCode = '" + carrCode + "'";
                        SqlCommand c1 = new SqlCommand(sql, CONN);
                        c1.ExecuteNonQuery();
                        c1.Dispose();
                    }
                }
                if (total_materials == 0)
                {
                    res = true;
                }
            }
            catch (Exception)
            {
            }
            return res;
        }
        #endregion
        #region Charge
        public List<Battery> GetAllBatterys()
        {
            List<Battery> list = new List<Battery>(0);
            string sql = "select * from Batterys";
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = null;
            try
            {
                reader = com.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Battery bt = new Battery();
                        bt.Code = reader["Code"].ToString();
                        bt.Status = Convert.ToInt32(reader["Status"]);
                        bt.Position = Convert.ToInt32(reader["Position"]);
                        bt.QueuePosition = Convert.ToInt32(reader["QueuePosition"]);
                        list.Add(bt);
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

        public List<Carrier> FindLowPowerCarriers()
        {
            string sql = "select * from Carriers where Battery < "+ (int)Carrier.LOW_POWER + " and Status = " + (int)CarrierStatus.Idle;
            List<Carrier> list = new List<Carrier>(0);
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

        public List<Robot> FindLowPowerRobots()
        {
            string sql = "select * from Robots where Battery < " + (int)Robot.LOW_POWER + " and Status = " + (int)RobotStatus.Idle;
            List<Robot> list = new List<Robot>(0);
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
                        robot.Status = Convert.ToInt32(reader["Status"]);
                        robot.Battery = reader["Battery"].ToString();
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

        public bool AddAGVToChargeQueue(string agvCode)
        {
            bool res = false;
            string sql = "insert into BatteryQueue(Number, Code) values((select ISNULL(max(number),0) from BatteryQueue)+1, '" + agvCode + "')";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }

        public List<ChargeQueue> GetChargeQueue()
        {
            string sql = "select * from Robots where Battery < " + (int)Robot.LOW_POWER + " and Status = " + (int)RobotStatus.Idle;
            List<ChargeQueue> list = new List<ChargeQueue>(0);
            SqlCommand com = new SqlCommand(sql, CONN);
            SqlDataReader reader = null;
            try
            {
                reader = com.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ChargeQueue queue = new ChargeQueue();
                        queue.Number = reader["Number"].ToString();
                        queue.Code = reader["Code"].ToString();
                        list.Add(queue);
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

        public string ChargeQueueAutoCheck()
        {
            string code = string.Empty;
            try
            {
                string sql = "select top 1 Code from BatteryQueue where Number = 0";
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    object agv = com.ExecuteScalar();
                    com.Dispose();
                    if (agv != null)
                    {
                        code = agv.ToString();
                    }
                    else
                    {
                        sql = "update BatteryQueue set Number = Number - 1";
                        com = new SqlCommand(sql, CONN);
                        com.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                code = null;
            }
            return code;
        }

        public bool ChargeQueueDeleteZero()
        {
            bool result = false;
            try
            {
                string sql = "delete ChargeQueue where Number = 0";
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
        private SqlConnection CONN;
    }
}
