using BrighterTools.Webhooks.Abstractions;
using BrighterTools.Webhooks.Models;
using BrighterTools.Webhooks.Services;

namespace BrighterTools.Webhooks.Tests;

public class WebhookServiceTests
{
    [Fact]
    public async Task QueueAsync_QueuesOnlyEnabledSubscriptionsWithEndpoints()
    {
        var subscriptionStore = new FakeSubscriptionStore(
        [
            new WebhookSubscription { Id = "sub-1", EndpointUrl = "https://example.com/a", Enabled = true },
            new WebhookSubscription { Id = "sub-2", EndpointUrl = "", Enabled = true },
            new WebhookSubscription { Id = "sub-3", EndpointUrl = "https://example.com/c", Enabled = false }
        ]);
        var deliveryStore = new FakeDeliveryStore();
        var payloadFactory = new StubPayloadFactory();
        var service = new WebhookService(subscriptionStore, deliveryStore, payloadFactory);

        var queued = await service.QueueAsync(new WebhookEvent
        {
            OwnerType = "tenant",
            OwnerId = "42",
            EventKey = "file.uploaded",
            Reference = "file-42"
        });

        Assert.Equal(1, queued);
        Assert.Single(deliveryStore.CreatedDeliveries);
        Assert.Equal("sub-1", deliveryStore.CreatedDeliveries[0].SubscriptionId);
        Assert.Equal("payload:delivery-1:0", deliveryStore.Payloads["delivery-1"]);
    }

    [Fact]
    public async Task QueueDirectAsync_CreatesDeliveryWithoutSubscription()
    {
        var service = new WebhookService(
            new FakeSubscriptionStore([]),
            new FakeDeliveryStore(),
            new StubPayloadFactory());

        var deliveryId = await service.QueueDirectAsync(new WebhookDirectRequest
        {
            OwnerType = "tenant",
            OwnerId = "42",
            EventKey = "file.uploaded",
            EndpointUrl = "https://example.com/direct",
            Reference = "file-42"
        });

        Assert.Equal("delivery-1", deliveryId);
    }

    private sealed class FakeSubscriptionStore(IReadOnlyList<WebhookSubscription> subscriptions) : IWebhookSubscriptionStore
    {
        public Task<IReadOnlyList<WebhookSubscription>> GetActiveSubscriptionsAsync(string ownerType, string ownerId, string eventKey, CancellationToken cancellationToken = default)
            => Task.FromResult(subscriptions);
    }

    private sealed class FakeDeliveryStore : IWebhookDeliveryStore
    {
        private int _nextId = 1;

        public List<WebhookDelivery> CreatedDeliveries { get; } = [];
        public Dictionary<string, string> Payloads { get; } = [];

        public Task<WebhookDelivery> CreateAsync(WebhookDelivery delivery, CancellationToken cancellationToken = default)
        {
            delivery.Id = $"delivery-{_nextId++}";
            CreatedDeliveries.Add(delivery);
            return Task.FromResult(delivery);
        }

        public Task SetPayloadAsync(string deliveryId, string payloadJson, CancellationToken cancellationToken = default)
        {
            Payloads[deliveryId] = payloadJson;
            return Task.CompletedTask;
        }

        public Task<WebhookDelivery?> GetByIdAsync(string deliveryId, CancellationToken cancellationToken = default)
            => Task.FromResult<WebhookDelivery?>(CreatedDeliveries.SingleOrDefault(x => x.Id == deliveryId));

        public Task<IReadOnlyList<string>> GetPendingIdsAsync(int batchSize, DateTimeOffset now, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<string>>([]);

        public Task MarkSucceededAsync(string deliveryId, int? statusCode, DateTimeOffset completedAt, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task MarkFailedAsync(string deliveryId, string? errorMessage, int? statusCode, int retryCount, DateTimeOffset attemptedAt, DateTimeOffset? nextAttemptAt, bool isTerminal, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class StubPayloadFactory : IWebhookPayloadFactory
    {
        public string CreatePayload(WebhookEvent webhookEvent, string deliveryId, string endpointUrl, int retryCount)
            => $"payload:{deliveryId}:{retryCount}";
    }
}
