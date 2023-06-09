using MOD4.Web.DomainService.Entity;
using Utility.Helper;
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
        private readonly IDefinitionNodeDescRepository _definitionNodeDescRepository;
        private readonly ILcmProductRepository _lcmProductRepository;

        public TargetSettingDomainService(ITargetSettingRepository targetSettingRepository,
            IDefinitionNodeDescRepository definitionNodeDescRepository,
            ILcmProductRepository lcmProductRepository)
        {
            _targetSettingRepository = targetSettingRepository;
            _definitionNodeDescRepository = definitionNodeDescRepository;
            _lcmProductRepository = lcmProductRepository;
        }


        public List<TargetSettingEntity> GetList(List<int> prodSn = null, List<int> nodeList = null)
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


        public string Update(int prodSn, List<TargetSettingEntity> settingList, UserEntity userEntity)
        {
            try
            {
                if (settingList.Any(a => a.Node == 0))
                    return "Node empty.";
                if (prodSn == 0)
                    return "no product sn.";

                var _updDao = settingList.CopyAToB<TargetSettingDao>();

                _updDao.ForEach(fe =>
                {
                    fe.lcmProdSn = prodSn;
                    fe.DownEquipment = string.IsNullOrEmpty(fe.DownEquipment) ? "" : fe.DownEquipment;
                    fe.UpdateTime = DateTime.Now;
                    fe.UpdateUser = userEntity.Name;
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

        public List<MTDProcessSettingEntity> GetSettingForMTD(List<int> prodSnList)
        {
            try
            {
                return _targetSettingRepository.SelectForMTDSetting(prodSnList).CopyAToB<MTDProcessSettingEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Migration()
        {
            var _nowTime = DateTime.Now;
            var _nodeList = _definitionNodeDescRepository.SelectByConditions();
            var _prodList = _lcmProductRepository.SelectByConditions();
            List<TargetSettingDao> _targetSettingDao = new List<TargetSettingDao>();

            foreach (var prodInfo in _prodList)
            {
                foreach (var nodeInfo in _nodeList)
                {
                    _targetSettingDao.Add(new TargetSettingDao
                    {
                        Node = nodeInfo.EqNo,
                        lcmProdSn = prodInfo.sn,
                        DownEquipment = "",
                        IsMTDTarget = false,
                        Time0730 = 60,
                        Time0830 = 60,
                        Time0930 = 60,
                        Time1030 = 60,
                        Time1130 = 60,
                        Time1230 = 60,
                        Time1330 = 60,
                        Time1430 = 60,
                        Time1530 = 60,
                        Time1630 = 60,
                        Time1730 = 60,
                        Time1830 = 60,
                        Time1930 = 60,
                        Time2030 = 60,
                        Time2130 = 60,
                        Time2230 = 60,
                        Time2330 = 60,
                        Time0030 = 60,
                        Time0130 = 60,
                        Time0230 = 60,
                        Time0330 = 60,
                        Time0430 = 60,
                        Time0530 = 60,
                        Time0630 = 60,
                        UpdateUser = "admin",
                        UpdateTime = _nowTime,
                        TimeTarget = 50
                    });
                }
            }

            using (TransactionScope _scope = new TransactionScope())
            {
                bool _insRes = false;

                _insRes = _targetSettingRepository.Insert(_targetSettingDao) == _targetSettingDao.Count;

                if (_insRes)
                    _scope.Complete();
            }
        }
    }
}
