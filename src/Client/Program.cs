using Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel; 
using JsonTooling.Functionality;
using JsonTooling;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;

IHost consoleHost = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddTransient<Main>();
    })
    .Build();

Main main = consoleHost.Services.GetRequiredService<Main>();
await main.ExecuteAsync(args);

class Main
{
    ILogger<Main> _logger;
    
    string _apiEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "<<Provide your Azure API Endpoint here>>";
    string _apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "<<Provide your Azure API Key here>>";
    string _modelDeploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENTNAME") ?? "<<Provide your Model Deployment Name here>>";

    private string _jsonSchema = @"{
        ""$schema"": ""http://json-schema.org/draft-07/schema#"",
        ""type"": ""object"",
        ""properties"": {
            ""name"": { ""type"": ""string"" },
            ""sentiment"": { ""type"": ""string""}
        },
        ""required"": [""Name"", ""Sentiment""]
    }";

    public Main (ILogger<Main> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(string[] args)
    {
        var kernelBuilder = new KernelBuilder()
            .WithAzureChatCompletionService(
                apiKey: _apiKey, 
                endpoint: _apiEndpoint, 
                deploymentName: _modelDeploymentName
        );
        IKernel kernel = kernelBuilder.Build();

        //Import Semantic Skills from directory
        string skillsDirectory = Path.Combine(Directory.GetCurrentDirectory(),  @".\..\SkillsLibrary\SemanticSkills"); 
        kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "IdentifySentiment");
        kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "ExtractEntities"); 

        //Import native Skills
        var jsonTooling = new JsonToolingFunctionality();
        var skill = new JsonToolingSkill(jsonTooling); 
        var jsonSkill = kernel.ImportSkill(skill, "JsonTooling");

        await ExecuteSkills(kernel); 
    }

    internal async Task ExecuteSkills(IKernel kernel)
    {
        //Execute Semantic Function 
        string input = "I have a complaint! I want to speak to a manager!";
        string skill = "IdentifySentiment";
        string function = "SimpleSentiment";

        ISKFunction sKFunction = kernel.Skills.GetFunction(skill, function); 
        ContextVariables contextVariables = new ContextVariables(input); 
        string jsonSentiment = (await kernel.RunAsync(contextVariables, sKFunction)).Result;
        Console.WriteLine(jsonSentiment);

        input = "It's about my order with the number 4711 and I'm John from Denver.";
        skill = "ExtractEntities";
        function = "PersonalInformation";
        sKFunction = kernel.Skills.GetFunction(skill, function); 

        contextVariables = new ContextVariables(input); 
        string jsonEntities = (await kernel.RunAsync(contextVariables, sKFunction)).Result;
        Console.WriteLine(jsonEntities);

        //Execute Native Function(s)
        skill = "JsonTooling";
        function = "CombineJsonInfoAsync";

        contextVariables = new ContextVariables(jsonSentiment);
        contextVariables.Set(JsonTooling.JsonToolingSkill.Parameters.jsonContent2, jsonEntities); 
        sKFunction = kernel.Skills.GetFunction(skill, function); 
        string combinedJson = (await kernel.RunAsync(contextVariables, sKFunction)).Result;
        Console.WriteLine(combinedJson);

        contextVariables = new ContextVariables(combinedJson);
        contextVariables.Set(JsonTooling.JsonToolingSkill.Parameters.jsonSchemaDefinition, _jsonSchema);

        skill = "JsonTooling";
        function = "ValidateJsonAgainstSchemaAsync";
        sKFunction = kernel.Skills.GetFunction(skill, function); 
        string validJson = (await kernel.RunAsync(contextVariables, sKFunction)).Result;
        Console.WriteLine(validJson);
    }
}