using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class FmsAction
    {
        public FmsAction(SendMessage sm)
        {
            CONTENT_TYPE = "application/json;charset=UTF-8";
            USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.5060.134 Safari/537.36 Edg/103.0.1264.77";
            TOKEN = "YWRtaW4sMTk3NDg2OTYzMDc2OSw3ODBjOGI5Mzk2YzgxMWVjMDVmODQ0YmQ0YjE1ZDA0Zg==";
            MOVE_URL = "http://192.168.30.101:8088/api/v2/orders";
            INFO_URL = "http://192.168.30.101:8088/api/v2/vehicles/";
            SYS_STATE_NAME = "sys_state";
            Message = sm;
            MAP_ID = 12;
        }

        public FmsActionResult AgvMove(string carrierCode, int position)
        {
            FmsActionResult result = FmsActionResult.Default;
            string resquest = MakeCarrierMoveRequestPostBody(carrierCode, position);
            Stream resStream = ReceivePostResponseStream(resquest, MOVE_URL);
            if (resStream != null)
            {
                result = FmsActionResult.Success;
            }
            else
            {
                result = FmsActionResult.Exceptions;
            }
            return result;
        }

        public string GetAgvInfo(string carrierCode)
        {
            //return GetAgvInfoByName(GetResponse(carrierCode, INFO_URL), SYS_STATE_NAME);
            return GetResponse(carrierCode, INFO_URL);
        }

        private string MakeCarrierMoveRequestPostBody(string carrierCode, int dest)
        {
            string requestString = "{\"appoint_vehicle_id\" : " + carrierCode + "," +
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
                Message("Response status code: " + response.StatusCode, MessageType.Result);
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
                Message("Get Agv Info Response status code: " + response.StatusCode, MessageType.Result);
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
                        if (strAry[i].Contains(name))
                        {
                            status = strAry[i].Substring(strAry[i].IndexOf(':') + 1).Replace("\"", "");
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

        private string CONTENT_TYPE, USER_AGENT, TOKEN, MOVE_URL, INFO_URL, SYS_STATE_NAME;
        private int MAP_ID;
        public SendMessage Message;
    }

    public enum FmsActionResult
    {
        Default = 0,
        Success = 1,
        Fail = 2,
        Exceptions = 3,
    }

    public enum FmsCarrierPosition
    {
        None = 0,
        SupplyLifter = 1,
        SupplyLifterQueue = 2,
        RetriveLifter = 3,
        RetriveLifterQueue = 4,
        Ready = 5,
        Dev1 = 6,
        Dev2 = 7,
        Dev3 = 8,
        ChargeQueue = 9,
        Charge = 10,
    }

    public enum AgvState
    {
        IDLE = 0,
        EXECUTING = 1,
    }
}
