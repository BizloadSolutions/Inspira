using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Inspira.Domain.Entities;

[BsonIgnoreExtraElements]
public sealed class Submission
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int SubmissionId { get; set; }

    [BsonElement("SubmissionPropertyId")]
    [BsonRepresentation(BsonType.Int32)]
    public int SubmissionPropertyId { get; set; }

    [BsonElement("FormId")]
    [BsonRepresentation(BsonType.Int32)]
    public int FormId { get; set; }
}
