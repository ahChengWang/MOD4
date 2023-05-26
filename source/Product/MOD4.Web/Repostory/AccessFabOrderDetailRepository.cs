using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOD4.Web.Repostory
{
    public class AccessFabOrderDetailRepository : BaseRepository, IAccessFabOrderDetailRepository
    {

        public List<AccessFabOrderDetailDao> SelectList(int accessFabOrderSn = 0, List<int> accessFabSnList = null, string guestName = "")
        {
            string sql = "select * from access_fab_order_detail where 1=1 ";

            if (accessFabOrderSn != 0)
                sql += " and accessFabOrderSn=@AccessFabOrderSn ";
            else if (accessFabSnList != null && accessFabSnList.Any())
                sql += " and accessFabOrderSn in @AccessFabOrderSnList ";

            if (!string.IsNullOrEmpty(guestName))
                sql += $" and Name like '%{guestName}%' ";

            var dao = _dbHelper.ExecuteQuery<AccessFabOrderDetailDao>(sql, new
            {
                AccessFabOrderSn = accessFabOrderSn,
                AccessFabOrderSnList = accessFabSnList,
                Name = guestName
            });

            return dao;
        }


        public int Insert(List<AccessFabOrderDetailDao> insAccessFabOrder)
        {
            string sql = @"INSERT INTO [dbo].[access_fab_order_detail]
([accessFabOrderSn]
,[companyName]
,[guestPhone]
,[name]
,[clotheSize]
,[shoesSize])
VALUES
(@accessFabOrderSn
,@companyName
,@guestPhone
,@name
,@clotheSize
,@shoesSize); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insAccessFabOrder);

            return dao;
        }

        public int Delete(List<int> delSnList)
        {
            string sql = @"Delete [dbo].[access_fab_order_detail] where sn in @sn ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
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
,clotheSize = @clotheSize 
,shoesSize = @shoesSize  
where sn = @sn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, updAccessFabOrder);

            return dao;
        }

    }
}
