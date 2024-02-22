using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class LightingLogDao
    {
        public int PanelSn { get; set; }
        public LightingCategoryEnum CategoryId { get; set; }
        public string PanelId { get; set; }
        public int StatusId { get; set; }
        public int DefectCatgId { get; set; }
        public string DefectCode { get; set; }
        public DateTime PanelDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int Floor { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }


        public bool Equals(LightingLogDao rwDao)
        {
            if (rwDao == null)
                return false;

            return this.StatusId == rwDao.StatusId && this.DefectCatgId == rwDao.DefectCatgId && this.DefectCode == rwDao.DefectCode;
        }

        public override bool Equals(object obj) => Equals(obj as LightingLogDao);
        public override int GetHashCode() => PanelSn.GetHashCode();
    }
}
