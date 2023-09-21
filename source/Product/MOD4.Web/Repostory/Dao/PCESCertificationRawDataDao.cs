using System;

namespace MOD4.Web.Repostory.Dao
{
    public class PCESCertificationRawDataDao
    {
        public string JobId { get; set; }
        public string Name { get; set; }
        public string Shift { get; set; }
        public string MainOperation { get; set; }
        public string Station { get; set; }
        public string StationType { get; set; }
        public string Category { get; set; }
        public string Item { get; set; }
        public string Level { get; set; }
        public string Status { get; set; }
        public DateTime? PassDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int SubjectScore { get; set; }
        public int TrainingScore { get; set; }
    }
}
