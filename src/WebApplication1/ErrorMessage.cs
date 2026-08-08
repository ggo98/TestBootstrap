using System.Web;

namespace WebApplication1
{
    internal class ErrorMessage
    {
        public int status { get; set; }

        public string statusDescription { get { return HttpWorkerRequest.GetStatusDescription(status); } }

        public string title { get; set; }

        public string detail { get; set; }

        public string timestamp { get; set; }
    }
}