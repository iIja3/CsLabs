using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace lad3
{
    public class Programm
    {
        public static String datest(string line)
        {
            string date = "";
            if (line.Contains("Событие началось"))
            {
                int index1 = line.IndexOf(' ');
                date = line.Substring(0, index1);
                
            }
            return date;
        }

        public static String winn(string line)
        {
            string text = "";
            if (line.Contains("объявлены победителями"))
            {
                int index1 = line.LastIndexOf(']') + 1;
                int index2= line.IndexOf(' ', index1 + 1) + 1;
                int index3 = line.IndexOf(' ', index1 + 1) + 1;
                text = line.Substring(index1, index3 - index1 + 5);
            }
            return text;
        }

        public static String winox(string line)
        {
            string win = winn(line);
            string text = "";
                if (line.Contains("очков события") && line.Contains(win))
                {
                    int index1 = line.LastIndexOf(']');
                    int index2 = line.LastIndexOf(' ');
                    text = line.Substring(index1 + 1, index2 - index1);
                }
                return text;

        }

        public static String lootw(string line)
        {
            string text = "";
            if (line.Contains("получили ивентовый предмет:"))
            {
                int index1 = line.LastIndexOf(']') + 1;
                text = line.Substring(index1, line.Length - index1);
            }
            return text;
        }

        public static String winol(string line)
        {
            string text = "";
            if (line.Contains("утешительную награду:"))
            {
                int index1 = line.LastIndexOf(']') + 1;
                text = line.Substring(index1, line.Length - index1);
            }
            return text;
        }

        public static int war(string line)
        {
            if (line.Contains("Warning"))
            {
                return 1;
            }
            return 0;
        }
        public static int err(string line)
        {
            if (line.Contains("Error"))
            {
                return 1;
            }
            return 0;
        }
        public static void Main()
        {
            string[] files = File.ReadAllLines("event_server.log");
            int totalWarning = 0;
            int totalError = 0;
            foreach (string file in files)
            {
                totalError += err(file);
                totalWarning += war(file);

                string dates = datest(file);
                string win = winn(file);
                string wino = winox(file);
                string loot = lootw(file);
                string winolu = winol(file);
                bool ower = false;
                if (!string.IsNullOrEmpty(dates))
                    Console.WriteLine("Дата:" + dates);
                if (!string.IsNullOrEmpty(win))
                    Console.WriteLine("Победители:" + win);
                if (!string.IsNullOrEmpty(wino))
                    Console.WriteLine("Очки:" + wino);
                if (!string.IsNullOrEmpty(loot))
                    Console.WriteLine("Ивентовый предмет:" + loot);
            }
            Console.WriteLine($"Всего Warning: {totalWarning}");
            Console.WriteLine($"Всего Error: {totalError}");
        }
    }
}