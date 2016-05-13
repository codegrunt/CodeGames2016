using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace AxcessAssistant.Controllers
{
    public class FilesController : ApiController
    {
        public HttpResponseMessage Get(string fileName)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            var path = fileName;
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            return result;
        }
    }
}