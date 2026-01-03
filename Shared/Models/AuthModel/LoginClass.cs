using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PPDIS.Shared.Models.AuthModel;

public class LoginClass
{
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("Email")]
    public string email { get; set; } = string.Empty;
    
    [BsonElement("Password")]
    public string password { get; set; } = string.Empty;
    
    [BsonElement("Role")]
    public string role { get; set; } = string.Empty;

    
    
}