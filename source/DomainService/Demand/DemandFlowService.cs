using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using MOD4.Web.Repostory;
using System.Collections.Generic;
using System.IO;
using System.Transactions;
using System;
using System.Linq;

namespace MOD4.Web.DomainService.Demand
{
    /// <summary>
    /// 需求單狀態流程
    /// </summary>
    public class DemandFlowService : IDemandFlowService
    {
        private DateTime _nowTime;
        private (bool, string) _updateRes = (false, "");
        private readonly string _url = "http://10.54.215.210/MOD4/Demand";

        /// <summary>
        /// role : manager
        /// scenario : 駁回申請單
        /// </summary>
        /// <param name="flowDataEntity"></param>
        /// <returns></returns>
        public (bool, string) DoRejectFlow(DemandFlowEntity flowDataEntity)
        {
            flowDataEntity.UpdateDemandOrder.rejectReason = flowDataEntity.InEntity.RejectReason ?? "";

            _updateRes = (false, "");

            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = flowDataEntity.DemandsRepository.UpdateToReject(flowDataEntity.UpdateDemandOrder) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    _updateRes = (true, $"需求單:{flowDataEntity.UpdateDemandOrder.orderNo} \n{flowDataEntity.UpdateDemandOrder.statusId.GetDescription()}");
                }
                else
                    _updateRes = (false, "更新失敗");
            }

            // 發送MApp & mail 給申請人
            if (_updateRes.Item1)
            {
                var _createAccInfo = flowDataEntity.AccountDomainService.GetAccountInfoByConditions(null, null, null, flowDataEntity.OldDemandOrder.createUser).FirstOrDefault();
                flowDataEntity.MailService.Send(new MailEntity
                {
                    To = _createAccInfo.Mail,
                    Subject = $"需求申請單 - 剔退通知",
                    Content = "<br /> Dear Sir <br /><br />" +
                    "您有 <a style='text-decoration:underline'>已剔退</a><a style='font-weight:900'> 需求申請單</a>， <br /><br />煩請上系統確認，" +
                   $"需求單連結：<a href='{_url}/Edit?sn={flowDataEntity.OldDemandOrder.orderSn}&orderId={flowDataEntity.OldDemandOrder.orderNo}' target='_blank'>" + flowDataEntity.OldDemandOrder.orderNo + "</a>" +
                    " <br /><br />謝謝"
                });
            }

            return _updateRes;
        }

        /// <summary>
        /// role : user
        /// scenario : 重送申請單
        /// </summary>
        /// <param name="flowDataEntity"></param>
        /// <returns></returns>
        public (bool, string) DoPendingFlow(DemandFlowEntity flowDataEntity)
        {
            _nowTime = DateTime.Now;
            _updateRes = (false, "");

            flowDataEntity.UpdateDemandOrder.categoryId = flowDataEntity.InEntity.CategoryId;
            flowDataEntity.UpdateDemandOrder.subject = flowDataEntity.InEntity.Subject;
            flowDataEntity.UpdateDemandOrder.content = flowDataEntity.InEntity.Content;
            flowDataEntity.UpdateDemandOrder.applicant = flowDataEntity.InEntity.Applicant;
            flowDataEntity.UpdateDemandOrder.jobNo = flowDataEntity.InEntity.JobNo;

            // user 檔案上傳到訂單建立時的目錄
            flowDataEntity.UpdateDemandOrder.uploadFiles = DoUploadFiles(
                flowDataEntity.InEntity.UploadFileList ?? new List<IFormFile>(),
                flowDataEntity.UserInfo.Account,
                flowDataEntity.UploadUrl,
                _nowTime,
                flowDataEntity.OldDemandOrder.createTime);

            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = flowDataEntity.DemandsRepository.UpdateToPending(flowDataEntity.UpdateDemandOrder) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    _updateRes = (true, $"需求單:{flowDataEntity.UpdateDemandOrder.orderNo} \n{flowDataEntity.UpdateDemandOrder.statusId.GetDescription()}");
                }
                else
                    _updateRes = (false, "更新失敗");
            }

            flowDataEntity.MAppDomainService.SendMsgToOneAsync($"新需求申請單  主旨: {flowDataEntity.UpdateDemandOrder.subject}, 申請人: {flowDataEntity.UpdateDemandOrder.applicant}", "253425");
            flowDataEntity.MailService.Send(new MailEntity
            {
                To = "WEITING.GUO@INNOLUX.COM",
                Subject = $"新需求申請單 - 申請人: {flowDataEntity.UpdateDemandOrder.applicant}",
                Content = "<br />Hi <br /><br />" +
                "您有新需求申請單， <br /><br />" +
                $"需求單連結：<a href='{_url}/Edit?sn={flowDataEntity.OldDemandOrder.orderSn}&orderId={flowDataEntity.OldDemandOrder.orderNo}' target='_blank'>" + flowDataEntity.OldDemandOrder.orderNo + "</a>"
            });

            return _updateRes;
        }

        /// <summary>
        /// role : manager
        /// scenario : 處理申請單
        /// </summary>
        /// <param name="flowDataEntity"></param>
        /// <returns></returns>
        public (bool, string) DoProcessingFlow(DemandFlowEntity flowDataEntity)
        {
            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = flowDataEntity.DemandsRepository.UpdateToProcess(flowDataEntity.UpdateDemandOrder) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    return (true, $"需求單:{flowDataEntity.UpdateDemandOrder.orderNo} \n{flowDataEntity.UpdateDemandOrder.statusId.GetDescription()}");
                }
                else
                    return (false, "更新失敗");
            }
        }

        /// <summary>
        /// role : manager
        /// scenario 1 : 申請單完成, 交由 user 確認
        /// scenario 2 : 申請單確認中, manager 更新申請單
        /// </summary>
        /// <param name="flowDataEntity"></param>
        /// <returns></returns>
        public (bool, string) DoVerifyFlow(DemandFlowEntity flowDataEntity)
        {
            _nowTime = DateTime.Now;
            _updateRes = (false, "");

            switch (flowDataEntity.OldDemandOrder.statusId)
            {
                case DemandStatusEnum.Processing:
                    // mgmt 檔案上傳
                    flowDataEntity.UpdateDemandOrder.completeFiles =
                        DoUploadFiles(flowDataEntity.InEntity.UploadFileList ?? new List<IFormFile>(),
                            flowDataEntity.UserInfo.Account,
                            flowDataEntity.UploadUrl,
                            _nowTime,
                            _nowTime);
                    flowDataEntity.UpdateDemandOrder.remark = flowDataEntity.InEntity.Remark;

                    _updateRes = DoUpdateToComplete(flowDataEntity.DemandsRepository, flowDataEntity.UpdateDemandOrder);

                    // 需求單 "驗證"&"完成" 發送 MApp & mail 給相關人員
                    var _createAccInfo = flowDataEntity.AccountDomainService.GetAccountInfoByConditions(null, null, null, flowDataEntity.OldDemandOrder.createUser).FirstOrDefault();

                    if (_createAccInfo == null)
                        return (true, "查無申請人, mail 無法發送");

                    // mail 通知
                    flowDataEntity.MailService.Send(new MailEntity
                    {
                        To = _createAccInfo.Mail,
                        Subject = $"需求申請單 - 待確認通知",
                        Content = "<br /> Dear Sir <br /><br />" +
                        "您有 <a style='text-decoration:underline'>待確認</a><a style='font-weight:900'> 需求申請單</a>， <br /><br />煩請上系統確認，" +
                       $"需求單連結：<a href='{_url}/Edit?sn={flowDataEntity.OldDemandOrder.orderSn}&orderId={flowDataEntity.OldDemandOrder.orderNo}' target='_blank'>" + flowDataEntity.OldDemandOrder.orderNo + "</a>" +
                        " <br /><br />謝謝"
                    });
                    return _updateRes;

                case DemandStatusEnum.Verify:
                    return DoManagerUpdOrder(ref flowDataEntity, _nowTime);
                default:
                    return (false, "需求單流程異常");
            }
        }

        public (bool, string) DoCompletedFlow(DemandFlowEntity flowDataEntity)
        {
            _nowTime = DateTime.Now;
            _updateRes = (false, "");
            AccountInfoEntity _createAccInfo = new AccountInfoEntity();

            switch (flowDataEntity.OldDemandOrder.statusId)
            {
                case DemandStatusEnum.Processing:
                    // mgmt 檔案上傳
                    flowDataEntity.UpdateDemandOrder.completeFiles =
                        DoUploadFiles(flowDataEntity.InEntity.UploadFileList ?? new List<IFormFile>(),
                            flowDataEntity.UserInfo.Account,
                            flowDataEntity.UploadUrl,
                            _nowTime,
                            _nowTime);
                    flowDataEntity.UpdateDemandOrder.remark = flowDataEntity.InEntity.Remark;

                    _updateRes = DoUpdateToComplete(flowDataEntity.DemandsRepository, flowDataEntity.UpdateDemandOrder);

                    return _updateRes;

                case DemandStatusEnum.Completed:

                    return DoManagerUpdOrder(ref flowDataEntity, _nowTime);

                case DemandStatusEnum.Verify:

                    flowDataEntity.UpdateDemandOrder.completeFiles = flowDataEntity.OldDemandOrder.completeFiles;
                    flowDataEntity.UpdateDemandOrder.remark = flowDataEntity.OldDemandOrder.remark;

                    _updateRes = DoUpdateToComplete(flowDataEntity.DemandsRepository, flowDataEntity.UpdateDemandOrder);

                    // 需求單 "驗證"&"完成" 發送 MApp & mail 給相關人員
                    _createAccInfo = flowDataEntity.AccountDomainService.GetAccountInfoByConditions(null, null, null, flowDataEntity.OldDemandOrder.createUser).FirstOrDefault();

                    if (_createAccInfo == null)
                        return (true, "查無申請人, mail 無法發送");

                    // 已完成 MApp 通知
                    flowDataEntity.MAppDomainService.SendMsgToOneAsync($"需求單已完成, 主旨: {flowDataEntity.UpdateDemandOrder.subject} 申請人: {_createAccInfo.Name}", "253425");

                    // mail 通知
                    flowDataEntity.MailService.Send(new MailEntity
                    {
                        To = "WEITING.GUO@INNOLUX.COM",
                        Subject = $"需求申請單 - 已完成通知 申請人:({_createAccInfo.Name})",
                        Content = "<br /> Dear Sir <br /><br />" +
                        "您有 <a style='text-decoration:underline'>已完成</a><a style='font-weight:900'> 需求申請單</a>， <br /><br />煩請上系統確認， " +
                        $"需求單連結：<a href='{_url}/Edit?sn={flowDataEntity.OldDemandOrder.orderSn}&orderId={flowDataEntity.OldDemandOrder.orderNo}' target='_blank'>" + flowDataEntity.OldDemandOrder.orderNo + "</a>" +
                        "<br /><br />謝謝"
                    });

                    return _updateRes;
                default:
                    return (false, "需求單流程異常");
            }
        }

        #region private
        /// <summary>
        /// manager 更新內容
        /// </summary>
        /// <param name="flowEntity"></param>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        private (bool, string) DoManagerUpdOrder(ref DemandFlowEntity flowEntity, DateTime nowTime)
        {
            flowEntity.UpdateDemandOrder.completeFiles = DoUploadFiles(
                flowEntity.InEntity.UploadFileList ?? new List<IFormFile>(),
                flowEntity.UserInfo.Account,
                flowEntity.UploadUrl,
                nowTime,
                nowTime);

            flowEntity.UpdateDemandOrder.remark = flowEntity.InEntity.Remark;

            return DoUpdateToComplete(flowEntity.DemandsRepository, flowEntity.UpdateDemandOrder);
        }

        private (bool, string) DoUpdateToComplete(IDemandsRepository demandsRepository, DemandsDao updateDao)
        {
            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = demandsRepository.UpdateToComplete(updateDao) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    return (true, $"需求單:{updateDao.orderNo} \n{updateDao.statusId.GetDescription()}");
                }
                else
                    return (false, "更新失敗");
            }
        }

        private string DoUploadFiles(List<IFormFile> uploadFiles, string userAcc, string url, DateTime nowTime, DateTime folderTime)
        {
            var _folder = $@"upload\{userAcc}\{folderTime.ToString("yyMMdd")}";
            var _fileNameStr = "";

            if (!Directory.Exists($@"{url}\{_folder}"))
            {
                Directory.CreateDirectory($@"{url}\{_folder}");
            }

            foreach (var file in uploadFiles)
            {
                if (file.Length > 0)
                {
                    var _fileArray = file.FileName.Split(".");

                    string _newFileName = "";

                    for (int i = 0; i < _fileArray.Length; i++)
                    {
                        _newFileName += i == (_fileArray.Length - 2) ? $"{_fileArray[i]}_{nowTime.ToString("ffff")}." : _fileArray[i];
                    }

                    var path = $@"{url}\{_folder}\{_newFileName}";

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    _fileNameStr += _newFileName + ",";
                }
            }

            // 移除結尾逗號
            if (_fileNameStr.Length > 0)
                _fileNameStr = _fileNameStr.Remove(_fileNameStr.Length - 1, 1);

            return _fileNameStr;
        }
        #endregion
    }
}
