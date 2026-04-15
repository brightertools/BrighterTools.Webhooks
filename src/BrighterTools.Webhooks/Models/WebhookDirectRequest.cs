namespace BrighterTools.Webhooks.Models;

/// <summary>
/// Represents a request for Webhook Direct.
/// </summary>
public class WebhookDirectRequest
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
    /// Gets or sets the Endpoint URL.
    /// </summary>
    public string EndpointUrl { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Data.
    /// </summary>
    public object? Data { get; set; }
    /// <summary>
    /// Gets or sets the Reference.
    /// </summary>
    public string? Reference { get; set; }
    /// <summary>
    /// Gets or sets the Auth.
    /// </summary>
    public WebhookAuthConfiguration? Auth { get; set; }
    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    public IReadOnlyDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}

