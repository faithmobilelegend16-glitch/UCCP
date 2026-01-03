using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Soil.Shared.Models.FinancialModel;

public class FinancialTransaction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId _id { get; set; } = ObjectId.GenerateNewId();

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("category")]
    public string Category { get; set; }

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("transactionDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime TransactionDate { get; set; }

    [BsonElement("paymentMethod")]
    public string PaymentMethod { get; set; }

    [BsonElement("transactionType")]
    public string TransactionType { get; set; }

    [BsonElement("notes")]
    public string Notes { get; set; }

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; }

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

public class CreateFinancialTransactionDto
{
    public string Description { get; set; }
    public string Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionType { get; set; }
    public string Notes { get; set; }
}

public class UpdateFinancialTransactionDto
{
    public string Description { get; set; }
    public string Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionType { get; set; }
    public string Notes { get; set; }
}

public class FinancialSummary
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public int TotalTransactions { get; set; }
    public int IncomeCount { get; set; }
    public int ExpenseCount { get; set; }
    public decimal AverageTransaction { get; set; }
}

public class MonthlySummary
{
    public string Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
}