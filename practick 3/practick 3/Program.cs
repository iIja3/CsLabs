using System;
using System.IO;
using System.Linq;

namespace prac3;

public class program
{
    public static string rarm(string[] lines)
    {
        return lines.OrderByDescending(Csaok).FirstOrDefault();
    }
    public static int Csaok(string lines)
    {
        return lines.Count(x => x == '2');
    }
    public static int Cok(string lines)
    {
        return lines.Count(x => x == '-');
    }

    public static void Main()
    {
        int e = 0;
        int y = 0;
        string[] map = File.ReadAllLines("map.txt");
        foreach (var line in map)
        {
            e += Cok(line);
            y += Csaok(line);

        }
        string maxLine = rarm(map);
        Console.WriteLine($"Строка с наибольшим количеством двоек: {maxLine}");
        Console.WriteLine($"всего сокровищ {y} и ловушек {e}");
    }
}