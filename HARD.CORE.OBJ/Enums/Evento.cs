
namespace HARD.CORE.OBJ.Enums
{
    public enum Evento
    {

        #region Eventos de Autenticación y Autorización (1000-1099)
        // Eventos de Autenticación y Autorización (1000-1099)
        /// <summary>   
        /// Estos eventos están relacionados con la autenticación y autorización de usuarios
        /// </summary>
        /// <remarks>
        /// Estos eventos son generados durante el proceso de login, validación de tokens y permisos de acceso.
        /// </remarks>
        /// <summary>
        /// Inicio de sesión exitoso
        /// </summary>
        AutenticacionInicioSesionExitoso = 1000,

        /// <summary>
        /// Inicio de sesión fallido
        /// </summary>
        AutenticacionInicioSesionFallido = 1001,

        /// <summary>
        /// Token JWT generado exitosamente
        /// </summary>
        AutenticacionTokenGenerado = 1002,

        /// <summary>
        /// Token JWT expirado
        /// </summary>
        AutenticacionTokenExpirado = 1003,

        /// <summary>
        /// Token JWT inválido  
        /// </summary>
        AutenticacionTokenInvalido = 1004,

        /// <summary>
        /// Usuario no autorizado para acceder al recurso   
        /// </summary>
        AutenticacionUsuarioNoAutorizado = 1005,

        /// <summary>
        /// Cambio de password requerido
        /// </summary>
        AutenticacionCambioPassword = 1006,

        /// <summary>
        /// Usuario bloqueado
        /// </summary>
        AutenticacionUsuarioBloqueado = 1007,
        #endregion

        #region Eventos de Operaciones CRUD (1100-1199)
        // Eventos de Operaciones CRUD (1100-1199)
        /// <summary>
        /// Registro creado exitosamente
        /// </summary>
        OperacionCreacionExitosa = 1100,
        /// <summary>
        /// Error al crear registro
        /// </summary>
        OperacionCreacionFallida = 1101,
        /// <summary>
        /// Consulta ejecutada exitosamente
        /// </summary>
        OperacionConsultaExitosa = 1102,
        /// <summary>
        /// Error en consulta
        /// </summary>
        OperacionConsultaFallida = 1103,
        /// <summary>
        /// Registro actualizado exitosamente
        /// </summary>
        OperacionActualizacionExitosa = 1104,
        /// <summary>
        /// Error al actualizar registro
        /// </summary>
        OperacionActualizacionFallida = 1105,
        /// <summary>
        /// Registro eliminado exitosamente
        /// </summary>
        OperacionEliminacionExitosa = 1106,
        /// <summary>
        /// Error al eliminar registro
        /// </summary>
        OperacionEliminacionFallida = 1107,
        /// <summary>
        /// Listado obtenido exitosamente
        /// </summary>
        OperacionListadoExitoso = 1108,
        /// <summary>
        /// Error al obtener listado
        /// </summary>
        OperacionListadoFallido = 1109,
        #endregion

        #region Eventos de Validación y Entrada (1200-1299)         
        // Eventos de Validación y Entrada (1200-1299)
        /// <summary>
        /// Validación de datos de entrada
        /// </summary>
        ValidacionDatosEntrada = 1200,
        /// <summary>
        /// Datos de entrada inválidos
        /// </summary>
        ValidacionDatosInvalidos = 1201,
        /// <summary>
        /// Modelo de datos inválido
        /// </summary>
        ValidacionModeloInvalido = 1202,
        /// <summary>
        /// Campo requerido no proporcionado
        /// </summary>
        ValidacionCampoRequerido = 1203,
        /// <summary>
        /// Formato de dato incorrecto
        /// </summary>
        ValidacionFormatoIncorrecto = 1204,
        /// <summary>
        /// Rango de valor inválido
        /// </summary>
        ValidacionRangoInvalido = 1205,
        /// <summary>
        /// Longitud de dato excedida
        /// </summary>
        ValidacionLongitudExcedida = 1206,
        #endregion

        #region Eventos de Recursos y Archivos (1300-1399)      
        // Eventos de Recursos y Archivos (1300-1399)
        /// <summary>
        /// Recurso no encontrado
        /// </summary>
        RecursoNoEncontrado = 1300,
        /// <summary>
        /// Recurso creado exitosamente
        /// </summary>
        RecursoCreado = 1301,
        /// <summary>
        /// Recurso actualizado exitosamente
        /// </summary>
        RecursoActualizado = 1302,
        /// <summary>
        /// Recurso eliminado exitosamente
        /// </summary>
        RecursoEliminado = 1303,
        /// <summary>
        /// Recurso duplicado   
        /// </summary>
        RecursoDuplicado = 1304,
        /// <summary>   
        /// Archivo subido exitosamente
        /// </summary>
        ArchivoSubido = 1305,
        /// <summary>       
        /// Archivo descargado exitosamente
        /// </summary>
        ArchivoDescargado = 1306,
        /// <summary>    
        /// Archivo eliminado exitosamente      
        /// </summary>
        ArchivoEliminado = 1307,
        /// <summary>    
        /// Archivo no encontrado
        /// </summary>
        ArchivoNoEncontrado = 1308,
        #endregion

        #region Eventos de Base de Datos (1400-1499)        
        // Eventos de Base de Datos (1400-1499)
        /// <summary>       
        /// Conexión a base de datos establecida
        /// </summary>
        BaseDatosConexionExitosa = 1400,
        /// <summary>       
        /// Error de conexión a base de datos   
        /// </summary>
        BaseDatosConexionFallida = 1401,
        /// <summary>    
        /// Timeout en operación de base de datos       
        /// </summary>
        BaseDatosTimeout = 1402,
        /// <summary>   
        /// Deadlock en operación de base de datos
        /// </summary>
        BaseDatosDeadlock = 1403,
        /// <summary>   
        /// Transacción de base de datos iniciada
        /// </summary>
        BaseDatosTransaccionIniciada = 1404,
        /// <summary>   
        /// Transacción de base de datos completada
        /// </summary>
        BaseDatosTransaccionCompletada = 1405,
        /// <summary>   
        /// Transacción de base de datos fallida    
        /// </summary>
        BaseDatosTransaccionFallida = 1406,
        /// <summary>   
        /// Violación de integridad de datos        
        /// </summary>  
        BaseDatosViolacionIntegridad = 1407,
        /// <summary>   
        /// Registro duplicado en base de datos 
        /// </summary>      
        BaseDatosRegistroDuplicado = 1408,
        #endregion

        #region Eventos de Servicios Externos (1500-1599)   
        // Eventos de Servicios Externos (1500-1599)
        /// <summary>
        /// Eventos relacionados con servicios externos
        /// </summary>
        /// <remarks>
        /// Esta sección incluye eventos que se generan al interactuar con servicios externos.
        /// </remarks>
        /// <summary>
        /// Llamada a servicio externo exitosa
        /// </summary>
        ServicioExternoLlamadaExitosa = 1500,
        /// <summary>    
        /// Llamada a servicio externo fallida
        /// </summary>
        ServicioExternoLlamadaFallida = 1501,
        /// <summary>
        /// Tiempo de espera excedido en servicio externo
        /// </summary>
        ServicioExternoTiempoExcedido = 1502,
        /// <summary>
        /// Servicio externo no disponible
        /// </summary>
        ServicioExternoNoDisponible = 1503,
        /// <summary>
        /// Respuesta inválida de servicio externo
        /// </summary>
        ServicioExternoRespuestaInvalida = 1504,
        /// <summary>
        /// API de terceros conectado exitosamente
        /// </summary>
        APITercerosConectado = 1505,
        /// <summary>
        /// Error en API de terceros        
        /// </summary>
        APITercerosError = 1506,
        #endregion

        #region Eventos de Seguridad (1600-1699)    
        // Eventos de Seguridad (1600-1699)
        /// <summary>
        /// Estos eventos están relacionados con la seguridad de la aplicación
        /// </summary>
        /// <remarks>
        /// Estos eventos son generados por el sistema de seguridad y auditoría.
        /// </remarks>
        /// <summary>
        /// Intento de acceso no autorizado
        /// </summary>
        SeguridadIntentoAccesoNoAutorizado = 1600,
        /// <summary>
        /// Violación de acceso a recurso protegido
        /// </summary>
        SeguridadViolacionAcceso = 1601,
        /// <summary>
        /// Cambio de permisos realizado
        /// </summary>
        SeguridadCambioPermisos = 1602,
        /// <summary>
        /// Configuración de seguridad modificada
        /// </summary>
        SeguridadConfiguracionModificada = 1603,
        /// <summary>
        /// Password reseteado exitosamente
        /// </summary>
        SeguridadPasswordReseteado = 1604,
        /// <summary>
        /// Cuenta de usuario eliminada
        /// </summary>
        SeguridadCuentaEliminada = 1605,
        #endregion

        #region Eventos del Sistema y Rendimiento (1700-1799) 
        // Eventos del Sistema y Rendimiento (1700-1799)
        /// <summary>
        /// Eventos del Sistema y Rendimiento (1700-1799)
        /// </summary>
        /// <remarks>
        /// Estos eventos están relacionados con el estado del sistema y su rendimiento
        /// </remarks>
        /// <summary>
        /// Sistema iniciado
        /// </summary>
        SistemaInicio = 1700,
        /// <summary>
        /// Sistema apagado
        /// </summary>
        SistemaApagado = 1701,
        /// <summary>
        /// Configuración del sistema cargada   
        /// </summary>
        SistemaConfiguracionCargada = 1702,
        /// <summary>
        /// Error crítico del sistema
        /// </summary>
        SistemaConfiguracionError = 1703,
        /// <summary>
        /// Consulta lenta detectada
        /// </summary>
        RendimientoConsultaLenta = 1704,
        /// <summary>
        /// Uso alto de memoria detectado
        /// </summary>
        RendimientoAltoUsoMemoria = 1705,
        /// <summary>   
        /// Uso alto de CPU detectado
        /// </summary>
        RendimientoAltoUsoCPU = 1706,
        /// <summary>
        /// Evento de caché hit
        /// </summary>
        CacheHit = 1707,
        /// <summary>
        /// Evento de caché miss    
        /// </summary>
        CacheMiss = 1708,
        /// <summary>
        /// Evento de caché limpiado
        /// </summary>
        CacheLimpiado = 1709,
        #endregion

        #region Eventos de Comunicación y Red (1800-1899)   
        // Eventos de Comunicación y Red (1800-1899)
        /// <summary>
        /// Estos eventos están relacionados con la comunicación y el estado de la red
        /// </summary>
        /// <remarks>
        /// Estos eventos son generados por el sistema de comunicación y red.
        /// </remarks>
        /// <summary>
        /// Evento de comunicación recibido
        /// </summary>
        ComunicacionRequestRecibido = 1800,
        /// <summary>
        /// Evento de comunicación enviado
        /// </summary>
        ComunicacionResponseEnviado = 1801,
        /// <summary>
        /// Evento de error en la comunicación
        /// </summary>
        ComunicacionError = 1802,
        /// <summary>
        /// Evento de timeout en la comunicación
        /// </summary>
        ComunicacionTimeout = 1803,
        /// <summary>
        /// Evento de conexión establecida
        /// </summary>
        RedConexionEstablecida = 1804,
        /// <summary>
        /// Evento de conexión perdida
        /// </summary>
        RedConexionPerdida = 1805,
        #endregion

        #region Eventos de Negocio Específicos (1900-1999)
        // Eventos de Negocio Específicos (1900-1999)
        /// <summary>
        /// Estos eventos están relacionados con reglas y procesos de negocio específicos de la aplicación
        /// </summary>
        /// <remarks>
        /// Estos eventos son generados por el motor de reglas de negocio y otros componentes relacionados.
        /// </remarks>
        /// Evento de regla de negocio aplicada
        /// </summary>
        NegocioReglaAplicada = 1900,
        /// <summary>
        /// Evento de violación de regla de negocio
        /// </summary>
        NegocioReglaViolada = 1901,
        /// <summary>
        /// Evento de proceso de negocio completado 
        /// </summary>
        NegocioProcesoCompletado = 1902,
        /// <summary>
        /// Evento de proceso de negocio fallido
        /// </summary>
        NegocioProcesoFallido = 1903,
        /// <summary>
        /// Evento de cambio de estado en negocio
        /// </summary>
        NegocioEstadoCambiado = 1904,
        /// <summary>
        /// Evento de inicio de workflow de negocio
        /// </summary>
        NegocioWorkflowIniciado = 1905,
        /// <summary>
        /// Evento de completado de workflow de negocio
        /// </summary>
        NegocioWorkflowCompletado = 1906,
        #endregion

        #region Eventos de Monitoreo y Health Check (2000-2099) 
        // Eventos de Monitoreo y Health Check (2000-2099)
        /// <summary>
        /// Estos eventos están relacionados con el monitoreo del sistema y las verificaciones de salud
        /// </summary>
        /// <remarks>
        /// Estos eventos son generados por el sistema de monitoreo y las herramientas de health check.
        /// </remarks>
        /// <summary>
        /// Evento de health check exitoso
        /// </summary>  
        HealthCheckExitoso = 2000,
        /// <summary>
        /// Evento de health check fallido
        /// </summary>
        HealthCheckFallido = 2001,
        /// <summary>
        /// Evento de métrica registrada
        /// </summary>
        MonitoreoMetricaRegistrada = 2002,
        /// <summary>
        /// Evento de alerta generada
        /// </summary>
        MonitoreoAlertaGenerada = 2003,
        /// <summary>
        /// Evento de umbral excedido
        /// </summary>
        MonitoreoUmbralExcedido = 2004,
        #endregion

        #region Eventos de Errores y Excepciones (3000-3999)    
        // Eventos de Errores y Excepciones (3000-3999)
        /// <summary>
        /// Estos eventos están relacionados con errores y excepciones ocurridas en la aplicación
        /// </summary>
        /// <remarks>
        /// Estos eventos son generados por el sistema de manejo de errores y excepciones.
        /// </remarks>
        /// <summary>
        /// Evento de error no manejado
        /// </summary>
        ErrorNoManejado = 3000,
        /// <summary>
        /// Evento de error manejado
        /// </summary>
        ErrorManejado = 3001,
        /// <summary>
        /// Evento de error crítico
        /// </summary>
        ErrorCritico = 3002,
        /// <summary>
        /// Excepción de base de datos
        /// </summary>
        ExcepcionBaseDatos = 3003,
        /// <summary>   
        /// Excepción de servicio externo
        /// </summary>
        ExcepcionServicioExterno = 3004,
        /// <summary>
        /// Excepción de validación
        /// </summary>
        ExcepcionValidacion = 3005,
        /// <summary>
        /// Excepción de seguridad
        /// </summary>
        ExcepcionSeguridad = 3006,
        /// <summary>
        /// Excepción de negocio
        /// </summary>
        ExcepcionNegocio = 3007,
        /// <summary>
        /// Excepción del sistema
        /// </summary>
        ExcepcionSistema = 3008,
        #endregion

        #region Eventos de Depuración y Desarrollo (4000-4099)
        // Eventos de Depuración y Desarrollo (4000-4099)
        /// <summary>
        /// Estos eventos están relacionados con la depuración y el desarrollo de la aplicación
        /// </summary>
        /// <remarks>
        /// Estos eventos son generados durante el proceso de desarrollo y depuración.
        /// </remarks>
        /// <summary>
        /// Evento de punto de control de depuración
        /// </summary>
        DepuracionPuntoControl = 4000,
        /// <summary>   
        /// Evento de variable de estado en depuración
        /// </summary>
        DepuracionVariableEstado = 4001,
        /// <summary>   
        /// Evento de flujo de ejecución en depuración  
        /// </summary>
        DepuracionFlujoEjecucion = 4002
        #endregion

    }
}