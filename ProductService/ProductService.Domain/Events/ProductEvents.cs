using MediatR;

namespace ProductService.Domain.Events
{
    public class ProductCreatedEvent : INotification
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class ProductUpdatedEvent : INotification
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ProductDeletedEvent : INotification
    {
        public Guid ProductId { get; set; }
        public DateTime DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
