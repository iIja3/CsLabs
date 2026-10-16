using Newtonsoft.Json;
using System;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.Arm;
namespace lab5;

public class SaveEntry
{
    public string Name { get; set; }
    public string FileName { get; set; }
    public string UpdatedAt { get; set; }
}
public class Position
{
    public int X { get; set; }
    public int Y { get; set; }
}
public class SaveData
{
    public string character { get; set; }
    public string map { get; set; }
    public Position position { get; set; }
    public int health { get; set; }
    public List<string> inventory { get; set; }
}
public class Program
{
    public static List<SaveEntry> LoadIndex(string indexPath)
    {
        if (!File.Exists(indexPath))
        {
            Console.WriteLine($"Файл индекса '{indexPath}' не найден на диске.");
            Console.WriteLine("Будет инициализирован новый пустой список сохранений.");
            return new List<SaveEntry>();
        }
            Console.WriteLine($"Чтение файла индекса: {indexPath}...");
            string jsonContent = File.ReadAllText(indexPath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                Console.WriteLine("Файл индекса пуст. Возвращаем пустой список.");
                return new List<SaveEntry>();
            }
            List<SaveEntry> entr = JsonConvert.DeserializeObject<List<SaveEntry>>(jsonContent);
            if (entr == null)
            {
                Console.WriteLine("Содержимое файла индекса равно null. Создаем новый пустой список.");
                return new List<SaveEntry>();
            }
            Console.WriteLine($"Индекс успешно загружен. Записей найдено: {entr.Count}");
            return entr;
    }

    public static SaveData LoadSave(string savesDirectory, SaveEntry entry)
    {
        if (entry == null || string.IsNullOrEmpty(entry.FileName))
        {
            Console.WriteLine("У выбранной записи отсутствует имя файла. Проверьте index.json.");
            return null;
        }
        string pach = Path.Combine(savesDirectory, entry.FileName);
        if (!File.Exists(pach))
        {
            Console.WriteLine("Файл сохранения не найден");
            return null;
        }
        var json = File.ReadAllText(pach);
        return JsonConvert.DeserializeObject<SaveData>(json);
    }

    public static void PrintSave(SaveEntry entry, SaveData save)
    {
        if (save == null) return;
            Console.WriteLine($"Информация о сохрании: {entry.Name}");
            Console.WriteLine($"Путь сохранения:{entry.FileName}");
            Console.WriteLine($"Последнее изменение: {entry.UpdatedAt}");
            Console.WriteLine($"Персанаж: {save.character}");
            Console.WriteLine($"Карта: {save.map}");
            Console.WriteLine($"Координаты: x = {save.position.X} y = {save.position.Y}");
            Console.WriteLine($"Здоровье: {save.health}");
            Console.WriteLine($"Инветарь: {(save.inventory != null && save.inventory.Count > 0 ? string.Join(", ", save.inventory) : "пусто")}");
    }

    public static SaveEntry FindByName(List<SaveEntry> entries, string name)
    {
        return entries.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
    public static void SaveGame(string savesDirectory, SaveEntry entry, SaveData save)
    {
        string Pach = Path.Combine(savesDirectory, entry.FileName);
        string json = JsonConvert.SerializeObject(save, Formatting.Indented);
        File.WriteAllText(Pach, json);
    }

    public static void SaveIndex(string indexPath, List<SaveEntry> entries)
    {
        string json = JsonConvert.SerializeObject(entries, Formatting.Indented);
        File.WriteAllText(indexPath, json);
    }

    public static void CreateBackup(string savesDirectory, string backupsDirectory, SaveEntry entry)
    {
        if (entry == null || string.IsNullOrEmpty(entry.FileName))
        {
            Console.WriteLine("У выбранной записи отсутствует имя файла. Проверьте index.json.");
            return;
        }
        string Pach = Path.Combine(savesDirectory, entry.FileName);
        if (!File.Exists(Pach))
        {
            Console.WriteLine("файл сохранения не найден, бэкпак не произведен");
        }
        Directory.CreateDirectory(backupsDirectory);
        string backPach = backupsDirectory + "/" + entry.FileName;

        File.Copy(Pach, backPach, true);
        Console.WriteLine($"Резервная копия успешно создана в папке {backupsDirectory}.");
    }

    public static void RestoreBackup(string savesDirectory, string backupsDirectory, SaveEntry entry)
    {
        if (entry == null || string.IsNullOrEmpty(entry.FileName))
        {
            Console.WriteLine("У выбранной записи отсутствует имя файла. Проверьте index.json.");
            return;
        }
        string backPach = Path.Combine(backupsDirectory, entry.FileName);
        if (!File.Exists(backPach))
        {
            Console.WriteLine("Резервная копия для сохранения не найдена в папке");
        }
        Directory.CreateDirectory(savesDirectory);
        string sPach = Path.Combine(savesDirectory, entry.FileName);

        File.Copy(backPach, sPach, true);
        entry.UpdatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        Console.WriteLine("Сохранение успешно восстановлено из резервной копии.");
    }
    public static void DeleteSave(string savesDirectory, string indexPath, List<SaveEntry> entries, SaveEntry entry)
    {
        if (entry == null || string.IsNullOrEmpty(entry.FileName))
        {
            Console.WriteLine("У выбранной записи отсутствует имя файла. Проверьте index.json.");
            return;
        }
        string Pach = Path.Combine(savesDirectory, entry.FileName);
        if (File.Exists(Pach))
        {
            File.Delete(Pach);
            Console.WriteLine("Файл сохранения удален с диска.");
        }
        else
        {
            Console.WriteLine("Файл сохранения на диске отсутствовал.");
        }
        entries.Remove(entry);
        SaveIndex(indexPath, entries);
        Console.WriteLine("Запись успешно удалена из index.json.");
    }
    public static void ShowAllSaves(List<SaveEntry> entries)
    {
        if (entries == null || entries.Count == 0)
        {
            Console.WriteLine("Список сохранений пуст.");
            return;
        }

        Console.WriteLine("Список доступных сохранений:");
        foreach (var e in entries)
        {
            Console.WriteLine($"Название: {e.Name} | Файл: {e.FileName} | Изменено: {e.UpdatedAt}");
        }
    }

    public static SaveEntry PromptForSave(List<SaveEntry> entries)
    {
        if (entries == null || entries.Count == 0)
        {
            Console.WriteLine("Список сохранений пуст.");
            return null;
        }

        ShowAllSaves(entries);
        Console.Write("Введите имя сохранения: ");
        string name = Console.ReadLine();

        var entry = FindByName(entries, name);
        if (entry == null)
        {
            Console.WriteLine($"Сохранение с именем '{name}' не найдено.");
        }
        return entry;
    }
    public static void EditSaveData(SaveData save)
    {
        Console.WriteLine("Редактирование данных (оставьте поле пустым и нажмите Enter, чтобы не менять значение):");

        Console.Write($"Текущая карта [{save.map}]. Новая карта: ");
        string map = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(map))
        {
            save.map = map;
        }
        Console.Write($"Текущий X [{save.position?.X}]. Новый X: ");
        string xInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(xInput) && int.TryParse(xInput, out int x))
        {
            if (save.position == null) save.position = new Position();
            save.position.X = x;
        }
        Console.Write($"Текущий Y [{save.position?.Y}]. Новый Y: ");
        string yInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(yInput) && int.TryParse(yInput, out int y))
        {
            if (save.position == null) save.position = new Position();
            save.position.Y = y;
        }
        Console.Write($"Текущее здоровье [{save.health}]. Новое здоровье: ");
        string healthInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(healthInput) && int.TryParse(healthInput, out int health))
        {
            save.health = health;
        }
        string currentInventory = save.inventory != null ? string.Join(", ", save.inventory) : "";
        Console.Write($"Текущий инвентарь [{currentInventory}]. Новый инвентарь (через запятую): ");
        string invInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(invInput))
        {
            var items = invInput.Split(',');
            save.inventory = new List<string>();
            foreach (var item in items)
            {
                save.inventory.Add(item.Trim());
            }
        }
    }
    public static void Main()
    {
        string indexPath = "index.json";
        string savesPach = "saves";
        string backPach = "backups";
        List<SaveEntry> entries = LoadIndex(indexPath);
        while (true)
        {
            Console.WriteLine("МЕНЕДЖЕР СОХРАНЕНИЙ ИГРЫ");
            Console.WriteLine("1. Показать список сохранений");
            Console.WriteLine("2. Посмотреть данные сохранения");
            Console.WriteLine("3. Изменить данные персонажа");
            Console.WriteLine("4. Создать резервную копию");
            Console.WriteLine("5. Восстановить резервную копию");
            Console.WriteLine("6. Удалить сохранение");
            Console.WriteLine("7. Выйти");
            Console.Write("Выберите пункт меню (1-7): ");
            string cho = Console.ReadLine();
            switch (cho)
            {
                case "1":
                    ShowAllSaves(entries);
                    break;
                case "2":
                    {
                        var entry = PromptForSave(entries);
                        if (entry != null)
                        {
                            var save = LoadSave(savesPach, entry);
                            PrintSave(entry, save);
                        }
                            break;
                    }
                case "3":
                    {
                        var entry = PromptForSave(entries);
                        if (entry != null)
                        {
                            var save = LoadSave(savesPach, entry);
                            if (save != null)
                            {
                                EditSaveData(save);
                                SaveGame(savesPach, entry, save);
                                SaveIndex(indexPath, entries);
                                Console.WriteLine("Данные сохранения успешно изменены и обновлены в индексе!");
                            }
                        }
                        break;
                    }

                case "4":
                    {
                        var entry = PromptForSave(entries);
                        if (entry != null)
                        {
                            CreateBackup(savesPach, backPach, entry);
                        }
                        break;
                    }

                case "5":
                    {
                        var entry = PromptForSave(entries);
                        if (entry != null)
                        {
                            RestoreBackup(savesPach, backPach, entry);
                            SaveIndex(indexPath, entries);
                        }
                        break;
                    }

                case "6":
                    {
                        var entry = PromptForSave(entries);
                        if (entry != null)
                        {
                            DeleteSave(savesPach, indexPath, entries, entry);
                        }
                        break;
                    }

                case "7":
                    Console.WriteLine("Выход из программы. До свидания!");
                    return;

                default:
                    Console.WriteLine("Неверный выбор. Пожалуйста, введите число от 1 до 7.");
                    break;
            }
        }
    }
}