using log4net.Appender;
using log4net.Core;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tesla
{
    public class FaridAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            // Do something with the logged data, like calling your web url
            PostEvent(loggingEvent).ConfigureAwait(false);
        }


        private async Task PostEvent(LoggingEvent loggingEvent)
        {
            using (var client = new HttpClient())
            {

                var content = loggingEvent.RenderedMessage;
                var response = await client.PostAsJsonAsync<string>("http://localhost:5181/api/testapi/info", content);

                var errorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

               
                response.EnsureSuccessStatusCode();

            }
        }
    }
}