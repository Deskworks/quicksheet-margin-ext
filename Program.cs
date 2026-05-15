using System.Text.Json;
using System.Text.Json.Serialization;

// QuickSheet Margin Extension — break-even & contribution margin calculator
// Protocol: JSON-lines on stdin/stdout
//
// Usage:
//   margin: price, variableCost, fixedCosts
//   margin: 50, 20, 9000
//
// Output (3 rows × 2 cols):
//   Row 0: Contribution margin per unit + margin %
//   Row 1: Break-even units + break-even revenue
//   Row 2: Status indicator + formula summary

var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

var register = new { type = "register", prefix = "margin", name = "Margin Calculator", version = "1.0.0" };
Console.WriteLine(JsonSerializer.Serialize(register, options));
Console.Out.Flush();

while (Console.ReadLine() is { } line)
{
    if (string.IsNullOrWhiteSpace(line)) continue;

    try
    {
        using var doc = JsonDocument.Parse(line);
        var root = doc.RootElement;
        string msgType = root.GetProperty("type").GetString() ?? "";

        if (msgType == "activate")
        {
            string id = root.GetProperty("id").GetString() ?? "";
            var parms = root.GetProperty("params");
            HandleActivate(id, parms, options);
        }
    }
    catch { /* ignore malformed messages */ }
}

static void HandleActivate(string id, JsonElement parms, JsonSerializerOptions options)
{
    string[] args = new string[parms.GetArrayLength()];
    for (int i = 0; i < args.Length; i++)
        args[i] = parms[i].GetString()?.Trim() ?? "";

    if (args.Length < 3)
    {
        SendError(id, "Usage: margin: sellingPrice, variableCostPerUnit, totalFixedCosts", options);
        return;
    }

    if (!double.TryParse(args[0], out double price) ||
        !double.TryParse(args[1], out double variableCost) ||
        !double.TryParse(args[2], out double fixedCosts))
    {
        SendError(id, "All three values must be numbers", options);
        return;
    }

    if (price <= 0)
    {
        SendError(id, "Selling price must be > 0", options);
        return;
    }

    double cm = price - variableCost;         // contribution margin per unit
    double cmRatio = (cm / price) * 100;      // contribution margin ratio %

    var cells = new List<object>();

    if (cm <= 0)
    {
        // Negative or zero margin — break-even impossible
        cells.Add(new { r = 0, c = 0, v = "🔴 Negative Margin" });
        cells.Add(new { r = 0, c = 1, v = $"CM: ${cm:N2}/unit ({cmRatio:F1}%)" });
        cells.Add(new { r = 1, c = 0, v = "Break-even: impossible" });
        cells.Add(new { r = 1, c = 1, v = "Variable cost ≥ price" });
        cells.Add(new { r = 2, c = 0, v = $"Price: ${price:N2}" });
        cells.Add(new { r = 2, c = 1, v = $"Var cost: ${variableCost:N2}" });
    }
    else
    {
        double beUnits = Math.Ceiling(fixedCosts / cm);   // break-even units (round up)
        double beRevenue = beUnits * price;                // break-even revenue

        // Status based on margin ratio
        string status = cmRatio switch
        {
            >= 60 => "🟢 Strong",
            >= 40 => "🟡 Moderate",
            >= 20 => "🟠 Thin",
            _ => "🔴 Razor-thin"
        };

        cells.Add(new { r = 0, c = 0, v = $"{status} margin" });
        cells.Add(new { r = 0, c = 1, v = $"CM: ${cm:N2}/unit ({cmRatio:F1}%)" });
        cells.Add(new { r = 1, c = 0, v = $"Break-even: {beUnits:N0} units" });
        cells.Add(new { r = 1, c = 1, v = $"BE revenue: ${beRevenue:N2}" });
        cells.Add(new { r = 2, c = 0, v = $"Fixed: ${fixedCosts:N2}" });
        cells.Add(new { r = 2, c = 1, v = $"Price ${price:N2} − Var ${variableCost:N2}" });
    }

    var write = new { type = "write", id, cells };
    Console.WriteLine(JsonSerializer.Serialize(write, options));
    Console.Out.Flush();
}

static void SendError(string id, string message, JsonSerializerOptions options)
{
    var error = new { type = "error", id, message };
    Console.WriteLine(JsonSerializer.Serialize(error, options));
    Console.Out.Flush();
}
