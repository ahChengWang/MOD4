using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class TargetSettingRepository : BaseRepository, ITargetSettingRepository
    {

        public List<TargetSettingDao> SelectByConditions(List<int> prodSn, List<int> nodeList)
        {
            string sql = $@"select def.descr, ts.* from Target_Setting ts 
join definition_node_desc def 
on ts.Node = def.eqNo where 1=1 ";

            if (prodSn != null && prodSn.Any())
            {
                sql += " and lcmProdSn in @lcmProdSn ";
            }

            if (nodeList != null && nodeList.Any())
            {
                sql += " and Node in @Node ";
            }

            var dao = _dbHelper.ExecuteQuery<TargetSettingDao>(sql, new
            {
                lcmProdSn = prodSn,
                Node = nodeList
            });

            return dao;
        }

        public int Update(List<TargetSettingDao> updSettingList)
        {
            string sql = @"UPDATE [dbo].[Target_Setting]
   SET [DownEquipment] = @DownEquipment
      ,[isMTDTarget] = @IsMTDTarget
      ,[Time0730] = @Time0730
      ,[Time0830] = @Time0830
      ,[Time0930] = @Time0930
      ,[Time1030] = @Time1030
      ,[Time1130] = @Time1130
      ,[Time1230] = @Time1230
      ,[Time1330] = @Time1330
      ,[Time1430] = @Time1430
      ,[Time1530] = @Time1530
      ,[Time1630] = @Time1630
      ,[Time1730] = @Time1730
      ,[Time1830] = @Time1830
      ,[Time1930] = @Time1930
      ,[Time2030] = @Time2030
      ,[Time2130] = @Time2130
      ,[Time2230] = @Time2230
      ,[Time2330] = @Time2330
      ,[Time0030] = @Time0030
      ,[Time0130] = @Time0130
      ,[Time0230] = @Time0230
      ,[Time0330] = @Time0330
      ,[Time0430] = @Time0430
      ,[Time0530] = @Time0530
      ,[Time0630] = @Time0630
      ,[UpdateUser] = @UpdateUser
      ,[UpdateTime] = @UpdateTime
      ,[TimeTarget] = @TimeTarget
 WHERE [Node] = @Node and [lcmProdSn] = @lcmProdSn; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updSettingList);

            return dao;
        }

        public int Insert(List<TargetSettingDao> insSettingList)
        {
            string sql = @"
INSERT INTO [dbo].[Target_Setting]
           (Node
           ,lcmProdSn
           ,DownEquipment
           ,isMTDTarget
           ,Time0730
           ,Time0830
           ,Time0930
           ,Time1030
           ,Time1130
           ,Time1230
           ,Time1330
           ,Time1430
           ,Time1530
           ,Time1630
           ,Time1730
           ,Time1830
           ,Time1930
           ,Time2030
           ,Time2130
           ,Time2230
           ,Time2330
           ,Time0030
           ,Time0130
           ,Time0230
           ,Time0330
           ,Time0430
           ,Time0530
           ,Time0630
           ,UpdateUser
           ,UpdateTime
           ,TimeTarget)
     VALUES
           (@Node
           ,@lcmProdSn
           ,@DownEquipment
           ,@isMTDTarget
           ,@Time0730
           ,@Time0830
           ,@Time0930
           ,@Time1030
           ,@Time1130
           ,@Time1230
           ,@Time1330
           ,@Time1430
           ,@Time1530
           ,@Time1630
           ,@Time1730
           ,@Time1830
           ,@Time1930
           ,@Time2030
           ,@Time2130
           ,@Time2230
           ,@Time2330
           ,@Time0030
           ,@Time0130
           ,@Time0230
           ,@Time0330
           ,@Time0430
           ,@Time0530
           ,@Time0630
           ,@UpdateUser
           ,@UpdateTime
           ,@TimeTarget); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insSettingList);

            return dao;
        }


        public List<MTDProcessSettingDao> SelectForMTDSetting(List<int> prodList)
        {
            string sql = $@"select defNode.sn,defProd.prodNo,ts.Node,ts.lcmProdSn,ts.DownEquipment as 'DownEq',defNode.process, defNode.descr
from definition_lcm_prod defProd
join Target_Setting ts
on defProd.sn = ts.lcmProdSn
join definition_node_desc defNode
on ts.Node = defNode.eqNo
where ts.isMTDTarget = 1 and defProd.sn IN @ProdSn ";

            var dao = _dbHelper.ExecuteQuery<MTDProcessSettingDao>(sql, new
            {
                ProdSn = prodList
            });

            return dao;
        }

        public List<MTDProcessSettingDao> SelectForUploadMTD(IEnumerable<string> prodNoList, IEnumerable<string> processList)
        {
            string sql = $@"
select defNode.process ,defPod.prodNo, setting.lcmProdSn, setting.Node 
  from Target_Setting setting
  join definition_lcm_prod defPod
    on setting.lcmProdSn = defPod.sn
  join definition_node_desc defNode
    on setting.Node = defNode.eqNo
 where setting.isMTDTarget = 1 and defPod.prodNo in @ProdNo and defNode.process in @Process ;
 ";

            var dao = _dbHelper.ExecuteQuery<MTDProcessSettingDao>(sql, new
            {
                ProdNo = prodNoList,
                Process = processList
            });

            return dao;
        }
    }
}
