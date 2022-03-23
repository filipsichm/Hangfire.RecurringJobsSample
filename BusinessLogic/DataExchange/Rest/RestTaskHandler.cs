using BusinessLogic.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange.Rest
{
    public class RestTaskHandler : ITaskHandler
    {
        private readonly HttpClient _client;
        public RestTaskHandler(HttpClient client)
        {
            _client = client;
        }

        public async Task Handle(TaskTemplate template, JobContext jobContext)
        {
            try
            {
                var source = InitializeSource(template.Source, template.TaskArguments);
                await source.Run(jobContext);
            }
            catch (Exception ex)
            {
                jobContext.Add(new TaskResult(template.Source) { Message = ex.ToString() });
            }
        }

        public IRecurringTask InitializeSource(string source, JObject args)
        {
            var instanceType = Type.GetType(source);
            var taskArgs = args.ToObject<RestTaskArgs>();

            return (IRecurringTask)Activator.CreateInstance(instanceType, new object[] { _client, taskArgs });
        }
    }
}
