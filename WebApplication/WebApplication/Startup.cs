using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using WebApplication.Repositories;
using WebApplication.Repositoties;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {


            services.AddCors(options =>
            {
                options.AddPolicy(name: "_allowAny",
                                  builder =>
                                  {
                                      builder.WithOrigins("*")
                                                  .AllowAnyOrigin()
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod();

                                  });
            });

            services.AddSignalR(e =>
            {
                e.MaximumReceiveMessageSize = 102400000;
            });
            //services.AddScoped<UserService>();
            services.AddControllers();
            services.AddHttpContextAccessor();
           //.AddJwtBearer(options =>
           //{
           //    options.TokenValidationParameters = new TokenValidationParameters
           //    {
           //        ValidateIssuer = true,
           //        ValidateAudience = true,
           //        ValidateLifetime = true,
           //        ValidateIssuerSigningKey = true,
           //        ValidIssuer = Configuration["Jwt:Issuer"],
           //        ValidAudience = Configuration["Jwt:Issuer"],
           //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
           //        ClockSkew = TimeSpan.Zero
           //    };

           //    options.Events = new JwtBearerEvents
           //    {
           //        OnMessageReceived = context =>
           //        {
           //            var accessToken = context.Request.Query["access_token"];

           //            // If the request is for our hub...
           //            var path = context.HttpContext.Request.Path;
           //            if (!string.IsNullOrEmpty(accessToken) &&
           //                (path.StartsWithSegments("/e2e") || path.StartsWithSegments("/gstt")))
           //            {
           //                // Read the token out of the query string
           //                context.Token = accessToken;
           //            }
           //            return Task.CompletedTask;
           //        }
           //    };
           //});

            services.AddMemoryCache();

            services.Configure<MongoDbSettings>(Configuration.GetSection("OfflineConferenceDatabaseSetting"));

            services.AddSingleton<IMongoDbSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

            services.AddScoped<IMongoContext, MongoContext>();
            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            //services.AddSingleton<IMailService, MailService>();
           
            services.AddCors();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "MyProjcet API docs",
                    Version = "v1",
                    Description = "MyProjcet API docs",
                });
            });

            //services.AddMvc()
            //            .AddJsonOptions(options =>
            //            {
            //                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
            app.UseCors("_allowAny");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

         
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseCors("AllowAllOrigins");
            app.UseSwagger();

            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyProject API docs"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
