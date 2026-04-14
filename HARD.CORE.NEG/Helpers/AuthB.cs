// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using HARD.CORE.DAT.Interfaces;
// using HARD.CORE.NEG.Interfaces;
// using HARD.CORE.OBJ;
// using HARD.CORE.OBJ.Models;

// namespace HARD.CORE.NEG
// {
//     public class AuthB
//     {
//         private readonly IServiceBase<Usuario, Usuario, BaseFilter, int> _usuarioService;

//         public AuthB(IServiceBase<Usuario, Usuario, BaseFilter, int> usuarioService,
//         IRepositoryBase<Usuario)
//         {
//             _usuarioService = usuarioService;
//         }

//         public async Task<bool> ValidateUser(string username, string password)
//         {
//             bool success = false;

//             BaseFilter filter = new BaseFilter { PageIndex = 1, PageSize = int.MaxValue, Nombre = username };

//             List<Usuario> usuarios = (await _usuarioService.GetAllAsync(filter)).Data.ToList();
//             Usuario usuario = usuarios.FirstOrDefault();

//             if (!string.IsNullOrEmpty(usuario.ClaveUsuario) && username.ToLower() == usuario.ClaveUsuario.ToLower())
//             {
//                 // Validar credenciales contra la base de datos
//                 success = await _usuarioService.AuthenticateUserAsync(new LoginModel { Username = username, Password = password }, usuario.Id).Result.Data;
//             }
//             return (success);
//         }

//     }
// }
