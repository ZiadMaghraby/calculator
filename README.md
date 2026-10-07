# Calculator

An interactive C# console calculator built with .NET 10.

## Features

- Addition, subtraction, multiplication, and division.
- Numeric input validation with a retry after invalid input.
- Division-by-zero protection.
- Multiple calculations in one session.

## Requirements

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

## Run locally

```sh
git clone https://github.com/ZiadMaghraby/calculator.git
cd calculator
dotnet run --project CalculatorApp.csproj
```

Enter the first number, an operator (`+`, `-`, `*`, or `/`), and the second number when prompted. Enter `y` to calculate again; any other response exits after a successful calculation.

## Example

```text
C# Calculator

Enter first number: 12
Enter an operation (+, -, *, /): /
Enter second number: 3
Result: 4

Perform another calculation? (y/n): n
Thank you for using Calculator <3
```

## Input and error handling

| Input | Behavior |
| --- | --- |
| Invalid first or second number | Prints `Invalid number format` and starts a new calculation. |
| Unsupported operator | Prints `Unsupported operation` and starts a new calculation. |
| Division by zero | Prints `Can't divide on zero` and starts a new calculation. |

Numbers use the system's current culture, so the decimal separator depends on your locale.

## Project structure

- `Program.cs`: interactive input loop, validation, and arithmetic operations.
- `CalculatorApp.csproj`: .NET 10 console application configuration.
