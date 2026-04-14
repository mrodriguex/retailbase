using System;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.OBJ.Models;
using Microsoft.Extensions.Logging;

namespace HARD.CORE.NEG.Services
{
    public class CryptographerService : ICryptographerService
    {
        private readonly ICryptographerB _cryptographer;
        private readonly ILogger<CryptographerService> _logger;

        public CryptographerService(ICryptographerB cryptographer, ILogger<CryptographerService> logger)
        {
            _cryptographer = cryptographer;
            _logger = logger;
        }

        public ResultModel<string> CreateHash(string input)
        {
            var webResult = new ResultModel<string>();
            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    webResult.Success = false;
                    webResult.Message = "Error en el modelo recibido";
                    webResult.Errors.Add("El campo input es requerido");
                    return webResult;
                }
                string decodedPlainText = Uri.UnescapeDataString(input);
                // Correct call - no algorithmName parameter
                webResult.Data = _cryptographer.CreateHash(input: decodedPlainText);
                webResult.Success = true;
                webResult.Message = "Hash creado exitosamente.";
                _logger.LogInformation("Hash created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el hash para el input: {Input}", input);
                webResult.Success = false;
                webResult.Message = "Error al crear el hash.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public ResultModel<bool> CompareHash(string input, string hash)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    webResult.Success = false;
                    webResult.Message = "Error en el modelo recibido";
                    webResult.Errors.Add("El campo input es requerido");
                    return webResult;
                }
                if (string.IsNullOrEmpty(hash))
                {
                    webResult.Success = false;
                    webResult.Message = "Error en el modelo recibido";
                    webResult.Errors.Add("El campo hash es requerido");
                    return webResult;
                }
                string decodedPlainText = Uri.UnescapeDataString(input);
                string decodedHash = Uri.UnescapeDataString(hash);
                // Correct call - no algorithmName parameter
                webResult.Data = _cryptographer.CompareHash(input: decodedPlainText, hash: decodedHash);
                webResult.Success = true;
                webResult.Message = "Comparación realizada exitosamente.";
                _logger.LogInformation("Hash comparison completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comparar el hash para el input: {Input} y hash: {Hash}", input, hash);
                webResult.Success = false;
                webResult.Message = "Error al comparar el hash.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
    }
}