﻿using MOD4.Web.DomainService.Entity;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MOD4.Web.DomainService
{
    public class MAppDomainService : BaseDomainService, IMAppDomainService
    {


        public MAppDomainService()
        {

        }


        public async Task SendMsgToOneAsync(string msg, string charSn)
        {
            try
            {
                string _account = "API_CarUX_Info";
                string _apiKey = "C6CFCF78-204C-096B-3EAC-127E9B3AE598";
                string _chatSn = charSn;

                var _form = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("account",_account),
                    new KeyValuePair<string, string>("api_key",_apiKey),
                    new KeyValuePair<string, string>("chat_sn",_chatSn),
                    new KeyValuePair<string, string>("content_type","1"),
                    //new KeyValuePair<string, string>("content","123475967"),
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

        public async Task SendMsgWithTagAsync(MAppTagUserEntity mapTagEntity)
        {
            try
            {
                var _request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:82/MAppTagUser");
                HttpContent _content = new StringContent(
                    JsonConvert.SerializeObject(mapTagEntity), Encoding.UTF8, "application/json");

                _request.Content = _content;

                using (HttpClient client = new HttpClient())
                {
                    await client.SendAsync(_request);
                }
            }
            catch (Exception ex)
            {
                _logHelper.WriteLog(LogLevel.Error, this.GetType().Name, $"tag user 異常：{ex.Message}");
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
