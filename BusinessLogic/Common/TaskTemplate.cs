using Newtonsoft.Json.Linq;

namespace BusinessLogic
{
    public class TaskTemplate
    {
        public string Source { get; set; }
        public string HandlerTypeName { get; set; }
        public JObject TaskArguments { get; set; }
    }
}
