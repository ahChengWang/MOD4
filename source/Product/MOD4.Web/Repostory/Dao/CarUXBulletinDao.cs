using System;

namespace MOD4.Web.Repostory.Dao
{
    public class CarUXBulletinDao
    {
        public int SerialNo { get; set; }
        public DateTime Date { get; set; }
        public int AuthorAccountId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string FilePath { get; set; }
        public string TargetSections { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
