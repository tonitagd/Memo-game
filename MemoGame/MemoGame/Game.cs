using MemoGame;
using System;
using System.Collections.Generic;
using System.Threading;

class Game
{
    private const int CARD_ROWS = 5;
    private const int CARD_COLS = 10;

    private Dictionary<byte, Card> _cardList;
    private int[,] _cardsMatrix;

    //distance between cards
    private const int cardDistanceX = 15;
    private const int cardDistanceY = 7;

    private const ConsoleColor white = ConsoleColor.White;
    private const ConsoleColor green = ConsoleColor.Green;
    private const ConsoleColor darkGreen = ConsoleColor.DarkGreen;
    private const ConsoleColor red = ConsoleColor.Red;
    private const ConsoleColor darkRed = ConsoleColor.DarkRed;
    private const ConsoleColor yellow = ConsoleColor.Yellow;

    static int startCardsFieldX = 25;
    static int startCardsFieldY = 15;

    int currentCoordinateX = 25;
    int currentCoordinateY = 15;

    int selectedCoordinateX = 0;
    int selectedCoordinateY = 0;
    bool isCardSelected = false;

    byte selectedFirstId = 0;
    byte selectedSecondId = 0;

    static string doubleSelectionErr = "You cannot select same card twice!";

    List<byte> OpenedCardList = new List<byte>();

    Card firstCard;

    static int numberOfTries = 0;

    static Score score;
    static HighScore highScore;
    static TimeSpan gameTimer;

    public Game()
    {
        Application.RemoveScrollBars();
        Initialize();
    }

    private void Initialize()
    {
        _cardList = new Dictionary<byte, Card>();
        //here will be readed all text files
        string[] fileNames = {"file1.txt" , "file2.txt" ,"file3.txt" ,"file4.txt" ,
                                 "file5.txt" ,"file6.txt" ,"file7.txt" , "file8.txt" ,
                                 "file9.txt" ,"file10.txt" ,"file11.txt" ,"file12.txt"};

        byte id = 1;
        foreach (var name in fileNames)
        {
            //use function from FileUtil class to get face
            char[,] face = FileReader.ReadFileAsMatrix(name, CARD_ROWS, CARD_COLS);
            Card card = new Card(id, face);

            //store all card in dictionary
            _cardList.Add(id, card);

            //increment id
            id++;
        }
    }

    private static void ValidateName(string name)
    {
        if (name.Length == 0)
        {
            throw new ArgumentException("Name cannot be empty .");
        }

        if (name.Length > 10)
        {
            throw new ArgumentException("Name cannot be more than 10 symbols.");
        }
    }

    public void StartGame()
    {
        //for calculate time of playing 
        DateTime startGameTimer = DateTime.Now;

        highScore = new HighScore();
        score = new Score();
        numberOfTries = 0;

        //First level will be 3x4
        int rows = 3;
        int cols = 4;

        int level = 1;
        ShowLevel(level);

        while (PlayLevel(rows, cols))
        {
            OpenedCardList.Clear();
            int bonus = rows * cols * 10;
            score.AddScore((ushort)bonus);
            Thread musicThread;
            musicThread = new Thread(new ThreadStart(PlayList.PlayLevelUpVar2));
            musicThread.IsBackground = true;
            musicThread.Start();

            if (level >= 4)
            {
                Console.Clear();
                Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 2);
                Console.WriteLine("Victory !!!");


                if (score.CurrentScore > highScore.Lowest)
                {
                    EnterHighScore();
                    Console.Clear();
                    Application.ShowMenu();
                }

                break;
            }

            EndOfLevel(startGameTimer, rows, cols);

            if (level % 2 != 0)
            {
                rows++;
            }
            else
            {
                cols++;
                startCardsFieldX -= cardDistanceX;
                startCardsFieldY -= cardDistanceY;
            }

            if ((rows * cols) % 2 != 0)
            {
                cols++;
                rows--;
            }

            level++;
            Console.Clear();
            numberOfTries = 0;
            ShowLevel(level);
            startGameTimer = DateTime.Now;
        }

        if (level < 4)
        {
            GameOver();
        }
    }

    private void ShowLevel(int level)
    {
        string message = "Level {0}/4";
        Console.SetCursorPosition(Console.WindowWidth / 2 - message.Length / 2, 0);
        Console.ForegroundColor = green;
        Console.WriteLine(message, level);
    }

    //initial and non guessed cards are red, guessed cards become green
    //selector and usefull information are in yellow
    //cards on selection change to darker color - non guessed cards change to dark red, guessed change to darn green

    private bool PlayLevel(int rows, int cols)
    {
        int endCardsFieldY = startCardsFieldY + rows * CARD_ROWS + (rows - 1) / 2 * cardDistanceY;

        Console.SetCursorPosition(startCardsFieldX, endCardsFieldY);
        Console.ForegroundColor = red;
        Console.WriteLine("BACK TO MAIN MENU  - press escape");

        PopulateArrayWithPictures(rows, cols);
        DrawAllCards(rows, cols, red);
        Thread.Sleep(2000);
        HideAllCards(rows, cols);
        OpenedCardList.Add(0);
        Application.DrawPlayField();

        while (true)
        {
            DrawScorePanel(score);

            DrawInformationPanel("Select first card", numberOfTries, cols);

            DrawAllCards(rows, cols, green);

            firstCard = ChooseCard();

            DrawInformationPanel("Select second card", numberOfTries, cols);

            Card cardToCompare = ChooseCard();
            numberOfTries++;

            Console.SetCursorPosition(startCardsFieldX + (cols - 1) * cardDistanceX, startCardsFieldY - 5);

            if (firstCard == null || cardToCompare == null)
            {
                Thread.Sleep(500);
                DrawCardBack(selectedCoordinateX, selectedCoordinateY, '█', white);
                DrawCardBack(currentCoordinateX, currentCoordinateY, '█', white);
                isCardSelected = false;
                continue;
            }

            if (!isCardSelected)
            {
                bool hasEqualFace = CompareFaceCards(firstCard, cardToCompare);

                //if cards has equal faces redraw them in green, otherwise close them
                if (hasEqualFace)
                {
                    DrawCard(selectedFirstId, selectedCoordinateX, selectedCoordinateY, green);
                    DrawCard(selectedSecondId, currentCoordinateX, currentCoordinateY, green);
                    OpenedCardList.Add(selectedFirstId);
                    score.AddScore(50);
                }
                else
                {
                    Thread.Sleep(500);
                    DrawCardBack(selectedCoordinateX, selectedCoordinateY, '█', white);
                    DrawCardBack(currentCoordinateX, currentCoordinateY, '█', white);
                    score.RemoveScore(50);
                }
            }

            if (score.CurrentScore <= 0)
            {
                return false;
            }
            else if (OpenedCardList.Count >= rows * cols)
            {
                return true;
            }

        }
    }

    private void PopulateArrayWithPictures(int rows, int cols)
    {
        //generate random indexes
        List<MatrixIndex> randomIndexes = new List<MatrixIndex>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                randomIndexes.Add(new MatrixIndex(i, j));
            }
        }
        //shuffle list
        Shuffle(randomIndexes);

        _cardsMatrix = new int[rows, cols];

        int cardId = 1;
        for (int i = 0; i < randomIndexes.Count - 1; i += 2)
        {
            //set picture id to matrix cell
            MatrixIndex indexes = randomIndexes[i];
            int rowIndex = indexes.RowIndex;
            int colIndex = indexes.ColIndex;
            _cardsMatrix[rowIndex, colIndex] = cardId;

            //set picture to another cell
            indexes = randomIndexes[i + 1];
            rowIndex = indexes.RowIndex;
            colIndex = indexes.ColIndex;
            _cardsMatrix[rowIndex, colIndex] = cardId;

            cardId++;
        }
    }

    private void Shuffle(IList<MatrixIndex> list)
    {
        Random rng = new Random(DateTime.Now.Second);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            MatrixIndex value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void DrawAllCards(int rows, int cols, ConsoleColor color)
    {
        int width = Console.WindowWidth;
        int heigth = Console.WindowHeight;

        int initialX = startCardsFieldX;
        int initialY = startCardsFieldY;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int cardId = _cardsMatrix[i, j];

                if (OpenedCardList.Contains((byte)cardId) || OpenedCardList.Count == 0)
                {
                    DrawCard((byte)cardId, initialX, initialY, color);
                }

                initialX += cardDistanceX;
            }

            initialX = startCardsFieldX;
            initialY += cardDistanceY;
        }
    }

    private void DrawCard(byte cardId, int coordX, int coordY, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        //get card form list
        Card card = _cardList[cardId];

        for (int i = 0; i < CARD_ROWS; i++)
        {
            Console.SetCursorPosition(coordX, coordY + i);
            string line = null;
            for (int col = 0; col < CARD_COLS; col++)
            {
                line += card.Face[i, col];
            }
            Console.Write(line);
        }
    }

    private void HideAllCards(int rows, int cols)
    {
        int width = Console.WindowWidth;
        int heigth = Console.WindowHeight;

        int initialX = startCardsFieldX;
        int initialY = startCardsFieldY;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int cardId = _cardsMatrix[i, j];
                DrawCardBack(initialX, initialY, '█', white);
                initialX += cardDistanceX;
            }

            initialX = startCardsFieldX;
            initialY += cardDistanceY;
        }
    }

    private void DrawScorePanel(Score score)
    {
        Console.ForegroundColor = green;
        Console.SetCursorPosition(85, 2);
        Console.Write("Score: ");
        Console.Write(score.CurrentScore);
        Console.Write("  ");
    }

    //clear inforamtion line and print current user information
    private void DrawInformationPanel(string information, int tries, int cols)
    {
        Console.ForegroundColor = yellow;
        ClearInformationLine();
        Console.SetCursorPosition(startCardsFieldX, startCardsFieldY - 5);
        Console.Write(information);
        Console.SetCursorPosition(startCardsFieldX + (cols - 1) * cardDistanceX, startCardsFieldY - 5);
        Console.Write("Tries: {0}", tries);
    }

    private static void ClearInformationLine()
    {
        Console.SetCursorPosition(startCardsFieldX, startCardsFieldY - 5);
        Console.WriteLine(new string(' ', doubleSelectionErr.Length));
    }

    private Card ChooseCard()
    {
        //Setting the movement boundaries
        int maxRows = _cardsMatrix.GetLength(0);
        int maxCols = _cardsMatrix.GetLength(1);

        int cardRow = 0;
        int cardCol = 0;

        byte selectedId = 0;

        currentCoordinateX = startCardsFieldX;
        currentCoordinateY = startCardsFieldY;

        selectedId = (byte)_cardsMatrix[cardRow, cardCol];
        DrawCardOnSelection(selectedId, currentCoordinateX, currentCoordinateY, darkRed, yellow, '░');

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Enter)
            {
                selectedId = (byte)_cardsMatrix[cardRow, cardCol];

                //if try to select again the first card change the position to the first card
                if ((isCardSelected) &&
                    (currentCoordinateX == selectedCoordinateX) &&
                    (currentCoordinateY == selectedCoordinateY) || OpenedCardList.Contains(selectedId))
                {
                    DrawInformationPanel("Select second card", numberOfTries, maxCols);
                    PlayList.PlayWrongCard();
                    Thread.Sleep(200);
                    if (!OpenedCardList.Contains(selectedId))
                    {
                        DrawCardBack(selectedCoordinateX, selectedCoordinateY, '█', white);
                    }
                    isCardSelected = false;
                    return null;
                }

                if (!isCardSelected)
                {
                    selectedCoordinateX = currentCoordinateX;
                    selectedCoordinateY = currentCoordinateY;
                    isCardSelected = true;
                    DrawCard(selectedId, currentCoordinateX, selectedCoordinateY, red);
                    selectedFirstId = (byte)_cardsMatrix[cardRow, cardCol];
                }
                else
                {
                    isCardSelected = false;
                    DrawCard(selectedId, currentCoordinateX, currentCoordinateY, red);
                    selectedSecondId = (byte)_cardsMatrix[cardRow, cardCol];
                }

                return _cardList[selectedId];
            }
            else if (key.Key == ConsoleKey.UpArrow && cardRow > 0)
            {
                selectedId = (byte)_cardsMatrix[cardRow, cardCol];
                cardRow--;
                DrawCardOnSelection(selectedId, currentCoordinateX, currentCoordinateY, red, white, '█');
                currentCoordinateY -= cardDistanceY;
                selectedId = (byte)_cardsMatrix[cardRow, cardCol];
                DrawCardOnSelection(selectedId, currentCoordinateX, currentCoordinateY, darkRed, yellow, '░');
            }
            else if (key.Key == ConsoleKey.DownArrow && cardRow < maxRows - 1)
            {
                selectedId = (byte)_cardsMatrix[cardRow, cardCol];
                cardRow++;
                DrawCardOnSelection(selectedId, currentCoordinateX, currentCoordinateY, red, white, '█');
                currentCoordinateY += cardDistanceY;
                selectedId = (byte)_cardsMatrix[cardRow, cardCol];
                DrawCardOnSelection(selectedId, currentCoordinateX, currentCoordinateY, darkRed, yellow, '░');
            }
            else if (key.Key == ConsoleKey.LeftArrow && cardCol > 0)
            {
                selectedId = (byte)_cardsMatrix[cardRow, cardCol];
                cardCol--;
                DrawCardOnSelection(selectedId, currentCoordinateX, currentCoordinateY, red, white, '█');
                currentCoordinateX -= cardDistanceX;
                selectedId = (byte)_cardsMatrix[cardRow, cardCol];
                DrawCardOnSelection(selectedId, currentCoordinateX, currentCoordinateY, darkRed, yellow, '░');
            }
            else if (key.Key == ConsoleKey.RightArrow && cardCol < maxCols - 1)
            {
                selectedId = (byte)_cardsMatrix[cardRow, cardCol];
                cardCol++;
                DrawCardOnSelection(selectedId, currentCoordinateX, currentCoordinateY, red, white, '█');
                currentCoordinateX += cardDistanceX;
                selectedId = (byte)_cardsMatrix[cardRow, cardCol];
                DrawCardOnSelection(selectedId, currentCoordinateX, currentCoordinateY, darkRed, yellow, '░');
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                Application.ShowMenu();
            }
            else
            {
                //removes entered chars
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write(" ");
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
        }
    }

    private void DrawCardOnSelection(byte id, int coordinateX, int coordinateY, ConsoleColor colorForFace, ConsoleColor colorForBack, char symbolForBack)
    {
        if ((isCardSelected) && (coordinateX == selectedCoordinateX) && (coordinateY == selectedCoordinateY))
        {
            //face selected
            DrawCard(id, coordinateX, coordinateY, colorForFace);
        }
        else
        {
            //back selected
            DrawCardBack(coordinateX, coordinateY, symbolForBack, colorForBack);
        }

        foreach (var openID in OpenedCardList)
        {
            if ((id == openID) && (colorForFace != darkRed)) //For no selected opened equal cards (green color)
            {
                DrawCard(id, coordinateX, coordinateY, green);
            }
            if ((id == openID) && (colorForFace == darkRed)) //For selected opened equal cards (green color turn into dark green color)
            {
                DrawCard(id, coordinateX, coordinateY, darkGreen);
            }
        }
    }

    private void DrawCardBack(int currentCoordianateX, int currentCoodtindateY, char symbol, ConsoleColor color)
    {
        Console.ForegroundColor = color;

        for (int i = 0; i < CARD_ROWS; i++)
        {
            Console.SetCursorPosition(currentCoordianateX, currentCoodtindateY + i);
            string line = null;
            for (int col = 0; col < CARD_COLS; col++)
            {
                line += symbol;
            }

            Console.Write(line);
        }
    }

    private bool CompareFaceCards(Card card1, Card card2)
    {
        if (selectedFirstId == selectedSecondId)
        {
            //add the opened card in list
            OpenedCardList.Add(selectedFirstId);
            return true;
        }
        else
        {
            return false;
        }
    }

    private static void GameOver()
    {
        Console.Clear();
        Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 2);
        Console.ForegroundColor = red;
        Console.WriteLine("Game Over!");
        PlayList.PlayGameOver();
        Thread.Sleep(1000);
        Console.Clear();
        Application.ShowMenu();
    }

    private static void EnterHighScore()
    {
        Console.ForegroundColor = white;
        Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2 + 2);
        Console.WriteLine("Congratulations! You are in the top 5 players!");
        while (true)
        {
            try
            {
                Console.ForegroundColor = white;
                Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2 + 5);
                Console.Write(new string(' ', 120));
                Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2 + 5);
                Console.Write("Please enter your name : ");
                string name = Console.ReadLine();
                ValidateName(name);
                highScore.AddScore(score.CurrentScore, name);
                highScore.SaveScores();
                break;
            }
            catch (ArgumentException ar)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2 + 3);
                Console.Write(new string(' ', 60));
                Console.ForegroundColor = red;
                Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2 + 3);
                Console.Write(ar.Message);
            }
        }
    }

    private void EndOfLevel(DateTime startGameTimer, int rows, int cols)
    {

        DateTime endGameTimer = DateTime.Now;
        Console.Clear();
        Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 2 - 10);
        gameTimer = CalculateTimeElapsed(startGameTimer, endGameTimer);
        ShowScores(rows * cols);

        //Ask for Exit or go to next level
        Console.ForegroundColor = ConsoleColor.Green;
        int left = Console.WindowWidth / 2 - 11;
        int top = Console.WindowHeight / 2 + 18;

        Console.SetCursorPosition(left - 3, top - 10);
        Console.WriteLine("BACK TO MAIN MENU - press escape");

        WaitSevenSeconds(left, top - 8, "Next level in: {0} seconds...");
    }

    private TimeSpan CalculateTimeElapsed(DateTime start, DateTime end)
    {
        TimeSpan timer = end - start;

        if (timer.Seconds < 60 && timer.Minutes < 1)
        {
            int bonusTime = (60 - timer.Seconds) * 10;
            score.AddScore((ushort)bonusTime);
        }

        return timer;
    }

    static void ShowScores(int numberOfCards)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        string memoryStatus = Game.ShowMemoryStatus(numberOfCards, numberOfTries);

        int left = Console.WindowWidth / 2 - 11;
        int top = Console.WindowHeight / 2;

        string separator = new string('-', top + 2);

        Console.SetCursorPosition(left, top - 12);
        Console.WriteLine(separator);

        Console.SetCursorPosition(left, top - 8);
        Console.WriteLine(separator);

        int memoryStartXCoordinate = left + separator.Length / 2 - memoryStatus.Length / 2;
        Console.SetCursorPosition(memoryStartXCoordinate, top - 10);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(memoryStatus);

        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(left - 14, top - 4);
        Console.WriteLine("You have completed the game in {0} tries in {1:mm\\:ss} minutes", numberOfTries, gameTimer);
        Console.SetCursorPosition(left + 6, top);
        Console.WriteLine("Score : {0}", Game.score.CurrentScore);
    }

    static string ShowMemoryStatus(int numberOfCards, int counterTries)
    {
        //only for complished level 
        string memoryStatus = "";

        //TODO associate tries, scores or time ? 

        if (counterTries <= numberOfCards / 2 + 2)  //every couple of cards is guessed in 1 try - 0-6 tries
        {
            memoryStatus = "Your memory is genial!";
        }
        else if (counterTries >= numberOfCards - 2) //10 tries and more
        {
            memoryStatus = "Your memory is very short !";
        }
        else
        {
            memoryStatus = "Your memory is not so bad !";
        }
        return memoryStatus;
    }

    private void WaitSevenSeconds(int left, int top, string message)
    {
        for (int sec = 7; sec >= 0; sec--)
        {
            Console.SetCursorPosition(left, top);
            Console.WriteLine(message, sec);
            Thread.Sleep(1000);
        }
    }
}

