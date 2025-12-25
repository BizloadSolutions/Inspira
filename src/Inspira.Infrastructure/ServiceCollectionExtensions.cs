using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Inspira.Domain.Repositories;
using Inspira.Infrastructure.Repositories;
using Inspira.Application.Services;
using Inspira.Infrastructure.Soap;

namespace Inspira.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoRepositories(this IServiceCollection services, IConfiguration config)
    {
        var settings = new MongoDbSettings();
        // Manually bind since Bind extension resides in Microsoft.Extensions.Configuration.Binder
        var section = config.GetSection("MongoDb");
        settings.ConnectionString = section["ConnectionString"] ?? string.Empty;
        settings.DatabaseName = section["DatabaseName"] ?? string.Empty;

        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        services.AddSingleton(database as IMongoDatabase);

        // Register repositories
        services.AddScoped<IFormRepository, FormRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();
        services.AddScoped<ISubmissionPropertyRepository, SubmissionPropertyRepository>();

        // Register SOAP client - configure HttpClient with base address of mock service by default
        services.AddHttpClient<MtcSoapClient>(c =>
        {
            // leave BaseAddress empty; MtcSoapClient will use endpoint provided below
        });

        services.AddScoped<IMtcSoapClient>(sp =>
        {
            var httpFactory = sp.GetRequiredService<System.Net.Http.IHttpClientFactory>();
            var http = httpFactory.CreateClient();
            var endpoint = sp.GetRequiredService<IConfiguration>()["MockMtcBaseUrl"] ?? "http://localhost:5000/mockmtc";
            return new MtcSoapClient(http, endpoint);
        });

        return services;
    }
}
