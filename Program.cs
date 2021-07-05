using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleOfLife
{
    public enum GameStatus { Running, Paused }
    class Program
    {
        public static string Version = "1.0.0";
        public static string Title => $"Console of Life v{Version}";

        public static int CW = 120;
        public static int CH = 30;

        public static ConsoleColor CellColor = ConsoleColor.Cyan;
        public static GameStatus Status = GameStatus.Paused;
        public static float Speed = 1.0f;
        public static List<List<bool>> Cells;
        

        static Program()
        {
            Cells = new(CH - 7);
            for (int i = 0; i < Cells.Capacity; i++)
            {
                List<bool> vals = new(CW - 4);
                for (int j = 0; j < vals.Capacity; j++)
                {
                    vals.Add(false);
                }
                Cells.Add(vals);
            }
        }

        static List<List<bool>> CreateCells()
        {
            List<List<bool>> newCells = new(CH - 7);
            for (int i = 0; i < Cells.Capacity; i++)
            {
                List<bool> vals = new(CW - 4);
                for (int j = 0; j < vals.Capacity; j++)
                {
                    vals.Add(false);
                }
                newCells.Add(vals);
            }
            return newCells;
        }
        static void CopyCells(List<List<bool>> source, List<List<bool>> target)
        {
            for (int y = 0; y < target.Capacity; y++)
            {
                for (int x = 0; x < target[y].Capacity; x++)
                {
                    target[y][x] = source[y][x];
                }
            }
        }

        static void ClearCells()
        {
            foreach (List<bool> Row in Cells)
            {
                for (int i = 0; i < Row.Capacity; i++)
                {
                    Row[i] = false;
                }
            }
        }

        static bool GetCell(int x, int y)
        {
            return Cells[y][x];
        }

        static void DrawCells()
        {
            for (int y = 0; y < Cells.Capacity; y++)
            {
                for (int x = 0; x < Cells[y].Capacity; x++)
                {
                    if (Cells[y][x])
                    {
                        Console.BackgroundColor = CellColor;
                        Console.Write($"\x1b[{y + 4};{x + 2}H ");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
            }
        }

        static void DrawBorders()
        {
            Console.Clear();
            // Draw top border                                                                              ┌─────────┐
            Console.Write($"\x1b(0l{new String('q', CW - 4)}k\n");
            // Draw title with shadow                                                                       │ Title   │▒
            Console.Write($"x \x1b(B{Title}\x1b(0{new String(' ', CW - 5 - 17 - Title.Length)}\x1b(BPress h for help\x1b(0 x\x61\n");
            // Close Title Box                                                                              ├─────────┤▒
            Console.Write($"\x74{new String('q', CW - 4)}\x75\x61\n");
            // Loop and draw game area, leaving space for bottom box and shadow
            for (int i = 0; i < CH - 7; i++) { Console.Write($"x{new String(' ', CW - 4)}x\x61\n"); }
            // Draw first part of bottom control boxes
            int controlw = (CW - 4) / 3;
            string controlYLines = new string('q', controlw);
            // Draw top control box
            Console.Write($"\x74{controlYLines}\x77{controlYLines}\x77{controlYLines}\x75\x61\n");
            // Draw controls
            Console.Write($"x \x1b(BStatus\x1b(0{new string(' ', controlw - 7)}x \x1b(BCell Color\x1b(0{new string(' ', controlw - 11)}x \x1b(BSpeed\x1b(0{new string(' ', controlw - 6)}x\x61\n");
            // Draw bottom of control box
            Console.Write($"\x6d{controlYLines}\x76{controlYLines}\x76{controlYLines}\x6a\x61\n");
            // Draw bottom shadow
            Console.Write(new string('\x61', CW - 1));
            // Leave DSC drawing mode
            Console.Write("\x1b(B");
        }
        static void ChangeCellColor(ConsoleColor newColor)
        {
            int controlw = (CW - 4) / 3 + 4;
            int controlh = (CH - 2);
            Console.ForegroundColor = newColor;
            Console.Write($"\x1b[{controlh};{controlw}HCell Color");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        static void SetStatus(GameStatus newStatus)
        {
            Status = newStatus;
            Console.Write($"\x1b[{CH - 2};10H       \x1b[{CH - 2};10H");
            switch (Status)
            {
                case GameStatus.Paused: Console.Write("Paused"); break;
                case GameStatus.Running: Console.Write("Running"); break;
            }
        }
        static void SetSpeed(float speed)
        {
            Speed = speed;
            int controlw = (CW - 4) / 3 * 2 + 5;
            int controlh = (CH - 2);
            Console.Write($"\x1b[{controlh};{controlw}HSpeed x{Speed}");
        }

        static void RunThread()
        {
            while (true)
            {
                Thread.Sleep((int)(500.0f * Speed));
                if (Status == GameStatus.Paused) { continue; }
                else
                {
                    List<List<bool>> newCells = CreateCells();
                    CopyCells(Cells, newCells);
                    for (int y = 0; y < newCells.Capacity; y++)
                    {
                        for (int x = 0; x < newCells[y].Capacity; x++)
                        {
                            int alive = 0;
                            for (int a = -1; a < 2; a++)
                            {
                                for (int b = -1; b < 2; b++)
                                {
                                    if (a == 0 && b == 0) { continue; }
                                    int dx = x;
                                    int dy = y;

                                    if (dx + a < 0) { dx = Cells[y].Capacity - 1; }
                                    else if (dx + a == Cells[y].Capacity) { dx = 0; }

                                    if (dy + b < 0) { dy = Cells.Capacity - 1; }
                                    else if (dy + b == Cells.Capacity) { dy = 0; }

                                    if (newCells[dy][dx]) { ++alive; }
                                }
                            }
                            if (alive < 2)
                            {
                                Cells[y][x] = false;
                            }
                            else if (alive == 3)
                            {
                                Cells[y][x] = true;
                            } else if (alive > 3)
                            {
                                Cells[y][x] = false;
                            }
                        }
                    }
                    DrawCells();
                }
            }
            
        }

        static void Main(string[] args)
        {
            DrawBorders();
            SetStatus(GameStatus.Paused);
            ChangeCellColor(ConsoleColor.Cyan);
            SetSpeed(Speed);
            Console.ReadKey(false);
            Cells[1][0] = true;
            Cells[2][1] = true;
            Cells[2][2] = true;
            Cells[1][2] = true;
            Cells[0][2] = true;
            DrawCells();
            //Thread derp = new();
            Status = GameStatus.Running;
            RunThread();
        }
    }
}
