using BrighterTools.Webhooks.Abstractions;
using BrighterTools.Webhooks.Models;
using BrighterTools.Webhooks.Options;
using Microsoft.Extensions.Options;

namespace BrighterTools.Webhooks.Services;

/// <summary>
/// Implements the policy for Fibonacci Webhook Retry.
/// </summary>
public class FibonacciWebhookRetryPolicy(IOptions<WebhookOptions> options) : IWebhookRetryPolicy
{
    /// <summary>
    /// Gets Next Attempt.
    /// </summary>
    public WebhookRetryDecision GetNextAttempt(int retryCount)
    {
        if (retryCount >= options.Value.MaxRetryAttempts)
        {
            return new WebhookRetryDecision { IsTerminal = true };
        }

        return new WebhookRetryDecision
        {
            IsTerminal = false,
            Delay = TimeSpan.FromMinutes(GetFibonacciValue(retryCount))
        };
    }

    private static int GetFibonacciValue(int n)
    {
        var a = 0;
        var b = 1;
        for (var i = 0; i < n; i++)
        {
            var temp = a;
            a = b;
            b = temp + b;
        }

        return a;
    }
}

