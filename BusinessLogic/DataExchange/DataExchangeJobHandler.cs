using BusinessLogic.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange
{
    public class DataExchangeJobHandler : IJobHandler
    {
        private readonly ITaskHandlerDispatcher _handlerDispatcher;

        public DataExchangeJobHandler(ITaskHandlerDispatcher dispatcher)
        {
            _handlerDispatcher = dispatcher;
        }

        public async Task<JobResult> Handle(Job job, CancellationToken cancellationToken)
        {
            var jobResult = new JobResult();
            var jobContext = (jobResult.Output = new JobContext()) as JobContext;
            try
            {
                var config = await File.ReadAllTextAsync(Constants.PathDataExchangeJobsConfig);
                var jobTemplates = JsonConvert.DeserializeObject<List<JobTemplate>>(config);

                var jobTemplate = jobTemplates.Single(x => x.Job == job);
                foreach (var task in jobTemplate.Tasks)
                {
                    var taskHandler = _handlerDispatcher.Dispatch(task.HandlerTypeName);
                    await taskHandler.Handle(task, jobContext);

                    var result = jobContext.Last();
                    if (!result.IsSuccess)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                jobResult.Message = ex.ToString();
            }
            return jobResult;
        }
    }
}
