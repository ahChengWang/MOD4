using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;
using System.IO;
using Ionic.Zip;
using Utility.Helper;
using System.Net;
using MOD4.Web.Repostory.Dao;
using MOD4.Web.Repostory;
using System.Transactions;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public class DepartmentDomainService : BaseDomainService, IDepartmentDomainService
    {
        private readonly IDefinitionDepartmentRepository _definitionDepartmentRepository;

        public DepartmentDomainService(IDefinitionDepartmentRepository definitionDepartmentRepository)
        {
            _definitionDepartmentRepository = definitionDepartmentRepository;
        }

        public List<DepartmentEntity> GetDeptSectionList()
        {
            try
            {
                return _definitionDepartmentRepository.SelectByDeptList().CopyAToB<DepartmentEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
