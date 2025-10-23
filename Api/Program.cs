using System;
using Api.Domain.Entities;
using Api.Domain.Interfaces;
using Api.Infrastructure.Repositories;
using Api.Services;
using Api.Dtos;
using Microsoft.AspNetCore.Builder;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Configuration Mongo
// Lire les valeurs depuis appsettings.json
var mongoConn = builder.Configuration["MongoDB:ConnectionString"];
var mongoDbName = builder.Configuration["MongoDB:DatabaseName"];


// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniTickets API", Version = "v1" });
});
builder.Services.Configure<JsonOptions>(opts =>
{
    opts.SerializerOptions.PropertyNamingPolicy = null;
});

// Mongo
var mongoClient = new MongoClient(mongoConn);
var database = mongoClient.GetDatabase(mongoDbName);
builder.Services.AddSingleton(database);
builder.Services.AddScoped<ITicketRepositorie, MongoTicketRepository>();
builder.Services.AddScoped<TicketService>();

// CORS
builder.Services.AddCors(opts => opts.AddDefaultPolicy(policy =>
{
    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
}));

var app = builder.Build();

app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure indexes
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<ITicketRepositorie>();
    await repo.EnsureIndexesAsync();
}

// Global error handling
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async ctx =>
    {
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred" });
    });
});

// Endpoints
app.MapPost("/api/tickets", async (TicketCreateDto dto, TicketService service) =>
{
    // model validation will be done by framework via attributes if annotated on parameters (in minimal api we need to check)
    if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Length < 3 || dto.Title.Length > 100)
        return Results.BadRequest(new { error = "Title must be 3..100 chars" });

    if (dto.Description?.Length > 500)
        return Results.BadRequest(new { error = "Description max 500 chars" });

    var created = await service.CreateAsync(dto);
    return Results.Created($"/api/tickets/{created.Id}", created);
});

app.MapGet("/api/tickets", async (TicketService service, string? status, int page = 1, int pageSize = 10) =>
{
    TicketStatuts? st = null;
    if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TicketStatuts>(status, true, out var p)) 
        st = p;

    page = Math.Max(1, page);
    pageSize = Math.Clamp(pageSize, 1, 100);

    var (items, total) = await service.ListAsync(st, page, pageSize);
    return Results.Ok(new { items, total, page, pageSize });
});


app.MapGet("/api/tickets/{id}", async (string id, TicketService service) =>
{
    var t = await service.GetByIdAsync(id);
    return t is null ? Results.NotFound() : Results.Ok(t);
});

app.MapPut("/api/tickets/{id}/status", async (string id, Api.Dtos.TicketStatusUpdateDto dto, TicketService service) =>
{
    if (dto.Status == null) return Results.BadRequest(new { error = "Status required" });

    try
    {
        await service.UpdateStatusAsync(id, dto.Status.Value);
        return Results.Ok();
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
});

app.Run();
