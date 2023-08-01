namespace KUSYS.Data.Repository
{
    public class DbOperationResult
    {
        public DbOperationResult(bool isSucceed)
        {
            IsSucceed = isSucceed;
            Message = "";
            Errors = new List<string>();
        }

        public DbOperationResult(bool isSucceed, string message)
        {
            IsSucceed = isSucceed;
            Message = message;
            Errors = new List<string>();
        }

        public DbOperationResult(bool isSucceed, string message, List<string> errors)
        {
            IsSucceed = isSucceed;
            Message = message;
            Errors = errors;
        }

        public bool IsSucceed { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }

    public class DbOperationResult<T>
    {
        public DbOperationResult(bool isSucceed, string message, T instance)
        {
            IsSucceed = isSucceed;
            Message = message;
            Instance = instance;
        }

        public DbOperationResult(bool isSucceed, string message)
        {
            IsSucceed = isSucceed;
            Message = message;
        }

        public DbOperationResult(bool isSucceed, string message, List<string> errors, T instance)
        {
            IsSucceed = isSucceed;
            Message = message;
            Instance = instance;
            Errors = errors;
        }

        public bool IsSucceed { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Instance { get; set; }
    }
}
