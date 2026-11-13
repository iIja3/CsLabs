using System;
using System.Text;

namespace lab8;

public enum CellType
{
    Wall = '#',
    Ground = '.',
    Water = 'W'
}
public struct Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y) 
    { 
        X = x; 
        Y = y; 
    }

    public override string ToString() => $"({X}, {Y})";
}

public struct Cell
{
    public CellType Type { get; set; }
    public bool HasPlayer { get; set; }
    public Cell(CellType type)
    {
        Type = type;
        HasPlayer = false;
    }
    public override string ToString() =>
        HasPlayer ? "P" : ((char)Type).ToString();
}
public struct Player
{
    public Position Pos { get; set; }
    public Player(Position pos)
    {
        Pos = pos;
    }
}
public class Map
{
    public int Height { get; private set; }
    public int Width { get; private set; }
    public Player Player { get; set; }
    public Cell[,] Cells { get; private set; }
    public Map(int width, int height)
    {
        Width = width;
        Height = height;
        Cells = new Cell[height, width];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                Cells[y, x] = new Cell(CellType.Ground);
    }
    public static Map Load(string pach)
    {
        string[] lines = File.ReadAllLines("map.txt");
        string[] header = lines[0].Split('|');
        int w = int.Parse(header[0]);
        int h = int.Parse(header[1]);

        var map = new Map(w, h);
        Position? playerPos = null;
        for (int y = 0; y < h;  y++)
        {
            string row = lines[y + 1];
            for (int x = 0; x < w; x++)
            {
                char c = row[x];
                switch (c)
                {
                    case '#':
                        map.Cells[y, x] = new Cell(CellType.Wall);
                        break;
                    case 'W':
                        map.Cells[y, x] = new Cell(CellType.Water); 
                        break;
                    case 'P':
                        map.Cells[y, x] = new Cell(CellType.Ground);
                        playerPos = new Position(x, y);
                        break;
                    default:
                        map.Cells[y, x] = new Cell(CellType.Ground);
                        break;
                }
            }
        }
        if (playerPos.HasValue)
            map.PlacePlayer(playerPos.Value);
        return map;
    }
    public bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }
    public void PlacePlayer(Position pos)
    {
        if (!InBounds(pos.X, pos.Y)) 
            return;
        if (Cells[pos.Y, pos.X].Type == CellType.Wall) 
            return;

        if (Player.Pos.X != pos.X || Player.Pos.Y != pos.Y)
        {
            var old = Player.Pos;
            if (InBounds(old.X, old.Y))
                Cells[old.Y, old.X].HasPlayer = false;
        }

        Player = new Player(pos);
        Cells[pos.Y, pos.X].HasPlayer = true;
    }
    public void SetCell(int x, int y, CellType type)
    {
        if (InBounds(x, y))
            Cells[y, x].Type = type;
    }
    public bool MovePlayer(int dx, int dy)
    {
        var target = new Position(Player.Pos.X + dx, Player.Pos.Y + dy);
        if (!InBounds(target.X, target.Y))
            return false;
        var cell = Cells[target.Y, target.X];
        if (cell.Type == CellType.Wall)
            return false;
        PlacePlayer(target);
        return true;
    }
    public void Print()
    {
        for (int y = 0; y < Height; y++)
        {
            var sd = new StringBuilder();
            for (int x = 0;  x < Width; x++)
                sd.Append(Cells[y, x].ToString());
            Console.WriteLine(sd.ToString());
        }
    }
    public Map Clone()
    {
        var copy = new Map(Width, Height);
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                copy.Cells[y, x] = Cells[y, x];
        copy.Player = Player;
        return copy;
    }
    public void RestoreFrom(Map backup)
    {
        Width = backup.Width; 
        Height = backup.Height;
        Cells = new Cell[Height, Width];
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Cells[y, x] = backup.Cells[y, x];
        Player = backup.Player;
    }
    public void Save(string path)
    {
        var sd = new StringBuilder();
        sd.AppendLine($"{Width}|{Height}");
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
                sd.Append(Cells[y, x].ToString());
            sd.AppendLine();
        }
        File.WriteAllText(path, sd.ToString());
    }
}
public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Map? map = null;
        Map? backup = null;
        const string defaultFile = "map.txt";

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("========= РЕДАКТОР КАРТ =========");
            Console.WriteLine($" Карта: {(map is null ? "не создана" : $"{map.Width}x{map.Height}")}");
            Console.WriteLine($" Бэкап: {(backup is null ? "нет" : "есть")}");
            Console.WriteLine($" Игрок: {(map is null ? "-" : map.Player.Pos.ToString())}");
            Console.WriteLine("---------------------------------");
            Console.WriteLine(" 1. Создать новую карту (задать размер)");
            Console.WriteLine(" 2. Загрузить карту из файла");
            Console.WriteLine(" 3. Изменить тип клетки");
            Console.WriteLine(" 4. Показать карту");
            Console.WriteLine(" 5. Разместить игрока");
            Console.WriteLine(" 6. Переместить игрока");
            Console.WriteLine(" 7. Сохранить бэкап (snapshot)");
            Console.WriteLine(" 8. Восстановить из бэкапа");
            Console.WriteLine(" 9. Сохранить карту в файл");
            Console.WriteLine("10. Запустить тесты");
            Console.WriteLine(" 0. Выход");
            Console.Write("Выбор: ");

            string? choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1": map = CreateNewMap(); break;
                    case "2": map = LoadMap(defaultFile); break;
                    case "3": if (RequireMap(map)) EditCell(map!); break;
                    case "4": if (RequireMap(map)) map!.Print(); break;
                    case "5": if (RequireMap(map)) PlacePlayer(map!); break;
                    case "6": if (RequireMap(map)) MovePlayer(map!); break;
                    case "7": if (RequireMap(map)) { backup = map!.Clone(); Console.WriteLine("Бэкап сохранён."); } break;
                    case "8": if (RequireMap(map)) Restore(map!, backup); break;
                    case "9": if (RequireMap(map)) SaveMap(map!, defaultFile); break;
                    case "10": RunTests(); break;
                    case "0": Console.WriteLine("Выход."); return;
                    default: Console.WriteLine("Неизвестный пункт меню."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] {ex.Message}");
            }
        }
    }

    private static bool RequireMap(Map? map)
    {
        if (map is null)
        {
            Console.WriteLine("Сначала создайте или загрузите карту (пункт 1 или 2).");
            return false;
        }
        return true;
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            if (int.TryParse(s, out int v)) return v;
            Console.WriteLine("Нужно целое число. Повторите.");
        }
    }

    private static Position ReadPosition(string prompt)
    {
        Console.WriteLine(prompt);
        int x = ReadInt("  X: ");
        int y = ReadInt("  Y: ");
        return new Position(x, y);
    }

    private static CellType ReadCellType()
    {
        while (true)
        {
            Console.Write("Тип клетки (# - стена, . - земля, W - вода): ");
            string? s = Console.ReadLine()?.Trim();
            switch (s)
            {
                case "#": return CellType.Wall;
                case ".": return CellType.Ground;
                case "W":
                case "w": return CellType.Water;
                default: Console.WriteLine("Введите #, . или W."); break;
            }
        }
    }

    private static Map CreateNewMap()
    {
        int w = ReadInt("Ширина: ");
        int h = ReadInt("Высота: ");
        if (w <= 0 || h <= 0)
        {
            Console.WriteLine("Размеры должны быть положительными. Создана карта 5x4.");
            w = 5; h = 4;
        }
        var map = new Map(w, h);
        Console.WriteLine($"Создана карта {w}x{h} (все клетки — земля).");
        return map;
    }

    private static Map LoadMap(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Файл '{path}' не найден.");
            return null!;
        }
        var map = Map.Load(path);
        Console.WriteLine($"Загружено из '{path}': {map.Width}x{map.Height}.");
        return map;
    }

    private static void EditCell(Map map)
    {
        var pos = ReadPosition("Координаты клетки:");
        if (!map.InBounds(pos.X, pos.Y))
        {
            Console.WriteLine("Координаты вне карты.");
            return;
        }
        var type = ReadCellType();
        map.SetCell(pos.X, pos.Y, type);
        Console.WriteLine($"Клетка ({pos.X},{pos.Y}) → {(char)type}.");
    }

    private static void PlacePlayer(Map map)
    {
        var pos = ReadPosition("Куда поставить игрока:");
        if (!map.InBounds(pos.X, pos.Y))
        {
            Console.WriteLine("Координаты вне карты.");
            return;
        }
        if (map.Cells[pos.Y, pos.X].Type == CellType.Wall)
        {
            Console.WriteLine("На стену ставить нельзя.");
            return;
        }
        map.PlacePlayer(pos);
        Console.WriteLine($"Игрок в {map.Player.Pos}.");
    }

    private static void MovePlayer(Map map)
    {
        Console.WriteLine("Смещение (dx, dy). Например, 1 0 — вправо, 0 -1 — вверх.");
        int dx = ReadInt("  dx: ");
        int dy = ReadInt("  dy: ");
        bool ok = map.MovePlayer(dx, dy);
        Console.WriteLine(ok
            ? $"Игрок теперь в {map.Player.Pos}."
            : "Перемещение невозможно (стена или граница).");
    }

    private static void Restore(Map map, Map? backup)
    {
        if (backup is null)
        {
            Console.WriteLine("Бэкапа нет — сначала сохраните (пункт 7).");
            return;
        }
        map.RestoreFrom(backup);
        Console.WriteLine("Карта восстановлена из бэкапа.");
    }

    private static void SaveMap(Map map, string path)
    {
        map.Save(path);
        Console.WriteLine($"Сохранено в '{path}'.");
    }

    private static void RunTests()
    {
        var map = Map.Load("map.txt");
        Console.WriteLine("=== Исходная карта ===");
        map.Print();

        // --- Эксперимент 1: backup / restore ---
        Console.WriteLine("\n=== Backup ===");
        var backup = map.Clone();

        Console.WriteLine("\n=== Меняем карту (ставим стену) ===");
        map.SetCell(1, 1, CellType.Wall);
        map.Print();

        Console.WriteLine("\n=== Восстанавливаем из backup ===");
        map.RestoreFrom(backup);
        map.Print();

        // --- Эксперимент 2: перемещение игрока ---
        Console.WriteLine("\n=== Перемещение игрока ===");
        Console.WriteLine($"До: {map.Player.Pos}");
        bool ok = map.MovePlayer(1, 0);
        Console.WriteLine($"MovePlayer(1,0) -> {ok}, теперь: {map.Player.Pos}");
        map.Print();

        ok = map.MovePlayer(0, 1);
        Console.WriteLine($"MovePlayer(0,1) -> {ok}, теперь: {map.Player.Pos}");
        map.Print();

        // --- Эксперимент 3: упор в стену ---
        Console.WriteLine("\n=== Пробуем пройти в стену ===");
        var wallPos = new Position(map.Player.Pos.X, map.Player.Pos.Y + 1);
        if (map.InBounds(wallPos.X, wallPos.Y) &&
            map.Cells[wallPos.Y, wallPos.X].Type == CellType.Wall)
        {
            ok = map.MovePlayer(0, 1);
            Console.WriteLine($"MovePlayer(0,1) в стену -> {ok}, позиция: {map.Player.Pos}");
        }

        map.Save("map_out.txt");
        Console.WriteLine("\nКарта сохранена в map_out.txt");
    }
}