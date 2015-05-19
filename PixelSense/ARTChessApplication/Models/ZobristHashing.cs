using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARTChessApplication.Models
{
    internal sealed class ZobristHashing
    {
        private static int[] _hashValues;

        static ZobristHashing()
        {
            Random random = new Random();

            _hashValues = new int[12 * 128];
            for (int i = 0; i < _hashValues.Length; i++)
                _hashValues[i] = random.Next();
        }

        public static int GetHashValue(Piece piece, byte square)
        {
            int pieceIndex = 0, colorIndex = 0;

            if (piece.type == typePiece.Pion)
                pieceIndex = 0;
            else if (piece.type == typePiece.Cavalier)
                pieceIndex = 1;
            else if (piece.type == typePiece.Fou)
                pieceIndex = 2;
            else if (piece.type == typePiece.Tour)
                pieceIndex = 3;
            else if (piece.type == typePiece.Dame)
                pieceIndex = 4;
            else if (piece.type == typePiece.Roi)
                pieceIndex = 5;

            if (piece.color == colorPiece.Noir)
                colorIndex = 1;
            else
                colorIndex = 0;

            return _hashValues[colorIndex * 6 * 64 + pieceIndex * 64 + square];
        }
    }
}
