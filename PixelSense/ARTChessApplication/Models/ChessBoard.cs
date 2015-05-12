using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARTChessApplication.Models
{
    internal sealed class ChessBoard
    {
        public Piece[] Pieces;
        public ChessBoard LastBoard;
        public colorPiece NowPlays;

        public int FiftyMoves;
        public int LastRoiMoves;
        public int BlancPieces;
        public int NoirPieces;

        public int EnPassantSquare;

        public bool CanBlancCastleLeft;
        public bool CanBlancCastleRight;
        public bool CanNoirCastleLeft;
        public bool CanNoirCastleRight;
        public bool BlancCastled;
        public bool NoirCastled;

        public bool IsBlancChecked;
        public bool IsNoirChecked;
        public bool IsBlancMated;
        public bool IsNoirMated;
        public bool IsStaleMate;

        public byte BlancRoiSquare;
        public byte NoirRoiSquare;

        public double Score;

        public ulong HashValue;

        public GamePhase Phase;

        public byte MovedFromSquare;
        public byte MovedToSquare;

        public byte CpuMovedFromSquare;
        public byte CpuMovedToSquare;

        public ChessBoard()
        {
            byte i, j, line = 0;
            int hash;

            HashValue = 0;
            Pieces = new Piece[128];
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    if (i == 0 && (j == 0 || j == 7))
                        Pieces[line + j] = new Piece(typePiece.Tour, colorPiece.Noir);
                    else if (i == 0 && (j == 1 || j == 6))
                        Pieces[line + j] = new Piece(typePiece.Cavalier, colorPiece.Noir);
                    else if (i == 0 && (j == 2 || j == 5))
                        Pieces[line + j] = new Piece(typePiece.Fou, colorPiece.Noir);
                    else if (i == 0 && j == 3)
                        Pieces[line + j] = new Piece(typePiece.Dame, colorPiece.Noir);
                    else if (i == 0 && j == 4)
                        Pieces[line + j] = new Piece(typePiece.Roi, colorPiece.Noir);
                    else if (i == 1)
                        Pieces[line + j] = new Piece(typePiece.Pion, colorPiece.Noir);
                    else if (i == 7 && (j == 0 || j == 7))
                        Pieces[line + j] = new Piece(typePiece.Tour, colorPiece.Blanc);
                    else if (i == 7 && (j == 1 || j == 6))
                        Pieces[line + j] = new Piece(typePiece.Cavalier, colorPiece.Blanc);
                    else if (i == 7 && (j == 2 || j == 5))
                        Pieces[line + j] = new Piece(typePiece.Fou, colorPiece.Blanc);
                    else if (i == 7 && j == 3)
                        Pieces[line + j] = new Piece(typePiece.Dame, colorPiece.Blanc);
                    else if (i == 7 && j == 4)
                        Pieces[line + j] = new Piece(typePiece.Roi, colorPiece.Blanc);
                    else if (i == 6)
                        Pieces[line + j] = new Piece(typePiece.Pion, colorPiece.Blanc);
                    else
                        Pieces[line + j] = new Piece(typePiece.Rien, colorPiece.Blanc);

                    if (Pieces[line + j].type != typePiece.Rien)
                    {
                        hash = ZobristHashing.GetHashValue(Pieces[line + j], (byte)(line + j));
                        HashValue ^= (ulong)hash;
                    }
                }
                line += 16;
            }

            LastBoard = null;
            NowPlays = colorPiece.Blanc;

            FiftyMoves = 50;
            BlancPieces = 16;
            NoirPieces = 16;
            LastRoiMoves = -1;

            EnPassantSquare = -1;

            CanNoirCastleLeft = true;
            CanNoirCastleRight = true;
            CanBlancCastleLeft = true;
            CanBlancCastleRight = true;
            BlancCastled = false;
            NoirCastled = false;

            IsBlancChecked = false;
            IsNoirChecked = false;
            IsBlancMated = false;
            IsNoirMated = false;
            IsStaleMate = false;

            BlancRoiSquare = 7 * 16 + 4;
            NoirRoiSquare = 4;

            Score = 0;

            Phase = GamePhase.Opening;

            MovedFromSquare = 250;
            MovedToSquare = 250;
        }

        private ChessBoard(object obj)
        {
        }

        public ChessBoard Clone()
        {
            byte i, j, line = 0;
            ChessBoard board = new ChessBoard(null);

            board.Pieces = new Piece[128];
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 8; j++)
                    board.Pieces[line + j] = (Piece)Pieces[line + j].Clone();
                line += 16;
            }
            board.LastBoard = LastBoard;
            board.NowPlays = NowPlays;

            board.FiftyMoves = FiftyMoves;
            board.BlancPieces = BlancPieces;
            board.NoirPieces = NoirPieces;
            board.LastRoiMoves = LastRoiMoves;

            board.EnPassantSquare = EnPassantSquare;

            board.CanNoirCastleLeft = CanNoirCastleLeft;
            board.CanNoirCastleRight = CanNoirCastleRight;
            board.CanBlancCastleLeft = CanBlancCastleLeft;
            board.CanBlancCastleRight = CanBlancCastleRight;
            board.BlancCastled = BlancCastled;
            board.NoirCastled = NoirCastled;

            board.IsBlancChecked = IsBlancChecked;
            board.IsNoirChecked = IsNoirChecked;
            board.IsBlancMated = IsBlancMated;
            board.IsNoirMated = IsNoirMated;
            board.IsStaleMate = IsStaleMate;

            board.BlancRoiSquare = BlancRoiSquare;
            board.NoirRoiSquare = NoirRoiSquare;

            board.Score = Score;
            board.HashValue = HashValue;

            board.Phase = Phase;

            board.MovedFromSquare = MovedFromSquare;
            board.MovedToSquare = MovedToSquare;

            return board;
        }

        public bool Equals(ChessBoard other)
        {
            byte i, j, line = 0;

            if (HashValue != other.HashValue)
                return false;
            if (other.CanNoirCastleLeft != CanNoirCastleLeft || other.CanNoirCastleRight != CanNoirCastleRight
                || other.CanBlancCastleLeft != CanBlancCastleLeft || other.CanBlancCastleRight != CanBlancCastleRight
                || other.EnPassantSquare != EnPassantSquare)
                return false;
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 8; j++)
                    if (Pieces[line + j].color != other.Pieces[line + j].color ||
                        Pieces[line + j].type != other.Pieces[line + j].type)
                        return false;
                line += 16;
            }
            return true;
        }

        public ChessBoard Move(byte fromSquare, byte toSquare, typePiece promotiontype = typePiece.Rien)
        {
            ChessBoard newBoard = Clone();
            newBoard.Pieces[toSquare] = newBoard.Pieces[fromSquare];
            newBoard.Pieces[fromSquare] = new Piece(typePiece.Rien, colorPiece.Noir);

            newBoard.MovedFromSquare = fromSquare;
            newBoard.MovedToSquare = toSquare;

            ComputeNewHash(newBoard, fromSquare, toSquare);

            newBoard.LastBoard = this;

            if (fromSquare == BlancRoiSquare)
            {
                newBoard.BlancRoiSquare = toSquare;
            }
            else if (fromSquare == NoirRoiSquare)
            {
                newBoard.NoirRoiSquare = toSquare;
            }

            CheckForEnPassant(newBoard, fromSquare, toSquare);
            IsEnPassant(newBoard, fromSquare, toSquare);
            CheckForCastle(newBoard, fromSquare, toSquare);
            CheckForPromotion(newBoard, fromSquare, toSquare, promotiontype);

            if (Pieces[toSquare].type != typePiece.Rien && Pieces[toSquare].color == colorPiece.Blanc)
            {
                newBoard.BlancPieces--;
            }
            else if (Pieces[toSquare].type != typePiece.Rien && Pieces[toSquare].color == colorPiece.Noir)
            {
                newBoard.NoirPieces--;
            }
            if (Pieces[fromSquare].type == typePiece.Pion || Pieces[toSquare].type != typePiece.Rien)
                newBoard.FiftyMoves = -1;

            if (NowPlays == colorPiece.Noir)
            {
                if (newBoard.FiftyMoves > 0)
                    newBoard.FiftyMoves--;
                if (newBoard.LastRoiMoves > 0)
                    newBoard.LastRoiMoves--;
            }

            if ((newBoard.BlancPieces == 1 || newBoard.NoirPieces == 1) && newBoard.LastRoiMoves == -1)
                newBoard.LastRoiMoves = 50;

            if (NowPlays == colorPiece.Blanc)
                newBoard.NowPlays = colorPiece.Noir;
            else
                newBoard.NowPlays = colorPiece.Blanc;

            if (newBoard.BlancPieces + newBoard.NoirPieces <= 10)
                newBoard.Phase = GamePhase.End;

            return newBoard;
        }

        private void CheckForPromotion(ChessBoard newBoard, byte fromSquare, byte toSquare,
            typePiece promotiontype)
        {
            if (NowPlays == colorPiece.Blanc && Pieces[fromSquare].type == typePiece.Pion &&
                toSquare < 16)
            {
                //if (promotiontype == typePiece.Rien)
                //{
                //    PromotionWindow window = new PromotionWindow();
                //    window.ShowDialog();
                //    promotiontype = window.ChosentypePiece;
                //}

                newBoard.HashValue ^= (ulong)ZobristHashing.GetHashValue(newBoard.Pieces[toSquare], toSquare);
                newBoard.Pieces[toSquare] = new Piece(promotiontype, colorPiece.Blanc);
                newBoard.HashValue ^= (ulong)ZobristHashing.GetHashValue(newBoard.Pieces[toSquare], toSquare);
            }
            else if (NowPlays == colorPiece.Noir && Pieces[fromSquare].type == typePiece.Pion
                && (toSquare / 16) == 7)
            {
                //if (promotiontype == typePiece.Rien)
                //{
                //    PromotionWindow window = new PromotionWindow();
                //    window.ShowDialog();
                //    promotiontype = window.ChosentypePiece;
                //}

                newBoard.HashValue ^= (ulong)ZobristHashing.GetHashValue(newBoard.Pieces[toSquare], toSquare);
                newBoard.Pieces[toSquare] = new Piece(promotiontype, colorPiece.Noir);
                newBoard.HashValue ^= (ulong)ZobristHashing.GetHashValue(newBoard.Pieces[toSquare], toSquare);
            }
        }

        private void CheckForCastle(ChessBoard newBoard, byte fromSquare, byte toSquare)
        {
            if (NowPlays == colorPiece.Blanc && fromSquare == (7 * 16 + 4) && toSquare == (7 * 16 + 2)
                && CanBlancCastleLeft == true)
            {
                newBoard.Pieces[7 * 16] = new Piece(typePiece.Rien, colorPiece.Noir);
                newBoard.Pieces[7 * 16 + 3] = new Piece(typePiece.Tour, colorPiece.Blanc);

                ComputeNewHash(newBoard, 7 * 16, 7 * 16 + 3);

                newBoard.BlancCastled = true;
            }
            if (NowPlays == colorPiece.Blanc && fromSquare == (7 * 16 + 4) && toSquare == (7 * 16 + 6)
                && CanBlancCastleRight == true)
            {
                newBoard.Pieces[7 * 16 + 7] = new Piece(typePiece.Rien, colorPiece.Noir);
                newBoard.Pieces[7 * 16 + 5] = new Piece(typePiece.Tour, colorPiece.Blanc);

                ComputeNewHash(newBoard, 7 * 16 + 7, 7 * 16 + 5);

                newBoard.BlancCastled = true;
            }
            if (NowPlays == colorPiece.Noir && fromSquare == 4 && toSquare == 2 && CanNoirCastleLeft == true)
            {
                newBoard.Pieces[0] = new Piece(typePiece.Rien, colorPiece.Noir);
                newBoard.Pieces[3] = new Piece(typePiece.Tour, colorPiece.Noir);

                ComputeNewHash(newBoard, 0, 3);

                newBoard.NoirCastled = true;
            }
            if (NowPlays == colorPiece.Noir && fromSquare == 4 && toSquare == 6 && CanNoirCastleRight == true)
            {
                newBoard.Pieces[7] = new Piece(typePiece.Rien, colorPiece.Noir);
                newBoard.Pieces[5] = new Piece(typePiece.Tour, colorPiece.Noir);

                ComputeNewHash(newBoard, 7, 5);

                newBoard.NoirCastled = true;
            }

            if (CanBlancCastleLeft == true && fromSquare == (7 * 16))
                newBoard.CanBlancCastleLeft = false;
            else if (CanBlancCastleRight == true && fromSquare == (7 * 16 + 7))
                newBoard.CanBlancCastleRight = false;
            else if (CanNoirCastleLeft == true && fromSquare == 0)
                newBoard.CanNoirCastleLeft = false;
            else if (CanNoirCastleRight == true && fromSquare == 7)
                newBoard.CanNoirCastleRight = false;
            else if ((CanBlancCastleLeft == true || CanBlancCastleRight == true) &&
                fromSquare == (7 * 16 + 4))
            {
                newBoard.CanBlancCastleLeft = false;
                newBoard.CanBlancCastleRight = false;
            }
            else if ((CanNoirCastleLeft == true || CanNoirCastleRight == true) &&
                fromSquare == 4)
            {
                newBoard.CanNoirCastleLeft = false;
                newBoard.CanNoirCastleRight = false;
            }
        }

        private void IsEnPassant(ChessBoard newBoard, byte fromSquare, byte toSquare)
        {
            if (toSquare == EnPassantSquare && Pieces[fromSquare].type == typePiece.Pion)
            {
                if (NowPlays == colorPiece.Blanc)
                {
                    newBoard.Pieces[toSquare + 16] = new Piece(typePiece.Rien, colorPiece.Noir);

                    newBoard.HashValue ^= (ulong)ZobristHashing.GetHashValue(newBoard.Pieces[toSquare + 16],
                        (byte)(toSquare + 16));

                    newBoard.NoirPieces--;
                }
                else
                {
                    newBoard.Pieces[toSquare - 16] = new Piece(typePiece.Rien, colorPiece.Noir);

                    newBoard.HashValue ^= (ulong)ZobristHashing.GetHashValue(newBoard.Pieces[toSquare - 16],
                        (byte)(toSquare - 16));

                    newBoard.BlancPieces--;
                }
            }
        }

        private void CheckForEnPassant(ChessBoard newBoard, byte fromSquare, byte toSquare)
        {
            if (Pieces[fromSquare].type != typePiece.Pion)
            {
                newBoard.EnPassantSquare = -1;
                return;
            }

            if (NowPlays == colorPiece.Blanc && (fromSquare / 16) == 6 && (toSquare / 16) == 4)
                newBoard.EnPassantSquare = toSquare + 16;
            else if (NowPlays == colorPiece.Noir && (fromSquare / 16) == 1 && (toSquare / 16) == 3)
                newBoard.EnPassantSquare = toSquare - 16;
            else
                newBoard.EnPassantSquare = -1;
        }

        public bool IsDraw()
        {
            if (FiftyMoves == 0 || LastRoiMoves == 0)
                return true;
            if (BlancPieces == 1 && NoirPieces == 1)
                return true;
            if (IsStaleMate == true)
                return true;

            if (NowPlays == colorPiece.Blanc)
            {
                ChessBoard temp = LastBoard;
                int occurences = 0;
                while (temp != null)
                {
                    if (Equals(temp) == true)
                        occurences++;
                    temp = temp.LastBoard;
                }
                if (occurences == 2)
                    return true;
            }

            return false;
        }

        public bool IsBoardValid()
        {
            if (NowPlays == colorPiece.Noir && IsBlancChecked == true)
                return false;
            if (NowPlays == colorPiece.Blanc && IsNoirChecked == true)
                return false;

            return true;
        }

        private void ComputeNewHash(ChessBoard newBoard, byte fromSquare, byte toSquare)
        {
            Piece fromPiece = Pieces[fromSquare];
            Piece toPiece = Pieces[toSquare];

            if (toPiece.type != typePiece.Rien)
                newBoard.HashValue ^= (ulong)ZobristHashing.GetHashValue(toPiece, toSquare);
            newBoard.HashValue ^= (ulong)ZobristHashing.GetHashValue(fromPiece, fromSquare);
            newBoard.HashValue ^= (ulong)ZobristHashing.GetHashValue(fromPiece, toSquare);
        }

        public string FenString()
        {
            StringBuilder builder = new StringBuilder();
            byte spacesCount = 0, i, j, line = 0;
            Piece tempPiece;

            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    tempPiece = Pieces[line + j];
                    if (tempPiece.type == typePiece.Rien)
                        spacesCount++;
                    else
                    {
                        if (spacesCount > 0)
                            builder.Append(spacesCount);
                        builder.Append(tempPiece.FenLetter());
                        spacesCount = 0;
                    }
                }
                if (spacesCount > 0)
                    builder.Append(spacesCount);
                spacesCount = 0;
                if (i < 7)
                    builder.Append('/');

                line += 16;
            }

            if (NowPlays == colorPiece.Noir)
                builder.Append(" b ");
            else
                builder.Append(" w ");

            if (CanBlancCastleRight == true)
                builder.Append('K');
            if (CanBlancCastleLeft == true)
                builder.Append('Q');
            if (CanNoirCastleRight == true)
                builder.Append('k');
            if (CanNoirCastleLeft == true)
                builder.Append('q');
            if (CanNoirCastleLeft == false && CanNoirCastleRight == false &&
                CanBlancCastleLeft == false && CanBlancCastleRight == false)
                builder.Append("- ");
            else
                builder.Append(' ');

            if (EnPassantSquare == -1)
                builder.Append('-');
            else
            {
                int row = EnPassantSquare / 16;
                int column = EnPassantSquare % 16;
                char fenColumn = 'a';
                fenColumn += (char)column;

                builder.Append(fenColumn);
                builder.Append(8 - row);
            }

            return builder.ToString();
        }
    }
}
