using System;

namespace MOD4.Web.Repostory.Dao
{
    public class MTDScheduleUpdateHistoryDao
    {
        public string FileName { get; set; }
        public int Floor { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
