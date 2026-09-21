using System;

namespace Lab_02;

public class Programl
{
    public static string CheckConfiguration(int peop, int ram, bool star, bool pass)
    {
        if (ram > 1)
        {
            if (peop > 0)
            {

                if (pass)
                {
                    return "Запуск возможен с предупреждением: публичный сервер защищён паролем.";
                }
                else if (ram <= 4 && peop >= 100)
                    return "Запуск возможен с предупреждением: для такого количества игроков рекомендуется больше оперативной памяти.";
                else
                {
                    return "Сервер готов к запуску.";
                }
            }
            else
                return "Запуск невозможен: количество игроков должно быть больше нуля.";
        } else
            return "Запуск невозможен: серверу недостаточно оперативной памяти.";
    }

    public static void Main()
    {
        Console.WriteLine("Welcome to the server check program");
        Console.Write("Enter the number of people on the server: ");
        string humen = Console.ReadLine();
        int peop = Convert.ToInt32(humen);

        Console.Write("Enter the amount of server RAM: ");
        string r = Console.ReadLine();
        int ram = Convert.ToInt32(r);
        
        Console.Write("Do you want to set a password?(true or false): ");
        string pas = Console.ReadLine();
        bool pass = Convert.ToBoolean(pas);

        Console.Write("Start the check?(true or false): ");
        string st = Console.ReadLine();
        bool start = Convert.ToBoolean(st);

        Console.Clear();

        if (start)
        {
            Console.WriteLine(CheckConfiguration(peop, ram, start, pass));
        } else
        {
            Console.WriteLine("ну лан");
        }
        
    }
}