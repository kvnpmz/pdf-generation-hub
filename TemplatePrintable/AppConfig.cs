public static class AppConfig 
{
    public static readonly string RootPath = Environment.GetEnvironmentVariable("PROJECT_ROOT") 
        ?? "/home/kevin/z_ob/etsy/TemplatePrintable";

    public static readonly string PythonPath = Environment.GetEnvironmentVariable("PYTHON_EXECUTABLE") 
        ?? "/home/kevin/z_ob/venv/bin/python";
}

