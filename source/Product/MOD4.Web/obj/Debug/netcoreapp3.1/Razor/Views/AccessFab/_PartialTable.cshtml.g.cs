#pragma checksum "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "842a3fd1b660cfdcc323f5cfb76712eae5ce3321"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_AccessFab__PartialTable), @"mvc.1.0.view", @"/Views/AccessFab/_PartialTable.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\_ViewImports.cshtml"
using MOD4.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\_ViewImports.cshtml"
using MOD4.Web.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"842a3fd1b660cfdcc323f5cfb76712eae5ce3321", @"/Views/AccessFab/_PartialTable.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6d1efcd6897d9e05e93e9d2e8d3f27b282c15526", @"/Views/_ViewImports.cshtml")]
    public class Views_AccessFab__PartialTable : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<MOD4.Web.ViewModel.AccessFabMainViewModel>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<table id=\"table1\" class=\"table table-bordered table-hover\">\r\n    <thead>\r\n        <tr>\r\n            <th>");
#nullable restore
#line 6 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
           Write(Html.DisplayNameFor(model => model.First().OrderNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th>");
#nullable restore
#line 7 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
           Write(Html.DisplayNameFor(model => model.First().FabInDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th>");
#nullable restore
#line 8 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
           Write(Html.DisplayNameFor(model => model.First().FabInCategory));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n");
            WriteLiteral("            <th>");
#nullable restore
#line 10 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
           Write(Html.DisplayNameFor(model => model.First().Content));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th>");
#nullable restore
#line 11 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
           Write(Html.DisplayNameFor(model => model.First().GustNames));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th>");
#nullable restore
#line 12 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
           Write(Html.DisplayNameFor(model => model.First().AuditAccount));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th>");
#nullable restore
#line 13 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
           Write(Html.DisplayNameFor(model => model.First().Date));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th>");
#nullable restore
#line 14 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
           Write(Html.DisplayNameFor(model => model.First().Applicant));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th>");
#nullable restore
#line 15 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
           Write(Html.DisplayNameFor(model => model.First().Status));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n");
#nullable restore
#line 19 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
          
            foreach (var order in Model)
            {
                string _color = "";
                if (order.StatusId == MOD4.Web.Enum.FabInOutStatusEnum.Rejected)
                    _color = "#e69a9a";


#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr");
            BeginWriteAttribute("style", " style=\"", 1208, "\"", 1242, 3);
            WriteAttributeValue("", 1216, "background-color:", 1216, 17, true);
#nullable restore
#line 26 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
WriteAttributeValue(" ", 1233, _color, 1234, 7, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue(" ", 1241, "", 1242, 1, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                    <td style=\"width:7%\"><a href=\"#\"");
            BeginWriteAttribute("onclick", " onclick=\"", 1298, "\"", 1349, 5);
            WriteAttributeValue("", 1308, "orderNoClick(\'", 1308, 14, true);
#nullable restore
#line 27 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
WriteAttributeValue("", 1322, order.Url, 1322, 10, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1332, "\',", 1332, 2, true);
#nullable restore
#line 27 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
WriteAttributeValue("", 1334, order.OrderSn, 1334, 14, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1348, ")", 1348, 1, true);
            EndWriteAttribute();
            WriteLiteral(">");
#nullable restore
#line 27 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
                                                                                                    Write(order.OrderNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a></td>\r\n                    <td style=\"width:7%;font-weight:600\">");
#nullable restore
#line 28 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
                                                    Write(order.FabInDate);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:7%\">");
#nullable restore
#line 29 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
                                    Write(order.FabInCategory);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n");
            WriteLiteral("                    <td style=\"width:12%\">");
#nullable restore
#line 31 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
                                     Write(order.Content);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:15%\">");
#nullable restore
#line 32 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
                                     Write(order.GustNames);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:8%\">");
#nullable restore
#line 33 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
                                    Write(order.AuditAccount);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:7%;font-weight:600\">");
#nullable restore
#line 34 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
                                                    Write(order.Date);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:7%\">");
#nullable restore
#line 35 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
                                    Write(order.Applicant);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:6%\">");
#nullable restore
#line 36 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
                                    Write(order.Status);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                </tr>\r\n");
#nullable restore
#line 38 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\_PartialTable.cshtml"
            }
        

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n</table>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<MOD4.Web.ViewModel.AccessFabMainViewModel>> Html { get; private set; }
    }
}
#pragma warning restore 1591
