using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class TakeBackKanbanViewModel
    {
        public string Date { get; set; }

        public string TakeBackDaily { get; set; }

        public string TakeBackDailyPercent { get; set; }

        public List<TakeBackInfoViewModel> DetailList { get; set; }

    }
}
