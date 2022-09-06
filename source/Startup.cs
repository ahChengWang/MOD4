using Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MOD4.Web.DomainService;
using MOD4.Web.Repostory;
using System;
using System.Text;

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
                x.LoginPath = "/Account/Index";//�n�J��
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

            services.AddSingleton<IEquipmentDomainService, EquipmentDomainService>();
            services.AddSingleton<IAccountDomainService, AccountDomainService>();
            services.AddSingleton<IMenuDomainService, MenuDomainService>();
            services.AddSingleton<IPerformanceDomainService, PerformanceDomainService>();
            services.AddSingleton<ITargetSettingDomainService, TargetSettingDomainService>();


            services.AddSingleton<IAlarmXmlRepository, AlarmXmlRepository>();
            services.AddSingleton<IEqpInfoRepository, EqpInfoRepository>();
            services.AddSingleton<IAccountInfoRepository, AccountInfoRepository>();
            services.AddSingleton<IMenuRepository, MenuRepository>();
            services.AddSingleton<IDailyEquipmentRepository, DailyEquipmentRepository>();
            services.AddSingleton<ITargetSettingRepository, TargetSettingRepository>();

            services.AddScoped<MenuService>();
            services.Add(new ServiceDescriptor(typeof(MSSqlDBHelper), new MSSqlDBHelper(Configuration)));
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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
