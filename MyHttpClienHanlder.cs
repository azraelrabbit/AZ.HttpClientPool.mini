using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AZHttpClientPool
{
    public class MyHttpClienHanlder : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //Console.WriteLine("max conn per server"+this.MaxConnectionsPerServer);
            //request.Headers.Referrer = new Uri("http://www.google.com/");

            //request.Headers.Add("UserAgent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727)");
            request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36");


            //Task<HttpResponseMessage> task = base.SendAsync(request, cancellationToken);
            //HttpResponseMessage response = task.Result;
            //MediaTypeHeaderValue contentType = response.Content.Headers.ContentType;
            //if (string.IsNullOrEmpty(contentType.CharSet))
            //{
            //    contentType.CharSet = "GBK";
            //}

            //return task;


            var response = await base.SendAsync(request, cancellationToken);

            var contentType = response.Content.Headers.ContentType;
            //contentType.CharSet = await getCharSetAsync(response.Content);
            if (string.IsNullOrEmpty(contentType.CharSet))
            {
                contentType.CharSet = "GBK";
            }
            return response;
            //return response;
        }

        private async Task<string> getCharSetAsync(HttpContent httpContent)
        {
            var charset = httpContent.Headers.ContentType.CharSet;
            if (!string.IsNullOrEmpty(charset))
                return charset;

            var content = await httpContent.ReadAsStringAsync();
            var match = Regex.Match(content, @"charset=(?<charset>.+?)""", RegexOptions.IgnoreCase);
            if (!match.Success)
                return charset;

            return match.Groups["charset"].Value;
        }
    }
}