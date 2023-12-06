using MOD4.Web.DomainService.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MOD4.Web
{
    public class INXReportService
    {
        private static string _rpt106Url = "http://ptnreportapi.innolux.com/SQLAgent/ApiWorkMulti";

        public INXReportService()
        {
                
        }

        public BaseINXRptEntity<INXRpt106Entity> Get106NewReport(DateTime startDate, DateTime endDate, string shift, string floor, List<string> prodList)
        {
            string _prodStr = string.Join("','", prodList);

            string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_OperationPerformance','FactoryType':'CARUX','FacId':'A','Building':'A','DateFrom':'{startDate:yyyy-MM-dd}','DateTo':'{endDate:yyyy-MM-dd}','Shift':'{shift.ToUpper()}','Floor':'{floor}','WorkOrder':'','LcmProductType':'ALL','Size':'ALL','BigProduct':'ALL','LcmOwner':'','LcdGrade':'','Product':'ALL','ProdId':'','Reworktype':'ALL','floor':'ALL','OptionProduct':'','prod_nbr':'','Input_Prod_nbr':\"{_prodStr}\",'owner_code':'TYPE-PROD'}}]";

            string _result = "";

            var data = new StringContent(_qStr, Encoding.UTF8, "text/plain");
            data.Headers.Add("Reporttoken", "VE5VSTIyMDA4MTYzMjAyMy0wNS0xMA==");

            using (var client = new HttpClient())
            {
                var response = client.PostAsync($"{_rpt106Url}?" + _qStr, data);

                _result = response.Result.Content.ReadAsStringAsync().Result;
            }

            return JsonConvert.DeserializeObject<BaseINXRptEntity<INXRpt106Entity>>(_result);
        }
    }
}
