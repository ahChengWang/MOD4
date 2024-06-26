﻿using MOD4.Web.Enum;
using Utility.Helper;

namespace MOD4.Web.Repostory.Dao
{
    public class HcmVwEmp01Dao
    {
        // 工號
        public string PERNR { get; set; }
        // 是否在職
        public string STAT2TXT { get; set; }
        // 廠區
        public string PTEXT { get; set; }
        // 職能
        public string PKTXT { get; set; }
        // 職稱ID
        public JobTitleEnum CSHORTID { get; set; }
        // 職稱代號
        public string CSHORT { get; set; }
        // 職稱
        public string CSTEXT { get; set; }
        // 部門代碼
        public string OSHORT { get; set; }
        // 課別
        public string OSTEXT { get; set; }
        // 姓氏
        public string NACHN { get; set; }
        // 名字
        public string VORNA { get; set; }
        // 國別
        public string NATIO { get; set; }
        // 公司郵件
        public string COMID2 { get; set; }
        // 班別
        public string SCHKZ { get; set; }
    }
}
