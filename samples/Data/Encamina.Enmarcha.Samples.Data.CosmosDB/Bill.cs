using Newtonsoft.Json;

namespace Encamina.Enmarcha.Samples.Data.CosmosDB;

internal class Bill
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("concept")]
    public string Concept { get; set; }

    [JsonProperty("amount")]
    public double Amount { get; set; }
}
