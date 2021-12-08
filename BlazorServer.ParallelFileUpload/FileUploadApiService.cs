using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BlazorServer.ParallelFileUpload
{
    public class FileUploadApiService
    {
        private const string UriString = "https://localhost:7034/";
        private readonly HttpClient httpClient;
        private readonly ProgressMessageHandler httpProgressHandler;

        public FileUploadApiService(IHttpContextAccessor httpContextAccessor)
        {
            var authHttpClientHandler = new AuthHttpClientHandler(httpContextAccessor);
            httpProgressHandler = new ProgressMessageHandler(authHttpClientHandler);

            httpClient = new HttpClient(httpProgressHandler);
            httpClient.BaseAddress = new Uri(uriString: UriString);

            httpClient.Timeout = TimeSpan.FromSeconds(120);
        }

        public async Task<bool> UploadFileAsync(StreamContent file)
        {
            var url = "upload";

            var formData = new MultipartFormDataContent();
            formData.Add(file, "file");

            var result = await httpClient.PostAsync(url, formData);

            
            return result.IsSuccessStatusCode;
        }

        public StreamContent CreateStreamContent(Stream stream, string fileName, string contentType)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = "\"" + fileName + "\""
            };
            fileContent.Headers.TryAddWithoutValidation("Content-Type", contentType);
            return fileContent;
        }
    }
}
