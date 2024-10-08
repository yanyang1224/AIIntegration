using AIIntegration.Library.Interfaces;
using AIIntegration.Library.Models;
using AIIntegration.Library.Factories;
using AIIntegration.Library.Services.OpenAI;
using AIIntegration.Library.Services.Claude;
using AIIntegration.Library.Services.DeepSeek;
using AIIntegration.Library.Services.Qwen;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using AIIntegration.API.Filters;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:7001") // Replace with your frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AI Integration API", Version = "v1" });
    
    // 使用枚举的字符串值
    c.SchemaFilter<EnumSchemaFilter>();

    // 设置 XML 注释文件路径
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Configure AI services
builder.Services.AddSingleton<AIServiceConfig>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new AIServiceConfig
    {
        OpenAI = configuration.GetSection("OpenAI").Get<ServiceConfig>()!,
        Claude = configuration.GetSection("Claude").Get<ServiceConfig>()!,
        DeepSeek = configuration.GetSection("DeepSeek").Get<ServiceConfig>()!,
        Qwen = configuration.GetSection("Qwen").Get<ServiceConfig>()!
    };
});

builder.Services.AddSingleton<AIServiceFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Integration API v1"));
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors();

// Use response compression
app.UseResponseCompression();

app.UseAuthorization();
app.MapControllers();

app.Run();