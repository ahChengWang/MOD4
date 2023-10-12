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
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using MOD4.Web.Controllers;
using Microsoft.AspNetCore.Http.Features;

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
            services.AddSingleton<IMonitorDomainService, MonitorDomainService>();
            services.AddSingleton<IMaterialDomainService, MaterialDomainService>(); 
            services.AddSingleton<IPCESCertificationDomainService, PCESCertificationDomainService>();
            services.AddSingleton<IBulletinDomainService, BulletinDomainService>();
            services.AddSingleton<IDepartmentDomainService, DepartmentDomainService>();

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
            services.AddSingleton<IEfficiencySettingRepository, EfficiencySettingRepository>();
            services.AddSingleton<IDailyEfficiencyRepository, DailyEfficiencyRepository>();
            services.AddSingleton<IAlarmXmlRepository, AlarmXmlRepository>(); 
            services.AddSingleton<ISAPMaterialRepository, SAPMaterialRepository>(); 
            services.AddSingleton<IPCESCertificationRepository, PCESCertificationRepository>();
            services.AddSingleton<ICarUXBulletinRepository, CarUXBulletinRepository>();
            services.AddSingleton<IDefinitionDepartmentRepository, DefinitionDepartmentRepository>();

            services.AddScoped<MenuService>();
            services.AddSingleton(new ServiceDescriptor(typeof(MSSqlDBHelper), new MSSqlDBHelper(Configuration)));
            services.AddSingleton(new ServiceDescriptor(typeof(CatchHelper), new CatchHelper(Configuration)));
            services.AddSingleton(new ServiceDescriptor(typeof(MailService), new MailService(Configuration)));
            services.AddSingleton(new ServiceDescriptor(typeof(FTPService), new FTPService(Configuration)));
            services.AddSingleton(new ServiceDescriptor(typeof(LogHelper), new LogHelper(Configuration)));
            services.AddSingleton<IDemadStatusFactory, DemadStatusFactory>();
            services.AddSingleton<IMTDProcessFactory, MTDProcessFactory>();
            //�[�JWebSocket�B�z�A��
            services.AddSingleton<WebSocketHandler>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddHttpContextAccessor();
            services.Configure<FormOptions>(options => options.ValueCountLimit = 1000);
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

            //�[�JWebSocket�B�z�A��
            //builder.Services.AddSingleton<WebSocketHandler>();

            //�[�J WebSocket �\��
            var wsOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            };

            app.UseWebSockets(wsOptions);

            //ı�o�� Controller ���� WebSocket �ӽ����A�ڧ�b Middleware �h�B�z
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/send")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using (WebSocket ws = await context.WebSockets.AcceptWebSocketAsync())
                        {
                            var wsHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
                            await wsHandler.ProcessWebSocket(ws);
                        }
                    }
                    else
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }

        //private async Task Send(HttpContext context, WebSocket webSocket)
        //{
        //    var buffer = new byte[1034 * 4];
        //    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
        //    if (result != null)
        //    {
        //        while (!result.CloseStatus.HasValue)
        //        {
        //            string msg = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));
        //            Console.WriteLine($"client says: {msg}");
        //            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Server says: {DateTime.UtcNow:f}")), result.MessageType, result.EndOfMessage, System.Threading.CancellationToken.None);
        //            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
        //        }

        //        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, System.Threading.CancellationToken.None);
        //    }

        //}
    }
}
