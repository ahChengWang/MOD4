using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class ProdPerfDetailEntity
    {
        public int WorkCtr { get; set; }
        public string EquipNo { get; set; }
        public int ProdId { get; set; }
        public string ProdDesc { get; set; }
        public DateTime TransDate { get; set; }
        
        public string TransDateStr 
        {
            get
            {
                return this.TransDate.ToString("HH:mm:ss");
            }
        }
        public string Operator { get; set; }
        public int TimeTarget { get; set; }
        public decimal? TackTime { get; set; }
                
        public string TackTimeStr 
        {
            get
            {
                return TackTime?.ToString("0") ?? "--"; ;
            }
        }
        public TTWarningLevelEnum TTWarningLevelId { get; set; }
    }
}
