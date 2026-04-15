# BrighterTools.Webhooks Integration

## Overview

`BrighterTools.Webhooks` is a storage-agnostic webhook delivery library.

It owns:
- event queueing
- subscription resolution through abstractions
- payload wrapping
- webhook authentication and signing
- retry policy
- delivery execution and processing

It does not own:
- EF persistence
- Hangfire or Windows services
- tenant ownership rules
- domain-specific event publishing

## Required Host Abstractions

Register host implementations for:
- `IWebhookSubscriptionStore`
- `IWebhookDeliveryStore`

The host can optionally add a scheduler that calls `IWebhookProcessor.ProcessPendingAsync(...)`.

## Registration

```csharp
services.AddBrighterToolsWebhooks(configuration);
services.AddScoped<IWebhookSubscriptionStore, MyWebhookSubscriptionStore>();
services.AddScoped<IWebhookDeliveryStore, MyWebhookDeliveryStore>();
```

## Queueing Events

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

## Direct Queueing

```csharp
await webhookService.QueueDirectAsync(new WebhookDirectRequest
{
    OwnerType = "tenant",
    OwnerId = tenantId.ToString(),
    EventKey = "file.uploaded",
    EndpointUrl = "https://example.com/webhooks/files",
    Data = payload,
    Auth = new WebhookAuthConfiguration { Type = WebhookAuthType.HeaderKey, ApiKeyName = "X-Api-Key", ApiKey = "secret" }
});
```

## Processing

Call one of:
- `IWebhookProcessor.ProcessPendingAsync(batchSize, maxDegreeOfParallelism)`
- `IWebhookProcessor.ProcessDeliveryAsync(deliveryId)`

A Hangfire job, Windows service, or other scheduler should own invocation.

## Event Keys

Use stable string event keys rather than enums where possible.
Examples:
- `file.uploaded`
- `file.processing.completed`
- `invoice.paid`

## Retry Policy

The default retry policy uses Fibonacci backoff and treats any `2xx` HTTP response as success.

## Authentication

Supported auth modes:
- none
- basic
- bearer token
- query string key
- header key
- HMAC SHA-256 signature
