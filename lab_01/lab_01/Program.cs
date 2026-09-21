using System;

namespace lab_01;

class Program
{
    public static void Main()
    {
        // онлайн людей
        long users = 5473254;
        int ouser = 42455;
        var ofuser = users - ouser;

        // характеристики
        byte oz = 8;
        short tb = 18;

        // загруженность
        float oz1 = 67.31f;
        double pr1 = 98.53;
        int pr2 = 100;
        double pr3 = 65.434123;
        decimal pr4 = 15.12412456234m;
        var tpr = (((int)pr1 + pr2 + (int)pr3 + (int)pr4) - 200);

        int ban = 412;
        byte op = 4;

        string name = "caple.net";
        float rei = 4.5f;
        string state = "Online";
        string stran = "RY";

        string res = "\x1B]8;;https://youtu.be/j-iheFkstFQ?si=2gDsZc7h7rzRk9JG\x1B\\перезагрузить сервер\x1B]8;;\x1B\\";

        Console.WriteLine($"Название сервера: {name}");
        Console.WriteLine($"рейтинг сервера: {rei}");
        Console.WriteLine($"Страна размещение сервера: {stran}");
        Console.WriteLine($"Статус: {state}");
        Console.WriteLine(res);
        Console.WriteLine();
        Console.WriteLine("Характеристика сервера");
        Console.WriteLine($"Озу: {oz}");
        Console.WriteLine($"Место на диске: {tb}");
        Console.WriteLine("4 ядерный процессор");
        Console.WriteLine();
        Console.WriteLine("Загруженность");
        Console.WriteLine($"Процессор: {tpr}%");
        Console.WriteLine($"Ядро 1: {pr1}%");
        Console.WriteLine($"Ядро 2: {pr2}%");
        Console.WriteLine($"Ядро 3: {pr3}%");
        Console.WriteLine($"Ядро 4: {pr4}%");
        Console.WriteLine($"ОЗУ: {oz1}%");

    }
}