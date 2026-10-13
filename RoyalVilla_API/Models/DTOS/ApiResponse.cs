namespace RoyalVilla_API.Models.DTOS
{
    public class ApiResponse<TData>
    {
        public bool Success { get; set; }
        public int SatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Errors { get; set; }
        public TData? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public  static ApiResponse<TData> Create(bool success,int statusCode,string message, TData? data = default, object? errors=null)
        {
            return new ApiResponse<TData>()
            {
                Success = success,
                SatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = errors,
               
            };
        }
        public static ApiResponse<TData> NotFound(string message,object? errors) => Create(false, 404, message,errors:errors);
        public static ApiResponse<TData> NoContent(string message, TData data) => Create(true, 204, message,data);
        public static ApiResponse<TData> Ok(string message, TData? data) => Create(true, 200, message,data);
        public static ApiResponse<TData> CreatedAt(string message, TData data) => Create(true, 201, message, data);
        public static ApiResponse<TData> Conflict(string message) => Create(true, 409, message);
        public static ApiResponse<TData> BadRequest(string message, object errors) => Create(false, 400, message, errors:errors);



    }
}
