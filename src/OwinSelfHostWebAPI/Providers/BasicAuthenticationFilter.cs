using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace OwinSelfHostWebAPI.Providers
{
    // http://www.asp.net/web-api/overview/security/authentication-filters

    public class BasicAuthenticationFilter : IAuthenticationFilter
    {
        public bool AllowMultiple
        {
            get { return true; }
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            // 1. Look for credentials in the request.
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            // 2. If there are no credentials, do nothing.
            if (authorization == null)
            {
                return;
            }

            // 3. If there are credentials but the filter does not recognize the
            //    authentication scheme, do nothing.
            if (authorization.Scheme != "Basic")
            {
                return;
            }

            // 4. If there are credentials that the filter understands, try to validate them.
            // 5. If the credentials are bad, set the error result.
            if (String.IsNullOrEmpty(authorization.Parameter))
            {
                //context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            Tuple<string, string> userNameAndPasword = ExtractUserNameAndPassword(authorization.Parameter);
            if (userNameAndPasword == null)
            {
                //context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
            }

            if (UserConfig.IsUserAllowd(userNameAndPasword.Item1, userNameAndPasword.Item2))
            {
                IPrincipal principal = CreatePrincipal(userNameAndPasword.Item1, userNameAndPasword.Item2);
                context.Principal = principal;
            }

            return;
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return;
        }

        private Tuple<string, string> ExtractUserNameAndPassword(string p)
        {
            // Decode the token from BASE64
            string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(p));

            // Extract username and password from decoded token
            string username = decodedToken.Substring(0, decodedToken.IndexOf(":"));
            string password = decodedToken.Substring(decodedToken.IndexOf(":") + 1);

            return Tuple.Create(username, password);
        }

        private IPrincipal CreatePrincipal(string userName, string password)
        {
            return new SimplePrincipal { Identity = new SimpleIdentyty { Name = userName } };
        }
    }

    public class SimplePrincipal : IPrincipal
    {
        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            return true;
        }
    }

    public class SimpleIdentyty : IIdentity
    {
        public string AuthenticationType
        {
            get { return string.Empty; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name { get; set; }
    }
}