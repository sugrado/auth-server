using System.Collections.Generic;

namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {
        public List<string> Errors { get; private set; } = new List<string>();

        public bool Display { get; private set; }

        public ErrorDto(string error, bool display)
        {
            Errors.Add(error);
            Display = display;
        }

        public ErrorDto(List<string> errors, bool display)
        {
            Errors = errors;
            Display = display;
        }
    }
}
