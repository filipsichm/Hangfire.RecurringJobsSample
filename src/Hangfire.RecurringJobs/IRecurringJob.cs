using BusinessLogic;
using System.Threading;
using System.Threading.Tasks;

namespace Hangfire.RecurringJobs.Common
{
    public interface IRecurringJob
    {
        Task<JobResult> Execute(RecurringJobTemplate template, CancellationToken cancellationToken);
    }
}
