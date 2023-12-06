using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class BaseINXRptEntity<T>
    {
        public INXRptDateEntity<T> Date { get; set; }
    }

    public class INXRptDateEntity<T>
    {
        public INXRptTable<T> Data { get; set; }
        public string SQL01 { get; set; }
        public string sql_m { get; set; }
        public string sql_m1 { get; set; }
        public string sql_m2 { get; set; }
        public string StrSql_w { get; set; }
        public string StrSql_we { get; set; }
        public string StrSql_w1 { get; set; }
        public string StrSql_w2 { get; set; }
        public string StrSql_w2e { get; set; }
        public string StrSql_w3 { get; set; }
        public string StrSql_w4 { get; set; }
        public string StrSql_ww { get; set; }
        public string sql_p { get; set; }
        public string StrSql_f { get; set; }
        public string SQLbu1 { get; set; }
        public string SQLbu2 { get; set; }
    }

    public class INXRptTable<T>
    {
        public List<T> Table { get; set; }
    }
}
