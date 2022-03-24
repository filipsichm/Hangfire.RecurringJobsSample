using BusinessLogic.DataExchange.Soap.References;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange.Soap.Tasks
{
    public class SoapRequestExample : SoapTaskBase<CalculatorSoap, (int a, int b), int>
    {
        public SoapRequestExample(SoapTaskArgs args) : base(args)
        {
        }

        protected override Binding GetBinding()
        {
            return CalculatorSoapClient.GetBindingForEndpoint(CalculatorSoapClient.EndpointConfiguration.CalculatorSoap);
        }

        protected override Task<IEnumerable<(int a, int b)>> GetRequestList()
        {
            var requests = (IEnumerable<(int, int)>)new List<(int, int)> { (1, 1) };
            return Task.FromResult(requests);
        }

        protected override async Task<int> SendRequest((int a, int b) request)
        {
            return await Client.AddAsync(request.a, request.b);
        }

        protected override Task ProcessResponse(int response, (int a, int b) req)
        {
            Console.WriteLine($"{req.a} + {req.b} = {response}");
            return Task.CompletedTask;
        }
    }
}