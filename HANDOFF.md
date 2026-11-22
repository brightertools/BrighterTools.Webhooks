# Durable inbox and callback transport — 17 November 2026

Local version 1.0.2. No NuGet publication in this task.

Additive `Inbox` abstractions separate persisted provider events from effects.
`IWebhookInboxStore` finds due IDs and acquires exclusive leases; storage MUST own
cross-process exclusivity, fence completion, and make abandoned work recoverable.
`DurableWebhookInbox` coordinates handling, capped backoff and safe failure codes;
shutdown propagates without acknowledging completion. Effects MUST remain idempotent
because a crash after an external effect can precede local completion. Financial
inbox retries are not silently discarded after a fixed attempt count.

Hosts own validation before enqueue, schema, store implementation, scheduling and
one dependency-injection scope per parallel item. Uniiite uses non-reconnecting SQL
session application locks plus persisted fencing tokens; a disconnected owner
cannot complete through a replacement connection. This API is opt-in; existing
outbound delivery-store interfaces and other host policies are unchanged. The old
outbound processor does not automatically gain a durable store by installing this
version; hosts must implement the required ownership boundary.

Shared HTTP sender already accepts all 2xx statuses. It now redacts transport
exception messages and propagates caller cancellation. Uniiite reuses this sender
for legacy callbacks, preserving payload/auth contracts and stable callback IDs,
with app-owned SQL claims and independent scopes. Other consumers retain their
existing persistence. Callbacks are not enabled by this update.

Tests: 23 passed, including inbox duplication, interrupted/lost completion, retry
backoff, 2xx acknowledgement and failure-message redaction. No real callbacks sent.

Uniiite deployment verified 17 November 2026: compatible sandbox destination enabled,
six historical account notifications reconciled across three owned accounts, two new
Stripe deliveries durably processed. Only Uniiite deployed; other consumers unchanged.
Source patches/hashes and rollback evidence are in Uniiite's
`.artifacts/stripe-webhook-fix-20261117/`; see
`Documentation/StripeWebhookRecovery-2026-11-17.md`.
