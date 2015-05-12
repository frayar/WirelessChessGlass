using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace ARTChessApplication.Models
{
    internal sealed class WindowBoard
    {
        public delegate bool ChosenHandler(int fromRow, int fromColumn, int toRow, int toColumn);
        public event ChosenHandler Chosen;

        public WindowPiece SelectedPiece;
        private byte _moveFromRow;
        private byte _moveFromColumn;
        private byte _moveToRow;
        private byte _moveToColumn;

        public WindowPiece[,] Pieces { private set; get; }

        public void SetBoard(ChessBoard board)
        {
            byte i, j, line = 0;

            _moveFromRow = 250;
            _moveFromColumn = 250;
            _moveToRow = 250;
            _moveToColumn = 250;
            Pieces = new WindowPiece[8, 8];
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    Pieces[i, j] = new WindowPiece(i, j, board.Pieces[line + j].type, board.Pieces[line + j].color);
                    Pieces[i, j].Chosen += new WindowPiece.ChosenHandler(WindowBoard_Chosen);
                    if (board.MovedFromSquare == (line + j) || board.MovedToSquare == (line + j))
                    {
                        Pieces[i, j].DisplayControl.BorderThickness = new Thickness(3, 3, 3, 3);
                        Pieces[i, j].DisplayControl.BorderBrush = new SolidColorBrush(Colors.Yellow);
                        if (board.MovedFromSquare == (line + j))
                        {
                            _moveFromRow = i;
                            _moveFromColumn = j;
                        }
                        else
                        {
                            _moveToRow = i;
                            _moveToColumn = j;
                        }
                    }
                }
                line += 16;
            }
            SelectedPiece = null;
        }

        public void WindowBoard_Chosen(int row, int column)
        {
            if (SelectedPiece == null)
            {
                SelectedPiece = Pieces[row, column];
                SelectedPiece.DisplayControl.BorderThickness = new Thickness(1, 1, 1, 1);
                SelectedPiece.DisplayControl.BorderBrush = new SolidColorBrush(Colors.Blue);

            }
            else if ((SelectedPiece.Row == row && SelectedPiece.Column == column) || Chosen == null ||
                Chosen(SelectedPiece.Row, SelectedPiece.Column, row, column) == false)
            {
                if ((_moveFromRow == SelectedPiece.Row && _moveFromColumn == SelectedPiece.Column) ||
                    (_moveToRow == SelectedPiece.Row && _moveToColumn == SelectedPiece.Column))
                {
                    SelectedPiece.DisplayControl.BorderThickness = new Thickness(3, 3, 3, 3);
                    SelectedPiece.DisplayControl.BorderBrush = new SolidColorBrush(Colors.Yellow);
                    SelectedPiece = null;
                }
                else
                {
                    SelectedPiece.DisplayControl.BorderThickness = new Thickness(0);
                    SelectedPiece = null;
                }
            }
        }

        public void Freeze()
        {
            int i, j;

            for (i = 0; i < 8; i++)
                for (j = 0; j < 8; j++)
                    Pieces[i, j].Chosen -= new WindowPiece.ChosenHandler(WindowBoard_Chosen);
        }
       
    }
}
