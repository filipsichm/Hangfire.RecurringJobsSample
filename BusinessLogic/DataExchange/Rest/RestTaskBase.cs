using BusinessLogic.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange.Rest
{
    public abstract class RestTaskBase : IRecurringTask
    {
        protected RestTaskArgs Args { get; }
        protected JobContext JobContext { get; set; }
        protected HttpClient Client { get; private set; }
        protected TaskResult Result { get; set; }
        protected bool Break { get; set; }

        public RestTaskBase(HttpClient httpClient, RestTaskArgs args)
        {
            Args = args;
            Client = httpClient;
            Result = new TaskResult(GetType().Name);
        }

        public async Task Run(JobContext jobContext)
        {
            JobContext = jobContext;
            try
            {
                var requests = await GetRequestList();
                foreach (var (req, data) in requests)
                {
                    try
                    {
                        var response = await SendAsync(req);
                        await ProcessResponse(response, data);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }

                    if (Break)
                        break;
                }

                await Postprocess();
            }
            catch (Exception ex)
            {
                Result.Message = ex.ToString();
            }

            JobContext.Add(Result);
        }

        protected virtual Task Postprocess()
        {
            if (!Break)
            {
                Result.IsSuccess = true;
            }
            return Task.CompletedTask;
        }

        protected abstract Task<List<(HttpRequestMessage req, dynamic data)>> GetRequestList();

        protected abstract Task ProcessResponse(HttpResponseMessage response, dynamic data);

        protected virtual bool HandleException(Exception ex) => throw ex;

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await Client.SendAsync(request);
        }

    }
}
