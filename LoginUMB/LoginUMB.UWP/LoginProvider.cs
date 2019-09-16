using LoginUMB.UWP;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

[assembly: Xamarin.Forms.Dependency(typeof(LoginProvider))]
namespace LoginUMB.UWP
{
    class LoginProvider : ILoginProvider
    {
        public async Task<string> LoginAsync()
        {
            var clientId = "{0oa1b3kqofpeKGOUb357}";
            var url = "https://dev-986035.okta.com/oauth2/default";
            var state = GetBase64UrlData(32);
            var nonce = GetBase64UrlData(12);
            var codeVerifier = GetBase64UrlData(32);
            var tmp = CryptographicBuffer.ConvertStringToBinary(codeVerifier, BinaryStringEncoding.Utf8);
            var codeVerifierBuffer = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256).HashData(tmp);
            var codeChallenge = GetBase64UrlData(null, codeVerifierBuffer);

            var absoluteUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri;
            var callback = Uri.EscapeDataString(absoluteUri);

            var authorizationRequest = string.Format(
                "{0}?response_type=id_token&scope=openid profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method=S256&nonce={5}",
                url, callback, clientId, state, codeChallenge, nonce);

            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None,
                new Uri(authorizationRequest), new Uri(absoluteUri));
            if (result.ResponseStatus != WebAuthenticationStatus.Success) return null;
            var queryParams = result.ResponseData.Split("#?&".ToCharArray());
            var idToken = queryParams.FirstOrDefault(p => p.StartsWith("id_token"));
            return idToken?.Substring(9);
        }

        public static string GetBase64UrlData(uint? length = null, IBuffer buffer = null)
        {
            if (length != null) buffer = CryptographicBuffer.GenerateRandom(length.Value);
            return CryptographicBuffer.EncodeToBase64String(buffer)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
