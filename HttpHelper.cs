using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TestE2EEZNS
{
    public class HttpHelper
    {
        public static string sendHttpPostRequestWithBody(string endpoint, Dictionary<string, dynamic> param, string body, Dictionary<string, string> header)
        {
            HttpClient httpClient = new HttpClient();
            if (header != null)
            {
                foreach (KeyValuePair<string, string> entry in header)
                {
                    httpClient.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                }
            }

            UriBuilder builder = new UriBuilder(endpoint);
            var query = HttpUtility.ParseQueryString(builder.Query);
            if (param != null)
            {
                foreach (KeyValuePair<string, dynamic> entry in param)
                {
                    if (entry.Value is string && !entry.Key.Equals("body"))
                    {
                        query[entry.Key] = entry.Value;
                    }
                }
            }
            builder.Query = query.ToString();

            if (body == null)
            {
                body = "";
            }
            var content = new StringContent(body, Encoding.UTF8, "application/json");
           
            HttpResponseMessage response = httpClient.PostAsync(builder.ToString(), content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

    }
}
