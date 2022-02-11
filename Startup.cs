
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DbWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DbWebAPI
{

    /// <summary>
    /// 
    ///     DbWebApi.Startup - Configure API services.
    /// 
    /// </summary>
    /// <remarks>
    /// 
    ///     2021-04-10 - Created. 
    ///     2021-04-11 - Implemented Swashbuckle for Swagger testing (replacing Postman).
    ///     2021-04-12 - Switched from Swashbuckle to Nswag.
    ///     2021-04-17 - Configured Razor Pages.
    /// 
    /// </remarks>
    public class Startup
    {   /// <summary>Setup Process called from Main()</summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary> API startup Configuration </summary>
        public IConfiguration Configuration { get; }

        /// <summary> This method gets called by the runtime. Use this method to add services to the container.</summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // AddMvc supports Razor Pages, conventional routing for Views and attribute routing for REST Api
            services.AddMvc(options => options.SuppressAsyncSuffixInActionNames = false).WithRazorPagesRoot("/Pages");

            //// Register the Swagger generator, defining 1 or more Swagger documents (Swashbuckle implementation)
            //services.AddSwaggerGen();

            // Register the Swagger services (NSwag implementation)
            //services.AddOpenApiDocument(); // add OpenAPI v3 document
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Title = "DbWebApi - Json EndPoints";
                    document.Info.Description = "ASP.NET Core Web API - Database CRUD EndPoints for Mobile App";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "DbWebAPI Home Page",
                        Url = "https://localhost:5001/Home"
                    };
                    //Email = "davey.mc@live.co.uk",
                    //document.Info.License = new NSwag.OpenApiLicense
                    //{
                    //    Name = "Use under LICX",
                    //    Url = "https://example.com/license"
                    //};
                };
            });

            // Register Database service
            services.AddDbContext<SCxItemContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDatabaseDeveloperPageExceptionFilter();

            // Register Database service
            //services.AddDbContext<SCxItemContext> (opt => opt.UseInMemoryDatabase("FSAdiaryDb"));
        }

        /// <summary> This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</summary>
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
            // app.UseHttpsRedirection(); - Commented out for localhost testing
            app.UseStaticFiles();

            //// Enable middleware to serve generated Swagger as a JSON endpoint (Swashbuckle implementation)
            //app.UseSwagger();

            //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            //// specifying the Swagger JSON endpoint.  - Swashbuckle implementation
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //});

            app.UseRouting();

            app.UseAuthorization();

            // Register the Swagger generator and the Swagger UI middlewares (NSwag implementation)
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();     // Use Attribute Routing for REST Api
                endpoints.MapRazorPages();      // Map Razor Page endpoints
                //endpoints.MapControllerRoute( // Use Convensional routing to map incomming Json to controller actions
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
