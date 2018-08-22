using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Xml;

namespace BotApp.Services
{
    public class MessageTranslatorService
    {
        private static readonly string TokenBaseURI = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
        private static readonly string TranslateBaseURI = "http://api.microsofttranslator.com/v2/Http.svc/Translate";

        private static string TranslationKey;
        private static DateTime TokenDate;
        private static string Token;

        private static readonly string DefaultFromLocale = "ja";
        private static readonly string DefaultToLocale = "en";

        private MessageTranslatorService()
        {
            TranslationKey = ConfigurationManager.AppSettings["TranslatorAPIKey"];
        }

        public static MessageTranslatorService Current = new MessageTranslatorService();

        public async Task<string> Translate(string text)
        {
            return await Translate(text, DefaultFromLocale, DefaultToLocale);
        }

        public async Task<string> Translate(string text, string FromLocale, string ToLocale)
        {
            //token times out 10 minutes.
            await RefreshToken();

            using (var httpClient = new HttpClient())
            {
                string urlEncodedText = HttpUtility.UrlEncode(text);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
                string url = $"{TranslateBaseURI}?text={urlEncodedText}&from={FromLocale}&to={ToLocale}";
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(responseContent);
                    return document.InnerText;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task RefreshToken()
        {
            //Ensure we have a token, and it isn't expired.
            if (Token == null || TokenDate.AddMinutes(8) < DateTime.Now)
            {
                Token = await GetTranslationToken();
                TokenDate = DateTime.Now;
            }
        }

        private async Task<string> GetTranslationToken()
        {
            using (var httpClient = new HttpClient())
            {
                string tokenURI = $"{TokenBaseURI}?subscription-key={TranslationKey}";
                HttpResponseMessage response = await httpClient.PostAsync(tokenURI, new HttpRequestMessage().Content);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}