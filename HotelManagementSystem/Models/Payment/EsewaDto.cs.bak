using System.Text.Json.Serialization;

namespace HotelManagementSystem.Models.Payment
{
    public class EsewaInitiateResponseDto
    {
        public string Amount { get; set; } = string.Empty;
        public string TaxAmount { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public string ProductServiceCharge { get; set; } = "0";
        public string ProductDeliveryCharge { get; set; } = "0";
        public string TransactionUuid { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string SuccessUrl { get; set; } = string.Empty;
        public string SignedFieldNames { get; set; }
        public string FailureUrl { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
    }

    public class EsewaCallbackDecodedData
    {
        public string transaction_code { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        // Allow reading numeric values that may be encoded as JSON strings
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? total_amount { get; set; }
        public string transaction_uuid { get; set; } = string.Empty;
        public string product_code { get; set; } = string.Empty;
        public string signed_field_names { get; set; } = string.Empty;
        public string signature { get; set; } = string.Empty;
    }

    public class EsewaStatusApiResponse
    {
        public string status { get; set; } = string.Empty;
        public string product_code { get; set; } = string.Empty;
        public string ref_id { get; set; } = string.Empty;
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? total_amount { get; set; }
        public string transaction_uuid { get; set; } = string.Empty;
    }
}
