namespace Pronative.MultiAgentTraining.Shared;

public static class DotEnvLoader
{
    public static void Load()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var envPath = Path.Combine(dir.FullName, ".env");
            if (File.Exists(envPath))
            {
                DotNetEnv.Env.Load(envPath);
                return;
            }
            dir = dir.Parent;
        }
    }
}
