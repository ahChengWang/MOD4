#pragma checksum "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c1317c1b889844eb38c669fd12d74f12195ce6b4"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Equipment__PartialEqHisotry), @"mvc.1.0.view", @"/Views/Equipment/_PartialEqHisotry.cshtml")]
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
#nullable restore
#line 2 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
using MOD4.Web.Enum;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c1317c1b889844eb38c669fd12d74f12195ce6b4", @"/Views/Equipment/_PartialEqHisotry.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6d1efcd6897d9e05e93e9d2e8d3f27b282c15526", @"/Views/_ViewImports.cshtml")]
    public class Views_Equipment__PartialEqHisotry : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MOD4.Web.ViewModel.EquipmentViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("eqimg"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/img/icons/pencil-2-128.gif"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n\r\n<table id=\"example2\" class=\"table table-bordered table-hover\">\r\n    <thead>\r\n        <tr>\r\n            <th class=\"myth2\">");
#nullable restore
#line 9 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                         Write(Html.DisplayNameFor(model => model.RepairedEqInfoList.First().MFGDay));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th class=\"myth2\">");
#nullable restore
#line 10 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                         Write(Html.DisplayNameFor(model => model.RepairedEqInfoList.First().ToolId));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th class=\"myth2\">");
#nullable restore
#line 11 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                         Write(Html.DisplayNameFor(model => model.RepairedEqInfoList.First().ToolStatus));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th class=\"myth2\">");
#nullable restore
#line 12 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                         Write(Html.DisplayNameFor(model => model.RepairedEqInfoList.First().StatusCdsc));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th class=\"myth2\">");
#nullable restore
#line 13 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                         Write(Html.DisplayNameFor(model => model.RepairedEqInfoList.First().Comment));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th class=\"myth2\">");
#nullable restore
#line 14 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                         Write(Html.DisplayNameFor(model => model.RepairedEqInfoList.First().LmTime));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th class=\"myth2\">");
#nullable restore
#line 15 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                         Write(Html.DisplayNameFor(model => model.RepairedEqInfoList.First().RepairedTime));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th class=\"myth2\">");
#nullable restore
#line 16 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                         Write(Html.DisplayNameFor(model => model.RepairedEqInfoList.First().UserId));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n            <th></th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n");
#nullable restore
#line 21 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
          
            foreach (var eqinfo in Model.RepairedEqInfoList)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td style=\"width:8%\">");
#nullable restore
#line 25 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                    Write(eqinfo.MFGDay);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:10%\">");
#nullable restore
#line 26 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                     Write(eqinfo.ToolId);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:5%\">");
#nullable restore
#line 27 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                    Write(eqinfo.ToolStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:12%\">");
#nullable restore
#line 28 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                     Write(eqinfo.StatusCdsc);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:25%\">");
#nullable restore
#line 29 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                     Write(eqinfo.Comment);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:10%\">");
#nullable restore
#line 30 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                     Write(eqinfo.LmTime);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:8%\">");
#nullable restore
#line 31 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                    Write(eqinfo.RepairedTime);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:8%\">");
#nullable restore
#line 32 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                    Write(eqinfo.UserId);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td style=\"width:10%\">\r\n");
#nullable restore
#line 34 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                          
                            if (eqinfo.StatusId == MOD4.Web.Enum.EqIssueStatusEnum.PendingPM)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <button name=\"PM\" class=\"btn btn-sm btn-secondary\"");
            BeginWriteAttribute("onclick", " onclick=\"", 2008, "\"", 2069, 5);
            WriteAttributeValue("", 2018, "editClick(", 2018, 10, true);
#nullable restore
#line 37 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
WriteAttributeValue("", 2028, eqinfo.sn, 2028, 10, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 2038, ",", 2038, 1, true);
#nullable restore
#line 37 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
WriteAttributeValue(" ", 2039, (int)eqinfo.StatusId, 2040, 23, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 2063, ",this)", 2063, 6, true);
            EndWriteAttribute();
            WriteLiteral(" style=\"position:relative\">\r\n                                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "c1317c1b889844eb38c669fd12d74f12195ce6b411478", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                                    <span>PM</span>\r\n                                </button>\r\n");
#nullable restore
#line 42 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                                                                                                   
                            }
                            else if (eqinfo.StatusId == MOD4.Web.Enum.EqIssueStatusEnum.PendingENG && ViewBag.RoleId == RoleEnum.Engineer)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <button name=\"Eng\" class=\"btn btn-sm btn-primary\"");
            BeginWriteAttribute("onclick", " onclick=\"", 2905, "\"", 2965, 5);
            WriteAttributeValue("", 2915, "editClick(", 2915, 10, true);
#nullable restore
#line 46 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
WriteAttributeValue("", 2925, eqinfo.sn, 2925, 10, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 2935, ",", 2935, 1, true);
#nullable restore
#line 46 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
WriteAttributeValue("", 2936, (int)eqinfo.StatusId, 2936, 23, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 2959, ",this)", 2959, 6, true);
            EndWriteAttribute();
            WriteLiteral(" style=\"position:relative\">\r\n                                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "c1317c1b889844eb38c669fd12d74f12195ce6b414217", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                                    <span style=\"font-size:5px\">工程師</span>\r\n                                </button>\r\n");
#nullable restore
#line 51 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                                                                                                                                                       
                            }
                            else if (eqinfo.StatusId == MOD4.Web.Enum.EqIssueStatusEnum.PendingENG)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <a class=\"btn btn-sm\" data-toggle=\"modal\" data-target=\"#detailModal\"");
            BeginWriteAttribute("onclick", " onclick=\"", 3766, "\"", 3804, 3);
            WriteAttributeValue("", 3776, "detailClick(", 3776, 12, true);
#nullable restore
#line 55 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
WriteAttributeValue("", 3788, eqinfo.sn, 3788, 10, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 3798, ",this)", 3798, 6, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                                    <i class=\"fa fa-search\" style=\"color:darkblue\">工程師確認中</i>\r\n                                </a>\r\n");
#nullable restore
#line 58 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                            }
                            else
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <a class=\"btn btn-sm\" data-toggle=\"modal\" style=\"font-size:15px\" data-target=\"#detailModal\"");
            BeginWriteAttribute("onclick", " onclick=\"", 4160, "\"", 4198, 3);
            WriteAttributeValue("", 4170, "detailClick(", 4170, 12, true);
#nullable restore
#line 61 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
WriteAttributeValue("", 4182, eqinfo.sn, 4182, 10, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 4192, ",this)", 4192, 6, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                                    <i class=\"fa fa-search\">已完成</i>\r\n                                </a>\r\n");
#nullable restore
#line 64 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
                                                                                                                                           
                            }
                        

#line default
#line hidden
#nullable disable
            WriteLiteral("                    </td>\r\n                </tr>\r\n");
#nullable restore
#line 69 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\_PartialEqHisotry.cshtml"
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MOD4.Web.ViewModel.EquipmentViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
