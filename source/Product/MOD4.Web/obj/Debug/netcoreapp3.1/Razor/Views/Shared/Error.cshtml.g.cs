#pragma checksum "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Shared\Error.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1196c913211ceb4c369b44064e355877a5be8a61"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared_Error), @"mvc.1.0.view", @"/Views/Shared/Error.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1196c913211ceb4c369b44064e355877a5be8a61", @"/Views/Shared/Error.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6d1efcd6897d9e05e93e9d2e8d3f27b282c15526", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared_Error : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ErrorViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Shared\Error.cshtml"
  
    ViewData["Title"] = "Error";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div class=""content-wrapper"">
    <div class=""content-header"">
        <div class=""container-fluid"">
            <div class=""row mb-2"">
                <div class=""col-sm-6"">
                    <h1 class=""text-danger"">Error.</h1>
                    <h2 class=""text-danger"">An error occurred while processing your request.</h2>

                    <p>
                        <strong>Request ID:</strong> <code>");
#nullable restore
#line 15 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Shared\Error.cshtml"
                                                      Write(Model.RequestId);

#line default
#line hidden
#nullable disable
            WriteLiteral("</code>\r\n                    </p>\r\n                    <p>\r\n                        <strong>錯誤訊息: </strong> <code style=\"font-size:23px;font-weight:300;font-family:Impact\">");
#nullable restore
#line 18 "D:\Repostories\MOD4_Backend\MOD4\source\Product\MOD4.Web\Views\Shared\Error.cshtml"
                                                                                                           Write(Model.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</code>\r\n                    </p>\r\n                </div><!-- /.col -->\r\n            </div><!-- /.row -->\r\n        </div><!-- /.container-fluid -->\r\n    </div>\r\n</div>\r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ErrorViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591