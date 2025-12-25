using System.Collections.Generic;
using System.Threading.Tasks;
using Inspira.Domain.Entities;

namespace Inspira.Domain.Repositories;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(int id);
    Task<IEnumerable<Submission>> GetAllAsync();
    Task CreateAsync(Submission submission);
    Task UpdateAsync(Submission submission);
    Task DeleteAsync(int id);
}
