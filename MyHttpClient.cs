using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AZHttpClientPool
{
    public class MyHttpClient : HttpClient
    {
        public int ReqCount
        {
            get => _reqCount;
            set => _reqCount = value;
        }

        private int _reqCount;

        public MyHttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
           
        }
        public MyHttpClient(HttpMessageHandler handler) : base(handler)
        {

        }

        public void IncrementReqCount()
        {
            Interlocked.Increment(ref _reqCount);
        }

        public void DecrementReqCount()
        {
            Interlocked.Decrement(ref _reqCount);
        }

        public new Task<string> GetStringAsync(string url)
        {
            IncrementReqCount();

           
            return base.GetStringAsync(url).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });

            
        }

        public new Task<byte[]> GetByteArrayAsync(string url)
        {
            IncrementReqCount();


            return base.GetByteArrayAsync(url).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }


        public new Task<Stream> GetStreamAsync(string url)
        {
            IncrementReqCount();


            return base.GetStreamAsync(url).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }

        public new Task<string> GetStringAsync(Uri uri)
        {
            IncrementReqCount();


            return base.GetStringAsync(uri).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }

        public new Task<HttpResponseMessage> GetAsync(string url)
        {
            IncrementReqCount();
            return base.GetAsync(url).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }
        public new Task GetAsync(string url, CancellationToken cancellationToken)
        {
            IncrementReqCount();
            return base.GetAsync(url, cancellationToken).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }
        public new Task GetAsync(string url, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            IncrementReqCount();
            return base.GetAsync(url,completionOption, cancellationToken).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IncrementReqCount();
            return base.SendAsync(request, cancellationToken).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }

        public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            IncrementReqCount();
            return base.SendAsync(request).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }
        public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            IncrementReqCount();
            return base.SendAsync(request, completionOption).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }

        public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            IncrementReqCount();
 
            return base.SendAsync(request, completionOption, cancellationToken).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }

        public new Task<HttpResponseMessage> DeleteAsync(string url)
        {
            IncrementReqCount();

            return base.DeleteAsync(url).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }

        public new Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancellationToken)
        {
            IncrementReqCount();

            return base.DeleteAsync(url,cancellationToken).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }


        public new Task<HttpResponseMessage> PutAsync(string url,HttpContent httpContent)
        {
            IncrementReqCount();

            return base.PutAsync(url, httpContent).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });
        }

        public new Task<HttpResponseMessage> PutAsync(string url, HttpContent httpContent, CancellationToken cancellationToken)
        {
            IncrementReqCount();

            return base.PutAsync(url, httpContent,cancellationToken).ContinueWith(t =>
            {

                this.DecrementReqCount();

                return t.Result;
            });

            
        }

    }
}
