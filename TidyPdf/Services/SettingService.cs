using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TidyPdf.Models;
using static TidyPdf.Helpers.ImageHelper;

namespace TidyPdf.Services
{
    public class SettingService : ISettingService
    {
        private readonly string settingFilePath;
        private readonly string settingFileDirectory;
        public AppSettings Settings { get; set; } = new AppSettings();
        
        public SettingService()
        {
            settingFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MepLab", "TidyPdf");
            settingFilePath = Path.Combine(settingFileDirectory, "appSettings.json");
        }

        public async Task<AppSettings> LoadAsync()
        {
            try
            {
                if (!File.Exists(settingFilePath))
                {
                    return new AppSettings();
                }

                string json = await File.ReadAllTextAsync(settingFilePath);

                Settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();

                return Settings;
            }
            catch
            {
                return new AppSettings();
            }
        }

        public async Task SaveAsync(AppSettings settings)
        {
            this.Settings = settings;

            Directory.CreateDirectory(settingFileDirectory);

            string json = JsonSerializer.Serialize<AppSettings>(Settings, new JsonSerializerOptions { WriteIndented = true });

            await File.WriteAllTextAsync(settingFilePath, json);

        }
    }
}
