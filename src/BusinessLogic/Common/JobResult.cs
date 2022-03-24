using System.Collections.Generic;

namespace BusinessLogic
{
    public class JobResult
    {
        public string Message { get; set; }
        public IEnumerable<TaskResult> Output { get; set; }
    }
}
