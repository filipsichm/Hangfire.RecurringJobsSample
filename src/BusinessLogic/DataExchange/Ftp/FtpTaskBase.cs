using BusinessLogic.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange.Ftp
{
    public abstract class FtpTaskBase : IRecurringTask
    {
        protected FtpTaskArgs Args { get; set; }
        protected FtpClient Client { get; private set; }
        protected JobContext JobContext { get; private set; }
        protected TaskResult Result { get; set; }
        protected bool Break { get; set; }

        protected string RequestBaseUri => Path.Combine(Args.Host, Args.Directory ?? "");

        public FtpTaskBase(FtpTaskArgs args)
        {
            Args = args;
            Client = new FtpClient(new NetworkCredential(Args.Username, Args.Password));
            Result = new TaskResult(GetType().Name);
        }

        public async Task Run(JobContext jobContext)
        {
            JobContext = jobContext;
            try
            {
                var requests = await GetRequestList();
                foreach (var req in requests)
                {
                    await SendRequest(req);

                    if (Break)
                        break;
                }

                await Postprocess();
            }
            catch (Exception ex)
            {
                Result.Message = ex.ToString();
            }

            jobContext.Add(Result);
        }

        protected virtual Task<IEnumerable<string>> GetRequestList()
        {
            return Task.FromResult<IEnumerable<string>>(Args.Filename.Split(","));
        }

        protected abstract Task SendRequest(string filename);

        protected virtual Task Postprocess()
        {
            if (!Break)
            {
                Result.IsSuccess = true;
            }
            return Task.CompletedTask;
        }
    }
}
