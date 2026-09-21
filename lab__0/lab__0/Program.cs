using System;

namespace lab_00;

class Program
{
    public static void Main()
    {
        Console.WriteLine("Welcome to creating a gaming profile!");
        Console.Write("Enter your real name: ");
        string rname = Console.ReadLine();
        Console.Write("Enter your user name: ");
        string uname = Console.ReadLine();

        Console.Write("How old are you? (number): ");
        string age = Console.ReadLine();
        int old = Convert.ToInt32(age);

        Console.Write("Tell us about yourself: ");
        string your = Console.ReadLine();

        Console.Write("You're  email: ");
        string email = Console.ReadLine();

        Console.Write("Enter password: ");
        string pass = Console.ReadLine();
        string change = "\x1B]8;;https://youtu.be/j-iheFkstFQ?si=2gDsZc7h7rzRk9JG\x1B\\Change password\x1B]8;;\x1B\\";

        Console.Clear();

        Console.WriteLine("YOU'RE A GAMING PROLILE");
        Console.WriteLine($"Name: {uname}");
        Console.WriteLine($"Real Name: {rname}");
        Console.WriteLine($"Age: {old}");
        Console.WriteLine($"About myself: {your}");
        Console.WriteLine($"Email: {email}");
        Console.WriteLine($"Password: {pass}");
        Console.WriteLine($"{change}");
    }
}