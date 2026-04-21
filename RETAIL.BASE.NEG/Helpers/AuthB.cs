// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using RETAIL.BASE.DAT.Interfaces;
// using RETAIL.BASE.NEG.Interfaces;
// using RETAIL.BASE.OBJ;
// using RETAIL.BASE.OBJ.Models;

// namespace RETAIL.BASE.NEG
// {
//     public class AuthB
//     {
//         private readonly IServiceBase<User, User, BaseFilter, int> _userservice;

//         public AuthB(IServiceBase<User, User, BaseFilter, int> userservice,
//         IRepositoryBase<User)
//         {
//             _userservice = userservice;
//         }

//         public async Task<bool> ValidateUser(string username, string password)
//         {
//             bool success = false;

//             BaseFilter filter = new BaseFilter { PageIndex = 1, PageSize = int.MaxValue, Name = username };

//             List<User> users = (await _userservice.GetAllAsync(filter)).Data.ToList();
//             User user = users.FirstOrDefault();

//             if (!string.IsNullOrEmpty(user.UserName) && username.ToLower() == user.UserName.ToLower())
//             {
//                 // Validar credenciales contra la base de datos
//                 success = await _userservice.AuthenticateUserAsync(new LoginModel { Username = username, Password = password }, user.Id).Result.Data;
//             }
//             return (success);
//         }

//     }
// }
