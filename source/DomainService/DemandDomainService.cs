using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class DemandDomainService : IDemandDomainService
    {
        private readonly IDemandsRepository _demandsRepository;

        public DemandDomainService(IDemandsRepository demandsRepository)
        {
            _demandsRepository = demandsRepository;
        }

        public List<DemandEntity> GetDemands(string sn = "")
        {
            try
            {
                return _demandsRepository.SelectByConditions(orderSn: sn).Select(s => new DemandEntity
                {
                    OrderSn = s.orderSn,
                    OrderNo = s.orderNo,
                    CategoryId = s.categoryId,
                    StatusId = s.statusId,
                    Subject = s.subject,
                    Content = s.content,
                    Applicant = s.applicant,
                    JobNo = s.jobNo,
                    UploadFiles = s.uploadFiles,
                    CreateUser = s.createUser,
                    CreateTime = s.createTime,
                    UpdateUser = s.updateUser,
                    UpdateTime = s.updateTime
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string InsertDemand(DemandEntity insertEntity, UserEntity userEntity)
        {
            try
            {
                var _nowTime = DateTime.Now;
                var _folder = $"D:\\UploadTest";
                var _fileNameStr = "";
                var _insResponse = "";

                DemandsDao _insDemandsDao = new DemandsDao
                {
                    createUser = userEntity.Name,
                    createTime = _nowTime,
                    updateUser = "",
                    updateTime = _nowTime
                };

                foreach (var file in insertEntity.UploadFileList)
                {
                    if (file.Length > 0)
                    {
                        var path = $@"{_folder}\{file.FileName}";
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        _fileNameStr += file.FileName + ",";
                    }
                }

                // 移除結尾逗號
                if (_fileNameStr.Length > 0)
                    _fileNameStr = _fileNameStr.Remove(_fileNameStr.Length - 1, 1);

                _insDemandsDao.orderNo = $"DE{_nowTime.ToString("yyyyMMddHHmmssff")}";
                _insDemandsDao.categoryId = insertEntity.CategoryId;
                _insDemandsDao.statusId = DemandStatusEnum.Pending;
                _insDemandsDao.subject = insertEntity.Subject;
                _insDemandsDao.content = insertEntity.Content;
                _insDemandsDao.applicant = insertEntity.Applicant;
                _insDemandsDao.jobNo = insertEntity.JobNo;
                _insDemandsDao.uploadFiles = _fileNameStr;

                using (var scope = new TransactionScope())
                {
                    var _insResult = false;

                    _insResult = _demandsRepository.Insert(_insDemandsDao) == 1;

                    if (_insResult)
                    {
                        //CatchHelper.Delete(new string[] { $"Eq_Edit:{editEntity.sn}" });
                        scope.Complete();
                    }
                    else
                        _insResponse = "新增失敗";
                }

                return _insResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
