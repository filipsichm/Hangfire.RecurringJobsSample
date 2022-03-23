using BusinessLogic.Common;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface IRecurringTask
    {
        Task Run(JobContext jobContext);
    }
}
