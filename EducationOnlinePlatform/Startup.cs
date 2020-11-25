using EducationOnlinePlatform.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using EducationOnlinePlatform.Models;

namespace EducationOnlinePlatform
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
            services.AddCors(o => o.AddPolicy("CorsApi", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            /*services.AddCors(options =>
            {
                options.AddPolicy("PolicyApi", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });*/
            // services.AddCors();
            services.AddMvc();
            services.AddControllers();
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "Title of API", Version = "v1" });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                                    ValidateIssuer = true,
                                    ValidIssuer = AuthOptions.ISSUER,

                                    ValidateAudience = true,
                                    ValidAudience = AuthOptions.AUDIENCE,
                                    ValidateLifetime = true,

                                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                                    ValidateIssuerSigningKey = true,
                    };
                });
            services.AddRouting(r => r.SuppressCheckForUnhandledSecurityMetadata = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // app.UseCors();
            //app.UseCors("PolicyApi", builder => builder.AllowAnyOrigin());
            /*app.UseCors(
                options => options.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()
            );*/
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger();

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
                option.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            /*app.UseCors(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });*/

            app.UseCors("CorsApi");

            //app.UseMvc();

            app.Use((context, next) =>
            {
                context.Items["__CorsMiddlewareInvoked"] = true;
                return next();
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
