using System.Threading.Tasks;

namespace Inspira.Application.Services;

public interface ISsnCheckService
{
    Task<string> SsnCheckAsync(int? submissionId, string ssn, string role);
}
