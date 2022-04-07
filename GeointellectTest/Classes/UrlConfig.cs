using System.Text.Json;

namespace GeointellectTest.Classes
{
    public class UrlApi
    {
        public string Url { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public List<string> PropertesForSave { get; set; } = new List<string>();

    }

    public class UrlConfig: IDisposable
    {
        private IList<string> _urls;
        public IList<string> Urls { get { return _urls; } }
        public UrlConfig()
        {
            _urls = new List<string>();
            ReadFromFile();
        }

        private void ReadFromFile()
        {
            if (File.Exists("config.json"))
            {
                using (FileStream fileStream = new FileStream("config.json", FileMode.Open))
                {
                    _urls = JsonSerializer.Deserialize<List<string>>(fileStream);
                }
            }

        }

        public async void SaveToFile()
        {
            if (File.Exists("config.json"))
                File.Delete("config.json");

            using (StreamWriter fileStream = File.CreateText("config.json"))
            {
                string json = JsonSerializer.Serialize(Urls);
                await fileStream.WriteAsync(json);
            }
        }

        public bool CheckProperty(string propertyName)
        {
            return _urls.Contains(propertyName);
        }

        public void Dispose()
        {
            _urls = null;
        }
    }
}
