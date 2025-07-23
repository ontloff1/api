using System;
using System.Globalization;
using System.IO.Compression;
using System.Net.Http;
using System.Xml;
using FiasApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FiasApi.Services;

public class FiasService
{
    private readonly FiasDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _dataDir;

    public FiasService(FiasDbContext db, IHttpClientFactory httpClientFactory)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _dataDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        Directory.CreateDirectory(_dataDir);
    }

    public async Task<int> LoadRegionAsync(string regionCode, bool useNextDay = false)
    {
        var zipPath = await DownloadArchiveAsync(useNextDay);
        var extractPath = ExtractZip(zipPath);
        var addrobjPath = Directory.EnumerateFiles(extractPath, "ADDROBJ*.XML", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (addrobjPath == null) throw new FileNotFoundException("ADDROBJ.XML не найден в архиве");

        return await ParseAndSaveAsync(addrobjPath, regionCode);
    }

    






private async Task<string> DownloadArchiveAsync(bool useNextDay = false)
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    var client = new HttpClient(handler);

    var infoUrl = "https://fias.nalog.ru/WebServices/Public/GetAllDownloadFileInfo";
    Console.WriteLine("🔍 Получаем JSON: " + infoUrl);
    var json = await client.GetStringAsync(infoUrl);

    var doc = System.Text.Json.JsonDocument.Parse(json);

    var entries = doc.RootElement.EnumerateArray()
        .Where(e => e.TryGetProperty("GarXMLFullURL", out var urlProp) &&
        if (string.IsNullOrEmpty(url))
        {
            Console.WriteLine("❌ GarXMLFullURL отсутствует или пуст.");
            return BadRequest(new { error = "Поле GarXMLFullURL отсутствует или пуст." });
        }
                    !string.IsNullOrWhiteSpace(urlProp.GetString()) &&
                    e.TryGetProperty("Date", out var dateProp) &&
                    DateTime.TryParseExact(dateProp.GetString(), "dd.MM.yyyy", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out _))
        .Select(e => new
        {
            Url = e.GetProperty("GarXMLFullURL").GetString(),
            Date = DateTime.ParseExact(
            e.GetProperty("Date").GetString()!,
            new[] { "dd.MM.yyyy", "yyyy.MM.dd" },
            CultureInfo.InvariantCulture,
            DateTimeStyles.None).GetString()!, "dd.MM.yyyy", CultureInfo.InvariantCulture)
        })
        .OrderByDescending(e => e.Date)
        .FirstOrDefault();

    if (entries == null || string.IsNullOrWhiteSpace(entries.Url))
        throw new Exception("Не удалось найти GarXMLFullURL");

    string finalUrl;
    if (useNextDay)
    {
        var nextDate = entries.Date.AddDays(1);
        finalUrl = $"https://fias-file.nalog.ru/downloads/{nextDate:yyyy.MM.dd}/gar_xml.zip";
        Console.WriteLine("📦 Скачиваем архив за следующий день: " + finalUrl);
    }
    else
    {
        finalUrl = entries.Url;
        Console.WriteLine("📦 Скачиваем текущий архив GarXMLFullURL: " + finalUrl);
    }

    var zipPath = Path.Combine(_dataDir, "fias_full.zip");
    if (File.Exists(zipPath)) File.Delete(zipPath);

    using var response = await client.GetAsync(finalUrl);
    if (!response.IsSuccessStatusCode)
    {
        throw new Exception($"Не удалось скачать архив. Код: {response.StatusCode}");
    }

    await using var fs = new FileStream(zipPath, FileMode.Create);
    await response.Content.CopyToAsync(fs);

    Console.WriteLine("✅ Архив успешно загружен: " + zipPath);
    return zipPath;
}






private string ExtractZip(string zipPath)
    {
        var extractPath = Path.Combine(_dataDir, "unzipped");
        if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);
        ZipFile.ExtractToDirectory(zipPath, extractPath);
        return extractPath;
    }

    private async Task<int> ParseAndSaveAsync(string filePath, string regionCode)
    {
        var added = 0;
        using var reader = XmlReader.Create(filePath, new XmlReaderSettings { Async = true });

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Object")
            {
                if (reader.GetAttribute("REGIONCODE") == regionCode)
                {
                    var obj = new AddrObj
                    {
                        AOGUID = Guid.Parse(reader["AOGUID"]),
                        FORMALNAME = reader["FORMALNAME"],
                        REGIONCODE = reader["REGIONCODE"],
                        AOID = Guid.Parse(reader["AOID"]),
                        AOLEVEL = int.Parse(reader["AOLEVEL"]),
                        SHORTNAME = reader["SHORTNAME"],
                        OFFNAME = reader["OFFNAME"]
                    };

                    if (!_db.AddrObjs.Any(x => x.AOGUID == obj.AOGUID))
                    {
                        _db.AddrObjs.Add(obj);
                        added++;
                    }
                }
            }
        }

        await _db.SaveChangesAsync();
        return added;
    }
}