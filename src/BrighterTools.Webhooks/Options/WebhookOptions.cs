namespace BrighterTools.Webhooks.Options;

/// <summary>
/// Represents configuration options for Webhook.
/// </summary>
public class WebhookOptions
{
    /// <summary>
    /// Gets the section Name value.
    /// </summary>
    public const string SectionName = "Webhooks";

    /// <summary>
    /// Gets or sets the Source System.
    /// </summary>
    public string SourceSystem { get; set; } = "Application";
    /// <summary>
    /// Gets or sets the HTTP Client Name.
    /// </summary>
    public string HttpClientName { get; set; } = "BrighterTools.Webhooks";
    /// <summary>
    /// Gets or sets the Max Retry Attempts.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 17;
    /// <summary>
    /// Gets or sets the Default Signature Header Name.
    /// </summary>
    public string DefaultSignatureHeaderName { get; set; } = "X-Webhook-Signature";
    /// <summary>
    /// Gets or sets the Default Timestamp Header Name.
    /// </summary>
    public string DefaultTimestampHeaderName { get; set; } = "X-Webhook-Timestamp";
}

