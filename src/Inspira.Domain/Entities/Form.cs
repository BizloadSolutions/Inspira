using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Inspira.Domain.Entities;

[BsonIgnoreExtraElements]
public sealed class Form
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int FormId { get; set; }

    [BsonElement("FormName")]
    public string FormName { get; set; } = string.Empty;
}
