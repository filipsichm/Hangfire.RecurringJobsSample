using BusinessLogic;
using BusinessLogic.DataExchange;
using Hangfire.RecurringJobs.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Hangfire.RecurringJobs
{
    public class DataExchangeJob : IRecurringJob
    {
        private readonly DataExchangeJobHandler _jobHandler;
        public DataExchangeJob(DataExchangeJobHandler jobHandler)
        {
            _jobHandler = jobHandler;
        }

        public async Task<JobResult> Execute(RecurringJobTemplate template, CancellationToken cancellationToken)
        {
            return await _jobHandler.Handle(template.Id, cancellationToken);
        }
    }
}
