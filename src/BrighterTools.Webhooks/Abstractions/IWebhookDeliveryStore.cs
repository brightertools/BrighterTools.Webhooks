using BrighterTools.Webhooks.Models;

namespace BrighterTools.Webhooks.Abstractions;

/// <summary>
/// Defines operations for Webhook Delivery Store.
/// </summary>
public interface IWebhookDeliveryStore
{
    /// <summary>
    /// Creates the create Async.
    /// </summary>
    /// <param name="delivery">The delivery value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<WebhookDelivery> CreateAsync(WebhookDelivery delivery, CancellationToken cancellationToken = default);
    /// <summary>
    /// Sets the set Payload Async.
    /// </summary>
    /// <param name="deliveryId">The deliveryId value.</param>
    /// <param name="payloadJson">The payloadJson value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetPayloadAsync(string deliveryId, string payloadJson, CancellationToken cancellationToken = default);
    /// <summary>
    /// Gets the get By Id Async.
    /// </summary>
    /// <param name="deliveryId">The deliveryId value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<WebhookDelivery?> GetByIdAsync(string deliveryId, CancellationToken cancellationToken = default);
    /// <summary>
    /// Gets the get Pending Ids Async.
    /// </summary>
    /// <param name="batchSize">The batchSize value.</param>
    /// <param name="now">The now value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<IReadOnlyList<string>> GetPendingIdsAsync(int batchSize, DateTimeOffset now, CancellationToken cancellationToken = default);
    /// <summary>
    /// Marks the mark Succeeded Async.
    /// </summary>
    /// <param name="deliveryId">The deliveryId value.</param>
    /// <param name="statusCode">The statusCode value.</param>
    /// <param name="completedAt">The completedAt value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task MarkSucceededAsync(string deliveryId, int? statusCode, DateTimeOffset completedAt, CancellationToken cancellationToken = default);
    /// <summary>
    /// Marks the mark Failed Async.
    /// </summary>
    /// <param name="deliveryId">The deliveryId value.</param>
    /// <param name="errorMessage">The errorMessage value.</param>
    /// <param name="statusCode">The statusCode value.</param>
    /// <param name="retryCount">The retryCount value.</param>
    /// <param name="attemptedAt">The attemptedAt value.</param>
    /// <param name="nextAttemptAt">The nextAttemptAt value.</param>
    /// <param name="isTerminal">The isTerminal value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task MarkFailedAsync(string deliveryId, string? errorMessage, int? statusCode, int retryCount, DateTimeOffset attemptedAt, DateTimeOffset? nextAttemptAt, bool isTerminal, CancellationToken cancellationToken = default);
}

