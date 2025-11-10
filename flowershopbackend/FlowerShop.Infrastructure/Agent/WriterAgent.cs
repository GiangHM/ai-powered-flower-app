using FlowerShop.Infrastructure.Agent.AgentPlugins;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FlowerShop.Infrastructure.Agent
{
    public sealed class WriterAgent
    {
        private readonly Kernel _defaultKernel;
        private readonly IOptions<GoogleTextSearchSettings> _settings;
        public WriterAgent(Kernel kernel, IOptions<GoogleTextSearchSettings> settings)
        {
            _defaultKernel = kernel;
            _settings = settings;
        }
        public WriterService CreateWriterAgentService()
        {
            var researcherKernel = _defaultKernel.Clone();
            GoogleTextSearchPlugin googleSearchPlugin = new GoogleTextSearchPlugin(_settings);
            researcherKernel.Plugins.Clear();
            researcherKernel.Plugins.AddFromObject(googleSearchPlugin);

            ChatCompletionAgent researcherAgent = new(ReadFileForPromptTemplateConfig("./Agent/Prompt/researcher.yaml"), templateFactory: new KernelPromptTemplateFactory())
            {
                Name = "Researcher",
                Kernel = researcherKernel,
                Arguments = new KernelArguments(new OpenAIPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Required(),
                }),
                LoggerFactory = _defaultKernel.LoggerFactory
            };

            ChatCompletionAgent writerAgent = new(ReadFileForPromptTemplateConfig("./Agent/Prompt/writer.yaml"), templateFactory: new KernelPromptTemplateFactory())
            {
                Name = "Writer",
                Kernel = _defaultKernel,
                Arguments = [],
                LoggerFactory = _defaultKernel.LoggerFactory
            };

            return new WriterService(researcherAgent, writerAgent);
        }

        private static PromptTemplateConfig ReadFileForPromptTemplateConfig(string fileName)
        {
            string yaml = File.ReadAllText(fileName);
            return KernelFunctionYaml.ToPromptTemplateConfig(yaml);
        }
    }
}
