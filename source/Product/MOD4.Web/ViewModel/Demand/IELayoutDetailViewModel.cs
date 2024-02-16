using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class IELayoutDetailViewModel
    {
        public IELayoutViewModel LayoutOrderInfo { get; set; }
        public List<IELayoutAuditViewModel> AuditHistory { get; set; }
    }
}