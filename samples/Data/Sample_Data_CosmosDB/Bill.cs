using Newtonsoft.Json;

namespace Sample_Data_CosmosDB;
public class Bill
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("concept")]
    public string Concept { get; set; }

    [JsonProperty("amount")]
    public double Amount { get; set; }
}
