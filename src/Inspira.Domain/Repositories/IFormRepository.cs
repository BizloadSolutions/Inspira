using System.Collections.Generic;
using System.Threading.Tasks;
using Inspira.Domain.Entities;

namespace Inspira.Domain.Repositories;

public interface IFormRepository
{
    Task<Form?> GetByIdAsync(int id);
    Task<IEnumerable<Form>> GetAllAsync();
    Task CreateAsync(Form form);
    Task UpdateAsync(Form form);
    Task DeleteAsync(int id);
}
