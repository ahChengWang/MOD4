#pragma checksum "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e0bbb4bb38b111ea1b4d9027886cef0e195d262d"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Equipment_Index), @"mvc.1.0.view", @"/Views/Equipment/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e0bbb4bb38b111ea1b4d9027886cef0e195d262d", @"/Views/Equipment/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6d1efcd6897d9e05e93e9d2e8d3f27b282c15526", @"/Views/_ViewImports.cshtml")]
    public class Views_Equipment_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MOD4.Web.ViewModel.EquipmentViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "_PartialUnrepaired", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "1", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "2", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "3", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "_PartialEqHisotry", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
  
    ViewBag.Title = "設備機況";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<style>
    .card-title {
        font-size: 30px;
        font-weight: 900;
    }

    .btn-outline-danger {
        background-color: #c3cadb;
        font-weight: 900;
        color: darkred;
        border-width: 3px;
    }

    table {
        box-shadow: 2px 2px 2px #999;
    }

    .card {
        box-shadow: 2px 2px 2px #999;
        background-color: #c3cadb;
    }

    td {
        font-size: 13px;
        font-weight: 550;
    }

    .myth1, .myth2 {
        font-size: 13px;
        font-weight: 800;
    }

    .my-control {
        display: initial;
        width: 70%;
        height: calc(2.25rem + 2px);
        padding: 0.375rem 0.75rem;
        font-size: 1rem;
        font-weight: 400;
        line-height: 1.5;
        color: #495057;
        background-color: #fff;
        background-clip: padding-box;
        border: 1px solid #ced4da;
        border-radius: 0.25rem;
        box-shadow: inset 0 0 0 transparent;
        transition: border-color ");
            WriteLiteral(@".15s ease-in-out,box-shadow .15s ease-in-out;
    }

    .select {
        display: initial;
        width: 100%;
        height: calc(2.25rem + 2px);
        padding: 0.375rem 0.75rem;
        font-size: 1rem;
        font-weight: 400;
        line-height: 1.5;
        color: #495057;
        background-color: #fff;
        background-clip: padding-box;
        border: 1px solid #ced4da;
        border-radius: 0.25rem;
        box-shadow: inset 0 0 0 transparent;
        transition: border-color .15s ease-in-out,box-shadow .15s ease-in-out;
    }

    .dropdown-menu {
        max-height: 300px;
        overflow: scroll;
        overflow-x: hidden;
    }

    .btn-sm {
        /*font-size: 0.1rem;
        line-height: 5px;*/
    }

    .eqimg {
        width: 12px;
        height: 12px;
        margin-top: -2px;
        /*margin-left: -3px;*/
    }
</style>

<!-- Content Wrapper. Contains page content -->
<div class=""content-wrapper"">
    <!-- Content Header (Page head");
            WriteLiteral(@"er) -->
    <!-- Main content -->
    <section class=""content"">
        <div class=""container-fluid"">
            <div class=""row"">
                <div class=""col-12"">
                    <div class=""card"">
                        <div class=""card-header"">
                            <h3 class=""card-title"">未處理機況</h3>
                        </div>
                        <!-- /.card-header -->
                        <div class=""card-body"">
                            <div id=""barDiv1"" style=""margin-bottom:15px"">
                                <fieldset id=""searchArea"">
                                    <legend id=""legend"">查詢區塊</legend>
                                    <div class=""row"">
                                        <div class=""col-3"">
                                            <label>日期：</label>
                                            <input id=""srcDate"" class=""my-control"" type=""date"" name=""name""");
            BeginWriteAttribute("value", " value=\"", 3082, "\"", 3090, 0);
            EndWriteAttribute();
            WriteLiteral(@" />
                                        </div>
                                        <div class=""col-3"">
                                            <label>線體：</label>
                                            <select class=""select"" id=""selLoc"" multiple=""multiple"">
");
#nullable restore
#line 117 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                  
                                                    foreach (var node in ViewBag.ToolId)
                                                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                        <optgroup");
            BeginWriteAttribute("label", " label=\"", 3632, "\"", 3651, 1);
#nullable restore
#line 120 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
WriteAttributeValue("", 3640, node.Item1, 3640, 11, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n");
#nullable restore
#line 121 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                             foreach (var eq in node.Item2)
                                                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e0bbb4bb38b111ea1b4d9027886cef0e195d262d9758", async() => {
#nullable restore
#line 123 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                                               Write(eq);

#line default
#line hidden
#nullable disable
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 123 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                                   WriteLiteral(eq);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 124 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                        </optgroup>\r\n");
#nullable restore
#line 126 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                    }
                                                

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                                            </select>
                                        </div>
                                        <div class=""col-3"">
                                            <input id=""btnSearch"" type=""button"" class=""btn btn-info""");
            BeginWriteAttribute("style", " style=\"", 4411, "\"", 4419, 0);
            EndWriteAttribute();
            WriteLiteral(@" name=""btn29"" value=""查詢"" />
                                        </div>
                                    </div>
                                    <br />
                                </fieldset>
                            </div>
                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "e0bbb4bb38b111ea1b4d9027886cef0e195d262d12961", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 137 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
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
            WriteLiteral(@"                        </div>
                        <!-- /.card-body -->
                    </div>
                    <!-- /.card -->

                    <div class=""card"">
                        <div class=""card-header"">
                            <h3 class=""card-title"">歷史機況</h3>
                        </div>
                        <!-- /.card-header -->
                        <div id=""hisBody"" class=""card-body"">
                            <fieldset id=""searchArea"">
                                <legend id=""legend"">查詢區塊</legend>
                                <div class=""row"" style=""margin-bottom:5px"">
                                    <div style=""margin-right:5px;"">
                                        <label>日期：</label>
                                        <input id=""srcHisDate"" class=""my-control"" type=""date"" name=""name""");
            BeginWriteAttribute("value", " value=\"", 7789, "\"", 7797, 0);
            EndWriteAttribute();
            WriteLiteral(@" />
                                    </div>
                                    <div id=""hisLine"" style=""margin-right: 5px;"">
                                        <label>線體：</label>
                                        <select class=""my-select"" id=""selTool"" multiple=""multiple"">
");
#nullable restore
#line 186 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                              
                                                foreach (var node in ViewBag.RepairedToolId)
                                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                    <optgroup");
            BeginWriteAttribute("label", " label=\"", 8345, "\"", 8364, 1);
#nullable restore
#line 189 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
WriteAttributeValue("", 8353, node.Item1, 8353, 11, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n");
#nullable restore
#line 190 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                         foreach (var eq in node.Item2)
                                                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e0bbb4bb38b111ea1b4d9027886cef0e195d262d17152", async() => {
#nullable restore
#line 192 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                                           Write(eq);

#line default
#line hidden
#nullable disable
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 192 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                               WriteLiteral(eq);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 193 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                    </optgroup>\r\n");
#nullable restore
#line 195 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                                }
                                            

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                                        </select>
                                    </div>
                                    <div style=""margin-right: 5px;"">
                                        <label>狀態：</label>
                                        <select class=""my-select"" id=""selStatus"" multiple=""multiple"">
                                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e0bbb4bb38b111ea1b4d9027886cef0e195d262d20019", async() => {
                WriteLiteral("待 PM 確認");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e0bbb4bb38b111ea1b4d9027886cef0e195d262d21224", async() => {
                WriteLiteral("待工程師確認");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e0bbb4bb38b111ea1b4d9027886cef0e195d262d22428", async() => {
                WriteLiteral("已完成");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                                        </select>
                                    </div>
                                    <div class=""icheck-primary d-inline"" style=""margin-right: 5px;"">
                                        <label for=""autoChk"">顯示Auto機況：</label>
                                        <input id=""autoChk"" type=""checkbox"" class=""custom-checkbox"" />
                                    </div>
                                    <div>
                                        <input id=""_btnSearchHis"" type=""button"" class=""btn btn-info"" style=""margin-left:7px"" name=""btn29"" value=""查詢"" onclick=""btnClickSearchHis()"" />
                                    </div>
                                </div>
                                <div class=""row"" style=""display:flex;"">
                                    <div>
                                        <label class=""col-form-label"">本日：</label>
                                    </div>
                                    <div cla");
            WriteLiteral(@"ss=""info-box"" style=""min-height: 50%; background-color: #6c757d; color: white; width:fit-content; margin-right: 5px;"">
                                        <i class=""fas fa-pen-alt""></i>
                                        <i id=""pmttl"">待 PM 確認 (0)</i>
                                    </div>
                                    <div class=""info-box"" style=""min-height: 50%; margin-left: 5px; background-color: #007bff; color: white; width: fit-content "">
                                        <i class=""fas fa-pen-alt""></i>
                                        <i id=""engttl"">待工程師確認 (0)</i>
                                    </div>
                                </div>
                            </fieldset>
                            <div id=""barDiv2"" class=""row"">
                                <input type=""button"" style=""margin-bottom:5px"" class=""btn"" name=""btn30"" value=""+新增"" onclick=""btnClickCreateHis()"" />
                            </div>
                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "e0bbb4bb38b111ea1b4d9027886cef0e195d262d25715", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
#nullable restore
#line 232 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
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
            WriteLiteral(@"
                        </div>
                    </div>
                    <!-- /.card-body -->
                </div>
                <!-- /.card -->
                <!-- Modal -->
                <div class=""modal fade"" id=""detailModal"" tabindex=""-1"" role=""dialog"" aria-labelledby=""detailModalLabel"" aria-hidden=""true"">
                </div>
            </div>
            <!-- /.col -->
        </div>
        <!-- /.row -->
        <!-- /.container-fluid -->
    </section>
    <!-- /.content -->
</div>

");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">
        let myFuc = {
            SearchUnrepairedEq() {
                globalFuc.LoadingPic();
                $.ajax({
                    url: ""./Equipment/SearchUnrepairedEq?date="" + $(""#srcDate"").val() + ""&toolIdList="" + $(""#selLoc"").val(),
                    type: ""GET"",
                    dataType: 'html',
                    success: function (result) {
                        $.unblockUI();
                        if (isJsonString(result)) {
                            var _res = JSON.parse(result);
                            if (result == '""""') {
                                alert('查無資料');
                            }
                            if ('isException' in _res) {
                                alert(_res.msg);
                                return false;
                            }
                        }
                        else {
                            $(""#example1_wrapper"").remove();
                    ");
                WriteLiteral(@"        $(""#barDiv1"").after(result);
                            $('#example1').DataTable({
                                ""responsive"": true
                                , ""lengthChange"": false
                                , ""autoWidth"": false
                                , ""order"": [[5, ""asc""]]
                                , ""buttons"": [""excel"", ""colvis""]
                            }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');
                        }
                    }
                });
            },
            SearchEqHis() {
                globalFuc.LoadingPic();
                $.ajax({
                    url: ""./Equipment/SearchRepairedEqHistory?date="" + $(""#srcHisDate"").val() + ""&toolIdList="" + $(""#selTool"").val() + ""&statusIdList="" + $(""#selStatus"").val() + ""&showAuto="" + $(""#autoChk"").val(),
                    type: ""GET"",
                    dataType: 'html',
                    //beforeSend: function () {
                    // ");
                WriteLiteral(@"   var _bfAjax = $.ajax({
                    //        url: ""Equipment/CheckUser?eqsn=2"",
                    //        type: ""GET"",
                    //        async: false,
                    //        dataType: 'json',
                    //        success: function (chkResponse) {
                    //            return chkResponse;
                    //        }
                    //    });
                    //    if (_bfAjax[""responseJSON""] != """") {
                    //        alert(_bfAjax[""responseJSON""]);
                    //        return false
                    //    }
                    //},
                    success: function (result) {
                        $.unblockUI();
                        if (isJsonString(result)) {
                            var _res = JSON.parse(result);
                            if (result == '""""') {
                                alert('查無資料');
                            }
                            if ('isException' in _");
                WriteLiteral(@"res) {
                                alert(_res.msg);
                                return false;
                            }
                        }
                        else {
                            $(""#example2_wrapper"").remove();
                            //$(""#hisBody"").children().remove();
                            $(""#barDiv2"").after(result);
                            //$(""#hisBody"").append(result);
                            $('#example2').DataTable({
                                ""responsive"": false
                                , ""lengthChange"": true
                                , ""autoWidth"": false
                                , ""order"": [[5, ""asc""]]
                                /*,""buttons"": [""excel"", ""colvis""]*/
                            }).buttons().container().appendTo('#example2_wrapper .col-md-6:eq(0)');
                        }
                    }
                });
            },
            CreateEqAlarm() {
                lo");
                WriteLiteral(@"cation.href = ""./Equipment/Create"";
            }
        }

        $(function () {
            $('#example1').DataTable({
                //""responsive"": true,
                //""lengthChange"": true,
                //""autoWidth"": false,
                //""paging"": true,
                //""searching"": false,
                //""ordering"": true,
                //""info"": true,
                ""responsive"": true,
                ""lengthChange"": false,
                ""autoWidth"": false,
                ""order"": [[5, ""asc""]],
                /*""buttons"": [""copy"", ""csv"", ""excel"", ""pdf"", ""print"", ""colvis""]*/
                ""buttons"": [""excel"", ""colvis""]
            }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');


            const m = new Date();
            const year = m.getFullYear();
            let month = '' + (m.getMonth() + 1);
            let day = '' + m.getDate();
            if (month.length < 2)
                month = '0' + month;
            if ");
                WriteLiteral("(day.length < 2)\r\n                day = \'0\' + day;\r\n            const dateString = year + \"-\" + month + \"-\" + day;\r\n            const _model = ");
#nullable restore
#line 366 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                      Write(Json.Serialize(Model));

#line default
#line hidden
#nullable disable
                WriteLiteral(";\r\n            const _repairedEqInfoList = ");
#nullable restore
#line 367 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Equipment\Index.cshtml"
                                   Write(Json.Serialize(Model.RepairedEqInfoList.FirstOrDefault()));

#line default
#line hidden
#nullable disable
                WriteLiteral(@";

            $('#pmttl').html(""待 PM 確認 ("" + _model[""pmPending""] + "")"");
            $('#engttl').html(""待工程師確認 ("" + _model[""engPending""] + "")"");

            if (_repairedEqInfoList != null && _repairedEqInfoList[""mfgDay""] != dateString) {
                $(""#srcHisDate"").val(_repairedEqInfoList[""mfgDay""]);
            }

            $(""#example2"").DataTable({
                ""responsive"": false
                , ""lengthChange"": true
                , ""autoWidth"": false
                , ""order"": [[5, ""asc""]]
                /*,""buttons"": [""excel"", ""colvis""]*/
            }).buttons().container().appendTo('#example2_wrapper .col-md-6:eq(0)');

            $(""#selStatus"").multiselect({
            });

            $(""#selLoc, #selTool"").multiselect({
                /*includeSelectAllOption: true,*/
                enableClickableOptGroups: true,
                enableFiltering: true,
                filterBehavior: 'value',
                enableCollapsibleOptGroups: true,
         ");
                WriteLiteral(@"       collapseOptGroupsByDefault: true
            });

            $('#srcHisDate').attr(""max"", new Date().toISOString().slice(0, 10));
        });

        $(""#btnSearch"").click(myFuc.SearchUnrepairedEq);
        //$(""#btnSearchHis"").click(myFuc.SearchEqHis);
        function btnClickSearchHis() {
            myFuc.SearchEqHis();
        }

        function btnClickCreateHis() {
            myFuc.CreateEqAlarm();
        }

        function editClick(id, statusId, isEng, e) {
            $.ajax({
                url: ""./Equipment/VerifyEqStatus?eqsn="" + id + ""&statusId="" + statusId,
                type: ""GET"",
                async: false,
                dataType: 'json',
                success: function (chkResponse) {
                    if (chkResponse == """") {
                        location.href = ""./Equipment/Edit?sn="" + id + ""&statusId="" + statusId + ""&searchVal="" + $(""#srcHisDate"").val() + "";"" + $(""#selTool"").val() + "";"" + $(""#selStatus"").val();
                    }
 ");
                WriteLiteral(@"                   else {
                        alert(chkResponse);
                    }
                }
            });
        }

        function detailClick(id, e) {
            $.ajax({
                url: ""./Equipment/Detail?sn="" + id,
                type: 'GET',
                dataType: 'html',
                success: function (result) {
                    if (isJsonString(result)) {
                        var _res = JSON.parse(result);
                        alert(_res.msg);
                        dismissModal();
                        return false;
                    }
                    else {
                        $('body').addClass('modal-open');
                        var ele = $('body').find('.modal-backdrop');
                        if (ele.length == 0) {
                            $('body').append('<div class=""modal-backdrop fade show""></div>');
                        }
                        $('#detailModal').html(result);
                     ");
                WriteLiteral(@"   $('#detailModal').addClass('show');
                        $('#detailModal').css('display', 'block');
                    }
                }
            });
        }

        function dismissModal() {
            $('body').removeClass('modal-open');
            $('body').css({ 'padding-right': '' });
            $('#detailModal').removeClass('show');
            $('#detailModal').css('display', 'none');
            $('.modal-backdrop').remove();
            $('.modal-dialog').remove();
        }

        $(""#autoChk"").click(function (e) {
            if ($(""#autoChk"")[0].checked) {
                $(""#autoChk"").attr('checked', true);
                $(""#autoChk"").val('true');
            }
            else {
                $(""#autoChk"").attr('checked', false);
                $(""#autoChk"").val('false');
            }
        });

        // reload unrepaired block
        setInterval(myFuc.SearchUnrepairedEq, 300000);

        // reload repaired history block
        /*se");
                WriteLiteral("tInterval(myFuc.SearchEqHis, 600000);*/\r\n\r\n    </script>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MOD4.Web.ViewModel.EquipmentViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
