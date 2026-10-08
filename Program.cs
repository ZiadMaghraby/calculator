using System;
using System.IO;

namespace CalculatorApp;

public class Program
{
    static void Main() => Run(Console.In, Console.Out);

    public static void Run(TextReader input, TextWriter output)
    {
        output.WriteLine("C# Calculator");
        while (TryReadNumber(input, output, "\nEnter first number: ", out double first))
        {
            string? operation;
            while (true)
            {
                output.Write("Enter an operation (+, -, *, /): ");
                operation = input.ReadLine()?.Trim();
                if (operation is null) { output.WriteLine("Thank you for using Calculator <3 "); return; }
                if (operation is "+" or "-" or "*" or "/") break;
                output.WriteLine("Unsupported operation");
            }

            if (!TryReadNumber(input, output, "Enter second number: ", out double second, operation == "/")) break;
            double result = operation switch
            {
                "+" => first + second,
                "-" => first - second,
                "*" => first * second,
                _ => first / second,
            };
            if (!double.IsFinite(result))
            {
                output.WriteLine("Result is outside the supported numeric range");
                continue;
            }
            output.WriteLine($"Result: {result}");
            output.Write("\nPerform another calculation? (y/n): ");
            if (!string.Equals(input.ReadLine()?.Trim(), "y", StringComparison.OrdinalIgnoreCase)) break;
        }
        output.WriteLine("Thank you for using Calculator <3 ");
    }

    private static bool TryReadNumber(TextReader input, TextWriter output, string prompt, out double value, bool nonZero = false)
    {
        while (true)
        {
            output.Write(prompt);
            string? text = input.ReadLine();
            value = 0;
            if (text is null) return false;
            if (!double.TryParse(text, out value) || !double.IsFinite(value))
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
