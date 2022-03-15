using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servly.Extensions;
using Shouldly;
using Xunit;

namespace Servly.AspNetCore.ModelBinding.Hybrid.FunctionalTests;

public class HybridModelBinderTests
{
    [Fact]
    public async Task ShouldBindWhenJustBody()
    {
        using var host = await CreateServer();
        using var client = host.GetTestClient();

        var response = await client.PostAsJsonAsync("/Test/Test", new TestModel
        {
            FieldOne = "FirstValue",
            FieldTwo = "SecondValue"
        });

        var result = await response.Content.ReadFromJsonAsync<TestModel>();
        result.ShouldNotBeNull();
        result.FieldOne.ShouldBe("FirstValue");
        result.FieldTwo.ShouldBe("SecondValue");
    }

    [Fact]
    public async Task ShouldBindWhenBodyAndQuery()
    {
        using var host = await CreateServer();
        using var client = host.GetTestClient();

        var response = await client.PostAsJsonAsync("/Test/Test?fieldOne=FirstValue", new TestModel
        {
            FieldTwo = "SecondValue"
        });

        var result = await response.Content.ReadFromJsonAsync<TestModel>();
        result.ShouldNotBeNull();
        result.FieldOne.ShouldBe("FirstValue");
        result.FieldTwo.ShouldBe("SecondValue");
    }

    [Fact]
    public async Task ShouldBindWhenBodyAndRoute()
    {
        using var host = await CreateServer();
        using var client = host.GetTestClient();

        var response = await client.PostAsJsonAsync("/TestWithRoute/SecondValue", new TestModel
        {
            FieldOne = "FirstValue"
        });

        var result = await response.Content.ReadFromJsonAsync<TestModel>();
        result.ShouldNotBeNull();
        result.FieldOne.ShouldBe("FirstValue");
        result.FieldTwo.ShouldBe("SecondValue");
    }

    [Fact]
    public async Task ShouldBindWhenQueryAndRoute()
    {
        using var host = await CreateServer();
        using var client = host.GetTestClient();

        var response = await client.PostAsJsonAsync("/TestWithRoute/SecondValue?fieldOne=FirstValue", new TestModel());

        var result = await response.Content.ReadFromJsonAsync<TestModel>();
        result.ShouldNotBeNull();
        result.FieldOne.ShouldBe("FirstValue");
        result.FieldTwo.ShouldBe("SecondValue");
    }

    private static Task<IHost> CreateServer()
    {
        return new HostBuilder()
            .ConfigureWebHost(builder => builder
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services
                        .AddControllers()
                        .AddApplicationPart(typeof(TestController).Assembly)
                        .AddHybridModelBinder();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(routeBuilder =>
                    {
                        routeBuilder.MapGet("/", context => context.Response.WriteAsync("Test"));
                        routeBuilder.MapDefaultControllerRoute();
                    });
                }))
            .StartAsync();
    }
}

public class TestController : Controller
{
    [HttpPost]
    public Task<IActionResult> Test([FromHybrid] TestModel model)
    {
        return Task.FromResult<IActionResult>(Ok(model));
    }

    [HttpPost("TestWithRoute/{FieldTwo}")]
    public Task<IActionResult> TestWithRoute([FromHybrid] TestModel model)
    {
        return Task.FromResult<IActionResult>(Ok(model));
    }
}

public class TestModel
{
    public string FieldOne { get; set; } = default!;

    public string FieldTwo { get; set; } = default!;
}
