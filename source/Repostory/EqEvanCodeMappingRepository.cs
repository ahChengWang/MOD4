using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MOD4.Web.Repostory
{
    public class EqEvanCodeMappingRepository : BaseRepository, IEqEvanCodeMappingRepository
    {

        public List<EqEvanCodeMappingDao> SelectList(int typeId = 0, int yId = 0, int subyId = 0, int xId = 0, int subxId = 0, int rId = 0)
        {
            string sql = "select * from eq_evan_code_mapping where 1=1 ";

            if (typeId != 0)
                sql += " and TypeId = @TypeId ";
            if (yId != 0)
                sql += " and YId = @YId ";
            if (subyId != 0)
                sql += " and SubYId = @SubYId ";
            if (xId != 0)
                sql += " and XId = @XId ";
            if (subxId != 0)
                sql += " and SubXId = @SubXId ";
            if (rId != 0)
                sql += " and RId = @RId ";

            var dao = _dbHelper.ExecuteQuery<EqEvanCodeMappingDao>(sql, new
            {
                TypeId = typeId,
                YId = yId,
                SubYId = subyId,
                XId = xId,
                SubXId = subxId,
                rId = rId
            });

            return dao;
        }
    }
}
