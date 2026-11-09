public static class Paths
{
    public static readonly string RootPath = Directory.GetCurrentDirectory();

    public static readonly string PythonPath =
        Path.Combine(RootPath, "venv", "bin", "python");
}
