using System;

class Program
{
    static void Main(string[] args)
    {
        bool keepRunning = true;
        Console.WriteLine("C# Calculator");

        while (keepRunning)
        {
            Console.Write("\nEnter first number: ");
            string? firstInput = Console.ReadLine();
            if (firstInput is null) break;
            if (!double.TryParse(firstInput, out double num1) || !double.IsFinite(num1))
            {
                Console.WriteLine("Invalid number format");
                continue;
            }

            Console.Write("Enter an operation (+, -, *, /): ");
            string? op = Console.ReadLine();
            if (op is null) break;

            Console.Write("Enter second number: ");
            string? secondInput = Console.ReadLine();
            if (secondInput is null) break;
            if (!double.TryParse(secondInput, out double num2) || !double.IsFinite(num2))
            {
                Console.WriteLine("Invalid number format");
                continue;
            }

            double result = 0;
            switch (op)
            {
                case "+": result = num1 + num2; break;
                case "-": result = num1 - num2; break;
                case "*": result = num1 * num2; break;
                case "/":
                    if (num2 == 0)
                    {
                        Console.WriteLine("Can't divide on zero");
                        continue;
                    }
                    result = num1 / num2; break;
                default:
                    Console.WriteLine("Unsupported operation");
                    continue;
            }

            if (!double.IsFinite(result))
            {
                Console.WriteLine("Result is outside the supported numeric range");
                continue;
            }

            Console.WriteLine($"Result: {result}");

            Console.Write("\nPerform another calculation? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() != "y")
            {
                keepRunning = false;
            }
        }
        Console.WriteLine("Thank you for using Calculator <3 ");
    }
}
