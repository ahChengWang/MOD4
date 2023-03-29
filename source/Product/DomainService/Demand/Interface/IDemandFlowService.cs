using MOD4.Web.DomainService.Entity;

namespace MOD4.Web.DomainService
{
    public interface IDemandFlowService
    {
        (bool, string) DoRejectFlow(DemandFlowEntity flowDataEntity);

        (bool, string) DoPendingFlow(DemandFlowEntity flowDataEntity);

        (bool, string) DoProcessingFlow(DemandFlowEntity flowDataEntity);

        (bool, string) DoVerifyFlow(DemandFlowEntity flowDataEntity);

        (bool, string) DoCompletedFlow(DemandFlowEntity flowDataEntity);

        (bool, string) DoCancelFlow(DemandFlowEntity flowDataEntity);
    }
}
