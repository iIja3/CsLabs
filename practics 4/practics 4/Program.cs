using System;

namespace parac2;

public class Program
{
    static void sesery(int[][] array)
    {
        int t = 0;
        int n = 0;
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < array[i].Length; j++)
            {
                t += array[i][j];
            }
            n++;
        }
        int sr = t / n;
        int[] resu = new int[array.Length];
        int f = 1;
        for (int i = 0; i < array.Length; i++) // записывает в список сумму элементов
        {
            int sum = 0;
            for (int j = 0; j < array[i].Length; j++)
            {
                sum += array[i][j];
            }
            resu[i] = sum;
        }
        foreach (int x in resu)
        {
            if (x > sr)
            {
                f++;
            }
        }
        Console.WriteLine($"Средняя сумма строки (sr):{f} {sr} {resu[f]}");
    }
    static void seseq(int[][] array)
    {
        if (array == null || array.Length == 0) return;

        int t = 0;
        int totalElements = 0;
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < array[i].Length; j++)
            {
                t += array[i][j];
                totalElements++;
            }
        }
        double sr = (double)t / totalElements;
        int[] resu = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            int sum = 0;
            for (int j = 0; j < array[i].Length; j++)
            {
                sum += array[i][j];
            }
            resu[i] = sum;
        }
        int closestIndex = 0;
        double minDifference = Math.Abs(resu[0] - sr);

        for (int i = 1; i < resu.Length; i++)
        {
            double difference = Math.Abs(resu[i] - sr);
            if (difference < minDifference)
            {
                minDifference = difference;
                closestIndex = i;
            }
        }
        Console.WriteLine($"Ближе всего к среднему бой {closestIndex + 1} с суммой {resu[closestIndex]}");
    }
    public static void Maxpe(int[][] array)
    {
        int[] resu = new int[array.Length];

        for (int i = 0; i < array.Length; i++) // записывает в список сумму элементов
        {
            int sum = 0;
            for (int j = 0; j < array[i].Length; j++)
            {
                sum += array[i][j];
            }
            resu[i] = sum;
        }

        int max = resu.Max();
        int f = 1;

        foreach (int a in resu)
        {
            if (a < max)
            {
                f++;
            }
        }
        Console.WriteLine($"боец {f} с max {max} баллами");
    }
    public static void Main()
    {
        int[][] battles =
        {
            new[] {15,20,10},
            new[] {30,25},
            new[] {40},
            new[] {20,20,15,10}
        };
        Maxpe(battles);
        seseq(battles);
        int max = battles.SelectMany(row => row).Max();
        Console.WriteLine($"Самый результативный бой: {max}");
    }
}