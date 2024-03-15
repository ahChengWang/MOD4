using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class TakeBackWTEditViewModel
    {
        public int Sn { get; set; }

        public string WTCategory { get; set; }

        public WTCategoryEnum? WTCategoryId 
        {
            get 
            {
                switch (this.WTCategory.ToLower())
                {
                    case "fronta":
                        return WTCategoryEnum.FrontA;
                    case "frontb":
                        return WTCategoryEnum.FrontB;
                    case "backenda":
                        return WTCategoryEnum.BackendA;
                    case "backendb":
                        return WTCategoryEnum.BackendB;
                    default:
                        return null;
                }
            }
        }

        public string Date { get; set; }

        public List<TakeBackWTProdViewModel> DetailList { get; set; }

        public List<TakeBackAttendanceViewModel> AttendanceList { get; set; }

    }
}
