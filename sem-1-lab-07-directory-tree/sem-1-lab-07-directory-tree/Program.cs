using System;
using System.IO;

namespace lab7;

internal class Program
{
    // Current line in file
    private static int _lineIndex;

    // Current nesting level
    private static int _depth;

    // File content
    private static string[] _lines = null!;

    private static void Main()
    {
        _lines = File.ReadAllLines("tree.txt");

        if (_lines.Length > 0)
        {
            Console.WriteLine(_lines[0]);
        }

        _lineIndex = 1;

        PrintTree();

        Console.ReadLine();
    }

    private static void PrintTree()
    {
        while (_lineIndex < _lines.Length)
        {
            var line = _lines[_lineIndex].Trim();

            if (string.IsNullOrEmpty(line))
            {
                _lineIndex++;
                continue;
            }

            if (line == "END")
            {
                _depth--;
                _lineIndex++;
                return;
            }

            if (line.StartsWith("BEGIN "))
            {
                var name = line.Substring(6).Trim();
                var isLast = CheckIfLast(_lineIndex);
                var branchSymbol = isLast ? "└─" : "├─";

                Console.Write(GetIndent());
                Console.WriteLine(branchSymbol + name);

                _depth++;
                _lineIndex++;

                PrintTree();
            }
        }
    }
    private static bool CheckIfLast(int currentIndex)
    {
        int depth = 0;
        int i = currentIndex + 1;

        while (i < _lines.Length)
        {
            var line = _lines[i].Trim();

            if (line.StartsWith("BEGIN "))
            {
                if (depth == 0) 
                    return false;
                depth++;
            }
            else if (line == "END")
            {
                if (depth == 0) 
                    return true;
                depth--;
            }
            i++;
        }

        return true;
    }

    private static string GetIndent()
    {
        var result = "";

        for (var i = 1; i <= _depth; i++)
        {
            result += "│ ";
        }

        return result;
    }
}