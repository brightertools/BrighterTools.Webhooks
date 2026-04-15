using BrighterTools.Webhooks.Models;

namespace BrighterTools.Webhooks.Abstractions;

/// <summary>
/// Defines operations for Webhook Service.
/// </summary>
public interface IWebhookService
{
    /// <summary>
    /// Queues the queue Async.
    /// </summary>
    /// <param name="webhookEvent">The webhookEvent value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<int> QueueAsync(WebhookEvent webhookEvent, CancellationToken cancellationToken = default);
    /// <summary>
    /// Queues the queue Direct Async.
    /// </summary>
    /// <param name="request">The request value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<string> QueueDirectAsync(WebhookDirectRequest request, CancellationToken cancellationToken = default);
}

