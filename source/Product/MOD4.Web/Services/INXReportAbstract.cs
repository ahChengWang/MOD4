using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MOD4.Web
{
    public abstract class INXReportAbstract
    {
        private static string _newReportUrl = "http://ptnreportapi.innolux.com/SQLAgent/ApiWorkMulti";

        public async Task<string> PostAsync(string queryStr)
        {
            string _result = "";

            var data = new StringContent(queryStr, Encoding.UTF8, "text/plain");
            data.Headers.Add("Reporttoken", "VE5VSTIyMDA4MTYzMjAyMy0wNS0xMA==");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync($"{_newReportUrl}?" + queryStr, data);

                _result = response.Content.ReadAsStringAsync().Result;
            }

            return _result;
        }
    }
}
