using System.Collections.Generic;

namespace ServianOps_Backend.Application.Common.DTOs
{
    public class StandardResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public StandardResponse()
        {
            Errors = new List<string>();
        }

        public static StandardResponse<T> Ok(T data, string message = null)
        {
            return new StandardResponse<T> { Success = true, Data = data, Message = message };
        }

        public static StandardResponse<T> Error(string message, List<string> errors = null)
        {
            return new StandardResponse<T> { Success = false, Message = message, Errors = errors ?? new List<string>() };
        }
    }
}
