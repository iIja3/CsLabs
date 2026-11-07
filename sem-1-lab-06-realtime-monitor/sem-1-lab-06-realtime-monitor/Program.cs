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
    public void RenderGraph()
    {
        Program.RenderGraph();
    }
}
public class Monitor : ServerMonitor
{
    public static string[] RenderGraph(double[] data) => Program.RenderGraph(data);
}

public class Program
{
    private static double[] cpuHi = new double[20];
    private static double[] tHi = new double[20];
    private static double[] ramHi = new double[20];
    private static int index = 0;
    private static int count = 0;
    public const int GraphWidth = 20;
    public const int GraphHeight = 15;

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
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        DrawDashboard(cpu, ram, temperature);
        RenderGraph();
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
    public static void RenderGraph()
    {
        var values = new double[count];
        int start = (count < GraphWidth) ? 0 : index;
        for (int i = 0; i < count; i++)
            values[i] = cpuHi[(start + i) % GraphWidth];

        var columns = RenderGraph(values);

        Console.WriteLine("График CPU (последние 20 измерений):");
        for (int row = GraphHeight - 1; row >= 0; row--)
        {
            Console.Write("|");
            for (int col = 0; col < GraphWidth; col++)
            {
                char c = (row < columns[col].Length) ? columns[col][row] : ' ';
                Console.Write(c);
            }
            Console.WriteLine("|");
        }
        Console.WriteLine("+" + new string('-', GraphWidth) + "+");
        Console.WriteLine();
    }
    public static string[] RenderGraph(double[] data)
    {
        var columns = new string[GraphWidth];

        if (data == null || data.Length == 0)
        {
            for (int i = 0; i < GraphWidth; i++)
                columns[i] = new string(' ', GraphHeight);
            return columns;
        }
        int take = Math.Min(data.Length, GraphWidth);
        int offset = GraphWidth - take;
        var values = new double[GraphWidth];
        for (int i = 0; i < take; i++)
            values[offset + i] = data[data.Length - take + i];

        for (int col = 0; col < GraphWidth; col++)
        {
            double v = Math.Clamp(values[col], 0, 100);
            int filled = (int)Math.Round(v / 100.0 * GraphHeight);
            columns[col] = new string('#', filled) + new string(' ', GraphHeight - filled);
        }

        return columns;
    }
}