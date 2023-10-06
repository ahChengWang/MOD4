using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;

namespace MOD4.Web.DomainService
{
    public interface IBulletinDomainService
    {
        (bool, string, string) Download();
        (byte[], string) Download(string jobId, ApplyAreaEnum applyAreaId, int itemId, UserEntity userEntity);
        string GetBulletinList(int accountId);
        string Create(BulletinCreateEntity createEntity, UserEntity userEntity);
    }
}