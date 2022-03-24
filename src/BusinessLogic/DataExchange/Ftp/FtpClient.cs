using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BusinessLogic.DataExchange.Ftp
{
    public class FtpClient
    {
        public ICredentials Credentials { get; set; }

        public FtpClient(ICredentials credentials)
        {
            Credentials = credentials;
        }

        public async Task DownloadFile(string uri, string localPath, Action<FtpWebRequest> customize = null)
        {
            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.Credentials = Credentials;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.KeepAlive = false;
            customize?.Invoke(request);

            using var response = (FtpWebResponse)request.GetResponse();
            using var responseStream = response.GetResponseStream();
            using var fileStream = new FileStream(localPath, FileMode.Create);
            await responseStream.CopyToAsync(fileStream);

            Console.WriteLine($"Ftp.DownloadFile Complete, status {response.StatusDescription}");
        }

        public async Task<IEnumerable<string>> ListDirectory(string uri, Action<FtpWebRequest> customize = null)
        {
            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = Credentials;
            customize?.Invoke(request);

            var response = (FtpWebResponse)request.GetResponse();
            using var responseStream = response.GetResponseStream();
            using var reader = new StreamReader(responseStream);

            var result = await reader.ReadToEndAsync();
            Console.WriteLine($"Ftp.ListDirectory Complete, status {response.StatusDescription}");

            return result.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task UploadFile(string uri, byte[] content, Action<FtpWebRequest> customize = null)
        {
            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = Credentials;
            request.KeepAlive = false;
            customize?.Invoke(request);

            request.ContentLength = content.Length;
            using (var requestStream = await request.GetRequestStreamAsync())
            {
                requestStream.Write(content, 0, content.Length);
            }

            using var response = (FtpWebResponse)await request.GetResponseAsync();
            Console.WriteLine($"Ftp.UploadFile Complete, status {response.StatusDescription}");
        }

    }
}
