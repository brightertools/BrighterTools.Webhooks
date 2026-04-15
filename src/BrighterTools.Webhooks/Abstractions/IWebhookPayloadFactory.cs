using BrighterTools.Webhooks.Models;

namespace BrighterTools.Webhooks.Abstractions;

/// <summary>
/// Defines operations for Webhook Payload Factory.
/// </summary>
public interface IWebhookPayloadFactory
{
    /// <summary>
    /// Creates the create Payload.
    /// </summary>
    /// <param name="webhookEvent">The webhookEvent value.</param>
    /// <param name="deliveryId">The deliveryId value.</param>
    /// <param name="endpointUrl">The endpointUrl value.</param>
    /// <param name="retryCount">The retryCount value.</param>
    /// <returns>The operation result.</returns>
    string CreatePayload(WebhookEvent webhookEvent, string deliveryId, string endpointUrl, int retryCount);
}

