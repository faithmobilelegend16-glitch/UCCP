using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Soil.Shared.Models.SaleModel;

public class Sale
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId _id { get; set; } = ObjectId.GenerateNewId();

    [BsonElement("productName")]
    public string ProductName { get; set; }

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("category")]
    public string Category { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("saleDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Helper property for API responses
    [BsonIgnore]
    public string Id
    {
        get => _id.ToString();
        set
        {
            if (ObjectId.TryParse(value, out var oid))
            {
                _id = oid;
            }
        }
    }
}

public class SalesStats
{
    [BsonElement("_id")]
    public string Id { get; set; }

    [BsonElement("label")]
    public string Label { get; set; }

    [BsonElement("totalSales")]
    public decimal TotalSales { get; set; }

    [BsonElement("totalQuantity")]
    public int TotalQuantity { get; set; }

    [BsonElement("count")]
    public int Count { get; set; }
}

public class CreateSaleDto
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
}

public class UpdateSaleDto
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public DateTime SaleDate { get; set; }
}