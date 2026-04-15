using System.Text.Json;
using BrighterTools.Webhooks.Abstractions;
using BrighterTools.Webhooks.Models;
using BrighterTools.Webhooks.Options;
using Microsoft.Extensions.Options;

namespace BrighterTools.Webhooks.Services;

/// <summary>
/// Creates Default Webhook Payload.
/// </summary>
public class DefaultWebhookPayloadFactory(IOptions<WebhookOptions> options) : IWebhookPayloadFactory
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    /// <summary>
    /// Creates Payload.
    /// </summary>
    public string CreatePayload(WebhookEvent webhookEvent, string deliveryId, string endpointUrl, int retryCount)
    {
        var payload = new
        {
            eventType = webhookEvent.EventKey,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            sourceSystem = options.Value.SourceSystem,
            data = webhookEvent.Data,
            retryCount,
            deliveryId,
            endpointUrl,
            reference = webhookEvent.Reference,
            metadata = webhookEvent.Metadata
        };

        return JsonSerializer.Serialize(payload, SerializerOptions);
    }
}

