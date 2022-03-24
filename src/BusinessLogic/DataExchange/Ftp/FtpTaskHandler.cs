using BusinessLogic.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange.Ftp
{
    public class FtpTaskHandler : ITaskHandler
    {
        public FtpTaskHandler()
        {
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
            var taskArgs = args.ToObject<FtpTaskArgs>();

            return (IRecurringTask)Activator.CreateInstance(instanceType, new object[] { taskArgs });
        }
    }
}
