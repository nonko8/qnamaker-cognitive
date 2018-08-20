using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

// NOTE: Install the Newtonsoft.Json NuGet package.
using Newtonsoft.Json;

namespace QnAMaker
{
    class UpdateQnA
    {
        // QnA MakerのHTTPリクエストURIを作成するため設定.
        static string host = "https://westus.api.cognitive.microsoft.com";
        static string service = "/qnamaker/v4.0";
        static string method = "/knowledgebases/";

        // NOTE: Azureダッシュボードからコピーしたキーに置き換えてください.
        static string key = ConfigurationManager.AppSettings["QnAMakerSubscriptionKey"];

        // NOTE: Replace this with a valid knowledge base ID.
        static string kbid = ConfigurationManager.AppSettings["QnAMakeKnowledgeBaseID"];

        /// <summary>
        /// ナレッジベースの更新に使用されるデータソースを定義.
        /// データソースには、QnAペア、メタデータ、QnA Maker FAQのURL、およびAzure BotサービスFAQのURLが含まれる.
        /// </summary>
        static string kb = @"
        {
          'add': {
            'qnaList': [
              {
                'id': 1,
                'answer': 'You can change the default message if you use the QnAMakerDialog. See this for details: https://docs.botframework.com/en-us/azure-bot-service/templates/qnamaker/#navtitle',
                'source': 'Custom Editorial',
                'questions': [
                  'How can I change the default message from QnA Maker?'
                ],
                'metadata': []
              }
            ],
            'urls': []
          },
          'update' : {
            'name' : 'New KB Name'
          },
          'delete': {
            'ids': [
              0
            ]
          }
        }
        ";
        /// <summary>
        /// Represents the HTTP response returned by an HTTP request.
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
        /// Formats and indents JSON for display.
        /// </summary>
        /// <param name="s">The JSON to format and indent.</param>
        /// <returns>A string containing formatted and indented JSON.</returns>
        static string PrettyPrint(string s)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(s), Formatting.Indented);
        }

        /// <summary>
        /// Asynchronously sends a PATCH HTTP request.
        /// </summary>
        /// <param name="uri">The URI of the HTTP request.</param>
        /// <param name="body">The body of the HTTP request.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task{TResult}(QnAMaker.Program.Response)"/> 
        /// object that represents the HTTP response."</returns>
        async static Task<Response> Patch(string uri, string body)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("PATCH");
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                return new Response(response.Headers, responseBody);
            }
        }

        /// <summary>
        /// Asynchronously sends a GET HTTP request.
        /// </summary>
        /// <param name="uri">The URI of the HTTP request.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task{TResult}(QnAMaker.Program.Response)"/> 
        /// object that represents the HTTP response."</returns>
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
        /// Updates a knowledge base.
        /// </summary>
        /// <param name="kbid">The ID for the existing knowledge base.</param>
        /// <param name="kb">The new data source for the updated knowledge base.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task{TResult}(QnAMaker.Program.Response)"/>
        /// object that represents the HTTP response."</returns>
        /// <remarks>Constructs the URI to update a knowledge base in QnA Maker,
        /// then asynchronously invokes the <see cref="QnAMaker.Program.Patch(string, string)"/>
        /// method to send the HTTP request.</remarks>
        async static Task<Response> PostUpdateKB(string kbid, string kb)
        {
            string uri = host + service + method + kbid;
            Console.WriteLine("Calling " + uri + ".");
            return await Patch(uri, kb);
        }

        /// <summary>
        /// Gets the status of the specified QnA Maker operation.
        /// </summary>
        /// <param name="operation">The QnA Maker operation to check.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task{TResult}(QnAMaker.Program.Response)"/>
        /// object that represents the HTTP response."</returns>
        /// <remarks>Constructs the URI to get the status of a QnA Maker
        /// operation, then asynchronously invokes the <see cref="QnAMaker.Program.Get(string)"/>
        /// method to send the HTTP request.</remarks>
        async static Task<Response> GetStatus(string operation)
        {
            string uri = host + service + operation;
            Console.WriteLine("Calling " + uri + ".");
            return await Get(uri);
        }

        /// <summary>
        /// Updates a knowledge base, periodically checking status
        /// until the knowledge base is updated.
        /// </summary>
        async static void UpdateKB(string kbid, string kb)
        {
            try
            {
                // Starts the QnA Maker operation to update the knowledge base.
                var response = await PostUpdateKB(kbid, kb);

                // Retrieves the operation ID, so the operation's status can be
                // checked periodically.
                var operation = response.headers.GetValues("Location").First();

                // Displays the JSON in the HTTP response returned by the 
                // PostUpdateKB(string, string) method.
                Console.WriteLine(PrettyPrint(response.response));

                // Iteratively gets the state of the operation updating the
                // knowledge base. Once the operation state is something other
                // than "Running" or "NotStarted", the loop ends.
                var done = false;
                while (true != done)
                {
                    // Gets the status of the operation.
                    response = await GetStatus(operation);
                    // Displays the JSON in the HTTP response returned by the
                    // GetStatus(string) method.
                    Console.WriteLine(PrettyPrint(response.response));

                    // Deserialize the JSON into key-value pairs, to retrieve the
                    // state of the operation.
                    var fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.response);

                    // Gets and checks the state of the operation.
                    String state = fields["operationState"];
                    if (state.CompareTo("Running") == 0 || state.CompareTo("NotStarted") == 0)
                    {
                        // QnA Maker is still updating the knowledge base. The thread is
                        // paused for a number of seconds equal to the Retry-After
                        // header value, and then the loop continues.
                        var wait = response.headers.GetValues("Retry-After").First();
                        Console.WriteLine("Waiting " + wait + " seconds...");
                        Thread.Sleep(Int32.Parse(wait) * 1000);
                    }
                    else
                    {
                        // QnA Maker has completed updating the knowledge base.
                        done = true;
                    }
                }
            }
            catch
            {
                // An error occurred while updating the knowledge base. Ensure that
                // you included your QnA Maker subscription key and knowledge base ID
                // where directed in the sample.
                Console.WriteLine("An error occurred while updating the knowledge base.");
            }
            finally
            {
                Console.WriteLine("Press any key to continue.");
            }
        }
        public async static void UpdateKB()
        {
            await Task.Run(() => UpdateKB(kbid, kb));
        }

    }
}