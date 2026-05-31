using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class ReturnRequest
    {
        public int Id { get; set; } // unique identifier for each return request

        public int OrderId { get; set; } // foreign key to the associated order for the return request

        public Order? Order { get; set; } // navigation property to the associated order for the return request, allowing access to order details and user information related to the return request

        public int ProductId { get; set; } // foreign key to the associated product for the return request

        public Product? Product { get; set; } //    navigation property to the associated product for the return request, allowing access to product details related to the return request

        public string Reason { get; set; } // reason for the return request, which is required and must be between 10 and 500 characters long, providing necessary information for processing the return and understanding the customer's issue with the product

        public string? Status { get; set; } //  status of the return request, which is optional and can be used to track the progress of the return (e.g., "Pending", "Approved", "Rejected"), allowing both customers and administrators to monitor the state of the return request

        public DateTime RequestDate { get; set; } // date and time when the return request was submitted, which is required and helps to track the timeline of the return process, including when the request was made and how long it has been pending or processed
    }
}
