using PetcoSuppliesMVCAppLskinner.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PetcoSuppliesMVCAppLskinner.Services
{
    public class InvoiceService
    {
        public byte[] GenerateInvoice(Order order) // method to generate a PDF invoice for a given order, accepts an Order object as a parameter, and returns the generated PDF as a byte array that can be sent to the client for download or display
        {
            QuestPDF.Settings.License = LicenseType.Community; // set the license type for QuestPDF to Community, which allows for free usage of the library with some limitations, such as a watermark on generated PDFs, but is suitable for development and testing purposes without requiring a commercial license

            var pdf = Document.Create(container => // create a new PDF document using QuestPDF, with a container that defines the layout and content of the PDF
            {
                container.Page(page =>
                {
                    // PAGE SETTINGS

                    page.Margin(40);

                    // HEADER

                    page.Header()
                        .Text("Petco Supplies Invoice")
                        .FontSize(24)
                        .Bold();

                    // CONTENT

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        // ORDER INFO

                        column.Item().Text($"Order ID: {order.Id}");

                        column.Item().Text(
                            $"Date: {order.OrderDate:dd/MM/yyyy}");

                        column.Item().Text(
                            $"Status: {order.Status}");

                        // CUSTOMER INFO

                        column.Item().Text(
                            $"Customer: {order.User.FirstName} {order.User.LastName}");

                        column.Item().Text(
                            $"Email: {order.User.Email}");

                        column.Item().Text(
                            $"Address: {order.User.Street}, " +
                            $"{order.User.Town}, " +
                            $"{order.User.PostCode}");

                        column.Item().PaddingTop(20);

                        // PRODUCTS TABLE

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            // HEADER

                            table.Header(header =>
                            {
                                header.Cell().Text("Product").Bold();

                                header.Cell().Text("Price").Bold();

                                header.Cell().Text("Qty").Bold();

                                header.Cell().Text("Total").Bold();
                            });

                            // ITEMS

                            foreach (var item in order.OrderItems)
                            {
                                table.Cell().Text(item.Product.Name);

                                table.Cell().Text(
                                    $"£{item.Price}");

                                table.Cell().Text(
                                    item.Quantity.ToString());

                                table.Cell().Text(
                                    $"£{item.Price * item.Quantity}");
                            }
                        });

                        // TOTAL

                        column.Item()
                            .PaddingTop(20)
                            .Text($"Total Amount: £{order.TotalAmount}")
                            .FontSize(18)
                            .Bold();
                    });

                    // FOOTER

                    page.Footer()
                        .AlignCenter()
                        .Text("Thank you for shopping with Petco Supplies!");
                });
            }).GeneratePdf();

            return pdf; // return the generated PDF as a byte array, which can be sent to the client for download or display, allowing customers to receive a professional-looking invoice for their orders
        }
    }
}