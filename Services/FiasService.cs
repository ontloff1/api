using FiasApi.Models;
using System.IO.Compression;
using System.Net.Http.Json;

namespace FiasApi.Services
{
    public class FiasService : IFiasService
    {
        private readonly HttpClient _httpClient;
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "storage");

        public FiasService()
        {
            _httpClient = new HttpClient();
            if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
        }

        public async Task<string> DownloadAndExtractAsync(int regionCode)
        {
            var infoUrl = "https://fias.nalog.ru/WebServices/Public/GetAllDownloadFileInfo";
            var downloadInfos = await _httpClient.GetFromJsonAsync<List<DownloadInfo>>(infoUrl);

            if (downloadInfos == null || downloadInfos.Count == 0)
                throw new Exception("Не удалось получить данные о файлах FIAS.");

            var latest = downloadInfos.Last();
            var fileUrl = latest.FiasDeltaXmlUrl ?? latest.FiasCompleteXmlUrl;

            var zipPath = Path.Combine(_storagePath, "fias.zip");
            using (var response = await _httpClient.GetAsync(fileUrl))
            {
                response.EnsureSuccessStatusCode();
                await using var fs = new FileStream(zipPath, FileMode.Create);
                await response.Content.CopyToAsync(fs);
            }

            return await ExtractFromLocalAsync(zipPath, regionCode);
        }

        public async Task<string> ExtractFromLocalAsync(string filePath, int regionCode)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            var extractPath = Path.Combine(_storagePath, "extracted");
            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);

            ZipFile.ExtractToDirectory(filePath, extractPath);

            var regionFiles = Directory.GetFiles(extractPath, $"*_{regionCode}_*.xml", SearchOption.AllDirectories);

            if (regionFiles.Length == 0)
                return $"Файлы для региона {regionCode} не найдены.";

            return $"Извлечено {regionFiles.Length} файлов для региона {regionCode}. Путь: {extractPath}";
        }
    }
}
