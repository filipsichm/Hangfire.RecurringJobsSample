using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange.Ftp.Tasks
{
    public class UploadFileExample : FtpTaskBase
    {
        public UploadFileExample(FtpTaskArgs args) : base(args)
        {
        }

        protected override Task<IEnumerable<string>> GetRequestList()
        {
            return Task.FromResult<IEnumerable<string>>(new List<string> { string.Empty });
        }

        protected override async Task SendRequest(string _)
        {
            var uri = Path.Combine(RequestBaseUri, Args.Filename);
            var data = JobContext.Single(x => x.Source == typeof(Rest.Tasks.GetRequestExample).Name).Data as string;
            var byteArray = Encoding.UTF8.GetBytes(data);

            try
            {
                await Client.UploadFile(uri, byteArray);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
