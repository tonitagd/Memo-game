using System;

class Card
{
    private const int ROWS = 5;
    private const int COLS = 10;

    private char[,] _face;
    private char[,] _back;
    private char[,] _selectedBack;
    private byte _index;

    public Card()
    {
        this._face = new char[ROWS, COLS];

        CreateCardBack();
        CreateSelectionBack();

        this._index = 0;
    }

    public Card(byte index)
    {
        this._face = new char[ROWS, COLS];

        CreateCardBack();
        CreateSelectionBack();

        this._index = index;
    }

    public Card(byte index, char[,] face)
    {
        this._face = new char[ROWS, COLS];

        CreateCardBack();
        CreateSelectionBack();

        this._face = face;
        this._index = index;
    }

    private void CreateCardBack()
    {
        this._back = new char[ROWS, COLS];
        FillArrayWithSymbols(_back, '█');
    }

    private void CreateSelectionBack()
    {
        this._selectedBack = new char[ROWS, COLS];
        FillArrayWithSymbols(_selectedBack, '░');
    }

    private void FillArrayWithSymbols(char[,] array, char symbol)
    {
        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                array[row, col] = symbol;
            }
        }
    }

    private void SetRandomFaceImage()
    {
        this._face = GetRadnomImage();
    }

    private char[,] GetRadnomImage()
    {
        //Create random name and search for it from the directory. It should be read with FileReader.ReadWholeFile(fileName)
        throw new NotImplementedException();
    }

    public void ClearImage()
    {
        this._face = new char[ROWS, COLS];
        FillArrayWithSymbols(_selectedBack, ' ');
    }

    // This is an example how to create a public getter and setter for the private field
    public byte Index
    {
        set { this._index = value; }
        get { return this._index; }
    }

    public char[,] Face
    {
        set { this._face = value; }
        get { return this._face; }
    }

    public char[,] Back
    {
        set { this._back = value; }
        get { return this._back; }
    }

    public char[,] SelectedBack
    {
        set { this._selectedBack = value; }
        get { return this._selectedBack; }
    }
}
