using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Api.Dtos;
using Api.Domain.Entities;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task CreateAndGetTicket_Succeeds()
    {
        var client = _factory.CreateClient();
        var create = new TicketCreateDto { Title = "Integration Test Ticket", Description = "desc" };
        var createResp = await client.PostAsJsonAsync("/api/tickets", create);
        createResp.EnsureSuccessStatusCode();
        var created = await createResp.Content.ReadFromJsonAsync<Ticket>();
        created.Should().NotBeNull();
        var getResp = await client.GetAsync($"/api/tickets/{created!.Id}");
        getResp.EnsureSuccessStatusCode();
        var got = await getResp.Content.ReadFromJsonAsync<Ticket>();
        got.Should().NotBeNull();
        got!.Title.Should().Be(create.Title);
    }
}
