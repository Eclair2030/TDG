using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    public class FmsAction
    {
        public FmsAction(SendMessage sm)
        {
            CONTENT_TYPE = "application/json;charset=UTF-8";
            USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.5060.134 Safari/537.36 Edg/103.0.1264.77";
            TOKEN = "YWRtaW4sMTk3NDg2OTYzMDc2OSw3ODBjOGI5Mzk2YzgxMWVjMDVmODQ0YmQ0YjE1ZDA0Zg==";
            MOVE_URL = "http://192.168.30.101:8088/api/v2/orders";
            INFO_URL = "http://192.168.30.101:8088/api/v2/vehicles/";
            SYS_STATE_NAME = "sys_state";
            SYS_STATION_NAME = "cur_station_no";
            SYS_BATTERY = "battery";
            Message = sm;
            MAP_ID = 22;
            MOVING = false;
        }

        public FmsActionResult AgvMove(string agvCode, int position)
        {
            FmsActionResult result = FmsActionResult.Default;
            if (GetAgvInfo(agvCode) == AgvState.IDLE.ToString())
            {
                int cu = GetAgvStation(agvCode);
                Message("Agv: " + agvCode + " current station no.:" + cu, MessageType.Default);
                if (cu != position)
                {
                    lock (OBJ)
                    {
                        if (!MOVING)
                        {
                            MOVING = true;
                        }
                    }
                    
                    string resquest = MakeCarrierMoveRequestPostBody(agvCode, position);
                    Stream resStream = ReceivePostResponseStream(resquest, MOVE_URL);
                    if (resStream != null)
                    {
                        result = FmsActionResult.Executing;
                        Message("Agv: " + agvCode + " start to move to position: " + position, MessageType.Default);
                    }
                    else
                    {
                        result = FmsActionResult.Exceptions;
                        Message("Agv: " + agvCode + " move to position: " + position + " fail", MessageType.Error);
                    }
                }
                else
                {
                    result = FmsActionResult.Success;
                    lock (OBJ)
                    {
                        MOVING = false;
                    }
                    
                    Message("Agv: " + agvCode + " has reached position: " + position, MessageType.Default);
                }
            }
            else
            {
                //Message("Agv: " + agvCode + " is moving right now", MessageType.Default);     V1234907695 755777478
                result = FmsActionResult.Busy;
            }
            return result;
        }

        public string GetAllAgvInfo(string agvCode)
        {
            return GetResponse(agvCode, INFO_URL);
        }

        public string GetAgvInfo(string agvCode)
        {
            return GetAgvInfoByName(GetResponse(agvCode, INFO_URL), SYS_STATE_NAME);
            //return GetResponse(agvCode, INFO_URL);
        }

        public int GetAgvStation(string agvCode)
        {
            int pos;
            int.TryParse(GetAgvInfoByName(GetResponse(agvCode, INFO_URL), SYS_STATION_NAME), out pos);
            return pos;
        }

        public float GetAgvBattery(string agvCode)
        {
            float bat;
            float.TryParse(GetAgvInfoByName(GetResponse(agvCode, INFO_URL), SYS_BATTERY), out bat);
            return bat;
        }

        private string MakeCarrierMoveRequestPostBody(string agvCode, int dest)
        {
            string requestString = "{\"appoint_vehicle_id\" : " + agvCode + "," +
                "\"mission\" : [ {" +
                "\"type\" : \"move\",\"destination\" : " + dest + ",\"map_id\" : " + MAP_ID + "," +
                "\"action_name\" : \"\",\"action_id\" : 0,\"action_param1\" : 0,\"action_param2\" : 0 } ]," +
                "\"priority\" : 0,\"user_id\" : 1}";
            
            return requestString;
        }

        private Stream ReceivePostResponseStream(string data, string url)
        {
            Stream resStream = null;
            byte[] body = Encoding.UTF8.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.KeepAlive = true;
            request.Host = "192.168.30.101:8088";
            request.ContentType = CONTENT_TYPE;
            request.ContentLength = body.Length;
            request.Headers.Add("token", TOKEN);
            request.UserAgent = USER_AGENT;
            try
            {
                Stream st = request.GetRequestStream();
                st.Write(body, 0, body.Length);
                st.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Message("Response status code: " + response.StatusCode, MessageType.Result);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    resStream = response.GetResponseStream();
                }
                response.Close();
            }
            catch (Exception ex)
            {
                Message(ex.Message, MessageType.Error);
            }
            return resStream;
        }

        private string GetResponse(string agvCode, string url)
        {
            string resStream = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + agvCode);
            request.Method = "GET";
            request.KeepAlive = true;
            request.Host = "192.168.30.101:8088";
            request.ContentType = CONTENT_TYPE;
            request.Headers.Add("token", TOKEN);
            request.UserAgent = USER_AGENT;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Message("Get Agv Info Response status code: " + response.StatusCode, MessageType.Result);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream st = response.GetResponseStream();
                    StreamReader sr = new StreamReader(st, Encoding.UTF8);
                    resStream = sr.ReadToEnd();
                    st.Close();
                }
                response.Close();
            }
            catch (Exception ex)
            {
                Message(ex.Message, MessageType.Error);
            }
            return resStream;
        }

        private string GetAgvInfoByName(string stream, string name)
        {
            string status = string.Empty;
            try
            {
                if (stream != null)
                {
                    string[] strAry = stream.Replace("{", "").Replace("}", "").Split(',');
                    for (int i = 0; i < strAry.Length; i++)
                    {
                        string[] pare = strAry[i].Replace("\"", "").Split(':');
                        if (pare[0] == name)
                        {
                            status = pare[1];
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                status = null;
            }
            return status;
        }

        private string CONTENT_TYPE, USER_AGENT, TOKEN, MOVE_URL, INFO_URL, SYS_STATE_NAME, SYS_STATION_NAME, SYS_BATTERY;
        private int MAP_ID;
        private bool MOVING;
        private static object OBJ = new object();
        public SendMessage Message;
    }

    public enum FmsActionResult
    {
        Default = 0,
        Success = 1,
        Fail = 2,
        Exceptions = 3,
        Busy = 4,
        Executing = 5,
    }

    public enum FmsCarrierPosition
    {
        None = 0,
        ChargeQueue = 1,
        ChargePre = 2,
        Charge = 3,
        SupplyQueue = 21,
        SupplyPre = 22,
        Supply = 23,
        RetriveQueue = 31,
        RetrivePre = 32,
        Retrive = 33,
        Idle = 41,
        Dev0 = 51,
    }

    public enum AgvState
    {
        IDLE = 0,
        EXECUTING = 1,
    }
}
