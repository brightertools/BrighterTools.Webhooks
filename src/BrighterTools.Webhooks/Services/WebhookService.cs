using BrighterTools.Webhooks.Abstractions;
using BrighterTools.Webhooks.Models;

namespace BrighterTools.Webhooks.Services;

/// <summary>
/// Provides Webhook operations.
/// </summary>
public class WebhookService(IWebhookSubscriptionStore subscriptionStore, IWebhookDeliveryStore deliveryStore, IWebhookPayloadFactory payloadFactory) : IWebhookService
{
    /// <summary>
    /// Executes Queue.
    /// </summary>
    public async Task<int> QueueAsync(WebhookEvent webhookEvent, CancellationToken cancellationToken = default)
    {
        var subscriptions = await subscriptionStore.GetActiveSubscriptionsAsync(webhookEvent.OwnerType, webhookEvent.OwnerId, webhookEvent.EventKey, cancellationToken);
        var queuedCount = 0;
        foreach (var subscription in subscriptions.Where(x => x.Enabled && !string.IsNullOrWhiteSpace(x.EndpointUrl)))
        {
            await QueueDeliveryInternalAsync(webhookEvent, subscription.EndpointUrl, webhookEvent.Reference, subscription.Id, subscription.Auth, subscription.Headers, cancellationToken);
            queuedCount++;
        }

        return queuedCount;
    }

    /// <summary>
    /// Executes Queue Direct.
    /// </summary>
    public Task<string> QueueDirectAsync(WebhookDirectRequest request, CancellationToken cancellationToken = default)
    {
        var webhookEvent = new WebhookEvent
        {
            OwnerType = request.OwnerType,
            OwnerId = request.OwnerId,
            EventKey = request.EventKey,
            Data = request.Data,
            Reference = request.Reference
        };

        return QueueDeliveryInternalAsync(webhookEvent, request.EndpointUrl, request.Reference, null, request.Auth, request.Headers, cancellationToken);
    }

    private async Task<string> QueueDeliveryInternalAsync(WebhookEvent webhookEvent, string endpointUrl, string? reference, string? subscriptionId, WebhookAuthConfiguration? auth, IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken)
    {
        var createdDelivery = await deliveryStore.CreateAsync(new WebhookDelivery
        {
            OwnerType = webhookEvent.OwnerType,
            OwnerId = webhookEvent.OwnerId,
            EventKey = webhookEvent.EventKey,
            EndpointUrl = endpointUrl,
            Reference = reference,
            SubscriptionId = subscriptionId,
            Auth = auth,
            Headers = headers,
            RetryCount = 0,
            NextAttemptAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        await deliveryStore.SetPayloadAsync(createdDelivery.Id, payloadFactory.CreatePayload(webhookEvent, createdDelivery.Id, endpointUrl, createdDelivery.RetryCount), cancellationToken);
        return createdDelivery.Id;
    }
}

