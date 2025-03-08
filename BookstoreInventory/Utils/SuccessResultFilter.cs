using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookstoreInventory.Utils
{
    public class SuccessResultFilter : IAsyncResultFilter
    {
        private readonly ILogger<SuccessResultFilter> _logger;

        public SuccessResultFilter(ILogger<SuccessResultFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult &&
                objectResult.StatusCode is >= 200 and < 300)
            {
                var wrappedResponse = new
                {
                    status = objectResult.StatusCode ?? 200,
                    data = objectResult.Value
                };

                objectResult.Value = wrappedResponse;
            }

            await next();
        }
    }
}
