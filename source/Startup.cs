using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Demand;
using MOD4.Web.Extension.Demand;
using MOD4.Web.Extension.Interface;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace MOD4.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //從組態讀取登入逾時設定
            int LoginExpireMinute = this.Configuration.GetSection("Logging").GetValue<int>("LoginExpireMinute");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
            {
                x.LoginPath = "/Account";//登入頁
                //x.LogoutPath = new PathString("/Home/Logout");//登出Action
                x.ExpireTimeSpan = TimeSpan.FromMinutes(LoginExpireMinute);
                //↓資安建議false，白箱弱掃軟體會要求cookie不能延展效期，這時設false變成絕對逾期時間
                //↓如果你的客戶反應明明一直在使用系統卻容易被自動登出的話，你再設為true(然後弱掃policy請客戶略過此項檢查) 
                x.SlidingExpiration = false;
            });

            services.AddControllersWithViews(options =>
            {
                //↓和CSRF資安有關，這裡就加入全域驗證範圍Filter的話，待會Controller就不必再加上[AutoValidateAntiforgeryToken]屬性
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            //    .AddJsonOptions(options =>
            //{
            //    //原本是 JsonNamingPolicy.CamelCase，強制頭文字轉小寫，我偏好維持原樣，設為null
            //    options.JsonSerializerOptions.PropertyNamingPolicy = null;
            //    //允許基本拉丁英文及中日韓文字維持原字元
            //    options.JsonSerializerOptions.Encoder =
            //    JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
            //});

            services.AddSingleton<IEquipmentDomainService, EquipmentDomainService>();
            services.AddSingleton<IAccountDomainService, AccountDomainService>();
            services.AddSingleton<IMenuDomainService, MenuDomainService>();
            services.AddSingleton<IPerformanceDomainService, PerformanceDomainService>();
            services.AddSingleton<ITargetSettingDomainService, TargetSettingDomainService>();
            services.AddSingleton<IOptionDomainService, OptionDomainService>();
            services.AddSingleton<IDemandDomainService, DemandDomainService>();
            services.AddSingleton<IUploadDomainService, UploadDomainService>();
            services.AddSingleton<IMAppDomainService, MAppDomainService>();
            services.AddSingleton<IAccessFabDomainService, AccessFabDomainService>();
            services.AddSingleton(new ServiceDescriptor(typeof(IUploadDomainService), new UploadDomainService(Configuration)));
            services.AddSingleton<IDemandFlowService, DemandFlowService>();
            services.AddSingleton<IBookingMeetingDomainService, BookingMeetingDomainService>();

            services.AddSingleton<IAlarmXmlRepository, AlarmXmlRepository>();
            services.AddSingleton<IEqpInfoRepository, EqpInfoRepository>();
            services.AddSingleton<IAccountInfoRepository, AccountInfoRepository>();
            services.AddSingleton<IMenuRepository, MenuRepository>();
            services.AddSingleton<IDailyEquipmentRepository, DailyEquipmentRepository>();
            services.AddSingleton<ITargetSettingRepository, TargetSettingRepository>();
            services.AddSingleton<ILineTTRepository, LineTTRepository>();
            services.AddSingleton<IEqSituationMappingRepository, EqSituationMappingRepository>();
            services.AddSingleton<IEqEvanCodeMappingRepository, EqEvanCodeMappingRepository>();
            services.AddSingleton<IEquipMappingRepository, EquipMappingRepository>();
            services.AddSingleton<IDemandsRepository, DemandsRepository>();
            services.AddSingleton<IAccessFabOrderRepository, AccessFabOrderRepository>();
            services.AddSingleton<IAccessFabOrderDetailRepository, AccessFabOrderDetailRepository>();
            services.AddSingleton<IAccessFabOrderAuditHistoryRepository, AccessFabOrderAuditHistoryRepository>();
            services.AddSingleton<ILcmProductRepository, LcmProductRepository>();
            services.AddSingleton<IBookingMeetingRepository, BookingMeetingRepository>();

            services.AddScoped<MenuService>();
            services.AddSingleton(new ServiceDescriptor(typeof(MSSqlDBHelper), new MSSqlDBHelper(Configuration)));
            services.AddSingleton(new ServiceDescriptor(typeof(CatchHelper), new CatchHelper(Configuration)));
            services.AddSingleton(new ServiceDescriptor(typeof(MailService), new MailService(Configuration)));
            services.AddSingleton<IDemadStatusFactory, DemadStatusFactory>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddHttpContextAccessor();

            services.AddSession();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
