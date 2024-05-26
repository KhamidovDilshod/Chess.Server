using System.Text.Json;
using MongoDB.Bson.Serialization.Attributes;

namespace Chess.Core.Persistence.Entities;

public class Board : Entity
{
    public string? StateJson { get; set; } = string.Empty;

    [BsonIgnore]
    public JsonDocument? State
    {
        get => string.IsNullOrEmpty(StateJson) ? null : JsonDocument.Parse(StateJson);
        set => StateJson = value?.RootElement.GetRawText();
    }


    public static Board Create()
    {
        return new Board
        {
            State = null
        };
    }
}