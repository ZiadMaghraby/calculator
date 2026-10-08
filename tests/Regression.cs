using System.Globalization;

int passed = 0;
void Check(string name, string input, string[] expected, string? culture = null)
{
    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture ?? "en-US");
    using var output = new StringWriter();
    CalculatorApp.Program.Run(new StringReader(input), output);
    var actual = output.ToString();
    foreach (string text in expected)
        if (!actual.Contains(text)) throw new Exception($"{name}: missing '{text}' in {actual}");
    if (actual.Split("Thank you for using Calculator").Length != 2)
        throw new Exception($"{name}: expected one clean exit");
    Console.WriteLine($"PASS {name}");
    passed++;
}
Check("retry first operand", "bad\n12\n+\n3\nn\n", ["Invalid number format", "Result: 15"]);
Check("retry operator preserves first operand", "12\n?\n/\n3\nn\n", ["Unsupported operation", "Result: 4"]);
Check("retry second operand preserves calculation", "12\n-\nbad\n2\nn\n", ["Invalid number format", "Result: 10"]);
Check("zero divisor retries second operand", "12\n/\n0\n-0\n3\nn\n", ["Can't divide on zero", "Result: 4"]);
Check("nonfinite input", "NaN\nInfinity\n2\n*\n3\nn\n", ["Invalid number format", "Result: 6"]);
Check("overflow recovery", "1e308\n*\n1e308\n5\n+\n2\nn\n", ["outside the supported numeric range", "Result: 7"]);
Check("repeat calculation", "1\n+\n2\n Y \n6\n/\n2\nn\n", ["Result: 3"]);
Check("locale decimal separator", "1,5\n+\n2,5\nn\n", ["Result: 4"], "fr-FR");
foreach (var input in new[] { "", "bad\n", "1\n", "1\n?\n", "1\n+\n", "1\n/\n0\n" })
    Check("EOF during prompt", input, []);
Console.WriteLine($"{passed} regression scenarios passed.");
