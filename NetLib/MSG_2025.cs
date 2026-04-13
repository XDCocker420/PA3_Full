public class MSG
{
    public enum Type { SEARCH, RESULT, None };
    public Type type = Type.None;
    public string? message = "";
}