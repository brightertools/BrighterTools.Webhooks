using BrighterTools.Webhooks.Models;

namespace BrighterTools.Webhooks.Abstractions;

/// <summary>
/// Defines operations for Webhook Subscription Store.
/// </summary>
public interface IWebhookSubscriptionStore
{
    /// <summary>
    /// Gets the get Active Subscriptions Async.
    /// </summary>
    /// <param name="ownerType">The ownerType value.</param>
    /// <param name="ownerId">The ownerId value.</param>
    /// <param name="eventKey">The eventKey value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<IReadOnlyList<WebhookSubscription>> GetActiveSubscriptionsAsync(string ownerType, string ownerId, string eventKey, CancellationToken cancellationToken = default);
}

