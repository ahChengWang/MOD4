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
            string sql = "select * from menu_info where isActive = 1 and href != '#';";

            var dao = _dbHelper.ExecuteQuery<MenuInfoDao>(sql);

            return dao;
        }

        public List<int> SelectParentMenu(List<int> snList)
        {
            string sql = "select parent_menu_sn from menu_info where sn in @sn group by parent_menu_sn;";

            var dao = _dbHelper.ExecuteQuery<int>(sql, new
            {
                sn = snList
            });

            return dao;
        }
    }
}
