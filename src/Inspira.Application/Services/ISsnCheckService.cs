using System.Threading.Tasks;

namespace Inspira.Application.Services;

public interface ISsnCheckService
{
    Task<SsnCheckServiceResult> SsnCheckAsync(int? submissionId, string ssn, string role);
}
