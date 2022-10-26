using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MOD4.Web.DomainService
{
    public class MAppDomainService : IMAppDomainService
    {


        public MAppDomainService()
        {

        }


        public async Task SendMsgToOneAsync(string msg, string charSn)
        {
            try
            {
                string _account = "22008163";
                string _apiKey = "F6AE8F72-A845-0659-C558-1726E4A3E9BB";
                string _chatSn = charSn;

                var _form = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("account",_account),
                    new KeyValuePair<string, string>("api_key",_apiKey),
                    new KeyValuePair<string, string>("chat_sn",_chatSn),
                    new KeyValuePair<string, string>("content_type","1"),
                    new KeyValuePair<string, string>("content","123475967"),
                    new KeyValuePair<string, string>("msg_content",msg),
                });

                //var _data = new StringContent(JsonConvert.SerializeObject(_requestData), Encoding.UTF8, "application/x-www-form-urlencodedn");

                await PostInxMAppChatMessage(_form);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task PostInxMAppChatMessage(FormUrlEncodedContent formData)
        {
            using (var client = new HttpClient())
            {
                await client.PostAsync("http://mapp.innolux.com/teamplus_innolux/API/IMService.ashx?ask=sendChatMessage", formData);
            }
        }

    }
}
