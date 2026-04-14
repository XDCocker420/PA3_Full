public class MSG
{
    public enum Type { SEARCH, RESULT, None };
    public Type type { get; set; } = Type.None;
    public string? message { get; set; } = "";
}
