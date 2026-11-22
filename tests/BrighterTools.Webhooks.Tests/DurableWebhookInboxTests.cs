using BrighterTools.Webhooks.Inbox;
namespace BrighterTools.Webhooks.Tests;
public class DurableWebhookInboxTests
{
    [Fact] public async Task FailureRemainsRetryableAndStoresOnlyClassification()
    {
        var store=new Store();
        Assert.False(await DurableWebhookInbox.ProcessAsync(store,"id",(_,_)=>throw new InvalidOperationException("private secret"),TimeProvider.System));
        Assert.Equal("InvalidOperationException",store.Error); Assert.False(store.Completed); Assert.True(store.RetryAt>DateTimeOffset.UtcNow);
        Assert.True(await DurableWebhookInbox.ProcessAsync(store,"id",(_,_)=>Task.FromResult(true),TimeProvider.System));
        Assert.True(store.Completed); Assert.False(await DurableWebhookInbox.ProcessAsync(store,"id",(_,_)=>throw new Exception(),TimeProvider.System));
    }
    [Fact] public async Task ConcurrentAcquisitionDoesNotInvokeEffectTwice()
    {
        var store=new Store(); var entered=new TaskCompletionSource(); var release=new TaskCompletionSource(); var calls=0;
        var first=DurableWebhookInbox.ProcessAsync(store,"id",async (_,_)=>{calls++;entered.SetResult();await release.Task;return true;},TimeProvider.System);
        await entered.Task;
        Assert.False(await DurableWebhookInbox.ProcessAsync(store,"id",(_,_)=>{calls++;return Task.FromResult(true);},TimeProvider.System));
        release.SetResult(); await first; Assert.Equal(1,calls);
    }
    [Fact] public async Task LostCompletionReplaysIdempotentEffect()
    {
        var store=new Store { FailCompletion=true }; var receipts=new HashSet<string>();
        Task<bool> Effect(WebhookInboxItem item,CancellationToken _) { receipts.Add(item.Id);return Task.FromResult(true); }
        Assert.False(await DurableWebhookInbox.ProcessAsync(store,"id",Effect,TimeProvider.System));
        store.FailCompletion=false;
        Assert.True(await DurableWebhookInbox.ProcessAsync(store,"id",Effect,TimeProvider.System));
        Assert.Single(receipts);
    }
    [Fact] public async Task ShutdownReleasesOwnershipWithoutAcknowledgement()
    {
        var store=new Store();using var cts=new CancellationTokenSource();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(()=>DurableWebhookInbox.ProcessAsync(store,"id",(_,_)=>{cts.Cancel();throw new OperationCanceledException(cts.Token);},TimeProvider.System,cts.Token));
        Assert.False(store.Held);Assert.False(store.Completed);Assert.Null(store.Error);
        Assert.True(await DurableWebhookInbox.ProcessAsync(store,"id",(_,_)=>Task.FromResult(true),TimeProvider.System));
    }
    [Theory] [InlineData(1,10)] [InlineData(2,20)] [InlineData(100,3600)]
    public void RetryBackoffIsBounded(int count,int seconds)=>Assert.Equal(TimeSpan.FromSeconds(seconds),DurableWebhookInbox.RetryDelay(count));
    sealed class Store : IWebhookInboxStore
    {
        public bool Held,Completed,FailCompletion; public string? Error; public DateTimeOffset? RetryAt;
        public Task<IReadOnlyList<string>> DueAsync(int limit,CancellationToken ct)=>Task.FromResult<IReadOnlyList<string>>(new[]{"id"});
        public Task<IWebhookInboxLease?> TryAcquireAsync(string id,CancellationToken ct)
        { if(Held||Completed)return Task.FromResult<IWebhookInboxLease?>(null);Held=true;return Task.FromResult<IWebhookInboxLease?>(new Lease(this)); }
        sealed class Lease(Store store):IWebhookInboxLease
        {
            public WebhookInboxItem Item=>new("id","{}",1);
            public Task CompleteAsync(bool ignored,CancellationToken ct){if(store.FailCompletion)throw new IOException();store.Completed=true;return Task.CompletedTask;}
            public Task RetryAsync(string code,DateTimeOffset at,CancellationToken ct){store.Error=code;store.RetryAt=at;return Task.CompletedTask;}
            public ValueTask DisposeAsync(){store.Held=false;return ValueTask.CompletedTask;}
        }
    }
}
