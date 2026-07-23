using System.Reflection;
using System.Runtime.Loader;
using OpenHardwarePlatform.Abstractions;

namespace OpenHardwarePlatform.Core;

/// <summary>
/// Discovers and loads plugins from specified directories.
/// </summary>
public sealed class PluginLoader
{
    private readonly IPluginContext _context;

    public PluginLoader(IPluginContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Loads all plugins from the specified directory.
    /// </summary>
    public async Task<IReadOnlyList<IPlugin>> LoadFromDirectoryAsync(
        string directoryPath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);

        if (!Directory.Exists(directoryPath))
        {
            _context.Log.Warn($"Plugin directory not found: {directoryPath}");
            return Array.Empty<IPlugin>();
        }

        var plugins = new List<IPlugin>();
        var pluginFiles = Directory.EnumerateFiles(directoryPath, "*.dll", SearchOption.TopDirectoryOnly);

        foreach (var file in pluginFiles)
        {
            try
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                var pluginTypes = assembly.GetExportedTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IPlugin).IsAssignableFrom(t));

                foreach (var type in pluginTypes)
                {
                    if (Activator.CreateInstance(type) is IPlugin plugin)
                    {
                        await plugin.InitializeAsync(_context, cancellationToken);
                        plugins.Add(plugin);
                        _context.Log.Info($"Loaded plugin: {plugin.Name} v{plugin.Version}");
                    }
                }
            }
            catch (Exception ex)
            {
                _context.Log.Error($"Failed to load plugin from '{file}': {ex.Message}");
            }
        }

        return plugins;
    }
}