using System;

namespace practic2;


public class Program 
{
    // 2026-09-01 12:16:01.101 [Info][User] User logged in: RavenFox

    public static DateTime GetLogDateTime(string line)
    {
        int index1 = line.IndexOf('[');
        int index2 = line.IndexOf(' ', index1+1);

        string date = line.Substring(0, index1);

        DateTime datetime = DateTime.Parse(date);
        return datetime;
    }

    public static string GetLogLevel(string line)
    {
        int index1 = line.IndexOf('[');
        int index2 = line.IndexOf(']') + 1;
        

        string lev = line.Substring(index1, index2 - index1);

        return lev;
    }

    public static string GetLogType(string line)
    {

        int ind = line.IndexOf(']');
        int index0 = line.IndexOf('[');
        int index1 = line.IndexOf('[', index0 + 1);
        int index2 = line.IndexOf(']', ind + 1) + 1;
        string Tupe = line.Substring(index1, index2 - index1);

        return Tupe;
    }

    public static string GetLogText(string line)
    {
        int ind = line.IndexOf(']');
        int index0 = line.IndexOf('[');
        int index1 = line.IndexOf('[', index0 + 1);
        int index2 = line.IndexOf(']', ind + 1);
        int index3 = line.IndexOf(' ', index2 + 1);

        string text = line.Substring(index3, line.Length - index3);
        return text;
    }



    public static void Main()
    {
        string[] Filep = File.ReadAllLines("event_server.log");
        foreach (string file in Filep)
        {
            DateTime dt = GetLogDateTime(file);
            string level = GetLogLevel(file);
            string Tupe = GetLogType(file);
            string text = GetLogText(file);
            Console.WriteLine(dt);
            Console.WriteLine(level);
            Console.WriteLine(Tupe);
            Console.WriteLine(text);
            break;
        }
    }
}