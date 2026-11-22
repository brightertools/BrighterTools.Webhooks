namespace BrighterTools.Webhooks.Inbox;

/// <summary>A persisted, authenticated delivery. Attempts count acquisitions, not duplicate receipts.</summary>
public sealed record WebhookInboxItem(string Id, string Payload, int Attempts);

/// <summary>Storage must exclusively own an item until disposal, including across processes.
/// Completion/retry writes must be fenced to this acquisition. A lost lease must not complete new work.
/// Disposing without completion must leave the item recoverable. Effects must be idempotent.</summary>
public interface IWebhookInboxLease : IAsyncDisposable
{
    WebhookInboxItem Item { get; }
    Task CompleteAsync(bool ignored, CancellationToken cancellationToken);
    Task RetryAsync(string errorCode, DateTimeOffset nextAttemptUtc, CancellationToken cancellationToken);
}

public interface IWebhookInboxStore
{
    Task<IReadOnlyList<string>> DueAsync(int limit, CancellationToken cancellationToken);
    Task<IWebhookInboxLease?> TryAcquireAsync(string id, CancellationToken cancellationToken);
}

/// <summary>Provider-neutral recovery policy. Hosts own persistence, scopes, scheduling and effects.</summary>
public static class DurableWebhookInbox
{
    public static TimeSpan RetryDelay(int attempts) => TimeSpan.FromSeconds(Math.Min(3600, 10 * Math.Pow(2, Math.Clamp(attempts - 1, 0, 9))));

    public static async Task<bool> ProcessAsync(IWebhookInboxStore store, string id,
        Func<WebhookInboxItem, CancellationToken, Task<bool>> handle,
        TimeProvider clock, CancellationToken cancellationToken = default)
    {
        await using var lease = await store.TryAcquireAsync(id, cancellationToken);
        if (lease == null) return false;
        try
        {
            var handled = await handle(lease.Item, cancellationToken);
            await lease.CompleteAsync(!handled, cancellationToken);
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            // Persist only a classification. Exception messages may contain provider payloads/secrets.
            await lease.RetryAsync(ex.GetType().Name, clock.GetUtcNow() + RetryDelay(lease.Item.Attempts), cancellationToken);
            return false;
        }
    }
}
