namespace BrighterTools.Webhooks.Enums;

/// <summary>
/// Specifies the available values for Webhook Auth Type.
/// </summary>
public enum WebhookAuthType
{
    /// <summary>
    /// Represents None.
    /// </summary>
    None = 0,
    /// <summary>
    /// Represents Basic.
    /// </summary>
    Basic = 1,
    /// <summary>
    /// Represents Bearer Token.
    /// </summary>
    BearerToken = 2,
    /// <summary>
    /// Represents Url Key.
    /// </summary>
    UrlKey = 3,
    /// <summary>
    /// Represents Header Key.
    /// </summary>
    HeaderKey = 4,
    /// <summary>
    /// Represents Hmac Sha256.
    /// </summary>
    HmacSha256 = 5
}

