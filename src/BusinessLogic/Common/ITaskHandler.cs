using BusinessLogic.Common;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface ITaskHandler
    {
        Task Handle(TaskTemplate template, JobContext jobContext);
    }
}
