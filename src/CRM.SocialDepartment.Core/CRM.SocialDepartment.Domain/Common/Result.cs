namespace CRM.SocialDepartment.Domain.Common
{
    /// <summary>
    /// Паттерн Result
    /// </summary>
    public record Result
    {
        public bool IsSuccess { get; }
        public List<string> Errors { get; }

        protected Result(bool isSuccess, List<string> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Success() => new(true, new List<string>());
        public static Result Failure(string error) => new(false, [error]);
        public static Result Failure(List<string> errors) => new(false, errors);
    }

    /// <summary>
    /// Паттерн Result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record Result<T> : Result, IResultWithValue
    {
        public T Value { get; }

        object IResultWithValue.Value => Value!;

        private Result(bool isSuccess, T value, List<string> errors)
            : base(isSuccess, errors)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, new List<string>());
        public static new Result<T> Failure(string error) => new(false, default!, new List<string> { error });
        public static new Result<T> Failure(List<string> errors) => new(false, default!, errors);

        public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            return IsSuccess
                ? Result<TResult>.Success(mapper(Value))
                : Result<TResult>.Failure(Errors);
        }
    }

    public interface IResultWithValue
    {
        object Value { get; }
    }
}
