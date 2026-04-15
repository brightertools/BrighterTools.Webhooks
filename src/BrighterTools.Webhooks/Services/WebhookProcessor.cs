using BrighterTools.Webhooks.Abstractions;
using Microsoft.Extensions.Logging;

namespace BrighterTools.Webhooks.Services;

/// <summary>
/// Represents Webhook Processor.
/// </summary>
public class WebhookProcessor(IWebhookDeliveryStore deliveryStore, IWebhookRetryPolicy retryPolicy, WebhookHttpSender sender, ILogger<WebhookProcessor> logger) : IWebhookProcessor
{
    /// <summary>
    /// Processes Pending.
    /// </summary>
    public async Task ProcessPendingAsync(int batchSize, int maxDegreeOfParallelism, CancellationToken cancellationToken = default)
    {
        var pendingIds = await deliveryStore.GetPendingIdsAsync(batchSize, DateTimeOffset.UtcNow, cancellationToken);
        using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
        var tasks = pendingIds.Select(async id =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                await ProcessDeliveryAsync(id, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Processes Delivery.
    /// </summary>
    public async Task<bool> ProcessDeliveryAsync(string deliveryId, CancellationToken cancellationToken = default)
    {
        var delivery = await deliveryStore.GetByIdAsync(deliveryId, cancellationToken);
        if (delivery == null || delivery.Processed || string.IsNullOrWhiteSpace(delivery.EndpointUrl))
        {
            return false;
        }

        var attemptedAt = DateTimeOffset.UtcNow;
        var result = await sender.SendAsync(delivery, cancellationToken);
        if (result.Succeeded)
        {
            await deliveryStore.MarkSucceededAsync(deliveryId, result.StatusCode, attemptedAt, cancellationToken);
            return true;
        }

        var nextRetryCount = delivery.RetryCount + 1;
        var decision = retryPolicy.GetNextAttempt(nextRetryCount);
        await deliveryStore.MarkFailedAsync(deliveryId, result.ErrorMessage, result.StatusCode, nextRetryCount, attemptedAt, decision.IsTerminal ? null : attemptedAt.Add(decision.Delay ?? TimeSpan.Zero), decision.IsTerminal, cancellationToken);
        logger.LogWarning("Webhook delivery {DeliveryId} failed with status {StatusCode}: {ErrorMessage}", deliveryId, result.StatusCode, result.ErrorMessage);
        return false;
    }
}

