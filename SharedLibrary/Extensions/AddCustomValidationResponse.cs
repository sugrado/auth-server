using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Dtos;
using System.Linq;

namespace SharedLibrary.Extensions
{
    public static class CustomValidationResponse
    {
        public static void UseCustomValidationResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.Where(p => p.Errors.Count > 0).SelectMany(p => p.Errors).Select(p => p.ErrorMessage);
                    ErrorDto errorDto = new ErrorDto(errors.ToList(), true);
                    var response = new Response<NoContentResult>(errorDto, 400);
                    return new BadRequestObjectResult(response);
                };
            });
        }
    }
}
