using Syncano;
using Newtonsoft.Json;

public class Record : SyncanoObject {

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("score")]
	public int Score { get; set; }

	public Record() { }

	public Record(string name, int score) { Name = name; Score = score; }
}