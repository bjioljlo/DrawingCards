using System.Text.Json;

namespace CardGame
{
    public static class CardLoader
    {
        public static List<Card> LoadCardsFromJson(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
                throw new FileNotFoundException($"File not found: {jsonFilePath}");

            string json = File.ReadAllText(jsonFilePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<Card>>(json, options) ?? new List<Card>();
        }
    }
}
