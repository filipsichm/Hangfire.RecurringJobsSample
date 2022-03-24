using BusinessLogic;

namespace Hangfire.RecurringJobs
{
    public class RecurringJobTemplate
    {
        public string Name { get; set; }
        public Job Id { get; set; }
        public string Cron { get; set; }
        public string JobTypeName { get; set; }
    }
}
