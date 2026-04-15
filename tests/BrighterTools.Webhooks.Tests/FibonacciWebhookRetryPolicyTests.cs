using BrighterTools.Webhooks.Options;
using BrighterTools.Webhooks.Services;
using Microsoft.Extensions.Options;

namespace BrighterTools.Webhooks.Tests;

public class FibonacciWebhookRetryPolicyTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 3)]
    public void GetNextAttempt_ReturnsExpectedFibonacciDelay(int retryCount, int expectedMinutes)
    {
        var policy = new FibonacciWebhookRetryPolicy(Microsoft.Extensions.Options.Options.Create(new WebhookOptions { MaxRetryAttempts = 10 }));

        var decision = policy.GetNextAttempt(retryCount);

        Assert.False(decision.IsTerminal);
        Assert.Equal(TimeSpan.FromMinutes(expectedMinutes), decision.Delay);
    }

    [Fact]
    public void GetNextAttempt_ReturnsTerminalDecision_WhenRetryCountReachesMaximum()
    {
        var policy = new FibonacciWebhookRetryPolicy(Microsoft.Extensions.Options.Options.Create(new WebhookOptions { MaxRetryAttempts = 3 }));

        var decision = policy.GetNextAttempt(3);

        Assert.True(decision.IsTerminal);
        Assert.Null(decision.Delay);
    }
}
