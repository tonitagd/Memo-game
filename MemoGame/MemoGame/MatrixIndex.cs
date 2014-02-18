using System;

namespace MemoGame
{
    //this is helper class for populating matrix with random values
    class MatrixIndex
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }

        public MatrixIndex(int rowIndex, int colIndex)
        {
            this.RowIndex = rowIndex;
            this.ColIndex = colIndex;
        }
    }
}
