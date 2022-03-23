using System;
using System.IO;

namespace BusinessLogic.Common
{
    public static class Constants
    {
        public static readonly string PathDataExchangeJobsConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configurations\dataExchangeJobs.json");
    }
}
