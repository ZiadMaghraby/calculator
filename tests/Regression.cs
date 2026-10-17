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
    var results = System.Text.RegularExpressions.Regex.Matches(actual, @"Result: ([^\r\n]+)")
        .Select(match => match.Value).ToArray();
    var expectedResults = expected.Where(text => text.StartsWith("Result: ")).ToArray();
    if (!results.SequenceEqual(expectedResults))
        throw new Exception($"{name}: unexpected result sequence: {string.Join(", ", results)}");
    if (actual.Split("Thank you for using Calculator").Length != 2)
        throw new Exception($"{name}: expected one clean exit");
    Console.WriteLine($"PASS {name}");
    passed++;
}
Check("retry first operand", "bad\n12\n+\n3\nn\n", ["Invalid number format", "Result: 15"]);
Check("retry operator preserves first operand", "12\ninvalid\n/\n3\nn\n", ["Unsupported operation", "Result: 4"]);
Check("retry second operand preserves calculation", "12\n-\nbad\n2\nn\n", ["Invalid number format", "Result: 10"]);
Check("zero divisor retries second operand", "12\n/\n0\n-0\n3\nn\n", ["Can't divide on zero", "Result: 4"]);
Check("nonfinite input", "NaN\nInfinity\n2\n*\n3\nn\n", ["Invalid number format", "Result: 6"]);
Check("overflow recovery", "1e308\n*\n1e308\n5\n+\n2\nn\n", ["outside the supported numeric range", "Result: 7"]);
Check("repeat calculation", "1\n+\n2\n Y \n6\n/\n2\nn\n", ["Result: 3", "Result: 3"]);
Check("locale decimal separator", "1,5\n+\n2,5\nn\n", ["Result: 4"], "fr-FR");
Check("comma is not silently treated as grouping", "1,5\n1.5\n+\n2.5\nn\n", ["Invalid number format", "Result: 4"]);
Check("repeated grouping separators are rejected", "1,,2\n12\n+\n3\nn\n", ["Invalid number format", "Result: 15"]);
Check("wrong decimal separator in German", "1.5\n1,5\n+\n2,5\nn\n", ["Invalid number format", "Result: 4"], "de-DE");
Check("scientific notation and whitespace remain supported", " 1.5e2 \n+\n-2.5e1\nn\n", ["Result: 125"]);
foreach (var input in new[] { "", "bad\n", "1\n", "1\ninvalid\n", "1\n+\n", "1\n/\n0\n" })
    Check("EOF during prompt", input, []);
foreach (bool missingInput in new[] { true, false })
{
    try
    {
        CalculatorApp.Program.Run(missingInput ? null! : new StringReader(""), missingInput ? new StringWriter() : null!);
        throw new Exception("Expected a null stream to be rejected");
    }
    catch (ArgumentNullException error)
    {
        if (error.ParamName != (missingInput ? "input" : "output")) throw;
        passed++;
    }
}
foreach (var input in new[] { "q\n", " QUIT \n", "1\nq\n", "1\n+\nquit\n", "1\n/\n0\nq\n" })
    Check("quit at calculation prompt", input, []);
Check("repeat retries invalid response", "1\n+\n2\nmaybe\n YES \n4\n-\n1\nNO\n", ["Result: 3", "Enter y/yes", "Result: 3"]);
Check("EOF at repeat", "1\n+\n2\n", ["Result: 3"]);
Check("quit at repeat", "1\n+\n2\nquit\n", ["Result: 3"]);
Check("operator help preserves operand", "12\nHELP\n?\n/\n3\nn\n", ["Operations:", "Result: 4"]);
Check("uppercase multiplication alias", "3\n X \n4\nn\n", ["Result: 12"]);
Check("typographic multiplication", "3\n×\n4\nn\n", ["Result: 12"]);
Check("typographic division", "12\n÷\n4\nn\n", ["Result: 3"]);
Check("typographic subtraction", "3\n−\n4\nn\n", ["Result: -1"]);
Check("remainder", "17\n%\n5\nn\n", ["Result: 2"]);
Check("negative remainder", "-17\n%\n5\nn\n", ["Result: -2"]);
Check("remainder zero recovery", "17\n%\n0\n5\nn\n", ["Can't divide on zero", "Result: 2"]);
Check("power", "2\n^\n10\nn\n", ["Result: 1024"]);
Check("negative exponent", "2\n^\n-2\nn\n", ["Result: 0.25"]);
Check("power domain recovery", "-1\n^\n0.5\n2\n+\n3\nn\n", ["no real-number result", "Result: 5"]);
Check("power overflow recovery", "10\n^\n1000\n2\n+\n3\nn\n", ["outside the supported numeric range", "Result: 5"]);
Check("ans requires previous result", "ans\n2\n+\n3\nn\n", ["No previous result", "Result: 5"]);
Check("ans in both operands", "2\n+\n3\ny\n ANS \n*\nans\nn\n", ["Result: 5", "Result: 25"]);
Check("ans still protects zero divisor", "1\n-\n1\ny\n4\n/\nans\n2\nn\n", ["Result: 0", "Can't divide on zero", "Result: 2"]);
Check("overflow preserves ans", "2\n+\n3\ny\n1e308\n*\n1e308\nans\n+\n1\nn\n", ["Result: 5", "outside the supported numeric range", "Result: 6"]);
Check("ans is local to a session", "ans\nq\n", ["No previous result"]);
Console.WriteLine($"{passed} regression scenarios passed.");
