using Newtonsoft.Json;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace DataComparisonEngine;

public class BaseReport
{
    [JsonProperty(Order = 0)]
    public bool Pass { get; set; } = true;

    [JsonProperty(Order = 1)]
    public string TargetName { get; set; } = "";

    [JsonProperty(Order = 2)]
    public string SourceName { get; set; } = "";

    [JsonProperty(Order = 3)]
    public int FailsCount => Fails.Count;

    [JsonProperty(Order = 4)]
    public int WarningsCount => Warnings.Count;

    [JsonProperty(Order = 5)]
    public int ErrorsCount => Errors.Count;

    [JsonProperty(Order = 6)]
    public int JoinedCount { get; set; } = 0;

    [JsonProperty(Order = 7)]
    public int TargetCount { get; set; } = 0;

    [JsonProperty(Order = 7)]
    public int SourceCount { get; set; } = 0;

    [JsonProperty(Order = 8)]
    public int TargetOnlyCount => TargetOnly.Count;

    [JsonProperty(Order = 9)]
    public int SourceOnlyCount => SourceOnly.Count;

    [JsonProperty(Order = 10)]
    public List<string> Errors { get; set; } = new();

    [JsonProperty(Order = 11)]
    public List<string> Warnings { get; set; } = new();

    [JsonProperty(Order = 12)]
    public List<string> Fails = new();

    [JsonProperty(Order = 13)]
    public List<string> TargetOnly = new();

    [JsonProperty(Order = 14)]
    public List<string> SourceOnly = new();

    [JsonIgnore]
    public Dictionary<object, int> JoinDuplicates = new();

    [JsonProperty(Order = 15)]
    public Dictionary<object, int> Duplicates
    {
        get
        {
            var uniqueDupes = new Dictionary<object, int>();
            foreach (var key in JoinDuplicates.Keys)
            {
                var count = JoinDuplicates[key];
                if (count > 1)
                {
                    if (!uniqueDupes.ContainsKey(key))
                    {
                        uniqueDupes.Add(key, count);
                    }
                    else
                    {
                        uniqueDupes[key] = count;
                    }
                }
            }

            return uniqueDupes;
        }
    }

    [JsonProperty(Order = 16)]
    public List<string> ComparedColumns = new();
}