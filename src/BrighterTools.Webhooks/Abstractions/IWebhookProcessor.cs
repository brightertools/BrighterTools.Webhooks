namespace BrighterTools.Webhooks.Abstractions;

/// <summary>
/// Defines operations for Webhook Processor.
/// </summary>
public interface IWebhookProcessor
{
    /// <summary>
    /// Processes the process Pending Async.
    /// </summary>
    /// <param name="batchSize">The batchSize value.</param>
    /// <param name="maxDegreeOfParallelism">The maxDegreeOfParallelism value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ProcessPendingAsync(int batchSize, int maxDegreeOfParallelism, CancellationToken cancellationToken = default);
    /// <summary>
    /// Processes the process Delivery Async.
    /// </summary>
    /// <param name="deliveryId">The deliveryId value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<bool> ProcessDeliveryAsync(string deliveryId, CancellationToken cancellationToken = default);
}

