namespace OpenHardwarePlatform.Abstractions;

/// <summary>
/// Represents a loadable plugin that extends the platform.
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Gets the unique identifier for this plugin.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the display name of this plugin.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the plugin version.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Gets a description of what this plugin provides.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Initializes the plugin with the platform context.
    /// </summary>
    Task InitializeAsync(IPluginContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when the plugin should clean up and shut down.
    /// </summary>
    Task ShutdownAsync(CancellationToken cancellationToken = default);
}