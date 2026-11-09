using System;
using System.Security.Cryptography;
namespace practic3;

class Program
{
    public static void Main()
    {
        string[] map = File.ReadAllLines("map.txt");
        int xplayer = 0;
        int yplayer = 0;
        int xmonster = 0;
        int ymonster = 0;

        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == 'A')
                {
                    xplayer = x;
                    yplayer = y;
                }
                if (map[y][x] == 'm')
                {
                    xmonster = x;
                    ymonster = y;
                }
            }
        }
        Random rnd = new Random();
        bool gameOver = false;
        while (!gameOver)
        {
            Console.Clear();
            DrawMap(map, xplayer, yplayer, xmonster, ymonster);
            Console.WriteLine("Стрелки - ход. Q — выход.");
            ConsoleKey key = Console.ReadKey(true).Key;
            int dx = 0, dy = 0;

            if (key == ConsoleKey.UpArrow) dy = -1;
            else if (key == ConsoleKey.DownArrow) dy = 1;
            else if (key == ConsoleKey.LeftArrow) dx = -1;
            else if (key == ConsoleKey.RightArrow) dx = 1;
            else if (key == ConsoleKey.Q) break;

            int newX = xplayer + dx;
            int newY = yplayer + dy;
            if (IsWalkable(map, newX, newY))
            {
                xplayer = newX;
                yplayer = newY;
            }
            if (xplayer == xmonster && yplayer == ymonster)
            {
                Console.Clear();
                DrawMap(map, xplayer, yplayer, xmonster, ymonster);
                Console.WriteLine("Монстр поймал тебя! Игра окончена.");
                gameOver = true;
            }
            int dir = rnd.Next(4);
            int mdx = 0, mdy = 0;
            if (dir == 0) mdy = -1;
            else if (dir == 1) mdy = 1;
            else if (dir == 2) mdx = -1;
            else mdx = 1;

            int mNewX = xmonster + mdx;
            int mNewY = ymonster + mdy;

            if (IsWalkable(map, mNewX, mNewY))
            {
                xmonster = mNewX;
                ymonster = mNewY;
            }


        }
    }
    static void DrawMap(string[] map, int px, int py, int mx, int my)
    {
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (x == px && y == py) 
                    Console.Write('A');
                else if (x == mx && y == my) 
                    Console.Write('m');
                else if (map[y][x] == 'A' || map[y][x] == 'm')
                    Console.Write(' ');
                else 
                    Console.Write(map[y][x]);
            }
            Console.WriteLine();
        }
    }
    static bool IsWalkable(string[] map, int x, int y)
    {
        if (y < 0 || y >= map.Length) return false;
        if (x < 0 || x >= map[y].Length) return false;
        return map[y][x] != '#';
    }
}

    
