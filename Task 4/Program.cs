using System;
using System.Text;

namespace LabOverloading;

// Допоміжний клас фізичних констант і спільної валідації (уникнення дублювання коду)
internal static class ThermalValidator
{
    public const double AbsoluteZeroOffset = 273.15;

    public static void ValidateKelvin(double kelvin)
    {
        if (double.IsNaN(kelvin) || double.IsInfinity(kelvin))
            throw new ArgumentException("Температура має бути скінченним числом.");

        if (kelvin < 0.0)
            throw new ArgumentOutOfRangeException(nameof(kelvin), kelvin,
                "Температура за Кельвіном не може бути меншою за 0 K (абсолютний нуль).");
    }
}

// Тип 1: Абсолютна температура в Кельвінах (інваріант: Value >= 0)
public readonly struct Kelvin
{
    public double Value { get; }

    public Kelvin(double value)
    {
        ThermalValidator.ValidateKelvin(value);
        Value = value;
    }

    // Неявне перетворення Kelvin -> double (безпечне читання числового значення у формулах)
    public static implicit operator double(Kelvin k) => k.Value;

    // Явне перетворення double -> Kelvin (може викинути виняток при значенні < 0)
    public static explicit operator Kelvin(double val) => new(val);

    // Явне перетворення Kelvin -> Celsius (зміна шкали вимірювання вимагає уваги програміста)
    public static explicit operator Celsius(Kelvin k) 
        => new(k.Value - ThermalValidator.AbsoluteZeroOffset);

    public override string ToString() => $"{Value:F2} K";
}

// Тип 2: Температура за шкалою Цельсія (інваріант: Degrees >= -273.15)
public readonly struct Celsius
{
    public double Degrees { get; }

    public Celsius(double degrees)
    {
        // Делегуємо перевірку через перерахунок у Кельвіни, щоб не дублювати логіку
        ThermalValidator.ValidateKelvin(degrees + ThermalValidator.AbsoluteZeroOffset);
        Degrees = degrees;
    }

    // Неявне перетворення Celsius -> Kelvin 
    public static implicit operator Kelvin(Celsius c) 
        => new(c.Degrees + ThermalValidator.AbsoluteZeroOffset);

    public override string ToString() => $"{Degrees:F2} °C";
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== 1. Неявні перетворення (implicit) ===");
        var roomTempC = new Celsius(20.0);
        
        // Присвоєння без явного приведення: Celsius -> Kelvin
        Kelvin roomTempK = roomTempC; 
        Console.WriteLine($"Celsius -> Kelvin (implicit): {roomTempC} = {roomTempK}");

        // Присвоєння без явного приведення: Kelvin -> double
        double rawKelvin = roomTempK;
        Console.WriteLine($"Kelvin -> double (implicit): {rawKelvin:F2}");

        Console.WriteLine("\n=== 2. Явні перетворення (explicit) ===");
        var boilingK = new Kelvin(373.15);

        // Помилка компіляції CS0266 без явного приведення:
        // Celsius boilingC = boilingK; 

        // Коректне присвоєння з обов'язковим приведенням: Kelvin -> Celsius
        var boilingC = (Celsius)boilingK;
        Console.WriteLine($"Kelvin -> Celsius (explicit): {boilingK} = {boilingC}");

        // Коректна ініціалізація з обов'язковим приведенням: double -> Kelvin
        var nitrogenK = (Kelvin)77.0;
        Console.WriteLine($"double -> Kelvin (explicit): 77.0 -> {nitrogenK}");

        Console.WriteLine("\n=== 3. Перевірка порушення інваріантів (валідація) ===");
        try
        {
            Console.WriteLine("Спроба виконати (Kelvin)(-15.0)...");
            var invalidK = (Kelvin)(-15.0);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"[Перехоплено виняток]: {ex.GetType().Name} — {ex.Message}");
        }

        try
        {
            Console.WriteLine("\nСпроба створити new Celsius(-300.0)...");
            var invalidC = new Celsius(-300.0);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"[Перехоплено виняток]: {ex.GetType().Name} — {ex.Message}");
        }
    }
}