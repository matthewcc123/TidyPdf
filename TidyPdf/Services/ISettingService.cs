using Microsoft.UI.System;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TidyPdf.Models;
using static TidyPdf.Helpers.ImageHelper;

namespace TidyPdf.Services
{
    public interface ISettingService
    {
        public AppSettings Settings { get; }
        public Task<AppSettings> LoadAsync();
        public Task SaveAsync(AppSettings settings);
    }
}
