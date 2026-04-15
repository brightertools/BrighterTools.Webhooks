using System.Text.Json;
using BrighterTools.Webhooks.Models;
using BrighterTools.Webhooks.Options;
using BrighterTools.Webhooks.Services;
using Microsoft.Extensions.Options;

namespace BrighterTools.Webhooks.Tests;

public class DefaultWebhookPayloadFactoryTests
{
    [Fact]
    public void CreatePayload_IncludesConfiguredSourceSystemAndEventFields()
    {
        var factory = new DefaultWebhookPayloadFactory(Microsoft.Extensions.Options.Options.Create(new WebhookOptions { SourceSystem = "Mosaio" }));
        var webhookEvent = new WebhookEvent
        {
            EventKey = "file.uploaded",
            Data = new { id = 42 },
            Reference = "file-42",
            Metadata = new Dictionary<string, string> { ["tenantId"] = "7" }
        };

        var payload = factory.CreatePayload(webhookEvent, "delivery-1", "https://example.com/webhooks", 2);
        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;

        Assert.Equal("file.uploaded", root.GetProperty("eventType").GetString());
        Assert.Equal("Mosaio", root.GetProperty("sourceSystem").GetString());
        Assert.Equal("delivery-1", root.GetProperty("deliveryId").GetString());
        Assert.Equal("https://example.com/webhooks", root.GetProperty("endpointUrl").GetString());
        Assert.Equal("file-42", root.GetProperty("reference").GetString());
        Assert.Equal(2, root.GetProperty("retryCount").GetInt32());
        Assert.Equal("7", root.GetProperty("metadata").GetProperty("tenantId").GetString());
        Assert.Equal(42, root.GetProperty("data").GetProperty("id").GetInt32());
    }
}
