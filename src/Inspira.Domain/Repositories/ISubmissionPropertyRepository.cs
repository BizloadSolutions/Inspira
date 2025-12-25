using System.Collections.Generic;
using System.Threading.Tasks;
using Inspira.Domain.Entities;

namespace Inspira.Domain.Repositories;

public interface ISubmissionPropertyRepository
{
    Task<SubmissionProperty?> GetByIdAsync(int id);
    Task<IEnumerable<SubmissionProperty>> GetAllAsync();
    Task CreateAsync(SubmissionProperty submissionProperty);
    Task UpdateAsync(SubmissionProperty submissionProperty);
    Task DeleteAsync(int id);
}
