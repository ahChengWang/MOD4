using System;

namespace MOD4.Web.Repostory.Dao
{
    public class ReclaimWTRptDao
    {
        public DateTime mfg_dt { get; set; }
        public string mes_opra_catg_cd { get; set; }
        public string mes_opra_id { get; set; }
        public string equip_id { get; set; }
        public string mes_prod_id { get; set; }
        public string owner_cate_cd { get; set; }
        public decimal pass_board_qty { get; set; }
        public decimal base_man_eq_ratio { get; set; }
        public decimal man_eq_ratio { get; set; }
        public decimal tct_tm_tgt_meas { get; set; }
        public decimal base_tct_tm_ret_meas { get; set; }
        public decimal tct_tm_ret_meas { get; set; }
    }
}
