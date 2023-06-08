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
using Utility.Helper;
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
            //�q�պAŪ���n�J�O�ɳ]�w
            int LoginExpireMinute = this.Configuration.GetSection("Logging").GetValue<int>("LoginExpireMinute");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
            {
                x.LoginPath = "/Account";//�n�J��
                //x.LogoutPath = new PathString("/Home/Logout");//�n�XAction
                x.ExpireTimeSpan = TimeSpan.FromMinutes(LoginExpireMinute);
                //����w��ĳfalse�A�սc�z���n��|�n�Dcookie���ੵ�i�Ĵ��A�o�ɳ]false�ܦ�����O���ɶ�
                //���p�G�A���Ȥ���������@���b�ϥΨt�Ϋo�e���Q�۰ʵn�X���ܡA�A�A�]��true(�M��z��policy�ЫȤᲤ�L�����ˬd) 
                x.SlidingExpiration = false;
            });

            services.AddControllersWithViews(options =>
            {
                //���MCSRF��w�����A�o�̴N�[�J�������ҽd��Filter���ܡA�ݷ|Controller�N�����A�[�W[AutoValidateAntiforgeryToken]�ݩ�
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            //    .AddJsonOptions(options =>
            //{
            //    //�쥻�O JsonNamingPolicy.CamelCase�A�j���Y��r��p�g�A�ڰ��n������ˡA�]��null
            //    options.JsonSerializerOptions.PropertyNamingPolicy = null;
            //    //���\�򥻩ԤB�^��Τ�������r������r��
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
            services.AddSingleton<IExtensionDomainService, ExtensionDomainService>();
            services.AddSingleton<ISPCReportDomainService, SPCReportDomainService>();
            services.AddSingleton<IMTDDashboardDomainService, MTDDashboardDomainService>();

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
            services.AddSingleton<ICertifiedAreaMappingRepository, CertifiedAreaMappingRepository>();
            services.AddSingleton<ISPCMicroScopeDataRepository, SPCMicroScopeDataRepository>();
            services.AddSingleton<ISPCChartSettingRepository, SPCChartSettingRepository>();
            services.AddSingleton<IMESPermissionRepository, MESPermissionRepository>();
            services.AddSingleton<IMESPermissionApplicantsRepository, MESPermissionApplicantsRepository>();
            services.AddSingleton<IMESPermissionAuditHistoryRepository, MESPermissionAuditHistoryRepository>();
            services.AddSingleton<IMTDProductionScheduleRepository, MTDProductionScheduleRepository>();
            services.AddSingleton<IMPSUploadHistoryRepository, MPSUploadHistoryRepository>();
            services.AddSingleton<ICIMTestBookingRepository, CIMTestBookingRepository>();
            services.AddSingleton<IDefinitionNodeDescRepository, DefinitionNodeDescRepository>();

            services.AddScoped<MenuService>();
            services.AddSingleton(new ServiceDescriptor(typeof(MSSqlDBHelper), new MSSqlDBHelper(Configuration)));
            services.AddSingleton(new ServiceDescriptor(typeof(CatchHelper), new CatchHelper(Configuration)));
            services.AddSingleton(new ServiceDescriptor(typeof(MailService), new MailService(Configuration)));
            services.AddSingleton(new ServiceDescriptor(typeof(FTPService), new FTPService(Configuration)));
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
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
