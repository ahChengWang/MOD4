using MOD4.Web.DomainService.Entity;
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

        /// <summary>
        /// Report [1.06 Operation Performance]
        /// </summary>
        public async Task<BaseINXRptEntity<T>> Get106NewReportAsync<T>(DateTime startDate, DateTime endDate, string shift, string floor, List<string> prodList)
        {
            string _prodStr = string.Join("','", prodList);

            string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_OperationPerformance','FactoryType':'CARUX','FacId':'A','Building':'A','DateFrom':'{startDate:yyyy-MM-dd}','DateTo':'{endDate:yyyy-MM-dd}','Shift':'{shift.ToUpper()}','Floor':'{floor}','WorkOrder':'','LcmProductType':'ALL','Size':'ALL','BigProduct':'ALL','LcmOwner':'','LcdGrade':'','Product':'ALL','ProdId':'','Reworktype':'ALL','floor':'ALL','OptionProduct':'','prod_nbr':'','Input_Prod_nbr':\"{_prodStr}\",'owner_code':'TYPE-PROD'}}]";

            return JsonConvert.DeserializeObject<BaseINXRptEntity<T>>(await PostAsync(_qStr));
        }

        /// <summary>
        /// Report [1.06 Operation Performance] 二階
        /// </summary>
        public async Task<BaseINXRptEntity<T>> Get106NewReportSubAsync<T>(DateTime startDate, DateTime endDate, string shift, int node, string floor, List<string> prodList, bool isProd = true)
        {
            string _prodStr = string.Join("','", prodList);

            string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_OperationPerformanceDetail3','FactoryType':'CARUX','G_FAC':'A','StrSql_ww':\" {(isProd ? "and lcm_owner in ('LCM0','LCME','PRDG','PROD','QTAP','RES0')" : "")} and prod_nbr in ('{_prodStr}') \",'StrSql_w4':'','col0':'','col1':'','row0':'','row1':{node},'row2':'','row3':'','sql_m':\" and acct_date >= '{startDate:yyyy-MM-dd}' and acct_date <='{endDate:yyyy-MM-dd}'\",'sql_m2':\" and trans_date >= '{startDate:yyyy-MM-dd} 07:30:00.000000' and trans_date <= '{endDate.AddDays(1):yyyy-MM-dd} 07:30:00.000000' \",'Sqlbu2':\" and shift_id = '{shift}' \",'vdate_e':'','vdate_s':'','vincludeprod':''}}]";

            return JsonConvert.DeserializeObject<BaseINXRptEntity<T>>(await PostAsync(_qStr));
        }

        /// <summary>
        /// Report [1.08 Equipment Utilization]
        /// </summary>
        public async Task<BaseINXRptEntity<T>> Get108NewReportAsync<T>(DateTime startDate, List<string> eqpIdList)
        {
            string _eqNoStr = string.Join("','", eqpIdList);

            string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_EquipmentUtilizationDataSet','FactoryType':'CARUX','G_FAC':'A','vDate_s':'{startDate:yyyy-MM-dd}','vDate_e1':'{startDate:yyyy-MM-dd}','Shift':'ALL','Interval':'equipment','Floor':'','EQPID':\"{_eqNoStr}\",'floor':'ALL','EQSTATUS':'DOWN','EQPGroup':'ALL','optionEquipment':\"{_eqNoStr}\",'#optionequipmenu':''}}]";

            return JsonConvert.DeserializeObject<BaseINXRptEntity<T>>(await PostAsync(_qStr));
        }

        /// <summary>
        /// Report [1.09 Entity Status Summary]
        /// </summary>
        public async Task<BaseINXRptEntity<T>> Get109NewReportAsync<T>(DateTime startDate, List<string> eqpIdList)
        {
            string _eqNoStr = string.Join("','", eqpIdList);

            string _qStr = $"apiJob=[{{'vDate_s':'{startDate:yyyy-MM-dd}','vDate_e1':'{startDate:yyyy-MM-dd}','Shift':'ALL','EQPGroup':'ALL','optionEquipment':\"{_eqNoStr}\",'#optionequipmenu':'','EQPID':\"{_eqNoStr}\",'apiName':'TN_EntityStatusSummaryDataSet','FactoryType':'CARUX','G_FAC':''}}]";

            return JsonConvert.DeserializeObject<BaseINXRptEntity<T>>(await PostAsync(_qStr));
        }

        /// <summary>
        /// Report [Entity Tact Time Maintain]
        /// </summary>
        public async Task<BaseINXRptEntity<T>> GetEntityTTMntReportAsync<T>(DateTime startDate, List<string> prodList)
        {
            string _prodStr = string.Join("','", prodList);

            //string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_EntityTactTimeMaintain','FactoryType':'CARUX','Action':'Query','sDateMonth':'{startDate:yyyyMM}','Size':'ALL','Product':'ALL','OptionProduct':'GDD340IA0090S','GDD340IA0100S','#optionmenu':','prod_nbr':'GDD340IA0090S','GDD340IA0100S'}}]";
            string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_EntityTactTimeMaintain','FactoryType':'CARUX','Action':'Query','sDateMonth':'{startDate:yyyyMM}','Size':'ALL','Product':'ALL','OptionProduct':\"{_prodStr}\",'#optionmenu':'','prod_nbr':\"{_prodStr}\"}}]";

            return JsonConvert.DeserializeObject<BaseINXRptEntity<T>>(await PostAsync(_qStr));
        }

        /// <summary>
        /// Report [4.18 WO Order Status Report]
        /// </summary>
        public async Task<BaseINXRptEntity<T>> Get418NewReportAsync<T>(DateTime startDate)
        {
            string _qStr = $"apiJob=[{{'name':'Date','apiName':'TN_WOOrderStatusDataSet','FactoryType':'CARUX','QueryRange':'','WorkOrderType':'ALL','isWoDueDay':'N','FromDate_DueDay':'{startDate:yyyy-MM-dd}','ToDate_DueDay':'{startDate:yyyy-MM-dd}','isWoStartDay':'N','FromDate_StartDay':'{startDate:yyyy-MM-dd}','ToDate_StartDay':'{startDate:yyyy-MM-dd}','WorkOrder':'','wo_status':'ALL','mvin_rate':'ALL','mvou_rate':'ALL'}}]";

            return JsonConvert.DeserializeObject<BaseINXRptEntity<T>>(await PostAsync(_qStr));
        }
    }
}
