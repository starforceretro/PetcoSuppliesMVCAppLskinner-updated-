namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class Payment
    {
        public int Id { get; set; } // Primary key for the payment

        public int OrderId { get; set; } // Foreign key to the order this payment is associated with

        public Order Order { get; set; } // Navigation property to the order this payment is associated with

        public string PaymentMethod { get; set; } // e.g., Credit Card, PayPal, etc.

        public string PaymentStatus { get; set; } // e.g., Pending, Completed, Failed

        public DateTime PaymentDate { get; set; } // Date when the payment was made
    }
}
