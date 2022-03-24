using BusinessLogic.Common;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange.Soap
{
    public abstract class SoapTaskBase<TChannel, TRequest, TResponse> : IRecurringTask
    {
        protected SoapTaskArgs Args { get; set; }
        protected TChannel Client { get; private set; }
        protected JobContext JobContext { get; private set; }
        protected TaskResult Result { get; set; }
        protected bool Break { get; set; }

        public SoapTaskBase(SoapTaskArgs args)
        {
            Args = args;
        }

        protected abstract Task<IEnumerable<TRequest>> GetRequestList();

        protected abstract Task ProcessResponse(TResponse response, TRequest request);

        protected abstract Task<TResponse> SendRequest(TRequest request);

        protected virtual Binding GetBinding()
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = binding.MaxBufferPoolSize = binding.MaxBufferSize = int.MaxValue;
            binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            return binding;
        }

        public async Task Run(JobContext jobContext)
        {
            JobContext = jobContext;
            ChannelFactory<TChannel> factory = null;
            try
            {
                factory = new ChannelFactory<TChannel>(GetBinding(), new EndpointAddress(Args.Uri));
                Client = factory.CreateChannel();

                var requests = await GetRequestList();
                foreach (var req in requests)
                {
                    try
                    {
                        var response = await SendRequest(req);
                        await ProcessResponse(response, req);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }

                    if (Break)
                        break;
                }
                factory.Close();
                await Postprocess();
            }
            catch (Exception ex)
            {
                Result.Message = ex.ToString();
                factory?.Abort();
            }

            jobContext.Add(Result);
        }

        protected virtual Task Postprocess()
        {
            if (!Break)
            {
                Result.IsSuccess = true;
            }
            return Task.CompletedTask;
        }

        protected virtual bool HandleException(Exception ex) => throw ex;
    }
}
