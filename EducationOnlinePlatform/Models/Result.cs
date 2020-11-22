using Newtonsoft.Json;
using System.Net;

namespace EducationOnlinePlatform.Models
{
    public class Result
    {
       public HttpStatusCode Status { get; set; }
       public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
