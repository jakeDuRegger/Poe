using Microsoft.Extensions.Configuration;

namespace Poe.Helpers.Dependency_Injection;

/// <summary>
/// Service to encapsulate access to configuration settings. This is where the fields for the config are stored.
/// </summary>
public class ConfigurationService
{
    /*This interface is part of the Microsoft.Extensions.Configuration library and is used to read settings from
     configuration files like config.json. It provides a flexible way to access these settings based on keys.*/
    private IConfiguration? _configuration;


    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration object which holds the settings.</param>
    public ConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
        // Load values from configuration or set defaults
        LastDocumentPath = _configuration["LastDocumentPath"] ?? string.Empty;
        Username = _configuration["UserSettings:Username"] ?? "DefaultUser";
        FirstTime = bool.TryParse(_configuration["UserSettings:FirstTime"], out var ft) && ft;
        AutoSaveEnabled = bool.TryParse(_configuration["EditorPreferences:AutoSaveEnabled"], out var ase) && ase; //todo figure out wtf these little characters mean
        AutoSaveInterval = int.TryParse(_configuration["EditorPreferences:AutoSaveInterval"], out var asi) ? asi : 300; // Default to 300
        Theme = _configuration["EditorPreferences:Theme"] ?? "Light"; // Default to "Light" theme
    }
    
    // Define fields for use in application from the config.json fields.
    public string LastDocumentPath { get; set; }
    public string Username { get; set; }
    public bool FirstTime { get; set; }
    public bool AutoSaveEnabled { get; set; }
    public int AutoSaveInterval { get; set; }
    public string Theme { get; set; }
    
}