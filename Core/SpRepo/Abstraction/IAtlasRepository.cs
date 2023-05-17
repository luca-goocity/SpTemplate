using SpRepo.Models.Entities;

namespace SpRepo.Abstraction;

public interface IAtlasRepository<T> : IRepository<T> where T : BaseEntity
{
}