using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Soil.Shared.Models.InventoryModel;

public class SignClass
{
    
}

public class UserAccount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }  // Let MongoDB generate automatically

    public string FullName { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public string Role { get; set; } = "User";
}

public class LoginResponse
{
    public string Token { get; set; }
}


public class SignUpDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}


public class SignInDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
