using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARTChessApplication.Models
{
    internal sealed class MoveValidator
    {
        public bool[] NoirAttacks;
        public bool[] BlancAttacks;
        private ChessBoard _chessBoard;
        private bool _searchForMate;

        public LinkedList<Move> ValidMoves;
        public int TotalBlancPionMoves;
        public int TotalNoirPionMoves;
        public int TotalBlancPiecesMoves;
        public int TotalNoirPiecesMoves;

        public bool BlancCastlesAttacked;
        public bool NoirCastlesAttacked;

        public MoveValidator(ChessBoard chessBoard, bool searchForMate = true)
        {
            NoirAttacks = new bool[128];
            BlancAttacks = new bool[128];
            _chessBoard = chessBoard;
            ValidMoves = new LinkedList<Move>();

            for (byte b = 0; b < 128; b++)
            {
                BlancAttacks[b] = false;
                NoirAttacks[b] = false;
            }
            _searchForMate = searchForMate;
            TotalNoirPionMoves = 0;
            TotalNoirPiecesMoves = 0;
            TotalBlancPionMoves = 0;
            TotalBlancPiecesMoves = 0;
        }

        public void Validate()
        {
            byte line = 0, i, j;
            Piece tempPiece;

            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    tempPiece = _chessBoard.Pieces[line + j];
                    if (tempPiece.type == typePiece.Pion && tempPiece.color == colorPiece.Blanc)
                        ValidateBlancPion((byte)(line + j));
                    else if (tempPiece.type == typePiece.Pion && tempPiece.color == colorPiece.Noir)
                        ValidateNoirPion((byte)(line + j));
                    else if (tempPiece.type == typePiece.Fou)
                        ValidateFou((byte)(line + j));
                    else if (tempPiece.type == typePiece.Tour)
                        ValidateTour((byte)(line + j));
                    else if (tempPiece.type == typePiece.Dame)
                        ValidateDame((byte)(line + j));
                    else if (tempPiece.type == typePiece.Cavalier)
                        ValidateCavalier((byte)(line + j));
                    else if (tempPiece.type == typePiece.Roi)
                        RoiAttacks((byte)(line + j));
                }
                line += 16;
            }
            ValidateRoi(_chessBoard.BlancRoiSquare);
            ValidateRoi(_chessBoard.NoirRoiSquare);

            if (BlancAttacks[_chessBoard.NoirRoiSquare] == true)
                _chessBoard.IsNoirChecked = true;
            else
                _chessBoard.IsNoirChecked = false;
            if (NoirAttacks[_chessBoard.BlancRoiSquare] == true)
                _chessBoard.IsBlancChecked = true;
            else
                _chessBoard.IsBlancChecked = false;

            ValidateCastles();

            if (_searchForMate == true)
            {
                _chessBoard.IsBlancMated = false;
                _chessBoard.IsNoirMated = false;
                if (ValidMoveExists() == false)
                {
                    if (_chessBoard.IsBlancChecked == true)
                        _chessBoard.IsBlancMated = true;
                    else if (_chessBoard.IsNoirChecked == true)
                        _chessBoard.IsNoirMated = true;
                    else
                        _chessBoard.IsStaleMate = true;
                }
            }
        }

        private void ValidateCastles()
        {
            byte b;
            bool canCastle, attackedBlancLeftCastle = false, attackedBlancRightCastle = false;
            bool attackedNoirLeftCastle = false, attackedNoirRightCastle = false;

            if (_chessBoard.IsBlancChecked == false)
            {
                if (_chessBoard.CanBlancCastleLeft == true)
                {
                    canCastle = true;
                    for (b = 7 * 16 + 3; b >= 7 * 16 + 1; b--)
                    {
                        if (NoirAttacks[b] == true || _chessBoard.Pieces[b].type != typePiece.Rien)
                        {
                            canCastle = false;
                            if (_chessBoard.Pieces[b].type == typePiece.Rien)
                                attackedBlancLeftCastle = true;
                            else
                            {
                                attackedBlancLeftCastle = false;
                                break;
                            }
                        }
                    }
                    if (canCastle == true)
                    {
                        AddMove(new Move(_chessBoard.BlancRoiSquare, 7 * 16 + 2));
                        BlancCastlesAttacked = false;
                    }
                }
                if (_chessBoard.CanBlancCastleRight == true)
                {
                    canCastle = true;
                    for (b = 7 * 16 + 5; b <= 7 * 16 + 6; b++)
                    {
                        if (NoirAttacks[b] == true || _chessBoard.Pieces[b].type != typePiece.Rien)
                        {
                            canCastle = false;
                            if (_chessBoard.Pieces[b].type == typePiece.Rien)
                                attackedBlancRightCastle = true;
                            else
                            {
                                attackedBlancRightCastle = false;
                                break;
                            }
                        }
                    }
                    if (canCastle == true)
                    {
                        AddMove(new Move(_chessBoard.BlancRoiSquare, 7 * 16 + 6));
                        BlancCastlesAttacked = false;
                    }
                }
            }
            if (_chessBoard.IsNoirChecked == false)
            {
                if (_chessBoard.CanNoirCastleLeft == true)
                {
                    canCastle = true;
                    for (b = 3; b >= 1; b--)
                        if (BlancAttacks[b] == true || _chessBoard.Pieces[b].type != typePiece.Rien)
                        {
                            canCastle = false;
                            if (_chessBoard.Pieces[b].type == typePiece.Rien)
                                attackedNoirLeftCastle = true;
                            else
                            {
                                attackedNoirLeftCastle = false;
                                break;
                            }
                        }
                    if (canCastle == true)
                        AddMove(new Move(_chessBoard.NoirRoiSquare, 2));
                }
                if (_chessBoard.CanNoirCastleRight == true)
                {
                    canCastle = true;
                    for (b = 5; b <= 6; b++)
                        if (BlancAttacks[b] == true || _chessBoard.Pieces[b].type != typePiece.Rien)
                        {
                            canCastle = false;
                            if (_chessBoard.Pieces[b].type == typePiece.Rien)
                                attackedNoirRightCastle = true;
                            else
                            {
                                attackedNoirRightCastle = false;
                                break;
                            }
                        }
                    if (canCastle == true)
                        AddMove(new Move(_chessBoard.NoirRoiSquare, 6));
                }
            }

            if (attackedBlancLeftCastle == true || attackedBlancRightCastle == true)
                BlancCastlesAttacked = true;
            if (attackedNoirLeftCastle == true || attackedNoirRightCastle == true)
                NoirCastlesAttacked = true;

        }

        private bool ValidMoveExists()
        {
            ChessBoard temp;

            foreach (Move move in ValidMoves)
            {
                temp = _chessBoard.Move(move.FromSquare, move.ToSquare, move.PromotionType);
                MoveValidator validator = new MoveValidator(temp, false);
                validator.Validate();
                if (temp.IsBoardValid() == true)
                    return true;
            }

            return false;
        }

        private void RoiAttacks(byte fromSquare)
        {
            int i, j;
            byte b;

            for (i = -16; i <= 16; i += 16)
                for (j = i - 1; j <= i + 1; j++)
                {
                    b = (byte)(fromSquare + j);
                    if ((b & 0x88) != 0)
                        continue;
                    if (b == fromSquare)
                        continue;

                    if (_chessBoard.Pieces[fromSquare].color == colorPiece.Blanc)
                        BlancAttacks[b] = true;
                    else
                        NoirAttacks[b] = true;

                    _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[b]);
                }
        }

        private void ValidateRoi(byte fromSquare)
        {
            int i, j;
            byte b;

            for (i = -16; i <= 16; i += 16)
                for (j = i - 1; j <= i + 1; j++)
                {
                    b = (byte)(fromSquare + j);
                    if ((b & 0x88) != 0)
                        continue;
                    if (b == fromSquare)
                        continue;
                    if (_chessBoard.Pieces[b].type != typePiece.Rien &&
                        _chessBoard.Pieces[b].color == _chessBoard.Pieces[fromSquare].color)
                        continue;

                    if (_chessBoard.Pieces[fromSquare].color == colorPiece.Blanc
                        && NoirAttacks[b] == false)
                    {
                        AddMove(new Move(fromSquare, b));
                    }
                    else if (_chessBoard.Pieces[fromSquare].color == colorPiece.Noir
                        && BlancAttacks[b] == false)
                    {
                        AddMove(new Move(fromSquare, b));
                    }
                }
        }

        private void ValidateCavalier(byte fromSquare)
        {
            bool[] attacks;
            if (_chessBoard.Pieces[fromSquare].color == colorPiece.Blanc)
                attacks = BlancAttacks;
            else
                attacks = NoirAttacks;

            if (((fromSquare - 18) & 0x88) == 0)
            {
                attacks[fromSquare - 18] = true;

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare - 18]);

                if (_chessBoard.Pieces[fromSquare - 18].type == typePiece.Rien ||
                    _chessBoard.Pieces[fromSquare - 18].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, (byte)(fromSquare - 18)));
                }
            }
            if (((fromSquare - 33) & 0x88) == 0)
            {
                attacks[fromSquare - 33] = true;

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare - 33]);

                if (_chessBoard.Pieces[fromSquare - 33].type == typePiece.Rien ||
                    _chessBoard.Pieces[fromSquare - 33].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, (byte)(fromSquare - 33)));
                }
            }
            if (((fromSquare - 31) & 0x88) == 0)
            {
                attacks[fromSquare - 31] = true;

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare - 31]);

                if (_chessBoard.Pieces[fromSquare - 31].type == typePiece.Rien ||
                    _chessBoard.Pieces[fromSquare - 31].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, (byte)(fromSquare - 31)));
                }
            }
            if (((fromSquare - 14) & 0x88) == 0)
            {
                attacks[fromSquare - 14] = true;

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare - 14]);

                if (_chessBoard.Pieces[fromSquare - 14].type == typePiece.Rien ||
                    _chessBoard.Pieces[fromSquare - 14].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, (byte)(fromSquare - 14)));
                }
            }
            if (((fromSquare + 14) & 0x88) == 0)
            {
                attacks[fromSquare + 14] = true;

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare + 14]);

                if (_chessBoard.Pieces[fromSquare + 14].type == typePiece.Rien ||
                    _chessBoard.Pieces[fromSquare + 14].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, (byte)(fromSquare + 14)));
                }
            }
            if (((fromSquare + 31) & 0x88) == 0)
            {
                attacks[fromSquare + 31] = true;

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare + 31]);

                if (_chessBoard.Pieces[fromSquare + 31].type == typePiece.Rien ||
                    _chessBoard.Pieces[fromSquare + 31].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, (byte)(fromSquare + 31)));
                }
            }
            if (((fromSquare + 33) & 0x88) == 0)
            {
                attacks[fromSquare + 33] = true;

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare + 33]);

                if (_chessBoard.Pieces[fromSquare + 33].type == typePiece.Rien ||
                    _chessBoard.Pieces[fromSquare + 33].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, (byte)(fromSquare + 33)));
                }
            }
            if (((fromSquare + 18) & 0x88) == 0)
            {
                attacks[fromSquare + 18] = true;

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare + 18]);

                if (_chessBoard.Pieces[fromSquare + 18].type == typePiece.Rien ||
                    _chessBoard.Pieces[fromSquare + 18].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, (byte)(fromSquare + 18)));
                }
            }
        }

        private void ValidateDame(byte fromSquare)
        {
            ValidateFou(fromSquare);
            ValidateTour(fromSquare);
        }

        private void ValidateTour(byte fromSquare)
        {
            byte b;

            for (b = (byte)(fromSquare - 16); (b & 0x88) == 0; b -= 16)
            {
                if (_chessBoard.Pieces[fromSquare].color == colorPiece.Noir)
                    NoirAttacks[b] = true;
                else
                    BlancAttacks[b] = true;
                if (_chessBoard.Pieces[b].type == typePiece.Rien ||
                    _chessBoard.Pieces[b].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, b));
                }

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[b]);

                if (_chessBoard.Pieces[b].type != typePiece.Rien)
                    break;
            }
            for (b = (byte)(fromSquare + 16); (b & 0x88) == 0; b += 16)
            {
                if (_chessBoard.Pieces[fromSquare].color == colorPiece.Noir)
                    NoirAttacks[b] = true;
                else
                    BlancAttacks[b] = true;
                if (_chessBoard.Pieces[b].type == typePiece.Rien ||
                    _chessBoard.Pieces[b].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, b));
                }

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[b]);

                if (_chessBoard.Pieces[b].type != typePiece.Rien)
                    break;
            }
            for (b = (byte)(fromSquare - 1); (b & 0x88) == 0; b--)
            {
                if (_chessBoard.Pieces[fromSquare].color == colorPiece.Noir)
                    NoirAttacks[b] = true;
                else
                    BlancAttacks[b] = true;
                if (_chessBoard.Pieces[b].type == typePiece.Rien ||
                    _chessBoard.Pieces[b].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, b));
                }

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[b]);

                if (_chessBoard.Pieces[b].type != typePiece.Rien)
                    break;
            }
            for (b = (byte)(fromSquare + 1); (b & 0x88) == 0; b++)
            {
                if (_chessBoard.Pieces[fromSquare].color == colorPiece.Noir)
                    NoirAttacks[b] = true;
                else
                    BlancAttacks[b] = true;
                if (_chessBoard.Pieces[b].type == typePiece.Rien ||
                    _chessBoard.Pieces[b].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, b));
                }

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[b]);

                if (_chessBoard.Pieces[b].type != typePiece.Rien)
                    break;
            }
        }

        private void ValidateFou(byte fromSquare)
        {
            byte b;

            for (b = (byte)(fromSquare - 17); (b & 0x88) == 0; b -= 17)
            {
                if (_chessBoard.Pieces[fromSquare].color == colorPiece.Noir)
                    NoirAttacks[b] = true;
                else
                    BlancAttacks[b] = true;
                if (_chessBoard.Pieces[b].type == typePiece.Rien ||
                    _chessBoard.Pieces[b].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, b));
                }

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[b]);

                if (_chessBoard.Pieces[b].type != typePiece.Rien)
                    break;
            }
            for (b = (byte)(fromSquare - 15); (b & 0x88) == 0; b -= 15)
            {
                if (_chessBoard.Pieces[fromSquare].color == colorPiece.Noir)
                    NoirAttacks[b] = true;
                else
                    BlancAttacks[b] = true;
                if (_chessBoard.Pieces[b].type == typePiece.Rien ||
                    _chessBoard.Pieces[b].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, b));
                }

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[b]);

                if (_chessBoard.Pieces[b].type != typePiece.Rien)
                    break;
            }
            for (b = (byte)(fromSquare + 15); (b & 0x88) == 0; b += 15)
            {
                if (_chessBoard.Pieces[fromSquare].color == colorPiece.Noir)
                    NoirAttacks[b] = true;
                else
                    BlancAttacks[b] = true;
                if (_chessBoard.Pieces[b].type == typePiece.Rien ||
                    _chessBoard.Pieces[b].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, b));
                }

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[b]);

                if (_chessBoard.Pieces[b].type != typePiece.Rien)
                    break;
            }
            for (b = (byte)(fromSquare + 17); (b & 0x88) == 0; b += 17)
            {
                if (_chessBoard.Pieces[fromSquare].color == colorPiece.Noir)
                    NoirAttacks[b] = true;
                else
                    BlancAttacks[b] = true;
                if (_chessBoard.Pieces[b].type == typePiece.Rien ||
                    _chessBoard.Pieces[b].color != _chessBoard.Pieces[fromSquare].color)
                {
                    AddMove(new Move(fromSquare, b));
                }

                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[b]);

                if (_chessBoard.Pieces[b].type != typePiece.Rien)
                    break;
            }
        }

        private void ValidateNoirPion(byte fromSquare)
        {
            LinkedList<Move> PionMoves = new LinkedList<Move>();

            if (((fromSquare + 17) & 0x88) == 0)
            {
                NoirAttacks[fromSquare + 17] = true;
                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare + 17]);
            }
            if (((fromSquare + 15) & 0x88) == 0)
            {
                NoirAttacks[fromSquare + 15] = true;
                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare + 15]);
            }

            if (_chessBoard.Pieces[fromSquare + 16].type == typePiece.Rien)
                PionMoves.AddLast(new Move(fromSquare, (byte)(fromSquare + 16)));
            if (_chessBoard.Pieces[fromSquare + 16].type == typePiece.Rien &&
                ((fromSquare + 32) & 0x88) == 0 && _chessBoard.Pieces[fromSquare + 32].type == typePiece.Rien &&
                (fromSquare / 16) == 1)
            {
                PionMoves.AddLast(new Move(fromSquare, (byte)(fromSquare + 32)));
            }
            if (((fromSquare + 17) & 0x88) == 0 && (_chessBoard.Pieces[fromSquare + 17].type != typePiece.Rien
                || (_chessBoard.EnPassantSquare == (fromSquare + 17))))
            {
                if (_chessBoard.EnPassantSquare == (fromSquare + 17) ||
                    _chessBoard.Pieces[fromSquare + 17].color == colorPiece.Blanc)
                {
                    PionMoves.AddLast(new Move(fromSquare, (byte)(fromSquare + 17)));
                }
            }
            if (((fromSquare + 15) & 0x88) == 0 && (_chessBoard.Pieces[fromSquare + 15].type != typePiece.Rien
                || (_chessBoard.EnPassantSquare == (fromSquare + 15))))
            {
                if (_chessBoard.EnPassantSquare == (fromSquare + 15) ||
                    _chessBoard.Pieces[fromSquare + 15].color == colorPiece.Blanc)
                {
                    PionMoves.AddLast(new Move(fromSquare, (byte)(fromSquare + 15)));
                }
            }

            if (_chessBoard.NowPlays == colorPiece.Noir)
            {
                foreach (Move move in PionMoves)
                {
                    if ((move.ToSquare / 16) == 7)
                    {
                        AddMove(new Move(move.FromSquare, move.ToSquare, typePiece.Dame));
                        AddMove(new Move(move.FromSquare, move.ToSquare, typePiece.Tour));
                        AddMove(new Move(move.FromSquare, move.ToSquare, typePiece.Fou));
                        AddMove(new Move(move.FromSquare, move.ToSquare, typePiece.Cavalier));
                    }
                    else
                        AddMove(move);
                }
            }
            TotalNoirPionMoves += PionMoves.Count;
        }

        private void ValidateBlancPion(byte fromSquare)
        {
            LinkedList<Move> PionMoves = new LinkedList<Move>();

            if (((fromSquare - 15) & 0x88) == 0)
            {
                BlancAttacks[fromSquare - 15] = true;
                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare - 15]);
            }
            if (((fromSquare - 17) & 0x88) == 0)
            {
                BlancAttacks[fromSquare - 17] = true;
                _chessBoard.Pieces[fromSquare].Step(_chessBoard.Pieces[fromSquare - 17]);
            }

            if (_chessBoard.Pieces[fromSquare - 16].type == typePiece.Rien)
                PionMoves.AddLast(new Move(fromSquare, (byte)(fromSquare - 16)));
            if (_chessBoard.Pieces[fromSquare - 16].type == typePiece.Rien &&
                ((fromSquare - 32) & 0x88) == 0 && _chessBoard.Pieces[fromSquare - 32].type == typePiece.Rien &&
                (fromSquare / 16) == 6)
            {
                PionMoves.AddLast(new Move(fromSquare, (byte)(fromSquare - 32)));
            }
            if (((fromSquare - 15) & 0x88) == 0 && (_chessBoard.Pieces[fromSquare - 15].type != typePiece.Rien
                || (_chessBoard.EnPassantSquare == (fromSquare - 15))))
            {
                if (_chessBoard.EnPassantSquare == (fromSquare - 15) ||
                    _chessBoard.Pieces[fromSquare - 15].color == colorPiece.Noir)
                {
                    PionMoves.AddLast(new Move(fromSquare, (byte)(fromSquare - 15)));
                }
            }
            if (((fromSquare - 17) & 0x88) == 0 && (_chessBoard.Pieces[fromSquare - 17].type != typePiece.Rien
                || (_chessBoard.EnPassantSquare == (fromSquare - 17))))
            {
                if (_chessBoard.EnPassantSquare == (fromSquare - 17) ||
                    _chessBoard.Pieces[fromSquare - 17].color == colorPiece.Noir)
                {
                    PionMoves.AddLast(new Move(fromSquare, (byte)(fromSquare - 17)));
                }
            }

            if (_chessBoard.NowPlays == colorPiece.Blanc)
            {
                foreach (Move move in PionMoves)
                {
                    if (move.ToSquare < 16)
                    {
                        AddMove(new Move(move.FromSquare, move.ToSquare, typePiece.Dame));
                        AddMove(new Move(move.FromSquare, move.ToSquare, typePiece.Tour));
                        AddMove(new Move(move.FromSquare, move.ToSquare, typePiece.Fou));
                        AddMove(new Move(move.FromSquare, move.ToSquare, typePiece.Cavalier));
                    }
                    else
                        AddMove(move);
                }
            }
            TotalBlancPionMoves += PionMoves.Count;
        }

        private void AddMove(Move move)
        {
            Piece fromPiece = _chessBoard.Pieces[move.FromSquare];
            Piece toPiece = _chessBoard.Pieces[move.ToSquare];

            if (_chessBoard.Pieces[move.ToSquare].type != typePiece.Rien)
                move.Score = toPiece.value * 10 + fromPiece.ActionValue;
            if (fromPiece.color == _chessBoard.NowPlays)
                ValidMoves.AddLast(move);
            if (fromPiece.type != typePiece.Pion)
            {
                if (fromPiece.color == colorPiece.Noir)
                    TotalNoirPiecesMoves++;
                else
                    TotalBlancPiecesMoves++;
            }
        }

        public bool IsMoveIn(byte fromSquare, byte toSquare)
        {
            return ValidMoves.FirstOrDefault(x => x.FromSquare == fromSquare && x.ToSquare == toSquare) != null;
        }
    }
}
