using System;
using System.Globalization;

namespace lab4;
public class LogEntry
    {
        public DateTime Timestamp;
        public string Level;
        public string Category;
        public string Message;
    }
public class Program
{
    public static LogEntry[] ParseLog(string[] lines)
    {
        var entr = new List<LogEntry>();
        foreach (var line in lines)
        {
            int time1 = line.IndexOf(' ');
            int time2 = line.IndexOf(' ', time1 + 1);
            DateTime time = DateTime.Parse(line.Substring(0, time2));

            int lelvel1 = line.IndexOf('[') + 1;
            int lelvel2 = line.IndexOf(']');
            string level = line.Substring(lelvel1, lelvel2 - lelvel1);

            int cat1 = line.IndexOf('[', lelvel2) + 1;
            int cat2 = line.IndexOf(']', cat1);
            string category = line.Substring(cat1, cat2 - cat1);

            string mass = line.Substring(cat2 + 2);

            entr.Add(new LogEntry
            {
                Timestamp = time,
                Level = level,
                Category = category,
                Message = mass
            });
        }
        return entr.ToArray();
    }

    public static LogEntry[] FilterByDate(LogEntry[] entries, DateTime date)
    {
        var result = new List<LogEntry>();
        foreach (var ent in entries)
        {
            if (ent.Timestamp.Date == date.Date)
            {
                result.Add(ent);
            }
        }
        return result.ToArray();
    }

    public static LogEntry[] FilterByLevel(LogEntry[] entries, string level)
    {
        var lev = new List<LogEntry>();
        foreach (var ent in entries)
        {
            if (ent.Level == level)
            {
                lev.Add(ent);
            }
        }
        return lev.ToArray();
    }

    public static LogEntry[] Search(LogEntry[] entries, string text)
    {
        var serc = new List<LogEntry>();
        foreach (var ent in entries)
        {
            if (ent.Message != null && ent.Message.Contains(text, StringComparison.OrdinalIgnoreCase))
            {
                serc.Add(ent);
            }
        }
        return serc.ToArray();
    }

    public static LogEntry[] FilterByCategory(LogEntry[] entries, string category)
    {
        var Car = new List<LogEntry>();
        foreach(var ent in entries)
        {
            if (ent.Category == category)
            {
                Car.Add(ent);
            }
        }
        return Car.ToArray();
    }

    public static int CountByLevel(LogEntry[] entries, string level)
    {
        int count = 0;
        foreach (var ent in entries)
        {
            if (ent.Level == level)
            {
                count++;
            }
        }
        return count;
    }

    public static string GetServerStatus(LogEntry[] entries)
    {
        bool fat = false;
        bool err = false;
        foreach (var ent in entries)
        {
            if ((ent.Level == "Fatal") && (ent.Category == "Server")) {
                fat = true;
                break;
            }
            if (ent.Level == "Error")
            {
                err= true;
            }
        }

        if (fat)
        {
            return "КРИТИЧЕСКАЯ ОШИБКА: сервер остановлен";
        }

        if (err)
        {
            return "Есть ошибки: требуется проверка";
        }

        return "Сервер работает штатно";
    }

    public static void Main()
    {
        Console.WriteLine("Добро пожаловать");
        string[] file = File.ReadAllLines("event_server.log");
        LogEntry[] entries = ParseLog(file);
        Console.WriteLine($"Успешно загружено логов: {entries.Length}");
        string status = GetServerStatus(entries);
        Console.WriteLine($"Статус сервера: {status}");
        int errorCount = CountByLevel(entries, "Error");
        int fatalCount = CountByLevel(entries, "Fatal");
        Console.WriteLine($"Количество ошибок (Error): {errorCount}");
        Console.WriteLine($"Количество критических ошибок (Fatal): {fatalCount}");
        Console.Write("Введите что ищете: ");
        string rea = Console.ReadLine();
        if (rea != null) 
        {
            LogEntry[] sear = Search(entries, rea);
            Console.WriteLine($"Найдено записей по запросу {rea}: {sear.Length}");
        }

    }
}