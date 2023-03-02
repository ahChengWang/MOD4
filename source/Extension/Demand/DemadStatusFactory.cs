using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Extension.Interface;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Extension.Demand
{
    /// <summary>
    /// status factory
    /// <returns>狀態對應 flow</returns>
    /// </summary>
    public class DemadStatusFactory : IDemadStatusFactory
    {
        private readonly Dictionary<DemandStatusEnum, Func<DemandFlowEntity, (bool, string)>> dicDemandFlow = new Dictionary<DemandStatusEnum, Func<DemandFlowEntity, (bool, string)>>();
        //private readonly IDemandFlowService _demandFlowService;

        public DemadStatusFactory(IDemandFlowService demandFlowService)
        {
            dicDemandFlow.Add(DemandStatusEnum.Rejected, demandFlowService.DoRejectFlow);
            dicDemandFlow.Add(DemandStatusEnum.Pending, demandFlowService.DoPendingFlow);
            dicDemandFlow.Add(DemandStatusEnum.Processing, demandFlowService.DoProcessingFlow);
            dicDemandFlow.Add(DemandStatusEnum.Verify, demandFlowService.DoVerifyFlow);
            dicDemandFlow.Add(DemandStatusEnum.Completed, demandFlowService.DoCompletedFlow);
            dicDemandFlow.Add(DemandStatusEnum.Cancel, demandFlowService.DoCancelFlow);
        }

        public Func<DemandFlowEntity, (bool, string)> GetFlow(DemandStatusEnum statusId)
        => dicDemandFlow[statusId];
    }
}
