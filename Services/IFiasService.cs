namespace FiasApi.Services
{
    public interface IFiasService
    {
        Task<string> DownloadAndExtractAsync(int regionCode);
        Task<string> ExtractFromLocalAsync(string filePath, int regionCode);
    }
}
