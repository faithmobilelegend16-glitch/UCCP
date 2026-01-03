using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Soil.Shared.Models.PurchaseRequests
{
    public class PrClass
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int BlackPearl { get; set; }
        public int PowderFlavor { get; set; }
        public int CupSmall { get; set; }
        public int CupMedium { get; set; }
        public int CupLarge { get; set; }
        public int Straw { get; set; }
        public int Ice { get; set; }
        public int WaterGallon { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime RequestDate { get; set; }
    }
}