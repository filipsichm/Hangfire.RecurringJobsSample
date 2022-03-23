using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange.Rest.Tasks
{
    public class GetRequestExample : RestTaskBase
    {
        private readonly string[] _symbols = new[] { "BTCEUR", "ETHEUR", "BTCUSDT" };
        private readonly List<string> _data;

        public GetRequestExample(HttpClient client, RestTaskArgs args) : base(client, args)
        {
            _data = new List<string>();
        }

        protected override Task<List<(HttpRequestMessage req, dynamic data)>> GetRequestList()
        {
            var requests = new List<(HttpRequestMessage req, dynamic data)>();
            foreach (var symbol in _symbols)
            {
                requests.Add((new HttpRequestMessage(HttpMethod.Get, string.Format(Args.Uri, symbol)), symbol));
            }
            return Task.FromResult(requests);
        }

        protected override async Task ProcessResponse(HttpResponseMessage response, dynamic _)
        {
            var content = await response.Content.ReadAsStringAsync();
            var o = JObject.Parse(content);
            _data.Add($"{DateTime.Now} {o["symbol"]} : {o["price"]}");
        }

        protected override Task Postprocess()
        {
            Result.Data = _data.Aggregate((line1, line2) => line1 + Environment.NewLine + line2);
            return base.Postprocess();
        }
    }
}
