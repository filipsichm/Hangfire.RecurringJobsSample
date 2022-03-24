using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface IJobHandler
    {
        Task<JobResult> Handle(Job job, CancellationToken cancellationToken);
    }
}
