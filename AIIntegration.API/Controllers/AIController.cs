using Microsoft.AspNetCore.Mvc;
using AIIntegration.Library.Interfaces;
using AIIntegration.Library.Models;
using AIIntegration.Library.Factories;
using AIIntegration.Library.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace AIIntegration.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AIController : ControllerBase
{
    private readonly AIServiceFactory _serviceFactory;

    public AIController(AIServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    /// <summary>
    /// 生成AI响应
    /// </summary>
    /// <param name="request">AI请求对象，包含提示和最大令牌数</param>
    /// <param name="serviceType">AI服务类型，如OpenAI、Claude、DeepSeek等</param>
    /// <param name="streamResponse">是否使用流式响应</param>
    /// <returns>AI响应或流式响应</returns>
    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        [FromBody] AIRequest request,
        [FromQuery, EnumDataType(typeof(AIServiceType))] AIServiceType serviceType,
        [FromQuery] bool streamResponse = false)
    {
        try
        {
            var selectedService = _serviceFactory.CreateService(serviceType);

            if (streamResponse)
            {
                var streamResult = selectedService.GenerateStreamResponseAsync(request, HttpContext.RequestAborted);
                return new StreamingResult(async (stream, httpContext, cancellationToken) =>
                {
                    await foreach (var chunk in streamResult.WithCancellation(cancellationToken))
                    {
                        var bytes = Encoding.UTF8.GetBytes($"data: {chunk}\n\n");
                        await stream.WriteAsync(bytes, cancellationToken);
                        await stream.FlushAsync(cancellationToken);
                    }
                });
            }
            else
            {
                var response = await selectedService.GenerateResponseAsync(request);
                return Ok(response);
            }
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // 添加这个自定义的 StreamingResult 类
    public class StreamingResult : IActionResult
    {
        private readonly Func<Stream, HttpContext, CancellationToken, Task> _callback;

        public StreamingResult(Func<Stream, HttpContext, CancellationToken, Task> callback)
        {
            _callback = callback;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.ContentType = "text/event-stream";
            context.HttpContext.Response.Headers.Add("Cache-Control", "no-cache");
            context.HttpContext.Response.Headers.Add("Connection", "keep-alive");

            await _callback(context.HttpContext.Response.Body, context.HttpContext, context.HttpContext.RequestAborted);
        }
    }
}