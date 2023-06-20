namespace JsonTooling.Functionality;

public interface IJsonToolingFunctionality
{
    public Task<string> CombineJsonContent(string file1, string file2);
    public Task<string> CheckJsonSchema(string jsonContent, string schema); 
}