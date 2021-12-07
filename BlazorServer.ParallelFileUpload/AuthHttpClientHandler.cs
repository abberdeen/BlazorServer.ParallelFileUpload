using System.Net.Http.Headers; 

namespace BlazorServer.ParallelFileUpload
{
    public class AuthHttpClientHandler : HttpClientHandler
    {
        private readonly IHttpContextAccessor accessor;

        public AuthHttpClientHandler(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (accessor.HttpContext != null)
            {
                var accessToken = "123";
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
