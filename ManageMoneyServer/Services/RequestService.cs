using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ManageMoneyServer.Services
{
    public class RequestService
    {
        private async Task<JToken> Request(string url, string method = "GET", byte[] data = null)
        {
            byte[] bytes = null;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Accept, "*/*");
                client.Encoding = Encoding.UTF8;
                switch(method.ToUpper())
                {
                    case "POST":
                        bytes = await client.UploadDataTaskAsync(url, method, data);
                        break;
                    case "GET":
                        bytes = await client.DownloadDataTaskAsync(url);
                        break;
                }
                string result = Encoding.UTF8.GetString(bytes);

                if (!string.IsNullOrEmpty(result))
                {
                    return JToken.Parse(result);
                }
            }

            return null;
        }
        public async Task<JToken> Get(string url, Dictionary<string, object> @params = null)
        {
            StringBuilder queryParam = new StringBuilder();

            if (@params != null && @params.Count > 0)
            {
                string splitter = "?";
                foreach (KeyValuePair<string, object> param in @params)
                {
                    queryParam.Append(splitter + param.Key + "=" + param.Value);
                    splitter = "&";
                }
            }

            return await Request(url.TrimEnd('/') + queryParam.ToString());
        }
    }
}
