#pragma checksum "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\AuditDetail.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "21a01bf73c9a66ec347630d8e32df39732a43ec0"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_AccessFab_AuditDetail), @"mvc.1.0.view", @"/Views/AccessFab/AuditDetail.cshtml")]
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
#line 2 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\AuditDetail.cshtml"
using MOD4.Web.ViewModel;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"21a01bf73c9a66ec347630d8e32df39732a43ec0", @"/Views/AccessFab/AuditDetail.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6d1efcd6897d9e05e93e9d2e8d3f27b282c15526", @"/Views/_ViewImports.cshtml")]
    public class Views_AccessFab_AuditDetail : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MOD4.Web.ViewModel.AccessFabDetailPageViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "_PartialDetailForm", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("detailForm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("col-form-label"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.LabelTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 4 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\AuditDetail.cshtml"
  
    ViewBag.Title = "申請單簽核";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"

<style>
    /*.card {
        box-shadow: 1px 2px 2px 2px #999;
        margin-top: 5px;
    }

    .card-header, .card-body, .card-footer {
        background-color: #c3cadb;
    }

    .card-title {
        font-family: ""Microsoft JhengHei"";
        font-weight: 900;
        font-style: italic;
        font-size: 30px;
    }*/

    label[name=""underline""] {
        text-decoration: underline;
    }

    div[name=""historyLine""] {
        display: flex;
    }
</style>

<div class=""content-wrapper"">
    <section class=""content"">
        <div class=""container-fluid"">
            <div class=""row"">
                <div class=""col-12"">
                    <div class=""card"">
                        <div class=""card-header"">
                            <h3 class=""card-title"">申 請 單 簽 核</h3>
                        </div>
                        <!-- /.card-header -->
                        <!-- form start -->

                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "21a01bf73c9a66ec347630d8e32df39732a43ec06025", async() => {
                WriteLiteral("\r\n                            <div class=\"card-body\">\r\n                                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "21a01bf73c9a66ec347630d8e32df39732a43ec06372", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
                __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_0.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 49 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\AuditDetail.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Model = Model;

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("model", __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Model, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 50 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\AuditDetail.cshtml"
                                  
                                    UserPermissionViewModel _test = (UserPermissionViewModel)ViewBag.UserPermission;

                                    if (Convert.ToBoolean(_test.AccountPermission & (int)MOD4.Web.Enum.PermissionEnum.Audit))
                                    {

#line default
#line hidden
#nullable disable
                WriteLiteral(@"                                        <div>
                                            <input class=""btn btn-success"" type=""button"" data-toggle=""modal"" data-target=""#modal-approve"" value=""核准"" />

                                            <input class=""btn btn-danger"" type=""button"" data-toggle=""modal"" data-target=""#modal-reject"" value=""剔退"" />
                                        </div>
");
#nullable restore
#line 60 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\AuditDetail.cshtml"
                                    }
                                

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                        ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                        <div class=""modal fade"" id=""modal-approve"">
                            <div class=""modal-dialog modal-sm"">
                                <div class=""modal-content"">
                                    <div class=""modal-header"" style=""padding: 5px"">
                                        <p>確認核准?</p>
                                        <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                                            <span aria-hidden=""true"">&times;</span>
                                        </button>
                                    </div>
                                    <div class=""modal-body"">
                                        <div>
                                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("label", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "21a01bf73c9a66ec347630d8e32df39732a43ec011174", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.LabelTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
#nullable restore
#line 75 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\AuditDetail.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.Remark);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                                        </div>
                                        <div>
                                            <textarea id=""approveRemark"" cols=""30"" rows=""5""></textarea>
                                        </div>
                                    </div>
                                    <div class=""modal-footer justify-content-between"">
                                        <button type=""button"" class=""btn btn-default"" data-dismiss=""modal"">否</button>
                                        <input type=""button"" class=""btn btn-success"" value=""確認"" onclick=""updateApprove()"" />
                                    </div>
                                </div>
                                <!-- /.modal-content -->
                            </div>
                        </div>
                        <div class=""modal fade"" id=""modal-reject"">
                            <div class=""modal-dialog modal-sm"">
                                <div class=""modal-cont");
            WriteLiteral(@"ent"">
                                    <div class=""modal-header"" style=""padding: 5px"">
                                        <p>確認剔退?</p>
                                        <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                                            <span aria-hidden=""true"">&times;</span>
                                        </button>
                                    </div>
                                    <div class=""modal-body"">
                                        <div>
                                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("label", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "21a01bf73c9a66ec347630d8e32df39732a43ec014401", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.LabelTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
#nullable restore
#line 100 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\AuditDetail.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.Remark);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                                        </div>
                                        <div>
                                            <textarea id=""rejectRemark"" cols=""30"" rows=""5""></textarea>
                                        </div>
                                    </div>
                                    <div class=""modal-footer justify-content-between"">
                                        <button type=""button"" class=""btn btn-default"" data-dismiss=""modal"">否</button>
                                        <input type=""button"" class=""btn btn-danger"" value=""確認"" onclick=""updateReject()"" />
                                    </div>
                                </div>
                                <!-- /.modal-content -->
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</div>

");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">

        function updateApprove() {

            let _requestModel = {};
            _requestModel['OrderSn'] = $(""#OrderSn"").val();
            _requestModel['OrderStatusId'] = 5;
            _requestModel['Remark'] = $(""#approveRemark"").val();

            //e.preventDefault();
            $.blockUI({
                message: '請稍等...',
                css: {
                    border: 'none',
                    padding: '5px',
                    backgroundColor: '#000',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: '.5',
                    color: '#fff',
                    fontSize: '25px',
                    fontFamily: '微軟正黑體',
                    fontWeight: 300,
                }
            });
            $.ajax({
                url: ""../Update"",
                type: ""POST"",
                data: { 'approveViewModel': _requestModel },
        ");
                WriteLiteral("        success: function (res) {\r\n                    $.unblockUI();\r\n                    if (res.isSuccess) {\r\n                        alert(\'已核准\');\r\n                        location.href = \"");
#nullable restore
#line 155 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\AuditDetail.cshtml"
                                    Write(Url.ActionLink("Audit", "AccessFab"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""";
                    }
                    else {
                        alert(res.msg);
                        return false;
                    }
                }
            });
        }

        function updateReject() {

            let _requestModel = {};
            _requestModel['OrderSn'] = $(""#OrderSn"").val();
            _requestModel['OrderStatusId'] = 2;
            _requestModel['Remark'] = $(""#rejectRemark"").val();

            //e.preventDefault();
            $.blockUI({
                message: '請稍等...',
                css: {
                    border: 'none',
                    padding: '5px',
                    backgroundColor: '#000',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: '.5',
                    color: '#fff',
                    fontSize: '25px',
                    fontFamily: '微軟正黑體',
                    fontWeight: 300,
                }
           ");
                WriteLiteral(@" });
            $.ajax({
                url: ""../Update"",
                type: ""POST"",
                data: { 'approveViewModel': _requestModel },
                success: function (res) {
                    $.unblockUI();
                    if (res.isSuccess) {
                        alert('剔退成功');
                        location.href = """);
#nullable restore
#line 196 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\AccessFab\AuditDetail.cshtml"
                                    Write(Url.ActionLink("Audit", "AccessFab"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\";\r\n                    }\r\n                    else {\r\n                        alert(res.msg);\r\n                        return false;\r\n                    }\r\n                }\r\n            });\r\n        }\r\n\r\n    </script>\r\n");
            }
            );
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MOD4.Web.ViewModel.AccessFabDetailPageViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591