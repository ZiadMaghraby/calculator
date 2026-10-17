using System;
using System.IO;
using System.Globalization;

namespace CalculatorApp;

public class Program
{
    static void Main() => Run(Console.In, Console.Out);

    public static void Run(TextReader input, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        output.WriteLine("C# Calculator");
        output.WriteLine("Type q or quit to exit; type help at the operation prompt; use ans for the last result.");
        double? answer = null;
        while (TryReadNumber(input, output, "\nEnter first number: ", out double first, answer: answer))
        {
            string? operation;
            while (true)
            {
                output.Write("Enter an operation (+, -, *, /, %, ^): ");
                operation = input.ReadLine()?.Trim();
                if (operation is null || IsQuit(operation)) { output.WriteLine("Thank you for using Calculator <3 "); return; }
                if (operation.Equals("help", StringComparison.OrdinalIgnoreCase) || operation == "?")
                {
                    output.WriteLine("Operations: + add, - subtract, * multiply, / divide, % remainder, ^ power. Type q to exit.");
                    continue;
                }
                operation = operation.ToLowerInvariant() switch
                {
                    "x" or "×" => "*",
                    "÷" => "/",
                    "−" => "-",
                    _ => operation,
                };
                if (operation is "+" or "-" or "*" or "/" or "%" or "^") break;
                output.WriteLine("Unsupported operation");
            }

            if (!TryReadNumber(input, output, "Enter second number: ", out double second, operation is "/" or "%", answer)) break;
            double result = operation switch
            {
                "+" => first + second,
                "-" => first - second,
                "*" => first * second,
                "%" => first % second,
                "^" => Math.Pow(first, second),
                _ => first / second,
            };
            if (double.IsNaN(result))
            {
                output.WriteLine("Operation has no real-number result");
                continue;
            }
            if (!double.IsFinite(result))
            {
                output.WriteLine("Result is outside the supported numeric range");
                continue;
            }
            answer = result;
            output.WriteLine($"Result: {result}");
            if (!ReadRepeat(input, output)) break;
        }
        output.WriteLine("Thank you for using Calculator <3 ");
    }

    private static bool ReadRepeat(TextReader input, TextWriter output)
    {
        while (true)
        {
            output.Write("\nPerform another calculation? (y/n): ");
            string? response = input.ReadLine()?.Trim().ToLowerInvariant();
            if (response is "y" or "yes") return true;
            if (response is null or "n" or "no" || IsQuit(response)) return false;
            output.WriteLine("Enter y/yes to continue or n/no to exit.");
        }
    }

    private static bool IsQuit(string text) =>
        text.Trim().Equals("q", StringComparison.OrdinalIgnoreCase) ||
        text.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase);

    private static bool TryReadNumber(TextReader input, TextWriter output, string prompt, out double value, bool nonZero = false, double? answer = null)
    {
        while (true)
        {
            output.Write(prompt);
            string? text = input.ReadLine();
            value = 0;
            if (text is null || IsQuit(text)) return false;
            if (text.Trim().Equals("ans", StringComparison.OrdinalIgnoreCase))
            {
                if (answer is null)
                {
                    output.WriteLine("No previous result; enter a number first.");
                    continue;
                }
                value = answer.Value;
            }
            else if (!double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value) || !double.IsFinite(value))
            {
                output.WriteLine("Invalid number format");
                continue;
            }
            if (nonZero && value == 0)
            {
                output.WriteLine("Can't divide on zero");
                continue;
            }
            return true;
        }
    }
}
