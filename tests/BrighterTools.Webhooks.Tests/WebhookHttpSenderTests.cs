using System.Net;
using BrighterTools.Webhooks.Models;
using BrighterTools.Webhooks.Options;
using BrighterTools.Webhooks.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
namespace BrighterTools.Webhooks.Tests;
public class WebhookHttpSenderTests
{
    [Theory] [InlineData(200,true)] [InlineData(201,true)] [InlineData(202,true)] [InlineData(204,true)] [InlineData(299,true)] [InlineData(400,false)] [InlineData(500,false)]
    public async Task ReceiverAcknowledgementUsesTheFull2xxRange(int status,bool expected)
    {
        var handler=new Handler { Status=status };
        var sender=new WebhookHttpSender(new Factory(handler),Microsoft.Extensions.Options.Options.Create(new WebhookOptions()),NullLogger<WebhookHttpSender>.Instance);
        var result=await sender.SendAsync(new WebhookDelivery { Id="stable",EndpointUrl="https://receiver.example.test/notify",PayloadJson="{}",Headers=new Dictionary<string,string> { ["Idempotency-Key"]="stable" } });
        Assert.Equal(expected,result.Succeeded);Assert.Equal(status,result.StatusCode);Assert.Equal("stable",handler.Key);
    }
    [Fact] public async Task ErrorCannotExposeDestinationCredentials()
    {
        var sender=new WebhookHttpSender(new Factory(new Handler { Throw=true }),Microsoft.Extensions.Options.Options.Create(new WebhookOptions()),NullLogger<WebhookHttpSender>.Instance);
        var result=await sender.SendAsync(new WebhookDelivery { Id="stable",EndpointUrl="https://receiver.example.test/notify",PayloadJson="{}" });
        Assert.Equal("HttpRequestException",result.ErrorMessage);
    }
    sealed class Factory(HttpMessageHandler handler):IHttpClientFactory { public HttpClient CreateClient(string name)=>new(handler,false); }
    sealed class Handler:HttpMessageHandler
    {
        public int Status;public bool Throw;public string? Key;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken ct)
        { if(Throw)throw new HttpRequestException("private credential-bearing URL");Key=request.Headers.GetValues("Idempotency-Key").Single();return Task.FromResult(new HttpResponseMessage((HttpStatusCode)Status)); }
    }
}
