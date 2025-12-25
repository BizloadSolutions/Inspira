using System.Threading.Tasks;

namespace Inspira.Application.Services;

public interface IMtcSoapClient
{
    Task<int> SsnInternalCheckAsync(string ssn, int role);
}
