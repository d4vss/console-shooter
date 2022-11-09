using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ShooterGame
{
    struct xy
    {
        public int x, y;
    }
    struct arrow_Info
    {
        public int x, y, state;
    }

    internal class Program
    {
        static string[,] field;
        static xy player, fieldsize;
        static arrow_Info[] arrowInfo;
        static bool running, attack, pause;
        static int hp, ticks, fps, points;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Config();
            startGame();
        }

        static void Config()
        {
            Console.SetWindowSize(200, 50);
            arrowInfo = new arrow_Info[0];
            hp = 20;
            fieldsize.y = 30; fieldsize.x = 30;
            field = new string[30, 30];
            for (int i = 0; i < fieldsize.y; i++)
            {
                field[i, 0] = "██";
                field[i, fieldsize.x - 1] = "██";
            }
            for (int i = 0; i < fieldsize.x; i++)
            {
                field[0, i] = "██";
                field[fieldsize.y - 1, i] = "██";
            }
            player.y = fieldsize.y / 2 - 1; player.x = fieldsize.x / 2 - 1;
        }

        static void startGame()
        {
            running = true; attack = true; pause = false;

            Task printFieldTask = Task.Factory.StartNew(() => printField());
            Task MovementTask = Task.Factory.StartNew(() => Movement());
            Task spawnArrowsTask = Task.Factory.StartNew(() => arrowSpawner());
            Task moveArrowsTask = Task.Factory.StartNew(() => moveArrows());
            Task FpsCounterTask = Task.Factory.StartNew(() => fpsCounter());
            Task.WaitAll(printFieldTask, MovementTask, spawnArrowsTask, moveArrowsTask);
        }

        static void fpsCounter()
        {
            Thread.Sleep(1000);
            fps = ticks;
            ticks = 0;
        }

        static void PauseMenu()
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("\nFPS: " + fps + "\n");
            for (int i = 0; i < fieldsize.y; i++)
            {
                Console.Write("          ");
                for (int j = 0; j < fieldsize.x; j++)
                {
                    if (i == player.y && j == player.x)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write(" ♥");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (field[i, j] == null)
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.Write(field[i, j]);
                    }
                }
                Console.WriteLine("      ");
            }
            Console.Write("\n                       ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(hp.ToString().PadLeft(2) + " ♥   ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("points: " + points);
            Console.SetCursorPosition(fieldsize.x + 6, fieldsize.y / 2);
            Console.WriteLine("██  ██");
            Console.SetCursorPosition(fieldsize.x + 6, fieldsize.y / 2 + 1);
            Console.WriteLine("██  ██");
            Console.SetCursorPosition(fieldsize.x + 6, fieldsize.y / 2 + 2);
            Console.WriteLine("██  ██");
        }

        static void Movement()
        {
            while (running)
            {
                ConsoleKeyInfo input = Console.ReadKey(true);

                switch (input.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (attack)
                        {
                            deleteWall();
                            field[player.y - 3, player.x] = "--"; field[player.y - 3, player.x - 1] = "--"; field[player.y - 3, player.x + 1] = "--";
                        }
                        else
                        {
                            if (player.y - 1 > 0)
                            {
                                player.y--;
                            }
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (attack)
                        {
                            deleteWall();
                            field[player.y + 3, player.x] = "--"; field[player.y + 3, player.x - 1] = "--"; field[player.y + 3, player.x + 1] = "--";
                        }
                        else
                        {
                            if (player.y + 1 < fieldsize.y - 1)
                            {
                                player.y++;
                            }
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (attack)
                        {
                            deleteWall();
                            field[player.y, player.x - 3] = "¦ "; field[player.y + 1, player.x - 3] = "¦ "; field[player.y - 1, player.x - 3] = "¦ ";
                        }
                        else
                        {
                            if (player.x - 1 > 0)
                            {
                                player.x--;
                            }
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (attack)
                        {
                            deleteWall();
                            field[player.y, player.x + 3] = " ¦"; field[player.y + 1, player.x + 3] = " ¦"; field[player.y - 1, player.x + 3] = " ¦";
                        }
                        else
                        {
                            if (player.x + 1 < fieldsize.x - 1)
                            {
                                player.x++;
                            }
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        if (!pause)
                        {
                            pause = true;
                        }
                        else
                        {
                            pause = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        static void deleteWall()
        {
            for (int i = 0; i < fieldsize.y; i++)
            {
                for (int j = 0; j < fieldsize.x; j++)
                {
                    if (field[i, j] == " ¦" || field[i, j] == "--" || field[i, j] == "¦ ")
                    {
                        field[i, j] = null;
                    }
                }
            }
        }

        static void printField()
        {
            while (running)
            {
                if (pause)
                {
                    PauseMenu();
                    Thread.Sleep(100);
                }
                else
                {
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("\nFPS: " + fps + "\n");
                    for (int i = 0; i < fieldsize.y; i++)
                    {
                        Console.Write("          ");
                        for (int j = 0; j < fieldsize.x; j++)
                        {
                            if (i == player.y && j == player.x)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                Console.Write(" ♥");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else if (field[i, j] == null)
                            {
                                Console.Write("  ");
                            }
                            else
                            {
                                Console.Write(field[i, j]);
                            }
                        }
                        Console.WriteLine("      ");
                    }
                    Console.Write("\n                       ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(hp.ToString().PadLeft(2) + " ♥   ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Points: " + points.ToString() + "  ");
                    ticks++;
                }
            }
        }

        static void arrowSpawner()
        {
            while (running)
            {
                while (attack)
                {
                    /* 0 = Oben
                     * 1 = Unten
                     * 2 = Links
                     * 3 = Rechts
                    */
                    if (!pause)
                    {
                        int richtung = rnd.Next(4);
                        Array.Resize(ref arrowInfo, arrowInfo.Length + 1);
                        switch (richtung)
                        {
                            case 0:
                                field[1, player.x] = " ▼";
                                arrowInfo[arrowInfo.Length - 1].y = 1; arrowInfo[arrowInfo.Length - 1].x = player.x;
                                break;
                            case 1:
                                field[fieldsize.y - 2, player.x] = " ▲";
                                arrowInfo[arrowInfo.Length - 1].y = fieldsize.y - 2; arrowInfo[arrowInfo.Length - 1].x = player.x;
                                break;
                            case 2:
                                field[player.y, 1] = " ►";
                                arrowInfo[arrowInfo.Length - 1].y = player.y; arrowInfo[arrowInfo.Length - 1].x = 1;
                                break;
                            case 3:
                                field[player.y, fieldsize.x - 2] = " ◄";
                                arrowInfo[arrowInfo.Length - 1].y = player.y; arrowInfo[arrowInfo.Length - 1].x = fieldsize.x - 2;
                                break;
                            default:
                                break;
                        }
                        Thread.Sleep(600);
                    }
                }
            }
        }

        static void moveArrows(int i, int j, int x, int y, string character, string hitcharacter)
        {
            int state = -1;
            int index = 0;
            for (int f = 0; f < arrowInfo.Length; f++)
            {
                if (arrowInfo[f].x == j && arrowInfo[f].y == i)
                {
                    state = arrowInfo[f].state;
                    index = f;
                }
            }
            if (state == 0)
            {
                field[i, j] = null;
                arrowInfo[index].state = 1;
                if (y == player.y && x == player.x)
                {
                    hp -= 2;
                    arrowInfo[index].state = -1;
                }
                else if (field[y, x] == hitcharacter)
                {
                    arrowInfo[index].state = -1;
                    points += 25;
                }
                else
                {
                    switch (character)
                    {
                        case " ▼":
                            arrowInfo[index].y++;
                            break;
                        case " ►":
                            arrowInfo[index].x++;
                            break;
                        case " ▲":
                            arrowInfo[index].y--;
                            break;
                        case " ◄":
                            arrowInfo[index].x--;
                            break;
                        default:
                            break;
                    }
                    field[y, x] = character;
                }
            }
            else if (state == 1)
            {
                arrowInfo[index].state = 0;
            }
        }

        static void moveArrows()
        {
            while (running)
            {
                while (attack)
                {
                    if (!pause)
                    {
                        Thread.Sleep(200);
                        for (int i = 1; i < fieldsize.y - 1; i++)
                        {
                            for (int j = 1; j < fieldsize.x - 1; j++)
                            {
                                switch (field[i, j])
                                {
                                    case " ▼":
                                        moveArrows(i, j, j, i + 1, " ▼", "--");
                                        break;
                                    case " ►":
                                        moveArrows(i, j, j + 1, i, " ►", "¦ ");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        for (int i = fieldsize.y - 1; i >= 1; i--)
                        {
                            for (int j = fieldsize.x - 1; j >= 1; j--)
                            {
                                switch (field[i, j])
                                {
                                    case " ▲":
                                        moveArrows(i, j, j, i - 1, " ▲", "--");
                                        break;
                                    case " ◄":
                                        moveArrows(i, j, j - 1, i, " ◄", " ¦");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        if (hp < 1)
                        {
                            attack = false;
                            running = false;
                            Thread.Sleep(2000);
                            System.Environment.Exit(1);
                        }
                    }
                }
            }
        }
    }
}
