using HARD.CORE.OBJ.Models;

namespace HARD.CORE.NEG.Interfaces
{
    public interface ICryptographerService
    {
        ResultModel<string> CreateHash(string input);
        ResultModel<bool> CompareHash(string input, string hash);
    }
}