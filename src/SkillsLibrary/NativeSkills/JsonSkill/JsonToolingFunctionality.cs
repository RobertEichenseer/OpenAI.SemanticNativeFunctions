namespace JsonTooling.Functionality;

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema; 

public class JsonToolingFunctionality : IJsonToolingFunctionality
{
    
    public async Task<string> CombineJsonContent(string jsonContent1, string jsonContent2) {

        //Function marked as async for future async implementation
        JObject json1 = JObject.Parse(jsonContent1);
        JObject json2 = JObject.Parse(jsonContent2);
        await Task.Run( () => {
            json1.Merge(json2, new JsonMergeSettings{MergeArrayHandling = MergeArrayHandling.Union});
        });
        return JsonConvert.SerializeObject(json1);
    }

    public async Task<string> CheckJsonSchema(string jsonContent, string schema) {

        //Function marked as async for future async implementation
        (JObject jObject, JSchema jSchema) = await Task.Run(() => {
            jObject = JObject.Parse(jsonContent);
            jSchema = JSchema.Parse(schema);
            return (jObject, jSchema); 
        });
        return jObject.IsValid(jSchema).ToString();
    }
}
