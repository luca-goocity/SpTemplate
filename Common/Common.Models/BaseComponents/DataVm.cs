namespace Common.Models.BaseComponents;

public record DataVm<T>
{
    public T? Data { get; init; }
}