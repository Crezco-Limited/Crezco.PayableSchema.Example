using System.Net.Http.Json;
using System.Text.Json;
using Crezco.PayableSchema;

Console.Write("Enter Country Code: ");
var countryCode = Console.ReadLine();

Console.Write("Enter Currency Code: ");
var currencyCode = Console.ReadLine();

var client = new HttpClient(new CrezcoHttpMessageHandler());

var httpResponseMessage =
    await client.GetAsync(
        $"https://localhost:5001/organisation/{Guid.NewGuid()}/pay-run/payables/schema?country={countryCode}&currency={currencyCode}");

httpResponseMessage.EnsureSuccessStatusCode();

var json = await httpResponseMessage.Content.ReadFromJsonAsync<JsonDocument>();

var requiredProperties = GetRequiredBankAccountProperties(json);
PrintRequiredBankAccountProperties(requiredProperties);

var possiblePurposeCodes = GetPossiblePurposeCodes(json);
PrintPossiblePurposeCodes(possiblePurposeCodes);

List<KeyValuePair<string, string[]>> GetRequiredBankAccountProperties(JsonDocument? jsonDocument)
{
    var bankAccountJsonElement = jsonDocument.RootElement.GetProperty("$defs")
        .EnumerateObject()
        .Single(x => x.Value.TryGetProperty("$id", out var property) && property.GetString() == "http://crezco.com/schemas/pay-run/payables/bank-account")
        .Value;

    List<KeyValuePair<string, string[]>> keyValuePairs = new();

    if (bankAccountJsonElement.TryGetProperty("oneOf", out var oneOfJsonElement))
    {
        foreach (var item in oneOfJsonElement.EnumerateArray())
        {
            var keyValuePair = CreateRequiredPropertiesPair(item);
            keyValuePairs.Add(keyValuePair!);
        }
    }
    else
    {
        var keyValuePair = CreateRequiredPropertiesPair(bankAccountJsonElement);
        keyValuePairs.Add(keyValuePair!);
    }

    return keyValuePairs;

    KeyValuePair<string?, string?[]> CreateRequiredPropertiesPair(JsonElement jsonElement)
    {
        var title = jsonElement.GetProperty("title")
            .GetString();
        var required = jsonElement.GetProperty("required")
            .EnumerateArray()
            .Select(x => x.GetString())
            .ToArray();

        return KeyValuePair.Create(title, required)!;
    }
}

void PrintRequiredBankAccountProperties(List<KeyValuePair<string, string[]>> list)
{
    Console.WriteLine("** Bank Account Properties **");
    foreach (var properties in list)
    {
        Console.WriteLine(properties.Key);
        Console.WriteLine("=============");
        foreach (var property in properties.Value)
        {
            Console.WriteLine(property);
        }

        Console.WriteLine();
    }
}

PurposeCode[]? GetPossiblePurposeCodes(JsonDocument? json1)
{
    var requiredPayRunProperties = json1!.RootElement.GetProperty("required").Deserialize<string[]>()!;
    PurposeCode[] strings = null;
    if (requiredPayRunProperties.Contains("purposeCode"))
    {
        strings = json1.RootElement.GetProperty("$defs").GetProperty("purposeCode")
            .GetProperty("oneOf")
            .Deserialize<PurposeCode[]>(JsonSerializerOptions.Web)!;
    }

    return strings;
}


void PrintPossiblePurposeCodes(PurposeCode[]? possiblePurposeCodes1)
{
    if (possiblePurposeCodes1 != null)
    {
        Console.WriteLine("Purpose Code is required");

        foreach (var purposeCode in possiblePurposeCodes1)
        {
            Console.WriteLine("- " + purposeCode.Title + " (" + purposeCode.Const + ") " + (purposeCode.Enabled ? "(Enabled)" : "(Disabled)"));
        }
    }
    
}
record PurposeCode(string Const, string Title, bool Enabled);
