using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange.Ftp.Tasks
{
    public class DownloadFileExample : FtpTaskBase
    {
        public DownloadFileExample(FtpTaskArgs args) : base(args)
        {
        }

        protected override async Task<IEnumerable<string>> GetRequestList()
        {
            var filenames = await Client.ListDirectory(RequestBaseUri);
            return filenames.Where(x => Regex.IsMatch(x, Args.Filename));
        }

        protected override async Task SendRequest(string filename)
        {
            var uri = Path.Combine(RequestBaseUri, filename);
            var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            try
            {
                await Client.DownloadFile(uri, localPath);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
