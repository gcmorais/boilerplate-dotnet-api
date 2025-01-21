using Project.Domain.Entities;

namespace Project.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task Create(T entity);
        Task Update(T entity);
        Task<T> GetById(Guid id, CancellationToken cancellationToken);
        Task<List<T>> GetAll(CancellationToken cancellationToken);
    }
}
