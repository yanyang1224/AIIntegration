using Microsoft.AspNetCore.Mvc;
using AIIntegration.Library.Interfaces;
using AIIntegration.Library.Models;
using AIIntegration.Library.Factories;
using AIIntegration.Library.Enums;
using System;
using System.ComponentModel.DataAnnotations;

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
    /// <returns>包含生成文本和使用的令牌数的AI响应</returns>
    [HttpPost("generate")]
    public async Task<ActionResult<AIResponse>> Generate(
        [FromBody] AIRequest request, 
        [FromQuery, EnumDataType(typeof(AIServiceType))] AIServiceType serviceType)
    {
        try
        {
            var selectedService = _serviceFactory.CreateService(serviceType);
            var response = await selectedService.GenerateResponseAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// 生成流式AI响应
    /// </summary>
    /// <param name="request">AI请求对象，包含提示和最大令牌数</param>
    /// <param name="serviceType">AI服务类型，如OpenAI、Claude、DeepSeek等</param>
    /// <returns>包含生成文本片段的流式响应</returns>
    [HttpPost("generate-stream")]
    public IActionResult GenerateStream(
        [FromBody] AIRequest request, 
        [FromQuery, EnumDataType(typeof(AIServiceType))] AIServiceType serviceType)
    {
        try
        {
            var selectedService = _serviceFactory.CreateService(serviceType);
            return Ok(selectedService.GenerateStreamResponseAsync(request, HttpContext.RequestAborted));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}