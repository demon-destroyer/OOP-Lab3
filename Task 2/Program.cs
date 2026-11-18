using System;

namespace LabOverloading;

public sealed class Vector2D : IEquatable<Vector2D>
{
    private const int PrecisionDigits = 6;
    public const double Epsilon = 1e-6;

    // Незмінювані координати після створення 
    public double X { get; }
    public double Y { get; }

    // Довжина вектора в евклідовій метриці
    public double Length => Math.Sqrt(X * X + Y * Y);

    // Основний конструктор та фабричні методи 
    public Vector2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static Vector2D Zero => new(0, 0);
    public static Vector2D FromPolar(double radius, double angleRadians) 
        => new(radius * Math.Cos(angleRadians), radius * Math.Sin(angleRadians));
    private static double Quantize(double val) => Math.Round(val, PrecisionDigits);

    // Бінарні оператори додавання та віднімання 
    public static Vector2D operator +(Vector2D a, Vector2D b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);
        return new Vector2D(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2D operator -(Vector2D a, Vector2D b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);
        return new Vector2D(a.X - b.X, a.Y - b.Y);
    }

    // Унарний оператор мінуса 
    public static Vector2D operator -(Vector2D v)
    {
        ArgumentNullException.ThrowIfNull(v);
        return new Vector2D(-v.X, -v.Y);
    }

    // Множення на скаляр: комутативна пара "вектор * скаляр" та "скаляр * вектор" 
    public static Vector2D operator *(Vector2D v, double scalar)
    {
        ArgumentNullException.ThrowIfNull(v);
        return new Vector2D(v.X * scalar, v.Y * scalar);
    }

    // Делегування виклику для уникнення дублювання коду
    public static Vector2D operator *(double scalar, Vector2D v) => v * scalar;

    // Ділення на скаляр із перевіркою порогу Epsilon 
    public static Vector2D operator /(Vector2D v, double scalar)
    {
        ArgumentNullException.ThrowIfNull(v);
        if (Math.Abs(scalar) < Epsilon)
            throw new DivideByZeroException($"Дільник ({scalar}) менший за допустимий поріг {Epsilon}.");
        return new Vector2D(v.X / scalar, v.Y / scalar);
    }

    // Оператори рівності та нерівності з урахуванням квантування 
    public static bool operator ==(Vector2D? a, Vector2D? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return Quantize(a.X) == Quantize(b.X) && Quantize(a.Y) == Quantize(b.Y);
    }

    public static bool operator !=(Vector2D? a, Vector2D? b) => !(a == b);

    // Оператори порівняння за довжиною вектора з урахуванням квантування 
    public static bool operator <(Vector2D? a, Vector2D? b)
    {
        if (a is null || b is null) return false;
        return Quantize(a.Length) < Quantize(b.Length);
    }

    public static bool operator >(Vector2D? a, Vector2D? b) => b < a;

    // Узгоджені методи Equals, GetHashCode та ToString 
    public bool Equals(Vector2D? other) => this == other;
    public override bool Equals(object? obj) => obj is Vector2D other && this == other;
    public override int GetHashCode() => HashCode.Combine(Quantize(X), Quantize(Y));
    public override string ToString() => $"({X:F2}; {Y:F2}) [|v| = {Length:F2}]";
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Демонстрація роботи класу Vector2D ===");
        var v1 = new Vector2D(3, 4);
        var v2 = Vector2D.FromPolar(2, 0); 
        Console.WriteLine($"v1 = {v1}");
        Console.WriteLine($"v2 (через фабрику FromPolar) = {v2}");

        Console.WriteLine("\n--- 1. Арифметичні та унарні оператори ---");
        Console.WriteLine($"v1 + v2 = {v1 + v2}");
        Console.WriteLine($"v1 - v2 = {v1 - v2}");
        Console.WriteLine($"-v1 (унарний мінус) = {-v1}");
        Console.WriteLine($"v1 * 2.5 = {v1 * 2.5}");
        Console.WriteLine($"2.5 * v1 (комутативність) = {2.5 * v1}");
        Console.WriteLine($"v1 / 2.0 = {v1 / 2.0}");

        Console.WriteLine("\n--- 2. Рівність, хеш-код і порівняння (з похибкою) ---");
        var v1Almost = new Vector2D(3.0000001, 3.9999999);
        Console.WriteLine($"v1 == v1Almost (різниця 10^-7): {v1 == v1Almost}");
        Console.WriteLine($"v1 != v2: {v1 != v2}");
        Console.WriteLine($"Hash(v1) == Hash(v1Almost): {v1.GetHashCode() == v1Almost.GetHashCode()} ({v1.GetHashCode()})");
        Console.WriteLine($"v2 < v1 (за модулем 2.0 < 5.0): {v2 < v1}");
        Console.WriteLine($"v1 > v2 (за модулем 5.0 > 2.0): {v1 > v2}");

        Console.WriteLine("\n--- 3. Перевірка ділення на значення нижче порогу ---");
        try
        {
            Console.WriteLine("Спроба виконати v1 / 1e-8...");
            var invalid = v1 / 1e-8;
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"Перехоплено виняток DivideByZeroException: {ex.Message}");
        }
    }
}