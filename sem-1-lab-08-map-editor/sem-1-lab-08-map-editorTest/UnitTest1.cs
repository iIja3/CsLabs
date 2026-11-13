using System.IO;
using NUnit.Framework;
using lab8;

[TestFixture]
public class MapTests
{
    private static string MapPath =>
        Path.Combine(TestContext.CurrentContext.TestDirectory, "map.txt");

    private static Map NewFlatMap(int w = 5, int h = 4) => new Map(w, h);
    private static Position? FindPassable(Map m, Position except)
    {
        for (int y = 0; y < m.Height; y++)
            for (int x = 0; x < m.Width; x++)
                if ((x != except.X || y != except.Y) &&
                    m.Cells[y, x].Type != CellType.Wall)
                    return new Position(x, y);
        return null;
    }

    [Test]
    public void Create_NewMap_HasGivenSize_AndAllGround()
    {
        var m = new Map(7, 3);

        Assert.That(m.Width, Is.EqualTo(7));
        Assert.That(m.Height, Is.EqualTo(3));
        Assert.That(m.Cells.GetLength(0), Is.EqualTo(3)); 
        Assert.That(m.Cells.GetLength(1), Is.EqualTo(7));

        for (int y = 0; y < m.Height; y++)
            for (int x = 0; x < m.Width; x++)
                Assert.That(m.Cells[y, x].Type, Is.EqualTo(CellType.Ground),
                    $"клетка ({x},{y}) должна быть землёй, а не default(Type=0)");
    }


    [Test]
    public void SetCell_ChangesType_AndIgnoresOutOfBounds()
    {
        var m = NewFlatMap();

        m.SetCell(1, 2, CellType.Wall);
        Assert.That(m.Cells[2, 1].Type, Is.EqualTo(CellType.Wall));

        m.SetCell(1, 2, CellType.Water);
        Assert.That(m.Cells[2, 1].Type, Is.EqualTo(CellType.Water));
        Assert.DoesNotThrow(() => m.SetCell(100, 100, CellType.Wall));
        Assert.DoesNotThrow(() => m.SetCell(-1, 0, CellType.Wall));
    }


    [Test]
    public void PlacePlayer_SetsPositionAndFlag()
    {
        var m = NewFlatMap();
        m.PlacePlayer(new Position(2, 1));

        Assert.That(m.Player.Pos, Is.EqualTo(new Position(2, 1)));
        Assert.That(m.Cells[1, 2].HasPlayer, Is.True);
    }

    [Test]
    public void PlacePlayer_OnWall_DoesNothing()
    {
        var m = NewFlatMap();
        m.PlacePlayer(new Position(1, 1));
        m.SetCell(3, 1, CellType.Wall);

        var before = m.Player.Pos;
        m.PlacePlayer(new Position(3, 1)); // на стену

        Assert.That(m.Player.Pos, Is.EqualTo(before));
    }

    [Test]
    public void PlacePlayer_KeepsOnlyOneHasPlayerFlag()
    {
        var m = NewFlatMap();
        m.PlacePlayer(new Position(1, 1));
        m.PlacePlayer(new Position(2, 1));

        Assert.That(m.Cells[1, 1].HasPlayer, Is.False, "старая клетка без флага");
        Assert.That(m.Cells[1, 2].HasPlayer, Is.True, "новая клетка с флагом");

        int count = 0;
        for (int y = 0; y < m.Height; y++)
            for (int x = 0; x < m.Width; x++)
                if (m.Cells[y, x].HasPlayer) count++;

        Assert.That(count, Is.EqualTo(1), "ровно одна клетка с игроком");
    }

    [Test]
    public void MovePlayer_ChangesPosition()
    {
        var m = NewFlatMap();
        m.PlacePlayer(new Position(1, 1));

        Assert.That(m.MovePlayer(1, 0), Is.True, "шаг вправо возможен");
        Assert.That(m.Player.Pos, Is.EqualTo(new Position(2, 1)));
    }

    [Test]
    public void MovePlayer_IntoWall_ReturnsFalse_AndDoesNotMove()
    {
        var m = NewFlatMap();
        m.PlacePlayer(new Position(1, 1));
        m.SetCell(2, 1, CellType.Wall);
        var before = m.Player.Pos;

        Assert.That(m.MovePlayer(1, 0), Is.False);
        Assert.That(m.Player.Pos, Is.EqualTo(before));
    }

    [Test]
    public void MovePlayer_OutOfBounds_ReturnsFalse()
    {
        var m = NewFlatMap();
        m.PlacePlayer(new Position(0, 0));

        Assert.That(m.MovePlayer(-1, 0), Is.False);
        Assert.That(m.Player.Pos, Is.EqualTo(new Position(0, 0)));
    }

    // ---------- 6. Сохранение ----------

    [Test]
    public void Save_WritesExpectedFormat()
    {
        var m = NewFlatMap(4, 3);
        m.SetCell(0, 0, CellType.Wall);
        m.SetCell(2, 1, CellType.Water);
        m.PlacePlayer(new Position(1, 1));

        var tmp = Path.GetTempFileName();
        try
        {
            m.Save(tmp);
            var lines = File.ReadAllLines(tmp);

            Assert.That(lines[0], Is.EqualTo("4|3"), "заголовок");
            Assert.That(lines.Length, Is.EqualTo(4), "заголовок + 3 строки");
            Assert.That(lines[1][0], Is.EqualTo('#'), "стена в (0,0)");
            Assert.That(lines[2][1], Is.EqualTo('P'), "игрок в (1,1)");
            Assert.That(lines[2][2], Is.EqualTo('W'), "вода в (2,1)");
        }
        finally { File.Delete(tmp); }
    }

    [Test]
    public void Clone_IsDeepCopy_OriginalChangesDoNotLeak()
    {
        var m = NewFlatMap();
        m.SetCell(1, 1, CellType.Water);
        var backup = m.Clone();

        m.SetCell(1, 1, CellType.Wall);

        Assert.That(backup.Cells[1, 1].Type, Is.EqualTo(CellType.Water),
            "правка оригинала не должна протекать в бэкап");
    }

    [Test]
    public void RestoreFrom_ReturnsCellsAndPlayer()
    {
        var m = NewFlatMap();
        m.SetCell(1, 1, CellType.Water);
        m.PlacePlayer(new Position(2, 2));

        var snapshot = m.Clone();
        m.SetCell(1, 1, CellType.Wall);
        m.SetCell(3, 3, CellType.Wall);
        m.PlacePlayer(new Position(0, 0));

        m.RestoreFrom(snapshot);

        Assert.That(m.Cells[1, 1].Type, Is.EqualTo(CellType.Water), "вода в (1,1)");
        Assert.That(m.Cells[3, 3].Type, Is.EqualTo(CellType.Ground), "земля в (3,3)");
        Assert.That(m.Player.Pos, Is.EqualTo(new Position(2, 2)), "позиция игрока");
    }
}