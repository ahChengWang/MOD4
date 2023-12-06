using MOD4.Web.DomainService.Entity;
using System;
using System.Collections.Generic;

namespace MOD4.Web
{
    public interface IINXReportService
    {
        BaseINXRptEntity<INXRpt106Entity> Get106NewReport(DateTime startDate, DateTime endDate, string shift, string floor, List<string> prodList);
    }
}