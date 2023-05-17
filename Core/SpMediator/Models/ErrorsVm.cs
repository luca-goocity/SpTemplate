namespace SpMediator.Models;

public record ErrorsVm
{
    public List<string> Errors { get; set; } = Enumerable.Empty<string>().ToList();
    public int TotalCount { get; set; }
    
    public bool HasErrors => Errors.Any();
}