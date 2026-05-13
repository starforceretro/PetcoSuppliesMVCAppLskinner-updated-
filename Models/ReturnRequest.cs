namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class ReturnRequest
    {
        
        public int Id { get; set; } // Primary key for the return request

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; }

        public DateTime RequestDate { get; set; } // Date when the return request was made
    }
}
