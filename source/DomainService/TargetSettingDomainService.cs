using MOD4.Web.DomainService.Entity;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class TargetSettingDomainService : ITargetSettingDomainService
    {
        private readonly ITargetSettingRepository _targetSettingRepository;

        public TargetSettingDomainService(ITargetSettingRepository targetSettingRepository)
        {
            _targetSettingRepository = targetSettingRepository;
        }


        public List<TargetSettingEntity> GetList(int prodSn = 1206,List<string> nodeList = null)
        {
            try
            {
                return _targetSettingRepository.SelectByConditions(prodSn, nodeList).CopyAToB<TargetSettingEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string Update(List<TargetSettingEntity> settingList)
        {
            try
            {
                if (settingList.Any(a => a.Node == ""))
                {
                    return "Node empty.";
                }

                var _updDao = settingList.CopyAToB<TargetSettingDao>();

                _updDao.ForEach(fe =>
                {
                    fe.UpdateTime = DateTime.Now;
                    fe.UpdateUser = "admin";
                });

                string _updRes = "";

                using (var scope = new TransactionScope())
                {
                    var result = _targetSettingRepository.Update(_updDao) == settingList.Count;

                    if (result)
                        scope.Complete();
                    else
                        _updRes = "更新失敗";
                }

                return _updRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
