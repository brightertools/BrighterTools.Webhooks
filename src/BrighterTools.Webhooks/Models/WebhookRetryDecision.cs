namespace BrighterTools.Webhooks.Models;

/// <summary>
/// Represents Webhook Retry Decision.
/// </summary>
public class WebhookRetryDecision
{
    /// <summary>
    /// Gets or sets the S Terminal.
    /// </summary>
    public bool IsTerminal { get; set; }
    /// <summary>
    /// Gets or sets the Delay.
    /// </summary>
    public TimeSpan? Delay { get; set; }
}

