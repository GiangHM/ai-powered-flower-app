using FlowerShop.Application.Dtos.ChatFeature;
using FlowerShop.Infrastructure.Agent;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlowerShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly WriterAgent _agent;
        private readonly IDeserializer _yamlDeserializer;

        public ChatController(WriterAgent agent) 
        {
            _agent = agent;
            _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        }

        [HttpPost("stream")]
        [Consumes("application/json")]
        public async Task ProcessStreamingMessage(AIChatRequest request)
        {
            var response = Response;
            response.Headers.Append("Content-Type", "application/x-ndjson");

            var agentService = _agent.CreateWriterAgentService();
            
            try
            {
                var userInput = request.Messages.Last();
                CreateWriterRequest createWriterRequest = _yamlDeserializer.Deserialize<CreateWriterRequest>(userInput.Content);
                await foreach (var delta in agentService.ProcessStreamingRequest(createWriterRequest))
                {
                    await response.WriteAsync($"{JsonSerializer.Serialize(delta)}\r\n");
                    await response.Body.FlushAsync();
                }
            }
            catch (YamlException ex)
            {
                var delta = new AIChatCompletionDelta(Delta: new AIChatMessageDelta
                {
                    Role = AIChatRole.System,
                    Content = "Error: Invalid YAML format, Details:  \n" + ex,
                });
                await response.WriteAsync($"{JsonSerializer.Serialize(delta)}\r\n");
                await response.Body.FlushAsync();
            }
            finally
            {
            }
        }
    }
}
