namespace SpMediator.Models;

public record BaseResponse<T> : ErrorsVm
{
    public T? Data { get; set; }
}