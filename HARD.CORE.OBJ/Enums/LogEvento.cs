using System;
using System.Collections.Generic;
using System.Text;

namespace HARD.CORE.OBJ.Enums
{
    public static class LogEventos
    {

        /// <summary>
        /// Diccionario que mapea los eventos a sus mensajes descriptivos.
        /// </summary>
        public static readonly Dictionary<Evento, string> EventoMensaje = new Dictionary<Evento, string>()
    {
        // Autenticación
        { Evento.AutenticacionInicioSesionExitoso, "Inicio de sesión exitoso" },
        { Evento.AutenticacionInicioSesionFallido, "Intento de inicio de sesión fallido" },
        { Evento.AutenticacionTokenGenerado, "Token JWT generado exitosamente" },
        { Evento.AutenticacionTokenExpirado, "Token JWT expirado" },
        { Evento.AutenticacionTokenInvalido, "Token JWT inválido" },
        { Evento.AutenticacionUsuarioNoAutorizado, "Usuario no autorizado para acceder al recurso" },
        
        // Operaciones CRUD
        { Evento.OperacionCreacionExitosa, "Registro creado exitosamente" },
        { Evento.OperacionCreacionFallida, "Error al crear registro" },
        { Evento.OperacionConsultaExitosa, "Consulta ejecutada exitosamente" },
        { Evento.OperacionConsultaFallida, "Error en consulta" },
        { Evento.OperacionActualizacionExitosa, "Registro actualizado exitosamente" },
        { Evento.OperacionActualizacionFallida, "Error al actualizar registro" },
        
        // Validación
        { Evento.ValidacionDatosEntrada, "Validación de datos de entrada" },
        { Evento.ValidacionDatosInvalidos, "Datos de entrada inválidos" },
        { Evento.ValidacionModeloInvalido, "Modelo de datos inválido" },
        
        // Recursos
        { Evento.RecursoNoEncontrado, "Recurso no encontrado" },
        { Evento.ArchivoSubido, "Archivo subido exitosamente" },
        { Evento.ArchivoNoEncontrado, "Archivo no encontrado" },
        
        // Base de datos
        { Evento.BaseDatosConexionExitosa, "Conexión a base de datos establecida" },
        { Evento.BaseDatosConexionFallida, "Error de conexión a base de datos" },
        { Evento.BaseDatosTimeout, "Timeout en operación de base de datos" },
        
        // Sistema
        { Evento.SistemaInicio, "Sistema iniciado" },
        { Evento.SistemaApagado, "Sistema apagado" },
        { Evento.RendimientoConsultaLenta, "Consulta lenta detectada" },
        
        // Errores
        { Evento.ErrorNoManejado, "Error no manejado en la aplicación" },
        { Evento.ErrorManejado, "Error manejado correctamente" },
        { Evento.ErrorCritico, "Error crítico del sistema" }
    };

        /// <summary>
        /// Obtiene el mensaje asociado a un evento específico.
        /// </summary>
        /// <param name="evento">
        /// Evento del que se desea obtener el mensaje.
        /// </param>
        /// <returns>
        /// Mensaje asociado al evento especificado.
        /// </returns>
        public static string ObtenerMensaje(Evento evento)
        {
            if (EventoMensaje.TryGetValue(evento, out string mensaje))
            {
                return mensaje;
            }
            return "Evento no especificado";
        }

    }
}