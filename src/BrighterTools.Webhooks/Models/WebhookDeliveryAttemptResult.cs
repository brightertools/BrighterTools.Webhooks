namespace BrighterTools.Webhooks.Models;

/// <summary>
/// Represents the result of Webhook Delivery Attempt.
/// </summary>
public class WebhookDeliveryAttemptResult
{
    /// <summary>
    /// Gets or sets the Succeeded.
    /// </summary>
    public bool Succeeded { get; set; }
    /// <summary>
    /// Gets or sets the Status Code.
    /// </summary>
    public int? StatusCode { get; set; }
    /// <summary>
    /// Gets or sets the Error Message.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

