using System.Threading.Tasks;

namespace ProductService.Application.Interfaces
{
    public interface IProductEventPublisher
    {
        Task PublishProductCreatedEvent(System.Guid productId, string name, decimal price);
        Task PublishProductUpdatedEvent(System.Guid productId, string name, decimal price);
        Task PublishProductDeletedEvent(System.Guid productId);
    }
}
