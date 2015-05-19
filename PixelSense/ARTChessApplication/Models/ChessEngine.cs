using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.SqlServerCe;

namespace ARTChessApplication.Models
{
    internal sealed class ChessEngine
    {
        public delegate void ProgressChangedHandler();
        public event ProgressChangedHandler ProgressChanged;

        private int _depth;
        private bool _isInOpening;
        private SqlCeConnection _connection;

        public ChessEngine(int depth = 6)
        {
            _depth = depth;
            _isInOpening = true;
        }

        public void RemoveHandler()
        {
            ProgressChanged = null;
        }

        public ChessBoard CpuMove(ChessBoard board, MoveValidator validator)
        {
            ChessBoard result;

            if (_isInOpening == false)
                result = AlphaBetaRoot(board, validator, -30000, 30000);
            else
            {
                result = GetOpeningMove(board);
                if (result == null)
                {
                    _isInOpening = false;
                    result = AlphaBetaRoot(board, validator, -30000, 30000);
                }
            }

            return result;
        }

        private ChessBoard GetOpeningMove(ChessBoard board)
        {
            int chosenFromSquare = 0, chosenToSquare = 0, chosenScore = -1;
            int fromSquare, toSquare, score;
            bool positionFound = false;
            Random random = new Random();

            if (_connection == null)
            {
                _connection = new SqlCeConnection("Data Source=books.sdf");
                
                _connection.Open();
            }
            
            string fenposition = board.FenString();
            string commandstring = string.Format("select * from moves where fenposition = \'{0}\' order by score desc", fenposition);
            SqlCeCommand command = new SqlCeCommand(commandstring, _connection);
            SqlCeDataReader reader = command.ExecuteReader();

            while (reader.Read() != false)
            {
                positionFound = true;

                fromSquare = (int)reader["fromSquare"];
                toSquare = (int)reader["toSquare"];
                score = (int)reader["score"];

                score = random.Next(0, score);
                if (score > chosenScore)
                {
                    chosenFromSquare = fromSquare;
                    chosenToSquare = toSquare;
                    chosenScore = score;
                }
            }
            reader.Close();

            if (positionFound == true)
            {
                board = board.Move((byte)chosenFromSquare, (byte)chosenToSquare, typePiece.Dame);
                board.CpuMovedFromSquare = (byte)chosenFromSquare;
                board.CpuMovedToSquare = (byte)chosenToSquare;
                return board;
            }
            _connection.Close();

            return null;
        }

        private ChessBoard NegaScoutRoot(ChessBoard board, MoveValidator validator, double alpha, double beta)
        {
            if (board.IsDraw() == true)
            {
                board.Score = 0;
                return board;
            }

            ChessBoard tempBoard, chosenBoard = null;
            MoveValidator tempValidator;
            double score, a, b;
            int i = 0;

            if (board.NowPlays == colorPiece.Blanc)
            {
                a = alpha;
                b = beta;
            }
            else
            {
                a = -beta;
                b = -alpha;
            }
            var query = from move in validator.ValidMoves
                        orderby move.Score descending
                        select move;
            foreach (Move move in query)
            {
                tempBoard = board.Move(move.FromSquare, move.ToSquare, move.PromotionType);
                tempBoard.CpuMovedFromSquare = (byte)move.FromSquare;
                tempBoard.CpuMovedToSquare = (byte)move.ToSquare;

                tempValidator = new MoveValidator(tempBoard, false);
                tempValidator.Validate();
                if (tempBoard.IsBoardValid() == false)
                {
                    if (ProgressChanged != null)
                        ProgressChanged();
                    continue;
                }
                score = -NegaScout(tempBoard, tempValidator, _depth - 1, -b, -a);
                i++;
                if (ProgressChanged != null)
                    ProgressChanged();
                if (score > a && score < beta && i > 1 && _depth > 1)
                    score = -NegaScout(tempBoard, tempValidator, _depth - 1, -beta, -score);
                if (score > a)
                {
                    a = score;
                    chosenBoard = tempBoard;
                    chosenBoard.Score = a;
                }
                if (a >= beta)
                    break;
                b = a + 1;
            }
            if (chosenBoard == null && board.IsBlancChecked == true)
            {
                board.Score = -10000;
                return board;
            }
            if (chosenBoard == null && board.IsNoirChecked == true)
            {
                board.Score = 10000;
                return board;
            }
            if (chosenBoard == null)
            {
                board.Score = 0;
                return board;
            }

            return chosenBoard;
        }

        private double NegaScout(ChessBoard board, MoveValidator validator, int depth, double alpha, double beta)
        {
            if (board.IsDraw() == true)
                return 0;
            if (depth == 0)
            {
                Evaluator.Evaluate(board, validator);
                return board.Score;
            }

            ChessBoard tempBoard;
            MoveValidator tempValidator;
            double score = 40000, a = alpha, b = beta;
            int i = 0;

            var query = from move in validator.ValidMoves
                        orderby move.Score descending
                        select move;
            foreach (Move move in query)
            {
                tempBoard = board.Move(move.FromSquare, move.ToSquare, move.PromotionType);
                tempBoard.CpuMovedFromSquare = (byte)move.FromSquare;
                tempBoard.CpuMovedToSquare = (byte)move.ToSquare;

                tempValidator = new MoveValidator(tempBoard, false);
                tempValidator.Validate();
                if (tempBoard.IsBoardValid() == false)
                    continue;
                score = -NegaScout(tempBoard, tempValidator, depth - 1, -b, -a);
                i++;
                if (score > a && score < beta && i > 1 && _depth > 1)
                    score = -NegaScout(tempBoard, tempValidator, depth - 1, -beta, -score);
                if (score > a)
                    a = score;
                if (a >= beta)
                    break;
                b = a + 1;
            }
            if (score == 40000 && board.IsBlancChecked == true)
                return -10000 - depth;
            if (score == 40000 && board.IsNoirChecked == true)
                return 10000 + depth;
            if (score == 40000)
                return 0;

            return a;
        }

        private ChessBoard AlphaBetaRoot(ChessBoard board, MoveValidator validator, double alpha, double beta)
        {
            if (board.IsDraw() == true)
            {
                board.Score = 0;
                return board;
            }

            ChessBoard tempBoard, chosenBoard = null;
            MoveValidator tempValidator;
            double merit = board.NowPlays == colorPiece.Blanc ? -30000 : 30000;
            double score;

            var query = from move in validator.ValidMoves
                        orderby move.Score descending
                        select move;
            if (board.NowPlays == colorPiece.Blanc)
            {
                foreach (Move move in query)
                {
                    tempBoard = board.Move(move.FromSquare, move.ToSquare, move.PromotionType);
                    tempBoard.CpuMovedFromSquare = (byte)move.FromSquare;
                    tempBoard.CpuMovedToSquare = (byte)move.ToSquare;

                    tempValidator = new MoveValidator(tempBoard, false);
                    tempValidator.Validate();
                    if (tempBoard.IsBoardValid() == false)
                    {
                        if (ProgressChanged != null)
                            ProgressChanged();
                        continue;
                    }
                    score = AlphaBeta(tempBoard, tempValidator, _depth - 1, alpha, beta, board.NoirPieces != tempBoard.NoirPieces);
                    if (ProgressChanged != null)
                        ProgressChanged();
                    if (score > merit)
                    {
                        merit = score;
                        chosenBoard = tempBoard;
                        chosenBoard.Score = score;
                    }
                    alpha = Math.Max(alpha, merit);
                    if (merit >= beta)
                        break;
                }
            }
            else
            {
                foreach (Move move in query)
                {
                    tempBoard = board.Move(move.FromSquare, move.ToSquare, move.PromotionType);
                    tempBoard.CpuMovedFromSquare = (byte)move.FromSquare;
                    tempBoard.CpuMovedToSquare = (byte)move.ToSquare;

                    tempValidator = new MoveValidator(tempBoard, false);
                    tempValidator.Validate();
                    if (tempBoard.IsBoardValid() == false)
                    {
                        if (ProgressChanged != null)
                            ProgressChanged();
                        continue;
                    }
                    score = AlphaBeta(tempBoard, tempValidator, _depth - 1, alpha, beta,
                        board.BlancPieces != tempBoard.BlancPieces);
                    if (ProgressChanged != null)
                        ProgressChanged();
                    if (score < merit)
                    {
                        merit = score;
                        chosenBoard = tempBoard;
                        chosenBoard.Score = score;
                    }
                    beta = Math.Min(beta, merit);
                    if (merit <= alpha)
                        break;
                }
            }
            if (chosenBoard == null && board.IsBlancChecked == true)
            {
                board.Score = -10000;
                return board;
            }
            if (chosenBoard == null && board.IsNoirChecked == true)
            {
                board.Score = 10000;
                return board;
            }
            if (chosenBoard == null)
            {
                board.Score = 0;
                return board;
            }

            return chosenBoard;
        }

        private double AlphaBeta(ChessBoard board, MoveValidator validator, int depth, double alpha, double beta, bool isCapture)
        {
            if (board.IsDraw() == true)
                return 0;
            if (depth == 0)
            {
                Evaluator.Evaluate(board, validator);
                return board.Score;
            }

            ChessBoard tempBoard;
            MoveValidator tempValidator;
            double merit = board.NowPlays == colorPiece.Blanc ? -30000 : 30000;
            double score = 40000;

            var query = from move in validator.ValidMoves
                        orderby move.Score descending
                        select move;
            if (board.NowPlays == colorPiece.Blanc)
            {
                foreach (Move move in query)
                {
                    tempBoard = board.Move(move.FromSquare, move.ToSquare, move.PromotionType);
                    tempBoard.CpuMovedFromSquare = (byte)move.FromSquare;
                    tempBoard.CpuMovedToSquare = (byte)move.ToSquare;

                    tempValidator = new MoveValidator(tempBoard, false);
                    tempValidator.Validate();
                    if (tempBoard.IsBoardValid() == false)
                        continue;
                    score = AlphaBeta(tempBoard, tempValidator, depth - 1, alpha, beta, board.NoirPieces != tempBoard.NoirPieces);
                    if (score > merit)
                        merit = score;
                    alpha = Math.Max(alpha, merit);
                    if (merit >= beta)
                        break;
                }
            }
            else
            {
                foreach (Move move in query)
                {
                    tempBoard = board.Move(move.FromSquare, move.ToSquare, move.PromotionType);
                    tempBoard.CpuMovedFromSquare = (byte)move.FromSquare;
                    tempBoard.CpuMovedToSquare = (byte)move.ToSquare;

                    tempValidator = new MoveValidator(tempBoard, false);
                    tempValidator.Validate();
                    if (tempBoard.IsBoardValid() == false)
                        continue;
                    score = AlphaBeta(tempBoard, tempValidator, depth - 1, alpha, beta,
                        board.BlancPieces != tempBoard.BlancPieces);
                    if (score < merit)
                        merit = score;
                    beta = Math.Min(beta, merit);
                    if (merit <= alpha)
                        break;
                }
            }
            if (score == 40000 && board.IsBlancChecked == true)
                return -10000 - depth;
            if (score == 40000 && board.IsNoirChecked == true)
                return 10000 + depth;
            if (score == 40000)
                return 0;

            return merit;
        }
    }
}
