using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using RestEndpoint.Services;
using SquareDMS.ActionFilters;
using SquareDMS.CacheAccess;
using SquareDMS.Core.Dispatchers;
using SquareDMS.DatabaseAccess;
using SquareDMS.RestEndpoint.Services;
using SquareDMS.Services;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SquareDMS.RestEndpoint
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
            var publicKey = Configuration["Jwt:PublicKey"];

            using RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);

            services.AddControllers().AddNewtonsoftJson(s =>
            {
                s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // TODO enable
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,         // should be true, if valid issuer is set
                    ValidateAudience = false,       // should be true, if valid audience is set
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new RsaSecurityKey(rsa),

                    CryptoProviderFactory = new CryptoProviderFactory()
                    {
                        CacheSignatureProviders = false
                    },

                    ClockSkew = TimeSpan.Zero
                };
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = null;
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            // add db to di
            services.AddScoped<ISquareDb, SquareDbMsSql>();

            // add cache to di as singleton
            services.AddSingleton<ISquareCache, SquareCacheRedis>();

            services.AddScoped<DocumentService>();
            services.AddScoped<DocumentDispatcher>();

            services.AddScoped<DocumentTypeService>();
            services.AddScoped<DocumentTypeDispatcher>();

            services.AddScoped<DocumentVersionService>();
            services.AddScoped<DocumentVersionDispatcher>();

            services.AddScoped<FileFormatService>();
            services.AddScoped<FileFormatDispatcher>();

            services.AddScoped<GroupService>();
            services.AddScoped<GroupDispatcher>();

            services.AddScoped<GroupMemberService>();
            services.AddScoped<GroupMemberDispatcher>();

            services.AddScoped<RightService>();
            services.AddScoped<RightDispatcher>();

            services.AddScoped<UserService>();
            services.AddScoped<UserDispatcher>();

            services.AddScoped<UserActionFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
