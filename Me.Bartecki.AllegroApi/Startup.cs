using Me.Bartecki.Allegro.Domain.Services;
using Me.Bartecki.Allegro.Domain.Services.Decorators;
using Me.Bartecki.Allegro.Domain.Services.Interfaces;
using Me.Bartecki.Allegro.Infrastructure.Integrations.RepositoryStores;
using Me.Bartecki.Allegro.Infrastructure.Integrations.RepositoryStores.Decorators;
using Me.Bartecki.Allegro.Infrastructure.Integrations.RepositoryStores.GitHub;
using Me.Bartecki.Allegro.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Me.Bartecki.Allegro.Api
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
            services.AddControllers().AddNewtonsoftJson();
            services.AddSingleton<IEmbeddedResourceService, EmbeddedResourceService>();
            services.AddSingleton<ILetterCounterService, LetterCounterService>();
            services.AddMemoryCache();
            services.AddGithubIntegration(Configuration.GetValue<string>("Integrations.Github.Token"));
            services.Decorate<IRepositoryStoreService, CachedRepositoryDecorator>();
            services.AddScoped<IRepoStatisticsService, RepoStatisticsService>();
            if(Configuration.GetValue<bool>("RoundNumbers"))
                services.Decorate<IRepoStatisticsService, RoundedStatisticsDecorator>();
            if (Configuration.GetValue<bool>("OrderLetters"))
                services.Decorate<IRepoStatisticsService, OrderedLettersStatisticsDecorator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
