using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
 
using System.Threading;
using System.Threading.Tasks;

namespace AZHttpClientPool
{
    public class HttpClientPool : IDisposable
    {

        ConcurrentBag<MyHttpClient> _httpClientPool;

        private int _poolSize;

        private bool _keepAlive;

        private string _baseUrl;

        private Uri _baseUri;

        private HttpClientHandler _httpClientHandler;


        public HttpClientPool(string baseUrl, int poolSize = 5, string defaultCharset = "UTF-8", HttpClientHandler clientHandler =null, bool keepAlive = false)
        {
            _baseUrl = baseUrl;

            _httpClientPool = new ConcurrentBag<MyHttpClient>();

            _keepAlive = keepAlive;

            _httpClientHandler = new MyHttpClienHanlder(defaultCharset: defaultCharset)
            { UseProxy = false, UseCookies = false, AllowAutoRedirect = true, AutomaticDecompression = DecompressionMethods.GZip };

            if (!_baseUrl.EndsWith("/"))
            {
                _baseUrl += "/";
            }

            _baseUri = new Uri(_baseUrl);

            if (clientHandler != null)
            {
                _httpClientHandler=clientHandler;
            }

            GrantPool(poolSize);
        }



        public MyHttpClient GetClient()
        {

            return _httpClientPool.OrderBy(p => p.ReqCount).FirstOrDefault();

            //return  _httpClientPool.FirstOrDefault(p => p.ReqCount == _httpClientPool.Min(v => v.ReqCount));
        }

        public void GrantPool(int poolSize)
        {
            _poolSize = poolSize;

            var toSize = _poolSize;
            var boolAdd = false;
            if (_httpClientPool.Count < poolSize)
            {
                toSize = poolSize - _httpClientPool.Count;
                boolAdd = true;
            }
            else
            {
                toSize = _httpClientPool.Count - poolSize;
                boolAdd = false;
            }

         

            if (boolAdd)
            {
                for (var i = 0; i < toSize; i++)
                {

                    var newclient = new MyHttpClient(_httpClientHandler, true);
                    CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
                    cacheControl.NoCache = true;
                    cacheControl.NoStore = true;
                    newclient.DefaultRequestHeaders.CacheControl = cacheControl;// 不使用缓存

                    if (_keepAlive)
                    {
                        newclient.DefaultRequestHeaders.Connection.Add("keep-alive");
                    }

                    if (!string.IsNullOrEmpty(_baseUrl))
                    {
                        //帮HttpClient热身
                        //newclient.SendAsync(new HttpRequestMessage
                        //{
                        //    Method = new HttpMethod("HEAD"),
                        //    RequestUri = _baseUri
                        //}).Result.EnsureSuccessStatusCode();

#if NET45_OR_GREATER || NETSTANDARD1_0_OR_GREATER

                        Task.Run(async () => {

                            await newclient.SendAsync(new HttpRequestMessage
                            {
                                Method = new HttpMethod("HEAD"),
                                RequestUri = _baseUri
                            });
                        });
                    
#else
                        TaskEx.Run(async () => {

                            await newclient.SendAsync(new HttpRequestMessage
                            {
                                Method = new HttpMethod("HEAD"),
                                RequestUri = _baseUri
                            });
                        });

#endif
                        _httpClientPool.Add(newclient);

                    }
                }
            }
            else
            {
                //remove count of to size from pool

                for (var i = 0; i < toSize; i++)
                {
                    MyHttpClient client;

                    
                    if (_httpClientPool.TryTake(out client))
                    {
                        try
                        {
                            client?.Dispose();
                        }
                        catch (Exception ex)
                        {
                            // ignored
                        }
                    }
                }
            }
        }

 
        public void Dispose()
        {
            if (_httpClientPool != null && _httpClientPool.Any())
            {
                foreach (var httpClient in _httpClientPool)
                {
                    try
                    {
                        httpClient?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        // ignored
                    }
                }

            }

            _httpClientPool = null;
        }
    }
}
