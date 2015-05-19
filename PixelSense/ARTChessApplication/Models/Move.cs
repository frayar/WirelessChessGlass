using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARTChessApplication.Models
{
    class Move
    {
        public byte FromSquare;
        public byte ToSquare;
        public typePiece PromotionType;
        public double Score;

        public Move(byte fromSquare, byte toSquare, typePiece promotionType = typePiece.Rien)
        {
            FromSquare = fromSquare;
            ToSquare = toSquare;
            PromotionType = promotionType;
            Score = 0;
        }

    }
}
