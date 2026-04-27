namespace LearningApi.Helpers;

using LearningApi.DTOs;

public static class ResponseHelper
{
    public static ApiResponse<T?> Success<T>(T? data, string message = "")
    {
        return new ApiResponse<T?>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<object?> Success(string message)
    {
        return new ApiResponse<object?>
        {
            Success = true,
            Message = message,
            Data = null
        };
    }

    public static ApiResponse<T?> Fail<T>(string message)
    {
        return new ApiResponse<T?>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }

    public static ApiResponse<object?> Fail(string message)
    {
        return new ApiResponse<object?>
        {
            Success = false,
            Message = message,
            Data = null
        };
    }
}