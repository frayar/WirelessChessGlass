using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARTChessApplication.Models
{
    internal class Evaluator
    {
        public static void Evaluate(ChessBoard board, MoveValidator validator)
        {
            double score = 0;

            if (board.Phase == GamePhase.Opening)
            {
                score += MaterialBalance(board, validator);
                score += CenterPions(board, validator);
                score += Mobility(board, validator);
                score += LostCastle(board, validator);
                score += AttackedCastle(board, validator);
                score += RoiPosition(board, validator);
                score += CastleShattering(board, validator);
                score += CastleShelter(board, validator);
            }
            else
            {
                score += MaterialBalance(board, validator);
                score += Mobility(board, validator);
                score += RoiEndGamePosition(board, validator);
            }
            if (board.IsBlancChecked == true)
                score -= 8;
            else if (board.IsNoirChecked == true)
                score += 8;

            board.Score = score;
        }

        private static double RoiEndGamePosition(ChessBoard board, MoveValidator validator)
        {
            double score = 0;

            if (board.BlancRoiSquare >= (5 * 16 + 2) && board.BlancRoiSquare <= (5 * 16 + 5))
                score += 2.5;
            else if (board.BlancRoiSquare >= (4 * 16 + 2) && board.BlancRoiSquare <= (4 * 16 + 5))
                score += 3.5;
            else if ((board.BlancRoiSquare / 16) == 3)
                score += 5;
            else if ((board.BlancRoiSquare / 16) == 2)
                score += 7.5;

            if (board.NoirRoiSquare >= (2 * 16 + 2) && board.NoirRoiSquare <= (2 * 16 + 5))
                score -= 2.5;
            else if (board.NoirRoiSquare >= (3 * 16 + 2) && board.NoirRoiSquare <= (3 * 16 + 5))
                score -= 3.5;
            else if ((board.NoirRoiSquare / 16) == 4)
                score -= 5;
            else if ((board.NoirRoiSquare / 16) == 5)
                score -= 7.5;

            return score;
        }

        private static double CastleShelter(ChessBoard board, MoveValidator validator)
        {
            double score = 0;
            byte i;
            bool[] BlancPionFound = { false, false, false };
            bool[] NoirPionFound = { false, false, false };

            if (board.BlancRoiSquare == (7 * 16 + 6) || board.BlancRoiSquare == (7 * 16 + 7))
            {
                for (i = 6 * 16; i >= 5 * 16; i -= 16)
                {
                    if (board.Pieces[i + 5].type == typePiece.Pion && board.Pieces[i + 5].color == colorPiece.Blanc)
                        BlancPionFound[0] = true;
                    if (board.Pieces[i + 6].type == typePiece.Pion && board.Pieces[i + 6].color == colorPiece.Blanc)
                        BlancPionFound[1] = true;
                    if (board.Pieces[i + 7].type == typePiece.Pion && board.Pieces[i + 7].color == colorPiece.Blanc)
                        BlancPionFound[2] = true;
                }
                foreach (bool found in BlancPionFound)
                    if (found == false)
                        score -= 4;
            }
            else if (board.BlancRoiSquare == (7 * 16) || board.BlancRoiSquare == (7 * 16 + 1) ||
                board.BlancRoiSquare == (7 * 16 + 2))
            {
                for (i = 6 * 16; i >= 5 * 16; i -= 16)
                {
                    if (board.Pieces[i].type == typePiece.Pion && board.Pieces[i].color == colorPiece.Blanc)
                        BlancPionFound[0] = true;
                    if (board.Pieces[i + 1].type == typePiece.Pion && board.Pieces[i + 1].color == colorPiece.Blanc)
                        BlancPionFound[1] = true;
                    if (board.Pieces[i + 2].type == typePiece.Pion && board.Pieces[i + 2].color == colorPiece.Blanc)
                        BlancPionFound[2] = true;
                }
                foreach (bool found in BlancPionFound)
                    if (found == false)
                        score -= 4;
            }
            if (board.NoirRoiSquare == 6 || board.NoirRoiSquare == 7)
            {
                for (i = 16; i <= 32; i += 16)
                {
                    if (board.Pieces[i + 5].type == typePiece.Pion && board.Pieces[i + 5].color == colorPiece.Noir)
                        NoirPionFound[0] = true;
                    if (board.Pieces[i + 6].type == typePiece.Pion && board.Pieces[i + 6].color == colorPiece.Noir)
                        NoirPionFound[1] = true;
                    if (board.Pieces[i + 7].type == typePiece.Pion && board.Pieces[i + 7].color == colorPiece.Noir)
                        NoirPionFound[2] = true;
                }
                foreach (bool found in NoirPionFound)
                    if (found == false)
                        score += 4;
            }
            else if (board.NoirRoiSquare == 0 || board.NoirRoiSquare == 1 && board.NoirRoiSquare == 2)
            {
                for (i = 16; i <= 32; i += 16)
                {
                    if (board.Pieces[i].type == typePiece.Pion && board.Pieces[i].color == colorPiece.Noir)
                        NoirPionFound[0] = true;
                    if (board.Pieces[i + 1].type == typePiece.Pion && board.Pieces[i + 1].color == colorPiece.Noir)
                        NoirPionFound[1] = true;
                    if (board.Pieces[i + 2].type == typePiece.Pion && board.Pieces[i + 2].color == colorPiece.Noir)
                        NoirPionFound[2] = true;
                }
                foreach (bool found in NoirPionFound)
                    if (found == false)
                        score += 4;
            }

            return score;
        }

        private static double CastleShattering(ChessBoard board, MoveValidator validator)
        {
            double score = 0;
            byte i;
            bool[] BlancPionFound = { false, false, false };
            bool[] NoirPionFound = { false, false, false };

            if (board.BlancRoiSquare == (7 * 16 + 6) || board.BlancRoiSquare == (7 * 16 + 7))
            {
                for (i = 6 * 16; (i & 0x88) == 0; i -= 16)
                {
                    if (board.Pieces[i + 5].type == typePiece.Pion && board.Pieces[i + 5].color == colorPiece.Blanc)
                        BlancPionFound[0] = true;
                    if (board.Pieces[i + 6].type == typePiece.Pion && board.Pieces[i + 6].color == colorPiece.Blanc)
                        BlancPionFound[1] = true;
                    if (board.Pieces[i + 7].type == typePiece.Pion && board.Pieces[i + 7].color == colorPiece.Blanc)
                        BlancPionFound[2] = true;
                }
                foreach (bool found in BlancPionFound)
                    if (found == false)
                        score -= 6;
            }
            else if (board.BlancRoiSquare == (7 * 16) || board.BlancRoiSquare == (7 * 16 + 1)
                || board.BlancRoiSquare == (7 * 16 + 2))
            {
                for (i = 6 * 16; (i & 0x88) == 0; i -= 16)
                {
                    if (board.Pieces[i].type == typePiece.Pion && board.Pieces[i].color == colorPiece.Blanc)
                        BlancPionFound[0] = true;
                    if (board.Pieces[i + 1].type == typePiece.Pion && board.Pieces[i + 1].color == colorPiece.Blanc)
                        BlancPionFound[1] = true;
                    if (board.Pieces[i + 2].type == typePiece.Pion && board.Pieces[i + 2].color == colorPiece.Blanc)
                        BlancPionFound[2] = true;
                }
                foreach (bool found in BlancPionFound)
                    if (found == false)
                        score -= 6;
            }
            if (board.NoirRoiSquare == 6 || board.NoirRoiSquare == 7)
            {
                for (i = 16; (i & 0x88) == 0; i += 16)
                {
                    if (board.Pieces[i + 5].type == typePiece.Pion && board.Pieces[i + 5].color == colorPiece.Noir)
                        NoirPionFound[0] = true;
                    if (board.Pieces[i + 6].type == typePiece.Pion && board.Pieces[i + 6].color == colorPiece.Noir)
                        NoirPionFound[1] = true;
                    if (board.Pieces[i + 7].type == typePiece.Pion && board.Pieces[i + 7].color == colorPiece.Noir)
                        NoirPionFound[2] = true;
                }
                foreach (bool found in NoirPionFound)
                    if (found == false)
                        score += 6;
            }
            else if (board.NoirRoiSquare == 0 || board.NoirRoiSquare == 1 && board.NoirRoiSquare == 2)
            {
                for (i = 16; (i & 0x88) == 0; i += 16)
                {
                    if (board.Pieces[i].type == typePiece.Pion && board.Pieces[i].color == colorPiece.Noir)
                        NoirPionFound[0] = true;
                    if (board.Pieces[i + 1].type == typePiece.Pion && board.Pieces[i + 1].color == colorPiece.Noir)
                        NoirPionFound[1] = true;
                    if (board.Pieces[i + 2].type == typePiece.Pion && board.Pieces[i + 2].color == colorPiece.Noir)
                        NoirPionFound[2] = true;
                }
                foreach (bool found in NoirPionFound)
                    if (found == false)
                        score += 6;
            }

            return score;
        }

        private static double RoiPosition(ChessBoard board, MoveValidator validator)
        {
            double score = 0;

            if (board.BlancRoiSquare == (7 * 16) || board.BlancRoiSquare == (7 * 16 + 1)
                || board.BlancRoiSquare == (7 * 16 + 6) || board.BlancRoiSquare == (7 * 16 + 7))
            {
                score += 4;
            }
            else if (board.BlancRoiSquare == (7 * 16 + 2) || board.BlancRoiSquare == (7 * 16 + 5))
            {
                score -= 2;
            }
            else if (board.BlancRoiSquare == (7 * 16 + 3) || board.BlancRoiSquare == (7 * 16 + 4))
            {
                score -= 1;
            }
            else if ((board.BlancRoiSquare / 16) == 6)
                score -= 4;
            else if ((board.BlancRoiSquare / 16) == 5)
                score -= 5;
            else
                score -= 7;

            if (board.NoirRoiSquare == 0 || board.NoirRoiSquare == 1 || board.NoirRoiSquare == 6
                || board.NoirRoiSquare == 7)
            {
                score -= 4;
            }
            else if (board.NoirRoiSquare == 2 || board.NoirRoiSquare == 5)
            {
                score += 2;
            }
            else if (board.NoirRoiSquare == 3 || board.NoirRoiSquare == 4)
            {
                score += 1;
            }
            else if ((board.NoirRoiSquare / 16) == 1)
                score += 1;
            else if ((board.NoirRoiSquare / 16) == 2)
                score += 3;
            else
                score += 5;

            return score;
        }

        private static double AttackedCastle(ChessBoard board, MoveValidator validator)
        {
            double score = 0;

            if (validator.BlancCastlesAttacked == true)
                score -= 5;
            if (validator.NoirCastlesAttacked == true)
                score += 5;

            return score;
        }

        private static double LostCastle(ChessBoard board, MoveValidator validator)
        {
            double score = 0;

            if (board.CanBlancCastleLeft == false && board.CanBlancCastleRight == false
                && board.BlancCastled == false)
            {
                score -= 5;
            }
            if (board.CanNoirCastleLeft == false && board.CanNoirCastleRight == false
                && board.NoirCastled == false)
            {
                score += 5;
            }

            return score;
        }

        private static double Mobility(ChessBoard board, MoveValidator validator)
        {
            return ((validator.TotalBlancPionMoves - validator.TotalNoirPionMoves) * 0.1
                + (validator.TotalBlancPiecesMoves - validator.TotalNoirPiecesMoves) * 0.2);
        }

        private static double CenterPions(ChessBoard board, MoveValidator validator)
        {
            byte i, j;
            double score = 0;
            Piece tempPiece;

            for (i = 3 * 16; i <= 4 * 16; i += 16)
                for (j = (byte)(3 + i); j <= 4 + i; j++)
                {
                    tempPiece = board.Pieces[j];

                    if (tempPiece.type == typePiece.Pion && tempPiece.color == colorPiece.Blanc
                        && tempPiece.IsWellDefended() == true)
                    {
                        score += 4;
                    }
                    else if (tempPiece.type == typePiece.Pion && tempPiece.color == colorPiece.Noir
                        && tempPiece.IsWellDefended() == true)
                    {
                        score -= 4;
                    }
                    else if (tempPiece.type != typePiece.Rien && tempPiece.color == colorPiece.Blanc
                        && tempPiece.IsWellDefended() == true)
                    {
                        score += 2;
                    }
                    else if (tempPiece.type != typePiece.Rien && tempPiece.color == colorPiece.Noir
                        && tempPiece.IsWellDefended() == true)
                    {
                        score -= 2;
                    }
                    else
                    {
                        if (tempPiece.type == typePiece.Rien && validator.BlancAttacks[j] == true
                            && tempPiece.IsWellDefended() == false)
                            score += 1;
                        if (tempPiece.type == typePiece.Rien && validator.NoirAttacks[j] == true
                            && tempPiece.IsWellDefended() == true)
                            score -= 1;
                    }
                }

            return score;
        }

        private static double MaterialBalance(ChessBoard board, MoveValidator validator)
        {
            byte i, j, line = 0, b;
            double score = 0;
            int BlancMinorPiecesDeveloped = 4, NoirMinorPiecesDeveloped = 4;
            bool BlancDameAdvanced = false, NoirDameAdvanced = false;
            int BlancToursAdvanced = 0, NoirToursAdvanced = 0;
            Piece tempPiece;

            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    b = (byte)(line + j);
                    tempPiece = board.Pieces[b];
                    if (tempPiece.type == typePiece.Rien || tempPiece.type == typePiece.Roi)
                        continue;
                    //if (tempPiece.IsWellDefended() == false)
                    //{
                    //    if (tempPiece.color == colorPiece.Noir)
                    //        score += tempPiece.value;
                    //    else
                    //        score -= tempPiece.value;
                    //}
                    if (tempPiece.color == colorPiece.Noir)
                        score -= tempPiece.value;
                    else
                        score += tempPiece.value;

                    if (board.Phase == GamePhase.Opening)
                    {
                        #region Early Development

                        if (tempPiece.type == typePiece.Cavalier && tempPiece.color == colorPiece.Noir &&
                            (b == 1 || b == 6))
                        {
                            NoirMinorPiecesDeveloped--;
                        }
                        else if (tempPiece.type == typePiece.Cavalier && tempPiece.color == colorPiece.Blanc &&
                            (b == (7 * 16 + 1) || j == (7 * 16 + 6)))
                        {
                            BlancMinorPiecesDeveloped--;
                        }
                        else if (tempPiece.type == typePiece.Fou && tempPiece.color == colorPiece.Noir &&
                            (b == 2 || b == 5))
                        {
                            NoirMinorPiecesDeveloped--;
                        }
                        else if (tempPiece.type == typePiece.Fou && tempPiece.color == colorPiece.Blanc &&
                            (b == (7 * 16 + 2) || b == (7 * 16 + 5)))
                        {
                            BlancMinorPiecesDeveloped--;
                        }
                        else if (tempPiece.type == typePiece.Dame && tempPiece.color == colorPiece.Noir &&
                            b != 3)
                        {
                            NoirDameAdvanced = true;
                        }
                        else if (tempPiece.type == typePiece.Dame && tempPiece.color == colorPiece.Blanc &&
                            b != (7 * 16 + 3))
                        {
                            BlancDameAdvanced = true;
                        }
                        else if (tempPiece.type == typePiece.Tour && tempPiece.color == colorPiece.Noir &&
                            (b != 0 && b != 7))
                        {
                            NoirToursAdvanced++;
                        }
                        else if (tempPiece.type == typePiece.Tour && tempPiece.color == colorPiece.Blanc &&
                            (b != (7 * 16) && b != (7 * 16 + 7)))
                        {
                            BlancToursAdvanced++;
                        }

                        #endregion
                        if (tempPiece.type == typePiece.Pion)
                        {
                            score += IsolatedPion(b, tempPiece, board, validator);
                            score += DoublePion(b, tempPiece, board, validator);
                            //    score += PassedPion(b, tempPiece, board, validator);
                        }
                    }
                    else
                    {
                        if (tempPiece.type == typePiece.Pion)
                        {
                            if (tempPiece.color == colorPiece.Noir && i == 5)
                                score -= 1;
                            else if (tempPiece.color == colorPiece.Blanc && i == 2)
                                score += 1;

                            score += IsolatedPion(b, tempPiece, board, validator);
                            score += DoublePion(b, tempPiece, board, validator);
                            score += PassedPion(b, tempPiece, board, validator);
                        }
                    }
                }
                line += 16;
            }
            if (BlancMinorPiecesDeveloped - NoirMinorPiecesDeveloped >= 2)
                score += 1.75;
            else if (NoirMinorPiecesDeveloped - BlancMinorPiecesDeveloped >= 2)
                score -= 1.75;

            if (BlancDameAdvanced == true && BlancMinorPiecesDeveloped < 2)
                score -= 3;
            if (NoirDameAdvanced == true && NoirMinorPiecesDeveloped < 2)
                score += 3;
            if (BlancToursAdvanced > 0 && BlancMinorPiecesDeveloped < 2)
                score -= (BlancToursAdvanced * 5);
            if (NoirToursAdvanced > 0 && NoirMinorPiecesDeveloped < 2)
                score += (NoirToursAdvanced * 5);

            return score;
        }

        private static double PassedPion(byte square, Piece tempPiece, ChessBoard board, MoveValidator validator)
        {
            double score = 0;
            byte i;
            double boost = 0;

            if (board.Phase == GamePhase.End)
                boost = 2.5;

            if (((square - 1) & 0x88) == 0 && board.Pieces[square - 1].IsPassedPion == true && board.Pieces[square - 1].color == tempPiece.color)
                score = 5;
            if (tempPiece.color == colorPiece.Noir)
                score *= -1;

            tempPiece.IsPassedPion = true;
            if (tempPiece.color == colorPiece.Noir)
            {
                for (i = (byte)(square + 16); (i & 0x88) == 0; i += 16)
                    if (board.Pieces[i].type == typePiece.Pion)
                    {
                        tempPiece.IsPassedPion = false;
                        break;
                    }
            }
            if (tempPiece.color == colorPiece.Blanc)
            {
                for (i = (byte)(square - 16); (i & 0x88) == 0; i -= 16)
                    if (board.Pieces[i].type == typePiece.Pion)
                    {
                        tempPiece.IsPassedPion = false;
                        break;
                    }
            }

            if (tempPiece.color == colorPiece.Noir && tempPiece.IsPassedPion == true)
            {
                if (((square - 17) & 0x88) == 0 && board.Pieces[square - 17].type == typePiece.Pion
                    && board.Pieces[square - 17].color == colorPiece.Noir)
                {
                    return score - (boost + 5);
                }
                if (((square - 15) & 0x88) == 0 && board.Pieces[square - 15].type == typePiece.Pion
                    && board.Pieces[square - 15].color == colorPiece.Noir)
                {
                    return score - (boost + 5);
                }

                return score - (boost + 2.5);
            }
            else if (tempPiece.color == colorPiece.Blanc && tempPiece.IsPassedPion == true)
            {
                tempPiece.IsPassedPion = true;

                if (((square + 15) & 0x88) == 0 && board.Pieces[square + 15].type == typePiece.Pion
                    && board.Pieces[square + 15].color == colorPiece.Blanc)
                {
                    return score + (boost + 5);
                }
                if (((square + 17) & 0x88) == 0 && board.Pieces[square + 17].type == typePiece.Pion
                    && board.Pieces[square + 17].color == colorPiece.Blanc)
                {
                    return score + (boost + 5);
                }

                return score + (boost + 2.5);
            }

            return 0;
        }

        private static double DoublePion(byte square, Piece tempPiece, ChessBoard board, MoveValidator validator)
        {
            if (tempPiece.color == colorPiece.Blanc && board.Pieces[square + 16].type == typePiece.Pion
                && board.Pieces[square + 16].color == colorPiece.Blanc)
                return -0.5;
            if (tempPiece.color == colorPiece.Noir && board.Pieces[square - 16].type == typePiece.Pion
                && board.Pieces[square - 16].color == colorPiece.Noir)
                return 0.5;

            return 0;
        }

        private static double IsolatedPion(byte square, Piece tempPiece, ChessBoard board, MoveValidator validator)
        {
            int i, j;
            byte b;
            bool isIsolated = true;
            int boost = 1;

            if (board.Phase == GamePhase.End)
                boost = 2;

            for (i = -16; i <= 16; i += 16)
            {
                for (j = i - 1; j <= i + 1; j++)
                {
                    b = (byte)(square + j);
                    if (b == square || (b & 0x88) != 0)
                        continue;
                    if (board.Pieces[b].type == typePiece.Pion && board.Pieces[b].color == tempPiece.color)
                    {
                        isIsolated = false;
                        break;
                    }
                }
                if (isIsolated == false)
                    break;
            }
            if (tempPiece.color == colorPiece.Noir && isIsolated == true)
                return boost * 2;
            if (tempPiece.color == colorPiece.Blanc && isIsolated == true)
                return -2 * boost;

            return 0;
        }
    }
}
