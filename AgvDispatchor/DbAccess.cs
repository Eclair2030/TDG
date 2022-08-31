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
            string sql = "select top 1 * from Materials where Status = " + (int)MaterialStatus.Carrier + " and CarrierCode = '" + carrCode + "' order by " +
                    "TargetDeviceCode asc, TargetDeviceArea asc, TargetDeviceIndex asc";
            try
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                SqlDataReader r = com.ExecuteReader();
                if (r != null && r.HasRows)
                {
                    while (r.Read())
                    {
                        int dCode = Convert.ToInt32(r["TargetDeviceCode"]);
                        int dArea = Convert.ToInt32(r["TargetDeviceArea"]);
                        int dIndex = Convert.ToInt32(r["TargetDeviceIndex"]);
                        pos = dCode * Material.TOTAL_MATERIAL_ONE_DEVICE / Material.TOTAL_MATERIAL_ONE_POSITION
                            + dArea * Material.TOTAL_MATERIAL_ONE_AREA / Material.TOTAL_MATERIAL_ONE_POSITION
                            + dIndex * Material.TOTAL_AGVS_ONE_POSITION / Material.TOTAL_MATERIAL_ONE_POSITION;
                    }
                    if (pos != -1)
                    {
                        sql = "update Carriers set TargetPosition = " + pos + " where Code = '" + carrCode + "'";
                        com = new SqlCommand(sql, CONN);
                        com.ExecuteNonQuery();
                    }
                }
                com.Dispose();
                r.Close();
            }
            catch (Exception)
            {
            }
            return pos;
        }

        public int GetCarrierStatusByRobot(string robCode, int coord)
        {
            int status = -1;
            string sql = "select status from Carriers where Robot_" + coord + " = '" + robCode + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                object obj = com.ExecuteScalar();
                com.Dispose();
                if (obj != null)
                {
                    status = Convert.ToInt32(obj);
                }
            }
            return status;
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

        public int GetDeviceStaffState(int tarCode, int tarArea, int tarIndex)
        {
            int state = -1;
            try
            {
                string sql = "select Status from Requests where DeviceCode = " + tarCode + " and DeviceArea = " + tarArea + " and DeviceIndex = " + tarIndex;
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    object s = com.ExecuteScalar();
                    com.Dispose();
                    if (s != null)
                    {
                        state = Convert.ToInt32(s);
                    }
                }
            }
            catch (Exception)
            {
            }
            return state;
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
                        robot.IpAddress = reader["IpAddress"].ToString();
                        robot.Port = Convert.ToInt32(reader["Port"]);
                        robot.Points = new List<RobotPoint>(10);
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

        public bool RefreshAllRobots(ref List<Robot> list)
        {
            bool result = false;
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
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].Code == reader["Code"].ToString())
                            {
                                list[i].Code = reader["Code"].ToString();
                                list[i].Status = Convert.ToInt32(reader["Status"]);
                                list[i].Battery = reader["Battery"].ToString();
                                list[i].Coordination = Convert.ToInt32(reader["Coordination"]);
                                list[i].Process = Convert.ToInt32(reader["Process"]);
                                list[i].DeviceIndex = Convert.ToInt32(reader["DeviceIndex"]);
                                list[i].CarrierIndex = Convert.ToInt32(reader["CarrierIndex"]);
                                list[i].Buffer1 = Convert.ToInt32(reader["Buffer1"]);
                                list[i].Buffer2 = Convert.ToInt32(reader["Buffer2"]);
                                list[i].Arm = Convert.ToInt32(reader["Arm"]);
                                list[i].IpAddress = reader["IpAddress"].ToString();
                                list[i].Port = Convert.ToInt32(reader["Port"]);
                                break;
                            }
                        }
                    }
                    if (reader != null)
                        reader.Close();
                    for (int i = 0; i < list.Count; i++)
                    {
                        sql = "select * from RobotPoints where Code = '" + list[i].Code + "'";
                        SqlCommand c = new SqlCommand(sql, CONN);
                        SqlDataReader r = c.ExecuteReader();
                        c.Dispose();
                        if (r != null && r.HasRows)
                        {
                            int j = 0;
                            while (r.Read())
                            {
                                list[i].Points[j].Code = list[i].Code;
                                list[i].Points[j].PtName = reader["PtName"].ToString();
                                list[i].Points[j].J1 = Convert.ToDouble(reader["J1"]);
                                list[i].Points[j].J2 = Convert.ToDouble(reader["J2"]);
                                list[i].Points[j].J3 = Convert.ToDouble(reader["J3"]);
                                list[i].Points[j].J4 = Convert.ToDouble(reader["J4"]);
                                list[i].Points[j].J5 = Convert.ToDouble(reader["J5"]);
                                list[i].Points[j].J6 = Convert.ToDouble(reader["J6"]);
                                list[i].Points[j].X = Convert.ToDouble(reader["X"]);
                                list[i].Points[j].Y = Convert.ToDouble(reader["Y"]);
                                list[i].Points[j].Z = Convert.ToDouble(reader["Z"]);
                                list[i].Points[j].RX = Convert.ToDouble(reader["RX"]);
                                list[i].Points[j].RY = Convert.ToDouble(reader["RY"]);
                                list[i].Points[j].RZ = Convert.ToDouble(reader["RZ"]);
                                j++;
                            }
                        }
                        if (r != null)
                            r.Close();
                    }
                    
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            com.Dispose();
            return result;
        }

        public bool SetRobotStatus(string code, RobotStatus status)
        {
            bool result = false;
            string sql = "update Robots set status = " + (int)status + " where Code = '" + code + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                result = true;
            }
            return result;
        }

        public bool SetRobotStatusAndProcess(string code, RobotStatus status, int process)
        {
            bool result = false;
            string sql = "update Robots set status = " + (int)status + ", Process = " + process + " where Code = '" + code + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                result = true;
            }
            return result;
        }

        public bool SetRobotIndexs(string code, int devIndex, int carrIndex)
        {
            bool result = false;
            string sql = "update Robots set DeviceIndex = " + devIndex + ", CarrierIndex = " + carrIndex + " where Code = '" + code + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                result = true;
            }
            return result;
        }

        public bool SetRobotArmStatus(string code, int state)
        {
            bool result = false;
            string sql = "update Robots set Arm = " + state + " where Code = '" + code + "'";
            try
            {
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

        public bool SendRobotForCarrier(string carrCode, int coord, out string robotCode)
        {
            bool result = false;
            robotCode = null;
            try
            {
                string sql = "select Robot_" + coord + " from Carriers where Robot_" + coord + " <> '' and Robot_" + coord + " <> null and Code = '" + carrCode + "'";
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    object rob = com.ExecuteScalar();
                    com.Dispose();
                    if (rob != null)
                    {
                        robotCode = rob.ToString();
                    }
                    else
                    {
                        sql = "select top 1 Code from robots where Status = " + RobotStatus.Standby + " and Battery > " + Robot.LOW_POWER + " and Buffer1 = 0 order by Battery desc";
                        com = new SqlCommand(sql, CONN);
                        rob = com.ExecuteScalar();
                        com.Dispose();
                        if (rob != null)
                        {
                            robotCode = rob.ToString();
                            sql = "update Carriers set Robot_" + coord + " = '" + robotCode + "' where Code = '" + carrCode + "'";
                            com = new SqlCommand(sql, CONN);
                            com.ExecuteNonQuery();
                            sql = "select top 1 TargetDeviceIndex from Materials where CarrierCode = '" + carrCode + "' ";
                            if (coord == 1)
                            {
                                sql += "and CarrierIndex = 18";
                            }
                            else if (coord == 2)
                            {
                                sql += "and CarrierIndex = 0";
                            }
                            rob = com.ExecuteScalar();
                            com.Dispose();
                            if (rob != null)
                            {
                                int deviceIndex = Convert.ToInt32(rob);
                                int carrierIndex = coord == 1 ? 18 : 0;
                                sql = "update Robots set Status = " + RobotStatus.Moving + ", Coordination = " + coord + ", CarrierIndex = " + carrierIndex + 
                                    ", DeviceIndex = " + deviceIndex + ", Process = 0 where Code = '" + robotCode + "'";
                                com = new SqlCommand(sql, CONN);
                                com.ExecuteNonQuery();
                                result = true;
                            }
                        }
                        else
                        {
                            robotCode = string.Empty;
                        }
                    }
                }
            }
            catch (Exception)
            {
                robotCode = null;
            }
            return result;
        }

        public int GetRobotTargetPosition(string robotCode, int coord)
        {
            int position = -1;
            try
            {
                string sql = "select top 1 TargetPosition from Carriers where Robot_" + coord + " = '" + robotCode + "'";
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    object pos = com.ExecuteScalar();
                    com.Dispose();
                    if (pos != null)
                    {
                        position = Convert.ToInt32(pos) + 1;
                    }
                    else
                    {
                        position = -1;
                    }
                }
            }
            catch (Exception)
            {
                position = -1;
            }
            return position;
        }

        public bool SavePawPosition(string robotCode, string name, RobotPoint pos)
        {
            bool result = false;
            string sql = "select count(*) from robotpoints where Code = '" + robotCode + "' and PtName = '" + name + "'";
            try
            {
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    object count = com.ExecuteScalar();
                    com.Dispose();
                    if (count != null)
                    {
                        if (Convert.ToInt32(count) > 0)
                        {
                            sql = "update RobotPoints set J1 = 0, J2 = 0, J3 = 0, J4 = 0, J5 = 0, J6 = 0, X = 0, Y = 0, Z = 0, RX = 0, RY = 0, RZ = 0 where Code = '' and PtName = ''";
                        }
                        else
                        {
                            sql = "insert into RobotPoints(Code, PtName, J1, J2, J3, J4, J5, J6, X, Y, Z, RX, RY, RZ) values('','',0,0,0,0,0,0,0,0,0,0,0,0)";
                        }
                    }
                    result = true;
                }
            }
            catch (Exception)
            {
            }
            return result;
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

        public bool AssignMaterialsTargetOnCarrier(string carrCode)
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

        public bool AssignMaterialsToRobot(string carrCode, int carrIndex, string robotCode)
        {
            bool res = false;
            string sql = "update Materials set Status = " + (int)MaterialStatus.Robot + ", RobotCode = '" + robotCode +
                "' where CarrierIndex = " + carrIndex + " and CarrierCode = '" + carrCode + "' and Status = " + (int)MaterialStatus.Carrier;
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                sql = "update Robots set Arm = 2 where Code = '" + robotCode + "'";
                com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }

        public bool AssignEmptyMaterialsFromDeviceToRobot(int devCode, int devArea, int devIndex, string robotCode)
        {
            bool res = false;
            string sql = "update Requests set Status = 0 where DeviceCode = " + devCode + " and DeviceArea = " + devArea + " and DeviceIndex = " + devIndex;
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                sql = "update Robots set Arm = 1 where Code = '" + robotCode + "'";
                com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }

        public bool AssignEmptyMaterialsFromBufferToRobot(string robotCode)
        {
            bool res = false;
            string sql = "update Robots set Arm = 1, Buffer1 = 0 where Code = '" + robotCode + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }

        public bool AssignEmptyMaterialsFromRobotToBuffer(string robotCode)
        {
            bool res = false;
            string sql = "update Robots set Arm = 0, Buffer1 = 1 where Code = '" + robotCode + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }

        public bool AssignEmptyMaterialsFromRobotToCarrier(string robotCode, string carrCode, int carrIndex)
        {
            bool res = false;
            string sql = "update Robots set Arm = 0 where Code = '" + robotCode + "'";
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                sql = "update Materials set Staff = 1 where CarrierCode = '" + carrCode + "' and CarrierIndex = " + carrIndex;
                com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }

        public bool AssignMaterialsToDevice(string robotCode, int code, int area, int index)
        {
            bool res = false;
            string sql = "update Materials set Status = " + (int)MaterialStatus.Complete + ", Staff = 2 where RobotCode = '" + 
                robotCode + "' and Status = " + (int)MaterialStatus.Robot;
            if (CONN.State == System.Data.ConnectionState.Open)
            {
                SqlCommand com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                sql = "update Requests set RequestCode = 0, Status = 2, LastResponseTime = GETDATE() where DeviceCode = " + 
                    code + " and DeviceArea = " + area + " and DeviceIndex = " + index;
                com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                sql = "update Robots set Arm = 0 where Code = '" + robotCode + "'";
                com = new SqlCommand(sql, CONN);
                com.ExecuteNonQuery();
                com.Dispose();
                res = true;
            }
            return res;
        }

        public int GetFirstMaterialOnCarrier(string robotCode, out string carrCode, out int targetCode, out int targetArea, out int targetIndex)
        {
            int carrierIndex = -1;
            targetCode = targetArea = targetIndex = -1;
            carrCode = null;
            string sql = "select Coordination from robots where Code = '" + robotCode + "'";
            try
            {
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    object coord = com.ExecuteScalar();
                    com.Dispose();
                    if (coord != null)
                    {
                        sql = "select CarrierCode,CarrierIndex,TargetDeviceCode,TargetDeviceArea,TargetDeviceIndex from Materials where CarrierCode = (select Code from Carriers where Robot_"
                            + coord + " = '" + robotCode + "') and Status = " + (int)MaterialStatus.Carrier;
                        com = new SqlCommand(sql, CONN);
                        SqlDataReader reader = com.ExecuteReader();
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                carrCode = reader["CarrierCode"].ToString();
                                carrierIndex = Convert.ToInt32(reader["CarrierIndex"]);
                                targetCode = Convert.ToInt32(reader["TargetDeviceCode"]);
                                targetArea = Convert.ToInt32(reader["TargetDeviceArea"]);
                                targetIndex = Convert.ToInt32(reader["TargetDeviceIndex"]);
                                break;
                            }
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception)
            {
            }
            return carrierIndex;
        }

        public bool ReassignMaterials(int oldDevCode, int oldDevArea, int oldDevIndex, string carrCode, int carrIndex)
        {
            bool res = false;
            string sql = "update Requests set RequestCode = 0, Status = 2, LastResponseTime = GETDATE() where DeviceCode = " 
                + oldDevCode + " and DeviceArea = " + oldDevArea + " and DeviceIndex = " + oldDevIndex;
            try
            {
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    com.ExecuteNonQuery();
                    com.Dispose();
                    sql = "select top 1 * from Requests where RequestCode = 1 and Status <> 2 order by DeviceCode asc, DeviceArea asc, DeviceIndex asc";
                    com = new SqlCommand(sql, CONN);
                    SqlDataReader reader = com.ExecuteReader();
                    int devCode = -1, devArea = -1, devIndex = -1;
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            devCode = Convert.ToInt32(reader["DeviceCode"]);
                            devArea = Convert.ToInt32(reader["DeviceArea"]);
                            devIndex = Convert.ToInt32(reader["DeviceIndex"]);
                        }
                    }
                    reader.Close();
                    if (devCode != -1 && devArea != -1 && devIndex != -1)
                    {
                        sql = "update Materials set TargetDeviceCode = " + devCode + ", TargetDeviceArea = " + devArea + ", TargetDeviceIndex = " + devIndex +
                            " where CarrierCode = '" + carrCode + "' and CarrierIndex = " + carrIndex + " and Status = " + (int)MaterialStatus.Carrier + " and Staff = 2";
                        com = new SqlCommand(sql, CONN);
                        com.ExecuteNonQuery();
                        com.Dispose();
                        res = true;
                    }
                }
            }
            catch (Exception)
            {
                res = false;
            }
            return res;
        }

        public int GetFirstNullStaffOnCarrier(string robotCode, int coordinate, out string carrCode)
        {
            int pos = -1;
            carrCode = null;
            try
            {
                int min = coordinate == 1 ? 0 : 18;
                int max = coordinate == 1 ? 17 : 35;
                string sql = "select top 1 carrierIndex,CarrierCode from Materials where CarrierIndex >= " + min + " and CarrierIndex <= " + max +
                    " and Staff = 0 and CarrierCode = (select Code from Carriers where Robot_" + coordinate + " = '" + robotCode + "') order by CarrierIndex asc";
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    SqlDataReader reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            pos = Convert.ToInt32(reader["carrierIndex"]);
                            carrCode = reader["CarrierCode"].ToString();
                        }
                    }
                    reader.Close();
                    com.Dispose();
                }
            }
            catch (Exception)
            {
                pos = -1;
                carrCode = null;
            }
            return pos;
        }

        public bool GetMaterialTargetOnRobot(string robotCode, out int devCode, out int devArea, out int devIndex)
        {
            bool result = false;
            devCode = devArea = devIndex = -1;
            try
            {
                string sql = "select * from Materials where RobotCode = '" + robotCode + "' and Status = " + (int)MaterialStatus.Robot;
                if (CONN.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(sql, CONN);
                    SqlDataReader reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            devCode = Convert.ToInt32(reader["TargetDeviceCode"]);
                            devArea = Convert.ToInt32(reader["TargetDeviceArea"]);
                            devIndex = Convert.ToInt32(reader["TargetDeviceIndex"]);
                            result = true;
                            break;
                        }
                    }
                    reader.Close();
                    com.Dispose();
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
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
