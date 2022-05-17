using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos;
using SharedLibrary.Exceptions;
using System.Text.Json;

namespace SharedLibrary.Extensions
{
    public static class CustomExceptionHandle
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorFeature != null)
                    {
                        var ex = errorFeature.Error;
                        ErrorDto errorDto = null;
                        errorDto = new ErrorDto(ex.Message, ex is CustomException);

                        var response = new Response<NoDataDto>(errorDto, 500);
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                });
            });
        }
    }
}
