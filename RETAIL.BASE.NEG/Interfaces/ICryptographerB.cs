namespace RETAIL.BASE.NEG.Interfaces
{
    public interface ICryptographerB
    {
        string CreateHash(string input);
        bool CompareHash(string input, string hash);
    }
}