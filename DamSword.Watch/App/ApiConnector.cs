using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DamSword.Watch
{
    // TODO: improve request speed
    public static class ApiConnector
    {
        private static readonly HttpClientHandler Handler = new HttpClientHandler();
        private static readonly HttpClient Client = new HttpClient(Handler);

        static ApiConnector()
        {
            Handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidationCallback;
            Handler.Proxy = null;
            Handler.UseProxy = false;
        }

        public static async Task<TResult> JsonRequest<TResult>(string url, object data = null)
        {
            var result = await Client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<TResult>(result);
        }

        private static bool ServerCertificateCustomValidationCallback(HttpRequestMessage request, X509Certificate2 cert, X509Chain chain, SslPolicyErrors errors)
        {
            return errors == SslPolicyErrors.None;
        }
    }
}