﻿using MOD4.Web.DomainService.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOD4.Web
{
    public class INXReportService : INXReportAbstract
    {
        public INXReportService()
        {

        }

        public async Task<BaseINXRptEntity<T>> Get106NewReportAsync<T>(DateTime startDate, DateTime endDate, string shift, string floor, List<string> prodList)
        {
            string _prodStr = string.Join("','", prodList);

            string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_OperationPerformance','FactoryType':'CARUX','FacId':'A','Building':'A','DateFrom':'{startDate:yyyy-MM-dd}','DateTo':'{endDate:yyyy-MM-dd}','Shift':'{shift.ToUpper()}','Floor':'{floor}','WorkOrder':'','LcmProductType':'ALL','Size':'ALL','BigProduct':'ALL','LcmOwner':'','LcdGrade':'','Product':'ALL','ProdId':'','Reworktype':'ALL','floor':'ALL','OptionProduct':'','prod_nbr':'','Input_Prod_nbr':\"{_prodStr}\",'owner_code':'TYPE-PROD'}}]";

            return JsonConvert.DeserializeObject<BaseINXRptEntity<T>>(await PostAsync(_qStr));
        }

        public async Task<BaseINXRptEntity<T>> Get108NewReportAsync<T>(DateTime startDate, List<string> eqpIdList)
        {
            string _eqNoStr = string.Join("','", eqpIdList);

            string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_EquipmentUtilizationDataSet','FactoryType':'CARUX','G_FAC':'A','vDate_s':'{startDate:yyyy-MM-dd}','vDate_e1':'{startDate:yyyy-MM-dd}','Shift':'ALL','Interval':'equipment','Floor':'','EQPID':\"{_eqNoStr}\",'floor':'ALL','EQSTATUS':'DOWN','EQPGroup':'ALL','optionEquipment':\"{_eqNoStr}\",'#optionequipmenu':''}}]";

            return JsonConvert.DeserializeObject<BaseINXRptEntity<T>>(await PostAsync(_qStr));
        }

        public async Task<BaseINXRptEntity<T>> Get109NewReportAsync<T>(DateTime startDate, List<string> eqpIdList)
        {
            string _eqNoStr = string.Join("','", eqpIdList);

            string _qStr = $"apiJob=[{{'vDate_s':'{startDate:yyyy-MM-dd}','vDate_e1':'{startDate:yyyy-MM-dd}','Shift':'ALL','EQPGroup':'ALL','optionEquipment':\"{_eqNoStr}\",'#optionequipmenu':'','EQPID':\"{_eqNoStr}\",'apiName':'TN_EntityStatusSummaryDataSet','FactoryType':'CARUX','G_FAC':''}}]";

            return JsonConvert.DeserializeObject<BaseINXRptEntity<T>>(await PostAsync(_qStr));
        }

        public async Task<BaseINXRptEntity<T>> GetEntityTTMntReportAsync<T>(DateTime startDate, List<string> prodList)
        {
            string _prodStr = string.Join("','", prodList);

            //string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_EntityTactTimeMaintain','FactoryType':'CARUX','Action':'Query','sDateMonth':'{startDate:yyyyMM}','Size':'ALL','Product':'ALL','OptionProduct':'GDD340IA0090S','GDD340IA0100S','#optionmenu':','prod_nbr':\"GDD340IA0090S','GDD340IA0100S\"}}]";
            string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_EntityTactTimeMaintain','FactoryType':'CARUX','Action':'Query','sDateMonth':'{startDate:yyyyMM}','Size':'ALL','Product':'ALL','OptionProduct':\"{_prodStr}\",'#optionmenu':'','prod_nbr':\"{_prodStr}\"}}]";

            return JsonConvert.DeserializeObject<BaseINXRptEntity<T>>(await PostAsync(_qStr));
        }
    }
}