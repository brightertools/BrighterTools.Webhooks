using BrighterTools.Webhooks.Enums;

namespace BrighterTools.Webhooks.Models;

/// <summary>
/// Represents Webhook Auth Configuration.
/// </summary>
public class WebhookAuthConfiguration
{
    /// <summary>
    /// Gets or sets the Type.
    /// </summary>
    public WebhookAuthType Type { get; set; } = WebhookAuthType.None;
    /// <summary>
    /// Gets or sets the Username.
    /// </summary>
    public string? Username { get; set; }
    /// <summary>
    /// Gets or sets the Password.
    /// </summary>
    public string? Password { get; set; }
    /// <summary>
    /// Gets or sets the API Key.
    /// </summary>
    public string? ApiKey { get; set; }
    /// <summary>
    /// Gets or sets the API Key Name.
    /// </summary>
    public string? ApiKeyName { get; set; } = "X-Api-Key";
    /// <summary>
    /// Gets or sets the Secret.
    /// </summary>
    public string? Secret { get; set; }
    /// <summary>
    /// Gets or sets the Signature Header Name.
    /// </summary>
    public string? SignatureHeaderName { get; set; }
    /// <summary>
    /// Gets or sets the Timestamp Header Name.
    /// </summary>
    public string? TimestampHeaderName { get; set; }
}

