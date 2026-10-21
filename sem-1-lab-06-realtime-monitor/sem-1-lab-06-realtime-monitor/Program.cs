using System;
using System.Timers;
namespace lab6;

public class ServerMonitor
{
    private readonly Random _random = new();
    private readonly System.Timers.Timer _timer;

    private double _cpu = 35;
    private double _ram = 42;
    private double _temperature = 48;

    public event Action<double, double, double>? DataReceived;

    public ServerMonitor()
    {
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (_, _) => GenerateData();
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    private void GenerateData()
    {
        _cpu += _random.NextDouble() * 10 - 5;
        _ram += _random.NextDouble() * 6 - 3;
        _temperature += _random.NextDouble() * 4 - 2;

        _cpu = Math.Clamp(_cpu, 0, 100);
        _ram = Math.Clamp(_ram, 10, 100);
        _temperature = Math.Clamp(_temperature, 30, 95);

        DataReceived?.Invoke(
            Math.Round(_cpu, 1),
            Math.Round(_ram, 1),
            Math.Round(_temperature, 1));
    }
}
public class Program
{
    private static double[] cpuHi = new double[20];
    private static double[] tHi = new double[20];
    private static double[] ramHi = new double[20];
    private static int index = 0;
    private static int count = 0;
    public static void Main()
    {
        var monitor = new ServerMonitor();
        monitor.DataReceived += OnDataReceived;
        monitor.Start();

        Console.WriteLine("Мониторинг запущен.");
        Console.WriteLine("Для завершения программы нажмите Enter.");

        Console.ReadLine();

        monitor.Stop();
    }
    public static void OnDataReceived(double cpu, double ram, double temperature)
    {
        cpuHi[index] = cpu;
        ramHi[index] = ram;
        tHi[index] = temperature;
        index = (index + 1) % 20;
        if (count < 20)
            count++;
        try
        {
            File.AppendAllText("measurements.txt", $"{cpu}|{ram}|{temperature}{Environment.NewLine}");
        }
        catch
        {

        }
        DrawDashboard(cpu, ram, temperature);
    }
    private static void DrawDashboard(double currentCpu, double currentRam, double currentTemp)
    {
        Console.SetCursorPosition(0, 2);

        Console.WriteLine($"Текущие значения:");
        Console.WriteLine($"CPU:         {currentCpu,5:F1}%   ");
        Console.WriteLine($"RAM:         {currentRam,5:F1}%   ");
        Console.WriteLine($"Temperature: {currentTemp,5:F1} °C ");
        Console.WriteLine();
    }
}