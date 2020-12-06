namespace NLIIS_Autoreferer.Services
{
    public interface IReferer
    {
        string GenerateRefer(string text);
        void Clear();
    }
}