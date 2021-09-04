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
            services.AddSignalR(); // ïîäêëþ÷åíèå SignalR

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(opts => {
                opts.User.RequireUniqueEmail = true;    
                opts.User.AllowedUserNameCharacters = ".@abcdefghijklmnopqrstuvwxyz1234567890"; 
                opts.Password.RequiredLength = 5;   
                opts.Password.RequireNonAlphanumeric = false;   
                opts.Password.RequireLowercase = false; 
                opts.Password.RequireUppercase = false; 
                opts.Password.RequireDigit = false; 
            }).AddEntityFrameworkStores<ApplicationContext>();

            services.AddControllersWithViews();
            services.AddTransient<ICollectData, CollectData>();
            services.AddTransient<IItemData, ItemData>();
            services.AddTransient<IItemLikeData, ItemLikeData>();
            services.AddTransient<ICommentData, CommentData>();

            


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
