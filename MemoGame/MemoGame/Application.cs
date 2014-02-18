using System;
using MemoGame;
using System.Collections.Generic;

class Application
{
    static int playFieldWidth = 100;
    static int playFieldHeight = 50;
    static int middleWidth = playFieldWidth / 2;
    static int startPositionMeniItems = 17;

    //for menu arrow
    static string arrow = "-->";
    static int arrowCoordinateX = middleWidth - 12;
    static int minArrowCoordinateY = startPositionMeniItems;
    static int arrowCoordinateY = minArrowCoordinateY;
    static int maxArrowCoordinateY = playFieldHeight - startPositionMeniItems;
    static int arrowChoice = 0; //for the first field(Instructions)

    static void Main()
    {
        ShowMenu();
    }

    public static void ShowMenu()
    {
        InitializeGameField();

        PrintArrow(arrowCoordinateY, arrow);
        DrawMenu();
        MoveArrow();
    }

    private static void InitializeGameField()
    {
        Console.Title = "Memory Game";
        Console.CursorVisible = false;
        //set the console 
        RemoveScrollBars();

        Console.SetWindowSize(playFieldWidth, playFieldHeight);

        //draw play field
        DrawPlayField();
    }

    public static void RemoveScrollBars()
    {
        Console.BufferHeight = Console.WindowHeight;
        Console.BufferWidth = Console.WindowWidth;
    }

    public static void DrawPlayField()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(1, 1);
        Console.WriteLine(new string('-', playFieldWidth - 2));

        Console.SetCursorPosition(1, playFieldHeight - 2);
        Console.WriteLine(new string('-', playFieldWidth - 2));


        for (int i = 2; i < playFieldHeight - 2; i++)
        {
            Console.SetCursorPosition(1, i);
            Console.WriteLine(new string('|', 1));
        }

        for (int i = 2; i < playFieldHeight - 2; i++)
        {
            Console.SetCursorPosition(playFieldWidth - 2, i);
            Console.WriteLine(new string('|', 1));
        }
    }


    private static void PrintArrow(int coordinateY, string pointer)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(arrowCoordinateX, coordinateY);
        for (int character = 0; character < pointer.Length; character++)
        {
            Console.Write(pointer[character]);
        }
    }

    static void DrawMenu()
    {
        int startPosition = 5;
        int intervalBetweenMenuItems = 6;
        int intervalBetweenShortcut = 2;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(middleWidth - 10, startPosition);
        Console.WriteLine("M E M O  G A M E");

        Console.SetCursorPosition(middleWidth - 7, 2 * startPosition);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("MAIN MENU");
        Console.SetCursorPosition(middleWidth - 8, 2 * startPosition + 1);
        Console.WriteLine(new string('-', 11));

        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(middleWidth - 8, startPositionMeniItems);
        Console.WriteLine("START NEW GAME");

        Console.SetCursorPosition(middleWidth - 8, startPositionMeniItems + intervalBetweenMenuItems);
        Console.WriteLine("INSTRUCTIONS");

        Console.SetCursorPosition(middleWidth - 8, startPositionMeniItems + 2 * intervalBetweenMenuItems);
        Console.WriteLine("HIGH SCORES");

        Console.SetCursorPosition(middleWidth - 8, startPositionMeniItems + 3 * intervalBetweenMenuItems);
        Console.WriteLine("EXIT");

        Console.SetCursorPosition(middleWidth - 6, playFieldHeight - (startPosition + intervalBetweenShortcut));
        Console.WriteLine("Game by");
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(middleWidth - 12, playFieldHeight - startPosition);
        Console.WriteLine("FRED FLINSTONE TEAM");
    }

    private static void MoveArrow()
    {
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.UpArrow && arrowCoordinateY > minArrowCoordinateY)
            {
                Console.Beep();
                arrowChoice--;
                MoveArrowUp();
                Console.SetCursorPosition(0, 0);
            }
            else if (key.Key == ConsoleKey.DownArrow && arrowCoordinateY < maxArrowCoordinateY)
            {
                Console.Beep();
                MoveArrowDown();
                arrowChoice++;
                Console.SetCursorPosition(0, 0);
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                switch (arrowChoice)
                {
                    case 0: StartNewGame();
                        break;
                    case 1: ShowInstructions();
                        break;
                    case 2: ShowHighScores();
                        break;
                    case 3: ExitGame();
                        break;
                    default:
                        break;
                }
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                ExitGame();
            }
            else
            {
                //removes entered chars
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write(" ");
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop); ;
            }
        }
    }

    private static void MoveArrowUp()
    {
        PrintArrow(arrowCoordinateY, "   ");
        arrowCoordinateY -= 6;
        PrintArrow(arrowCoordinateY, arrow);
    }

    private static void MoveArrowDown()
    {
        PrintArrow(arrowCoordinateY, "   ");
        arrowCoordinateY += 6;
        PrintArrow(arrowCoordinateY, arrow);
    }


    static void ShowInstructions()
    {
        int startX = Console.WindowWidth / 2 - 30;
        int startY = Console.WindowHeight / 2 - 13;
        int lineDistance = 2;

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(startX + 25, startY - 6);
        Console.WriteLine("INSTRUCTIONS");

        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(startX, startY - 1);
        Console.WriteLine("Test your memory!");
        Console.SetCursorPosition(startX, startY + lineDistance);
        Console.WriteLine(@"Collect all the pairs of matching images in the cards below.");
        Console.SetCursorPosition(startX, startY + 2 * lineDistance);
        Console.WriteLine("If the selected cards are matching they stay opened, otherwise");
        Console.SetCursorPosition(startX, startY + 3 * lineDistance);
        Console.WriteLine("they are turned back over and you can continue with your next try.");
        Console.SetCursorPosition(startX, startY + 4 * lineDistance + 1);
        Console.WriteLine("The game starts with initial score 3000 points.");
        Console.SetCursorPosition(startX, startY + 5 * lineDistance + 1);
        Console.WriteLine("Every successful try add 50 points to your score and every unguessed ");
        Console.SetCursorPosition(startX, startY + 6 * lineDistance + 1);
        Console.WriteLine("cards remove 50 points from your current score.");
        Console.SetCursorPosition(startX, startY + 8 * lineDistance);
        Console.WriteLine("The game finishes when all the cards are opened and you have passed");
        Console.SetCursorPosition(startX, startY + 9 * lineDistance);
        Console.WriteLine("all the 4 levels or is over when you have 0 points.");

        Console.SetCursorPosition(startX, startY + 11 * lineDistance);
        Console.WriteLine("MOVE LEFT  - LEFT ARROW  ←");
        Console.SetCursorPosition(startX + 35, startY + 11 * lineDistance);
        Console.WriteLine("MOVE UP   - UP ARROW   ↑");
        Console.SetCursorPosition(startX, startY + 12 * lineDistance);
        Console.WriteLine("MOVE RIGHT - RIGHT ARROW →");
        Console.SetCursorPosition(startX + 35, startY + 12 * lineDistance);
        Console.WriteLine("MOVE DOWN - DOWN ARROW ↓");
        Console.SetCursorPosition(startX, startY + 13 * lineDistance);
        Console.WriteLine("SELECT CARD - ENTER");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(startX + 15, startY + 32);
        Console.WriteLine("BACK TO MAIN MENU - press escape");
        BackToMainMenu();
    }

    static void StartNewGame()
    {
        Console.Clear();

        DrawPlayField();

        Game newGame = new Game();
        newGame.StartGame();

        BackToMainMenu();
    }

    static void ShowHighScores()
    {
        int startPosition = 15;
        int indexJustifyPosition = 8;

        int incrementY = 3;       

        Console.Clear();
        Console.SetCursorPosition(middleWidth - indexJustifyPosition, startPosition);
        Console.WriteLine(" HIGH SCORES");
              
        foreach (var item in HighScore.LoadScores())
        {
            Console.SetCursorPosition(middleWidth - indexJustifyPosition - 10, startPosition + incrementY);
            Console.WriteLine(item);
            incrementY += 2;
        }

        Console.SetCursorPosition(middleWidth - 2 * indexJustifyPosition, startPosition + 15);
        Console.WriteLine("BACK TO MAIN MENU - press ESCAPE");
        BackToMainMenu();
    }

    static void ExitGame()
    {
        Console.Clear();
        Environment.Exit(0);
    }

    static void BackToMainMenu()
    {
        while (true)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    ShowMenu();
                }

            }
        }
    }
}
