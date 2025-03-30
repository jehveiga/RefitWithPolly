using Refit;

namespace WebApi.ExternalApis.WebApiFail
{
    public interface IWebApiFail
    {
        [Get("/test")]
        Task<string> GetTestAsync();
    }
}