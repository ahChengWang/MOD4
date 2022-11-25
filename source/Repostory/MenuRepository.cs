using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class MenuRepository : BaseRepository, IMenuRepository
    {

        public List<MenuDao> SelectByConditions(int sn)
        {
            string sql = "select * from vw_account_menu where sn=@sn";

            var dao = _dbHelper.ExecuteQuery<MenuDao>(sql, new
            {
                sn = sn
            });

            return dao;
        }

        public List<MenuInfoDao> SelectAllMenu()
        {
            string sql = "select * from menu_info where sn not in (3,7,8,15) and href != '#';";

            var dao = _dbHelper.ExecuteQuery<MenuInfoDao>(sql);

            return dao;
        }
    }
}
