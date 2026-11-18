using System;
using System.Text;

namespace LabOverloading;

public class Matrix
{
    private readonly double[,] _data;

    public int Rows { get; }
    public int Cols { get; }

    public Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
            throw new ArgumentException($"Розміри матриці мають бути додатними (передано: {rows}x{cols}).");

        Rows = rows;
        Cols = cols;
        _data = new double[rows, cols];
    }
    
    private void ValidateIndices(int row, int col)
    {
        if (row < 0 || row >= Rows || col < 0 || col >= Cols)
        {
            throw new IndexOutOfRangeException(
                $"Індекси ({row}, {col}) виходять за межі. " +
                $"Очікуваний діапазон: рядки [0..{Rows - 1}], стовпці [0..{Cols - 1}].");
        }
    }
    
    public double this[int row, int col]
    {
        get
        {
            ValidateIndices(row, col);
            return _data[row, col];
        }
        set
        {
            ValidateIndices(row, col);
            _data[row, col] = value;
        }
    }
    
    public double this[int linearIndex]
    {
        get
        {
            if (linearIndex < 0 || linearIndex >= Rows * Cols)
                throw new IndexOutOfRangeException(
                    $"Лінійний індекс {linearIndex} поза межами. Очікуваний діапазон: [0..{Rows * Cols - 1}].");
            return this[linearIndex / Cols, linearIndex % Cols];
        }
        set
        {
            if (linearIndex < 0 || linearIndex >= Rows * Cols)
                throw new IndexOutOfRangeException(
                    $"Лінійний індекс {linearIndex} поза межами. Очікуваний діапазон: [0..{Rows * Cols - 1}].");
            this[linearIndex / Cols, linearIndex % Cols] = value;
        }
    }

    // Оператор додавання двох матриць однакового розміру 
    public static Matrix operator +(Matrix a, Matrix b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        if (a.Rows != b.Rows || a.Cols != b.Cols)
            throw new InvalidOperationException(
                $"Неможливо додати матриці різних розмірів: {a.Rows}x{a.Cols} та {b.Rows}x{b.Cols}.");

        var result = new Matrix(a.Rows, a.Cols);
        for (int r = 0; r < a.Rows; r++)
            for (int c = 0; c < a.Cols; c++)
                result._data[r, c] = a._data[r, c] + b._data[r, c];

        return result;
    }

    // Оператор множення матриці на скаляр 
    public static Matrix operator *(Matrix m, double scalar)
    {
        ArgumentNullException.ThrowIfNull(m);

        var result = new Matrix(m.Rows, m.Cols);
        for (int r = 0; r < m.Rows; r++)
            for (int c = 0; c < m.Cols; c++)
                result._data[r, c] = m._data[r, c] * scalar;

        return result;
    }

    // Комутативна пара "скаляр * матриця" через делегування
    public static Matrix operator *(double scalar, Matrix m) => m * scalar;

    // Форматований табличний вивід із вирівнюванням стовпців 
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int r = 0; r < Rows; r++)
        {
            sb.Append("| ");
            for (int c = 0; c < Cols; c++)
                sb.Append($"{_data[r, c],6:F1} ");
            sb.AppendLine("|");
        }
        return sb.ToString();
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine(" 1. Заповнення матриць через індексатори ");
        var mA = new Matrix(2, 3);
        mA[0, 0] = 1.5;  mA[0, 1] = 2.0;  mA[0, 2] = -1.0;
        mA[1, 0] = 4.0;  mA[1, 1] = 0.5;  mA[1, 2] = 3.0;

        var mB = new Matrix(2, 3);
        mB[0, 0] = 0.5;  mB[0, 1] = -2.0; mB[0, 2] = 5.0;
        mB[1, 0] = 1.0;  mB[1, 1] = 3.5;  mB[1, 2] = -1.0;

        Console.WriteLine("Матриця A:");
        Console.Write(mA);
        Console.WriteLine("Матриця B:");
        Console.Write(mB);

        Console.WriteLine("\n 2. Додавання матриць та множення на скаляр ");
        Console.WriteLine("Сума (A + B):");
        Console.Write(mA + mB);

        Console.WriteLine("Масштабована матриця (A * 2.0):");
        Console.Write(mA * 2.0);

        Console.WriteLine("\n 3. Доступ через двовимірний і лінійний індексатори ");
        
        Console.WriteLine($"Звернення mA[1, 2] = {mA[1, 2]}");
        Console.WriteLine($"Звернення mA[5] (лінійний індекс) = {mA[5]}");
        mA[5] = 99.0; 
        Console.WriteLine($"Після mA[5] = 99.0 -> mA[1, 2] = {mA[1, 2]}");

        Console.WriteLine("\n 4. Перевірка обробки помилок (межі та розмірність) ");
        try
        {
            var x = mA[2, 5]; 
        }
        catch (IndexOutOfRangeException ex)
        {
            Console.WriteLine($"[Вихід за межі індексатора]: {ex.Message}");
        }

        try
        {
            var mC = new Matrix(3, 3);
            var invalidSum = mA + mC; 
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[Неспівпадіння розмірів при +]: {ex.Message}");
        }
    }
}