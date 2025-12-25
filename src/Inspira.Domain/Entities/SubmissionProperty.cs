using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Inspira.Domain.Entities;

[BsonIgnoreExtraElements]
public sealed class SubmissionProperty
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int SubmissionPropertyId { get; set; }

    [BsonElement("owner: tax id")]
    public string OwnerTaxId { get; set; } = string.Empty;

    [BsonElement("owner: contact id")]
    public string OwnerContactId { get; set; } = string.Empty;
}
