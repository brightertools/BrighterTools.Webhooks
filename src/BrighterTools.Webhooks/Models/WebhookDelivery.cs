namespace BrighterTools.Webhooks.Models;

/// <summary>
/// Represents Webhook Delivery.
/// </summary>
public class WebhookDelivery
{
    /// <summary>
    /// Gets or sets the D.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Owner Type.
    /// </summary>
    public string OwnerType { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Owner ID.
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Event Key.
    /// </summary>
    public string EventKey { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Endpoint URL.
    /// </summary>
    public string EndpointUrl { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Payload Json.
    /// </summary>
    public string PayloadJson { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Reference.
    /// </summary>
    public string? Reference { get; set; }
    /// <summary>
    /// Gets or sets the Subscription ID.
    /// </summary>
    public string? SubscriptionId { get; set; }
    /// <summary>
    /// Gets or sets the Processed.
    /// </summary>
    public bool Processed { get; set; }
    /// <summary>
    /// Gets or sets the Failed.
    /// </summary>
    public bool Failed { get; set; }
    /// <summary>
    /// Gets or sets the Retry Count.
    /// </summary>
    public int RetryCount { get; set; }
    /// <summary>
    /// Gets or sets the Next Attempt AT.
    /// </summary>
    public DateTimeOffset? NextAttemptAt { get; set; }
    /// <summary>
    /// Gets or sets the Last Attempt AT.
    /// </summary>
    public DateTimeOffset? LastAttemptAt { get; set; }
    /// <summary>
    /// Gets or sets the Completed AT.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }
    /// <summary>
    /// Gets or sets the Last Error.
    /// </summary>
    public string? LastError { get; set; }
    /// <summary>
    /// Gets or sets the Last HTTP Status Code.
    /// </summary>
    public int? LastHttpStatusCode { get; set; }
    /// <summary>
    /// Gets or sets the Auth.
    /// </summary>
    public WebhookAuthConfiguration? Auth { get; set; }
    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    public IReadOnlyDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}

