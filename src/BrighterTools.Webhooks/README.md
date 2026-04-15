# BrighterTools.Webhooks

`BrighterTools.Webhooks` provides storage-agnostic webhook queueing and delivery primitives.

It does not own persistence or scheduling. The consuming application must provide:
- `IWebhookSubscriptionStore`
- `IWebhookDeliveryStore`

## Install

```powershell
dotnet add package BrighterTools.Webhooks
```

## Register

```csharp
services.AddBrighterToolsWebhooks(configuration);
services.AddScoped<IWebhookSubscriptionStore, MyWebhookSubscriptionStore>();
services.AddScoped<IWebhookDeliveryStore, MyWebhookDeliveryStore>();
```

## Queue Events

```csharp
await webhookService.QueueAsync(new WebhookEvent
{
    OwnerType = "tenant",
    OwnerId = tenantId.ToString(),
    EventKey = "file.uploaded",
    Data = payload,
    Reference = fileGuid.ToString()
});
```

## Process Deliveries

```csharp
await webhookProcessor.ProcessPendingAsync(batchSize: 100, maxDegreeOfParallelism: 8);
```

The host should invoke processing from Hangfire, a hosted service, a worker, or another scheduler.
