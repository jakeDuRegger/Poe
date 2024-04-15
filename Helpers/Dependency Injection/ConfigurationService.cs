using Microsoft.Extensions.Configuration;

namespace Poe.Helpers.Dependency_Injection;

/// <summary>
/// Service to encapsulate access to configuration settings. This is where the fields for the config are stored.
/// </summary>
public class ConfigurationService
{
    /*This interface is part of the Microsoft.Extensions.Configuration library and is used to read settings from
     configuration files like config.json. It provides a flexible way to access these settings based on keys.*/
    private readonly IConfiguration _configuration;
    

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration object which holds the settings.</param>
    public ConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    
    /*
     * FIELDS FOR CONFIG
     */
    
    // Define fields for use in application from the config.json fields.
    public string? LastDocumentPath => _configuration["LastDocumentPath"];
    public string? Username => _configuration["UserSettings:Username"];
    public bool LoadedBefore => bool.Parse(_configuration["UserSettings:FirstTime"] ?? string.Empty);
    public bool AutoSaveEnabled => bool.Parse(_configuration["EditorPreferences:AutoSaveEnabled"] ?? string.Empty);
    public int AutoSaveInterval => int.Parse(_configuration["EditorPreferences:AutoSaveInterval"] ?? string.Empty);
    public string Theme => _configuration["EditorPreferences:Theme"] ?? string.Empty;
    
    
    /*
     * 
     */
    
    
}