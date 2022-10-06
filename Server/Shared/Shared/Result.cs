namespace Shared
{
    using Newtonsoft.Json;

    using Shared.Interfaces;

    public class Result<T> : IResult<T>
    {
        [JsonProperty(Order = -1)]
        public bool Success { get; set; }

        [JsonProperty(Order = 1)]
        public virtual T Data { get; set; }

        [JsonProperty(Order = 2)]
        public List<string> Errors { get; set; }

        public Result()
        {
            Errors = new List<string>();
        }

        public static Result<T> SuccessResult(T data, string message = "")
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
            };
        }

        public static Result<T> Failure(List<string> errors)
        {
            return new Result<T>
            {
                Success = false,
                Errors = errors,
            };
        }

        public static Result<T> Failure(string error)
        {
            return Failure(new List<string> { error });
        }

        public static Task<Result<T>> SuccessAsync(T data)
            => Task.FromResult(SuccessResult(data));

        public static Task<Result<T>> FailureAsync(List<string> errors)
            => Task.FromResult(Failure(errors));

        public static Task<Result<T>> FailureAsync(string error)
            => Task.FromResult(Failure(error));
    }
}
