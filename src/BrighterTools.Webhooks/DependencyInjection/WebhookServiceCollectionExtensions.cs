using BrighterTools.Webhooks.Abstractions;
using BrighterTools.Webhooks.Options;
using BrighterTools.Webhooks.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrighterTools.Webhooks.DependencyInjection;

/// <summary>
/// Provides extension methods for Webhook Service Collection.
/// </summary>
public static class WebhookServiceCollectionExtensions
{
    /// <summary>
    /// Adds Brighter Tools Webhooks.
    /// </summary>
    public static IServiceCollection AddBrighterToolsWebhooks(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WebhookOptions>(configuration.GetSection(WebhookOptions.SectionName));
        services.AddHttpClient("BrighterTools.Webhooks");
        services.AddScoped<IWebhookPayloadFactory, DefaultWebhookPayloadFactory>();
        services.AddScoped<IWebhookRetryPolicy, FibonacciWebhookRetryPolicy>();
        services.AddScoped<IWebhookService, WebhookService>();
        services.AddScoped<IWebhookProcessor, WebhookProcessor>();
        services.AddScoped<WebhookHttpSender>();
        return services;
    }
}

