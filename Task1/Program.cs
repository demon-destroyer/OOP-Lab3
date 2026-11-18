using System;

namespace LabOverloading;


public static class MathHelper
{
    private static T MaxCore<T>(T a, T b) where T : IComparable<T> 
        => a.CompareTo(b) >= 0 ? a : b;
    
    public static int FindMax(int a, int b) => MaxCore(a, b);
    
    public static double FindMax(double a, double b) => MaxCore(a, b);
    
    public static int FindMax(params int[] values)
    {
        if (values is null || values.Length == 0)
            throw new ArgumentException("Масив не може бути порожнім.", nameof(values));

        int max = values[0];
        for (int i = 1; i < values.Length; i++)
            max = MaxCore(max, values[i]);
        return max;
    }
    
    public static T FindMax<T>(T a, T b) where T : IComparable<T> => MaxCore(a, b);

    // Інший тип повернення при тих самих параметрах
    // Розкоментування викличе помилку компіляції CS0111:
    //public static long FindMax(int a, int b) => a > b ? a : b;
}


public static class AmbiguityDemo
{
    public static double Combine(int a, double b) => a + b;
    public static double Combine(double a, int b) => a * b;
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine(" 1.Вибір перевантажень MathHelper.FindMax");
        
        Console.WriteLine($"Два int (10, 25): {MathHelper.FindMax(10, 25)}");
        
        Console.WriteLine($"Два double (3.14, 2.71): {MathHelper.FindMax(3.14, 2.71)}");
        
        Console.WriteLine($"Список int (4, 18, -3, 12): {MathHelper.FindMax(4, 18, -3, 12)}");
        
        int[] numbers = { 7, 42, 15 };
        Console.WriteLine($"Масив int[] {{7, 42, 15}}: {MathHelper.FindMax(numbers)}");
        
        Console.WriteLine($"Два string (\"Альфа\", \"Бета\"): {MathHelper.FindMax("Альфа", "Бета")}");

        Console.WriteLine("\n 2. Усунення неоднозначності AmbiguityDemo.Combine ");

        // Обидва аргументи int (5, 10)
        // Розкоментування викличе помилку CS0121:
        //Console.WriteLine(AmbiguityDemo.Combine(5, 10));

        
        Console.WriteLine($"Combine(5, (double)10) [додавання]: {AmbiguityDemo.Combine(5, (double)10)}");
        
        Console.WriteLine($"Combine((double)5, 10) [множення]: {AmbiguityDemo.Combine((double)5, 10)}");
    }
}