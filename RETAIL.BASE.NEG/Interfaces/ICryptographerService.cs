using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.NEG.Interfaces
{
    public interface ICryptographerService
    {
        ResultModel<string> CreateHash(string input);
        ResultModel<bool> CompareHash(string input, string hash);
    }
}