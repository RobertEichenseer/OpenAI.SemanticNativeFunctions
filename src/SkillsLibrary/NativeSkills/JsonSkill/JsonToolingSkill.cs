using Microsoft.Extensions.Logging; 
using Microsoft.Extensions.Logging.Abstractions; 
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using JsonTooling.Functionality; 

namespace JsonTooling;

public class JsonToolingSkill {

    public static class Parameters {
        public const string jsonContent1 = "jsonContent1";
        public const string jsonContent2 = "jsonContent2";
        public const string jsonSchemaDefinition = "jsonSchema";
    }

    IJsonToolingFunctionality _jsonTooling;
    ILogger<JsonToolingSkill> _logger;  

    public JsonToolingSkill(IJsonToolingFunctionality jsonTooling, ILogger<JsonToolingSkill>? logger = null) {
        _jsonTooling = jsonTooling; 
        _logger = logger ?? new NullLogger<JsonToolingSkill>(); 
    }

    [SKFunction("Merges JSON information from two strings into one string")]
    [SKFunctionInput(Description = "1st JSON content")]
    [SKFunctionContextParameter(Name = Parameters.jsonContent2, Description = "2nd JSON content")]
    public async Task<string> CombineJsonInfoAsync(string jsonContent1, SKContext skContext)
    {
        skContext.Variables.TryGetValue(Parameters.jsonContent2, out string? jsonContent2);
        
        string result = await _jsonTooling.CombineJsonContent(
            Environment.ExpandEnvironmentVariables(jsonContent1),
            Environment.ExpandEnvironmentVariables(jsonContent2 ?? "")
        ); 
        return result;
    }

    [SKFunction("Validates a JSON string against a JSON schema")]
    [SKFunctionInput(Description = "JSON string")]
    [SKFunctionContextParameter(Name = Parameters.jsonSchemaDefinition, Description = "JSON schema definition")]
    public async Task<string> ValidateJsonAgainstSchemaAsync(string jsonContent1, SKContext skContext)
    {

        skContext.Variables.TryGetValue(Parameters.jsonSchemaDefinition, out string? jsonSchemaDefinition);
        
        string result = await _jsonTooling.CheckJsonSchema(
            Environment.ExpandEnvironmentVariables(jsonContent1),
            Environment.ExpandEnvironmentVariables(jsonSchemaDefinition ?? "{}")
        ); 

        return result;
    }
}