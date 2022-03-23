using Newtonsoft.Json;

namespace BusinessLogic
{
    public class TaskResult
    {
        public string Source { get; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        [JsonIgnore]
        public dynamic Data { get; set; }

        public TaskResult(string source)
        {
            Source = source;
        }
    }
}
