using AIIntegration.Interfaces;
using AIIntegration.Models;
using AIIntegration.Services.OpenAI;
using AIIntegration.Services.Claude;
using AIIntegration.Services.DeepSpeek;
using AIIntegration.Services.Qwen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AI services
builder.Services.AddSingleton(new AIServiceConfig
{
    ApiUrl = builder.Configuration["OpenAI:ApiUrl"]!,
    ApiKey = builder.Configuration["OpenAI:ApiKey"]!,
    Model = builder.Configuration["OpenAI:Model"]!
});

builder.Services.AddHttpClient<IAIService, OpenAIService>();
builder.Services.AddHttpClient<IAIService, ClaudeService>();
builder.Services.AddHttpClient<IAIService, DeepSpeekService>();
builder.Services.AddHttpClient<IAIService, QwenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();