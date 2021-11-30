using System.Text.Json.Serialization;

namespace SharedLibrary.Dtos
{
    public class Response<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }

        [JsonIgnore]
        public bool Success { get; private set; }

        public ErrorDto Error { get; private set; }

        public Response(T data, int statusCode)
        {
            Data = data;
            StatusCode = statusCode;
            Success = true;
        }

        public Response(int statusCode)
        {
            Data = default;
            StatusCode = statusCode;
            Success = true;
        }

        public Response(ErrorDto errorDto, int statusCode)
        {
            Error = errorDto;
            StatusCode = statusCode;
            Success = false;
        }

        public Response(string errorMessage, int statusCode, bool isShow)
        {
            Error = new ErrorDto(errorMessage, isShow);
            StatusCode = statusCode;
            Success = false;
        }
    }
}
