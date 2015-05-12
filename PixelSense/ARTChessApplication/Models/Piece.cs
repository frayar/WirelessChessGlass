using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARTChessApplication.Models
{
    class Piece
    {
        public typePiece type;
        public colorPiece color;
        public int value;

        public int ActionValue;

        public int NumberOfAttackers;
        public int NumberOfDefenders;
        public int AttackersValue;
        public int DefendersValue;

        public bool IsPassedPion;

        private int _minimumAttackerValue = Int32.MaxValue;

        public Piece(typePiece type, colorPiece color)
        {
            this.type = type;
            this.color = color;
            this.value = Piece.GetValue(this.type);
            ActionValue = Piece.GetActionValue(this.type);
            NumberOfAttackers = 0;
            NumberOfDefenders = 0;
            AttackersValue = 0;
            DefendersValue = 0;
            IsPassedPion = false;
        }

        public Piece Clone()
        {
            Piece piece = new Piece(this.type, this.color);
            return piece;
        }

        public static int GetValue(typePiece type)
        {
            if (type == typePiece.Fou)
                return 300;
            else if (type == typePiece.Roi)
                return 1000;
            else if (type == typePiece.Cavalier)
                return 300;
            else if (type == typePiece.Pion)
                return 100;
            else if (type == typePiece.Dame)
                return 900;
            else if (type == typePiece.Tour)
                return 500;

            return 0;
        }

        public static int GetActionValue(typePiece type)
        {
            if (type == typePiece.Fou)
                return 300;
            else if (type == typePiece.Roi)
                return 100;
            else if (type == typePiece.Cavalier)
                return 300;
            else if (type == typePiece.Pion)
                return 600;
            else if (type == typePiece.Dame)
                return 100;
            else if (type == typePiece.Tour)
                return 200;

            return 0;
        }
        private void Attack(Piece piece)
        {
            piece.NumberOfAttackers++;
            piece.AttackersValue += ActionValue;
            if (value < piece._minimumAttackerValue)
                piece._minimumAttackerValue = value;
        }

        private void Defend(Piece piece)
        {
            piece.NumberOfDefenders++;
            piece.DefendersValue += ActionValue;
        }

        public void Step(Piece piece)
        {
            if (piece.type == typePiece.Rien && this.color == colorPiece.Blanc)
                Attack(piece);
            else if (piece.type == typePiece.Rien && this.color == colorPiece.Noir)
                Defend(piece);
            else if (piece.color == this.color)
                Defend(piece);
            else
                Attack(piece);
        }

        public bool IsWellDefended()
        {
            if (NumberOfAttackers == 0)
                return true;
            if (NumberOfAttackers > 0 && NumberOfDefenders == 0)
                return false;
            if (_minimumAttackerValue < this.value)
                return false;
            if (NumberOfAttackers >= NumberOfDefenders && AttackersValue >= (DefendersValue + ActionValue) && NumberOfAttackers > 0)
                return false;
            return true;
        }

        public char FenLetter()
        {
            char letter = ' ';

            if (this.type == typePiece.Fou)
                letter = 'B';
            else if (this.type == typePiece.Roi)
                letter = 'K';
            else if (this.type == typePiece.Cavalier)
                letter = 'N';
            else if (this.type == typePiece.Pion)
                letter = 'P';
            else if (this.type == typePiece.Dame)
                letter = 'Q';
            else if (this.type == typePiece.Tour)
                letter = 'R';

            if (this.color == colorPiece.Noir)
                letter = Char.ToLower(letter);

            return letter;
        }
    }
}
