using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class AccessFabOrderDetailRepository : BaseRepository, IAccessFabOrderDetailRepository
    {


        public List<AccessFabOrderDetailDao> SelectList(int accessFabOrderSn)
        {
            string sql = "select * from access_fab_order_detail where accessFabOrderSn=@AccessFabOrderSn ";

            var dao = _dbHelper.ExecuteQuery<AccessFabOrderDetailDao>(sql, new
            {
                AccessFabOrderSn = accessFabOrderSn
            });

            return dao;
        }


        public int Insert(List<AccessFabOrderDetailDao> insAccessFabOrder)
        {
            string sql = @"INSERT INTO [dbo].[access_fab_order_detail]
([accessFabOrderSn]
,[companyName]
,[guestPhone]
,[name])
VALUES
(@accessFabOrderSn
,@companyName
,@guestPhone
,@name); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insAccessFabOrder);

            return dao;
        }

        public int Delete(List<int> delSnList)
        {
            string sql = @"Delete [dbo].[access_fab_order_detail] where sn in @sn ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new {
                sn = delSnList
            });

            return dao;
        }

        public int Update(List<AccessFabOrderDetailDao> updAccessFabOrder)
        {
            string sql = @"Update [dbo].[access_fab_order_detail] set 
 companyName = @companyName
,guestPhone = @guestPhone 
,name = @name 
where sn = @sn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, updAccessFabOrder);

            return dao;
        }

    }
}
