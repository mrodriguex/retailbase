using System;
using System.Collections.Generic;
using System.Linq;
using HARD.CORE.OBJ.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

public class ConfigService
{
    private readonly ILogger<ConfigService> _logger;
    private readonly string _filePath = "appsettings.json";

    public ConfigService(ILogger<ConfigService> logger)
    {
        _logger = logger;
    }

    public ResultModel<bool> UpdateAppSetting(string key, string newValue)
    {
        ResultModel<bool> webResult = new ResultModel<bool>();
        try
        {
            // Leer el archivo appsettings.json
            var json = System.IO.File.ReadAllText(_filePath);

            // Convertir el archivo a JObject
            var jsonObj = JObject.Parse(json);

            // Dividir la ruta del key en caso de ser anidado
            var sectionPath = key.Split(':');

            // Buscar la sección y modificarla
            var configSection = jsonObj;
            for (int i = 0; i < sectionPath.Length - 1; i++)
            {
                var nextSection = configSection[sectionPath[i]];
                if (nextSection == null || nextSection.Type != JTokenType.Object)
                {
                    webResult.Success = false;
                    webResult.Message = $"Section '{sectionPath[i]}' not found or is not an object.";
                    return webResult;
                }
                configSection = (JObject)nextSection;
            }
            configSection[sectionPath.Last()] = newValue;

            // Guardar los cambios en el archivo appsettings.json
            System.IO.File.WriteAllText(_filePath, jsonObj.ToString());
            webResult.Success = true;
            webResult.Message = $"Key '{key}' updated to '{newValue}' successfully.";
            webResult.Data = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appsettings.json for key: {Key}", key);
            webResult.Success = false;
            webResult.Message = $"Error updating appsettings.json: {ex.Message}";
            webResult.Errors.Add(ex.Message);
        }
        return webResult;
    }

    public ResultModel<string> GetAppSetting(string key)
    {
        var webResult = new ResultModel<string>();
        try
        {
            // Leer el archivo appsettings.json
            var json = System.IO.File.ReadAllText(_filePath);

            // Convertir el archivo a JObject
            var jsonObj = JObject.Parse(json);

            // Dividir la ruta del key en caso de ser anidado
            var sectionPath = key.Split(':');

            // Buscar la sección y modificarla
            var configSection = jsonObj;
            for (int i = 0; i < sectionPath.Length - 1; i++)
            {
                var nextSection = configSection[sectionPath[i]];
                if (nextSection == null || nextSection.Type != JTokenType.Object)
                {
                    webResult.Success = false;
                    webResult.Message = $"Section '{sectionPath[i]}' not found or is not an object.";
                    return webResult;
                }
                configSection = (JObject)nextSection;
            }

            Dictionary<string, object> result = new Dictionary<string, object>();
            var value = configSection[sectionPath.Last()];
            if (value == null)
            {
                webResult.Success = false;
                webResult.Message = $"Key '{key}' not found.";
                return webResult;
            }
            result.Add(key, value);

            webResult.Success = true;
            webResult.Message = $"Key '{key}' retrieved successfully.";
            webResult.Data = value.ToString();

        }
        catch (Exception ex)
        {
            webResult.Success = false;
            webResult.Message = $"Error reading appsettings.json: {ex.Message}";
            webResult.Errors.Add(ex.Message);

        }
        return webResult;
    }
}

