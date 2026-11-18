using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TidyPdf.Models;

namespace TidyPdf.Serialization
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AppSettings))]
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
