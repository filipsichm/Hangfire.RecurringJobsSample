using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic
{
    public class TaskHandlerDispatcher : ITaskHandlerDispatcher
    {
        private readonly IEnumerable<ITaskHandler> _taskHandlers;

        public TaskHandlerDispatcher(IEnumerable<ITaskHandler> taskHandlers)
        {
            _taskHandlers = taskHandlers;
        }

        public ITaskHandler Dispatch(string handlerTypeName)
        {
            return _taskHandlers.First(x => x.GetType().Name == handlerTypeName);
        }
    }
}
