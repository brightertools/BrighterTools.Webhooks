using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BrighterTools.Webhooks.Models;
using BrighterTools.Webhooks.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BrighterTools.Webhooks.Services;

/// <summary>
/// Represents Webhook HTTP Sender.
/// </summary>
public class WebhookHttpSender(IHttpClientFactory httpClientFactory, IOptions<WebhookOptions> options, ILogger<WebhookHttpSender> logger)
{
    /// <summary>
    /// Sends the operation.
    /// </summary>
    public async Task<WebhookDeliveryAttemptResult> SendAsync(WebhookDelivery delivery, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(options.Value.HttpClientName);
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildRequestUrl(delivery))
        {
            Content = new StringContent(delivery.PayloadJson, Encoding.UTF8, "application/json")
        };

        ApplyAuthentication(request, delivery);
        ApplyHeaders(request, delivery);

        try
        {
            using var response = await client.SendAsync(request, cancellationToken);
            var success = (int)response.StatusCode >= 200 && (int)response.StatusCode <= 299;
            return new WebhookDeliveryAttemptResult
            {
                Succeeded = success,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = success ? null : $"Webhook response status {(int)response.StatusCode}"
            };
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error sending webhook delivery {DeliveryId}", delivery.Id);
            return new WebhookDeliveryAttemptResult
            {
                Succeeded = false,
                ErrorMessage = exception.Message
            };
        }
    }

    private string BuildRequestUrl(WebhookDelivery delivery)
    {
        var requestUrl = delivery.EndpointUrl;
        if (delivery.Auth?.Type == Enums.WebhookAuthType.UrlKey && !string.IsNullOrWhiteSpace(delivery.Auth.ApiKeyName) && !string.IsNullOrWhiteSpace(delivery.Auth.ApiKey))
        {
            var separator = requestUrl.Contains('?') ? "&" : "?";
            requestUrl += $"{separator}{delivery.Auth.ApiKeyName}={Uri.EscapeDataString(delivery.Auth.ApiKey)}";
        }

        return requestUrl;
    }

    private void ApplyAuthentication(HttpRequestMessage request, WebhookDelivery delivery)
    {
        if (delivery.Auth == null)
        {
            return;
        }

        switch (delivery.Auth.Type)
        {
            case Enums.WebhookAuthType.Basic:
                var value = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{delivery.Auth.Username}:{delivery.Auth.Password}"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", value);
                break;
            case Enums.WebhookAuthType.BearerToken:
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", delivery.Auth.ApiKey);
                break;
            case Enums.WebhookAuthType.HeaderKey:
                if (!string.IsNullOrWhiteSpace(delivery.Auth.ApiKeyName) && !string.IsNullOrWhiteSpace(delivery.Auth.ApiKey))
                {
                    request.Headers.TryAddWithoutValidation(delivery.Auth.ApiKeyName, delivery.Auth.ApiKey);
                }
                break;
            case Enums.WebhookAuthType.HmacSha256:
                ApplyHmacSignature(request, delivery);
                break;
            case Enums.WebhookAuthType.UrlKey:
            case Enums.WebhookAuthType.None:
            default:
                break;
        }
    }

    private void ApplyHeaders(HttpRequestMessage request, WebhookDelivery delivery)
    {
        foreach (var header in delivery.Headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
    }

    private void ApplyHmacSignature(HttpRequestMessage request, WebhookDelivery delivery)
    {
        if (string.IsNullOrWhiteSpace(delivery.Auth?.Secret))
        {
            return;
        }

        var timestampHeader = delivery.Auth.TimestampHeaderName ?? options.Value.DefaultTimestampHeaderName;
        var signatureHeader = delivery.Auth.SignatureHeaderName ?? options.Value.DefaultSignatureHeaderName;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var signaturePayload = $"{timestamp}.{delivery.PayloadJson}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(delivery.Auth.Secret));
        var signature = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(signaturePayload))).ToLowerInvariant();
        request.Headers.TryAddWithoutValidation(timestampHeader, timestamp);
        request.Headers.TryAddWithoutValidation(signatureHeader, signature);
    }
}

