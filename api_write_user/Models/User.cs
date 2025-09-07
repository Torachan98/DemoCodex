using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiWriteUser.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("username")]
    public string Username { get; set; } = default!;

    [BsonElement("email")]
    public string Email { get; set; } = default!;

    [BsonElement("is_active")]
    public bool IsActive { get; set; }

    [BsonElement("isSync")]
    public bool IsSync { get; set; }

    [BsonElement("dateCreated")]
    public DateTime DateCreated { get; set; }

    [BsonElement("dateUpdated")]
    public DateTime DateUpdated { get; set; }

    [BsonElement("dateSynced")]
    public DateTime? DateSynced { get; set; }
}

public class UserCreate
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
}

