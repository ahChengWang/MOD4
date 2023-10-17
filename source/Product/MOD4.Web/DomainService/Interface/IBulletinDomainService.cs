using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IBulletinDomainService
    {
        List<BulletinEntity> GetBulletinList(UserEntity userInfo, DateTime? startDate = null, DateTime? endDate = null, string readStatus = "");

        List<BulletinEntity> GetBulletinByConditions(UserEntity userInfo, string dateRange, string readStatus);

        BulletinEntity GetBulletinDetail(int bulletinSn);

        string Create(BulletinCreateEntity createEntity, UserEntity userEntity);

        string UpdateDetail(int bulletinSn, UserEntity userEntity);

        Tuple<bool, string, string> Download(int bulletinSn, UserEntity userEntity);

        (bool, string, string) DownloadReadInfoFile(int bulletinSn);
    }
}