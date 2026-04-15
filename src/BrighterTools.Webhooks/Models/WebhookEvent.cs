namespace BrighterTools.Webhooks.Models;

/// <summary>
/// Represents Webhook Event.
/// </summary>
public class WebhookEvent
{
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
    /// Gets or sets the Data.
    /// </summary>
    public object? Data { get; set; }
    /// <summary>
    /// Gets or sets the Reference.
    /// </summary>
    public string? Reference { get; set; }
    /// <summary>
    /// Gets or sets the metadata.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}

