using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

// NOTE: Newtonsoft.Json NuGet パッケージをインストールしてください.
using Newtonsoft.Json;

namespace QnAMaker
{
    class CreateQnA
    {
        // QnA MakerのHTTPリクエストURIを作成するため設定.
        static string host = "https://westus.api.cognitive.microsoft.com";
        static string service = "/qnamaker/v4.0";
        static string method = "/knowledgebases/create";

        // NOTE: Azureダッシュボードからコピーしたキーに置き換えてください.
        static string key = ConfigurationManager.AppSettings["QnAMakerSubscriptionKey"];

        /// <summary>
        /// ナレッジベースの作成に使用されるデータソースを定義.
        /// データソースには、QnAペア、メタデータ、QnA Maker FAQのURL、およびAzure BotサービスFAQのURLが含まれる.
        /// </summary>
        static string kb = @"
{
  'name': 'QnA Maker FAQ',
  'qnaList': [
    {
      'id': 0,
      'answer': 'You can use our REST APIs to manage your knowledge base. See here for details: https://westus.dev.cognitive.microsoft.com/docs/services/58994a073d9e04097c7ba6fe/operations/58994a073d9e041ad42d9baa',
      'source': 'Custom Editorial',
      'questions': [
        'How do I programmatically update my knowledge base?'
      ],
      'metadata': [
        {
          'name': 'category',
          'value': 'api'
        }
      ]
    }
  ],
  'urls': [
    'https://docs.microsoft.com/ja-jp/azure/cognitive-services/qnamaker/faqs',
    'https://docs.microsoft.com/ja-jp/bot-framework/resources-bot-framework-faq'
  ],
  'files': []
}
";

        /// <summary>
        /// HTTPリクエストによって返されるHTTPレスポンス.
        /// </summary>
        public struct Response
        {
            public HttpResponseHeaders headers;
            public string response;

            public Response(HttpResponseHeaders headers, string response)
            {
                this.headers = headers;
                this.response = response;
            }
        }

        /// <summary>
        /// 表示用JSONの書式設定とインデント.
        /// </summary>
        /// <param name="s">JSONの書式設定とインデント.</param>
        /// <returns>書式設定されインデントされたJSONを含む文字列.</returns>
        static string PrettyPrint(string s)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(s), Formatting.Indented);
        }

        /// <summary>
        /// HTTPリクエストの非同期POST通信.
        /// </summary>
        /// <param name="uri">HTTPリクエストURI.</param>
        /// <param name="body">HTTPリクエストBODY.</param>
        /// <returns>HTTP <see cref="System.Threading.Tasks.Task{TResult}(QnAMaker.Program.Response)"/></returns>
        async static Task<Response> Post(string uri, string body)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                return new Response(response.Headers, responseBody);
            }
        }

        /// <summary>
        /// HTTPリクエストの非同期GET通信.
        /// </summary>
        /// <param name="uri">HTTPリクエストURI.</param>
        /// <returns>HTTP <see cref="System.Threading.Tasks.Task{TResult}(QnAMaker.Program.Response)"/></returns>
        async static Task<Response> Get(string uri)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri);
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                return new Response(response.Headers, responseBody);
            }
        }

        /// <summary>
        /// ナレッジベースの作成.
        /// </summary>
        /// <param name="kb">ナレッジベースのデータソース.</param>
        /// <returns>HTTP <see cref="System.Threading.Tasks.Task{TResult}(QnAMaker.Program.Response)"/></returns>
        /// <remarks>
        /// QnA Makerでナレッジベースを作成するためのURIを生成し, 
        /// HTTPリクエストの非同期 <see cref="QnAMaker.Program.Post(string, string)"/> メソッドを呼び出す.
        /// </remarks>
        async static Task<Response> PostCreateKB(string kb)
        {
            string uri = host + service + method;

            Console.WriteLine("Calling " + uri + ".");

            return await Post(uri, kb);
        }

        /// <summary>
        /// 指定されたQnA Makerオペレーションのステータスを取得.
        /// </summary>
        /// <param name="operation">The QnA Maker operation to check.</param>
        /// <returns>HTTP <see cref="System.Threading.Tasks.Task{TResult}(QnAMaker.Program.Response)"/></returns>
        /// <remarks>
        /// QnA Makerでナレッジベースを作成するためのURIを生成し, 
        /// HTTPリクエストの非同期  <see cref="QnAMaker.Program.Get(string)"/> メソッドを呼び出す.
        /// </remarks>
        async static Task<Response> GetStatus(string operation)
        {
            string uri = host + service + operation;

            Console.WriteLine("Calling " + uri + ".");

            return await Get(uri);
        }

        /// <summary>
        /// ナレッジベースを作成し、ナレッジベースが作成されるまで定期的にステータスをチェックする
        /// </summary>
        public async static void CreateKB()
        {
            try
            {
                var response = await PostCreateKB(kb);

                var operation = response.headers.GetValues("Location").First();

                Console.WriteLine(PrettyPrint(response.response));

                var done = false;
                while (true != done)
                {
                    response = await GetStatus(operation);

                    Console.WriteLine(PrettyPrint(response.response));

                    var fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.response);

                    String state = fields["operationState"];
                    if (state.CompareTo("Running") == 0 || state.CompareTo("NotStarted") == 0)
                    {
                        // QnA Maker ナレッジベースの作成中.
                        // スレッドは、Retry-Afterヘッダー値と同じ秒数だけ一時停止され、ループが続行される.
                        var wait = response.headers.GetValues("Retry-After").First();
                        Console.WriteLine("Waiting " + wait + " seconds...");
                        Thread.Sleep(Int32.Parse(wait) * 1000);
                    }
                    else
                    {
                        //  QnA Maker ナレッジベースの作成完了. 
                        done = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the knowledge base.");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("Press any key to continue.");
            }

        }
    }
}