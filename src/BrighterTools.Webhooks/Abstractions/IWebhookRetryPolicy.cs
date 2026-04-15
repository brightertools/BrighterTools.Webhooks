using BrighterTools.Webhooks.Models;

namespace BrighterTools.Webhooks.Abstractions;

/// <summary>
/// Defines operations for Webhook Retry Policy.
/// </summary>
public interface IWebhookRetryPolicy
{
    /// <summary>
    /// Gets the get Next Attempt.
    /// </summary>
    /// <param name="retryCount">The retryCount value.</param>
    /// <returns>The operation result.</returns>
    WebhookRetryDecision GetNextAttempt(int retryCount);
}

