using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthTask
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) //part of ms built in dependency injection system authentication is setuo here
        {
            services.AddControllersWithViews();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;


            })
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/denied";
                    options.Events = new CookieAuthenticationEvents()
                    {
                        OnSigningIn = async context =>
                        {
                            var scheme = context.Properties.Items.Where(k=>k.Key ==".AuthScheme ").FirstOrDefault();
                            var claim = new Claim(scheme.Key, scheme.Value);
                            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                            claimsIdentity.AddClaim(claim);
                        }
                    };  
                })

                .AddOpenIdConnect("google", options =>
                {
                    options.Authority = "https://accounts.google.com";
                    options.ClientId = "650092115932-aa9j0agmg6htq48jc8boeg57m7qo64vb.apps.googleusercontent.com";
                    options.ClientSecret = "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";
                    options.CallbackPath = "/auth";
                    options.SaveTokens = true;
                    options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents()
                    {
                        OnTokenValidated = async context =>
                        {
                            //assigning the role to a user based on the unique google name identifier
                            if (context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "109043313742709501133")
                            {
                                var claim = new Claim(ClaimTypes.Role, "Admin");
                                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                                claimsIdentity.AddClaim(claim);
                            }

                        } 
                    };
                })
                .AddOpenIdConnect("okta", options =>
                {
                    options.Authority = "https://dev-49918587.okta.com/oauth2/default";
                    options.ClientId = "0oa4n2v6varhxArOI5d7";
                    options.ClientSecret = "khLdLkvMZCYZlQpLsMv5TEGrjAK56lwhWvVt0DZC";
                    options.CallbackPath = "/okta-auth";
                    options.ResponseType = "code"; 
                })
                ;

                //.AddGoogle(options =>
                //{
                //    options.ClientId = "650092115932-aa9j0agmg6htq48jc8boeg57m7qo64vb.apps.googleusercontent.com";
                //    options.ClientSecret = "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";
                //    options.CallbackPath = "/auth";
                //});
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

            app.UseRouting();

            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }//houses the pipeline for processing request
    }
}
