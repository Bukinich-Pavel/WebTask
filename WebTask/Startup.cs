using WebTask.Models;
using WebTask.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using WebTask.Data;
using Westwind.AspNetCore.Markdown;
using CloudinaryDotNet;
using WebTask.SignalR;

namespace WebTask
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR(); // подключение SignalR

            services.AddDbContext<WebTask.Models.ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(opts => {
                opts.User.RequireUniqueEmail = true;    // уникальный email
                opts.User.AllowedUserNameCharacters = ".@abcdefghijklmnopqrstuvwxyz1234567890"; // допустимые символы
                opts.Password.RequiredLength = 5;   // минимальная длина
                opts.Password.RequireNonAlphanumeric = false;   // требуются ли не алфавитно-цифровые символы
                opts.Password.RequireLowercase = false; // требуются ли символы в нижнем регистре
                opts.Password.RequireUppercase = false; // требуются ли символы в верхнем регистре
                opts.Password.RequireDigit = false; // требуются ли цифры
            }).AddEntityFrameworkStores<ApplicationContext>();

            services.AddControllersWithViews();
            services.AddTransient<ICollectData, CollectData>();
            services.AddTransient<IItemData, ItemData>();
            services.AddTransient<IItemLikeData, ItemLikeData>();
            services.AddTransient<ICommentData, CommentData>();

            services.AddAuthentication()
                .AddGoogle(opts =>
                {
                    opts.ClientId = "715582416158-4u5fpf4j77unl7dmavd42vcsecibnfff.apps.googleusercontent.com";
                    opts.ClientSecret = "gUCeBaIHLxcnzdYQ2E0rO3Nd";
                    opts.SignInScheme = IdentityConstants.ExternalScheme;
                });
            //services.AddAuthentication()
            //.AddFacebook(options =>
            //{
            //    options.AppId = Configuration["784223934101-c5dqf3g257c3buh0kkb2eav5qd54msej.apps.googleusercontent.com"];
            //    options.AppSecret = Configuration["MeIiguYk1wHVGTCgy87P3O04"];
            //}).AddGoogle(options =>
            //{
            //    options.ClientId = Configuration["715582416158-4u5fpf4j77unl7dmavd42vcsecibnfff.apps.googleusercontent.com"];
            //    options.ClientSecret = Configuration["gUCeBaIHLxcnzdYQ2E0rO3Nd"];
            //});


            services.AddMarkdown();
            services.AddMvc().AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage(); // SignalR

            app.UseDefaultFiles();

            app.UseHttpsRedirection();

            app.UseMarkdown(); 
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat"); // SignalR
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }

    }
}
