using System.Net.Http.Json;
using System.Text.Json;
using Crezco.PayableSchema;

Console.Write("Enter Country Code: ");
var countryCode = Console.ReadLine();

Console.Write("Enter Currency Code: ");
var currencyCode = Console.ReadLine();


var client = new HttpClient(
    new AuthorizationDelegatingHandler
    {
        InnerHandler = new VersionDelegatingHandler
        {
            InnerHandler = new HttpClientHandler()
        }
    }
)
{
    BaseAddress = new Uri("https://api.partners.crezco.com/")
};

var httpResponseMessage =
    await client.GetAsync(
        $"/pay-runs/payables/schema?country={countryCode}&currency={currencyCode}");

httpResponseMessage.EnsureSuccessStatusCode();

var json = await httpResponseMessage.Content.ReadFromJsonAsync<JsonDocument>()
           ?? throw new InvalidOperationException("Failed to read JSON document");


var possibleBeneficiaries = GetPossibleBeneficiaries(json);
Console.WriteLine("** Possible Beneficiaries **");
foreach (var beneficiary in possibleBeneficiaries)
{
    Console.WriteLine(beneficiary);
}

Console.WriteLine();

var requiredProperties = GetRequiredBankAccountProperties(json);
PrintRequiredBankAccountProperties(requiredProperties);

Console.WriteLine();

var possiblePurposeCodes = GetPossiblePurposeCodes(json);
PrintPossiblePurposeCodes(possiblePurposeCodes);

IReadOnlyCollection<string> GetPossibleBeneficiaries(JsonDocument jsonDocument)
{
    var beneficiaryJsonElement = jsonDocument.RootElement.GetProperty("$defs")
        .GetProperty("beneficiary");
    
    return 
        beneficiaryJsonElement.GetProperty("oneOf")
        .EnumerateArray()
        .Select(x => 
            x.GetProperty("title").GetString() ?? throw new InvalidOperationException("Title not found"))
        .ToArray();

}
List<KeyValuePair<string, string[]>> GetRequiredBankAccountProperties(JsonDocument jsonDocument)
{
    var bankAccountJsonElement = jsonDocument.RootElement.GetProperty("$defs")
        .GetProperty("bankAccount");

    return bankAccountJsonElement.GetProperty("oneOf")
        .EnumerateArray()
        .Select(CreateRequiredPropertiesPair)
        .ToList();

    KeyValuePair<string, string[]> CreateRequiredPropertiesPair(JsonElement jsonElement)
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

(bool required, PurposeCode[] purposeCodes) GetPossiblePurposeCodes(JsonDocument schema)
{
    var requiredPurposeCode = schema.RootElement.GetProperty("required")
        .EnumerateArray().Any(x => x.GetString() == "purposeCode");

    var purposeCodeJsonElement = schema.RootElement.GetProperty("$defs")
        .GetProperty("purposeCode");
    var oneOf = purposeCodeJsonElement
        .GetProperty("oneOf")
        .EnumerateArray()
        .Select(x => new PurposeCode(
                Const: x.GetProperty("const").GetString(),
                Title: x.TryGetProperty("title", out var titleProperty) switch
                {
                    true => titleProperty.GetString(),
                    false => null
                },
                Enabled: true
            )
        );

    var notAnyOf = purposeCodeJsonElement
        .GetProperty("not")
        .GetProperty("anyOf")
        .EnumerateArray()
        .Select(x => new PurposeCode(
                Const: x.GetProperty("const").GetString(),
                Title: x.GetProperty("title").GetString(),
                Enabled: false
            )
        );

    return (requiredPurposeCode, oneOf.Concat(notAnyOf).ToArray());
}


void PrintPossiblePurposeCodes((bool required, PurposeCode[] purposeCodes) input)
{
    Console.WriteLine($"Purpose Code is {input.required switch {
        false => "*not* ",
        _ => ""
    }}required");


    Console.WriteLine("** Possible Purpose Codes **");
    foreach (var purposeCode in input.purposeCodes)
    {
        Console.WriteLine(
            $"Const: {purposeCode.Const ?? "'null'"}, Title: {purposeCode.Title} (Enabled: {purposeCode.Enabled})");
    }
}

record PurposeCode(string? Const, string? Title, bool Enabled);