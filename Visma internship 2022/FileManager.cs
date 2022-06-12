using System.Text.Json;

namespace Visma_internship_2022
{
    internal class FileManager
    {

        private const string Filename = "./meetings.json";
        public static void WriteToJsonFile(string jsonString, string filename = Filename)
        {
            File.WriteAllText(filename,jsonString);
        }

        public static string GetJsonStringFromFile(string filename = Filename)
        {
            if (!File.Exists(filename)) File.Create(filename).Close();
            string jsonString = File.ReadAllText(filename);

            if (IsJson(jsonString)) return jsonString;
            else 
            {
                WriteToJsonFile("[]",filename);
                return "[]";
            }
        }

        private static bool IsJson(string source)
        {
            if (source == null) return false;

            try
            {
                JsonDocument.Parse(source);
                return true;
            } catch (JsonException)
            {
                return false;
            }
        }
    }
}
