using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Threading;

namespace Dian.NTest.WebApi
{
    /// <summary>
    /// 集成了 WEB API 单元测试的 Post/Put/Delete/Get 四种测试方法，分别是：
    /// InvokePostRequest
    /// InvokePutRequest
    /// InvokeDeleteRequest
    /// InvokeGetRequest
    /// </summary>
    /// <code>
    /// 使用示例：
    /// 
    /// [TestFixture]
    /// public class UnitTest1 
    /// {
    ///     private ExpressTool _et;
    ///     
    ///     [SetUp]
    ///     public void Setup()
    ///     {
    ///         _et = new ExpressTool();
    ///         _et.InvokeEvent += Target;
    ///     }
    /// 
    ///     private void Target(HttpConfiguration config)
    ///     {
    ///         WebApiConfig.Register(config);
    ///     }
    ///     
    ///     [Test]
    ///     public void PostTest()
    ///     {
    ///         var response = _et.InvokePostRequest("Url/xxx", new Vm { ddd = "ddd" });
    ///         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    ///     }
    /// }
    /// </code>
    public class TestTool
    {
        private string GetBaseAddress()
        {
            var r = new Random();
            var port = r.Next(9000, 40000);
            return string.Format("http://localhost:{0}/", port);
        }

        public HttpResponseMessage InvokeGetRequest(string api, IDictionary<string, string> header = null)
        {
            using (var invoker = CreateMessageInvoker())
            {
                using (var cts = new CancellationTokenSource())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, GetBaseAddress() + api);
                    if (header != null)
                    {
                        foreach (var pair in header)
                        {
                            request.Headers.Add(pair.Key, pair.Value);
                        }
                    }
                    var result = invoker.SendAsync(request, cts.Token).Result;
                    Trace.WriteLine(string.Format("单元测试 - {0}，返回的正文如下：", api));
                    Trace.WriteLine(result.Content.ReadAsStringAsync().Result);
                    return result;
                }
            }
        }

        private HttpResponseMessage InvokePostRequest<TArguemnt>(HttpMethod method, string api, TArguemnt arg,
            IDictionary<string, string> header = null)
        {
            var invoker = CreateMessageInvoker();
            using (var cts = new CancellationTokenSource())
            {
                var request = new HttpRequestMessage(method, GetBaseAddress() + api);
                if (header != null)
                    foreach (var pair in header)
                    {
                        request.Headers.Add(pair.Key, pair.Value);
                    }

                request.Content = new ObjectContent<TArguemnt>(arg, new JsonMediaTypeFormatter());
                var result = invoker.SendAsync(request, cts.Token).Result;
                Trace.WriteLine(string.Format("单元测试 - {0}，返回的正文如下：", api));
                Trace.WriteLine(result.Content.ReadAsStringAsync().Result);
                return result;
            }
        }

        public HttpResponseMessage InvokePostRequest<TArguemnt>(string api, TArguemnt arg,
            IDictionary<string, string> header = null)
        {
            return InvokePostRequest(HttpMethod.Post, api, arg, header);
        }

        public HttpResponseMessage InvokeDeleteRequest<TArguemnt>(string api, TArguemnt arg,
            IDictionary<string, string> header = null)
        {
            return InvokePostRequest(HttpMethod.Delete, api, arg, header);
        }

        public HttpResponseMessage InvokePutRequest<TArguemnt>(string api, TArguemnt arg,
            IDictionary<string, string> header = null)
        {
            return InvokePostRequest(HttpMethod.Put, api, arg, header);
        }

        private HttpMessageInvoker CreateMessageInvoker()
        {
            var config = new HttpConfiguration();

            InvokeEvent?.Invoke(config);

            var server = new HttpServer(config);
            var messageInvoker = new HttpMessageInvoker(server);
            return messageInvoker;
        }

        public event InvokeCallback InvokeEvent;
        public delegate void InvokeCallback(HttpConfiguration config);
    }
}