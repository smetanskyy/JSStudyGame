using JSStudyGame.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JSStudyGame.Helpers
{
    public abstract class ServerWorker
    {
        /// <summary>
        /// Add new player to server and return id of player
        /// return 0 if error; if -1 login already use; if -2 email already in use; -3 both
        /// </summary>
        /// <param name="player"> new player </param>
        /// <param name="url"> uniform resource locator</param>
        /// <returns></returns>
        public static int SendPlayerToServer(object player, string url)
        {
            if (player == null)
                return 0;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(player);
                streamWriter.Write(json);
            }
            try
            {
                String resultStr = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    resultStr = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }
                return JsonConvert.DeserializeObject<int>(resultStr);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Get special type of player from server
        /// </summary>
        /// <typeparam name="PlayerType"></typeparam>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public static PlayerType GetInfoFromServer<PlayerType>(string requestUrl)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
            request.Method = "GET";
            String resultStr = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                resultStr = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }
            PlayerType player = JsonConvert.DeserializeObject<PlayerType>(resultStr);
            return player;
        }

        public static int ChangePlayerToServer(object player, string url)
        {
            if (player == null)
                return 0;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(player);
                streamWriter.Write(json);
            }
            try
            {
                String resultStr = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    resultStr = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }
                return JsonConvert.DeserializeObject<int>(resultStr);
            }
            catch
            {
                return 0;
            }
        }

        public static bool DeletePlayer(string requestUrl)
        {
            bool result = false;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
                request.Method = "DELETE";
                String resultStr = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    resultStr = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }
                result = JsonConvert.DeserializeObject<bool>(resultStr);
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }
    }
}
