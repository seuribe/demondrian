using System;

namespace com.perroelectrico.demondrian.core.test {

    public abstract class TestConstants {
        public static readonly Coord c00 = new Coord { col = 0, row = 0 };
        public static readonly Coord c01 = new Coord { col = 0, row = 1 };
        public static readonly Coord c02 = new Coord { col = 0, row = 2 };
        public static readonly Coord c03 = new Coord { col = 0, row = 3 };
        public static readonly Coord c04 = new Coord { col = 0, row = 4 };

        public static readonly Coord c10 = new Coord { col = 1, row = 0 };
        public static readonly Coord c11 = new Coord { col = 1, row = 1 };
        public static readonly Coord c12 = new Coord { col = 1, row = 2 };
        public static readonly Coord c13 = new Coord { col = 1, row = 3 };
        public static readonly Coord c14 = new Coord { col = 1, row = 4 };

        public static readonly Coord c20 = new Coord { col = 2, row = 0 };
        public static readonly Coord c21 = new Coord { col = 2, row = 1 };
        public static readonly Coord c22 = new Coord { col = 2, row = 2 };
        public static readonly Coord c23 = new Coord { col = 2, row = 3 };
        public static readonly Coord c24 = new Coord { col = 2, row = 4 };

        public static readonly Coord c30 = new Coord { col = 3, row = 0 };
        public static readonly Coord c31 = new Coord { col = 3, row = 1 };
        public static readonly Coord c32 = new Coord { col = 3, row = 2 };
        public static readonly Coord c33 = new Coord { col = 3, row = 3 };
        public static readonly Coord c34 = new Coord { col = 3, row = 4 };

        public static readonly Coord c40 = new Coord { col = 4, row = 0 };
        public static readonly Coord c41 = new Coord { col = 4, row = 1 };
        public static readonly Coord c42 = new Coord { col = 4, row = 2 };
        public static readonly Coord c43 = new Coord { col = 4, row = 3 };
        public static readonly Coord c44 = new Coord { col = 4, row = 4 };

        public static readonly Coord c55 = new Coord { col = 5, row = 5 };
        public static readonly Coord c66 = new Coord { col = 6, row = 6 };

        public static readonly PieceType t0 = new PieceType(0);
        public static readonly PieceType t1 = new PieceType(1);
        public static readonly PieceType t2 = new PieceType(2);
        public static readonly PieceType t3 = new PieceType(3);
        public static readonly PieceType t4 = new PieceType(4);

        public static readonly Piece piece_t0_s1 = new Piece(t0, 1);
        public static readonly Piece piece_t0_s2 = new Piece(t0, 2);
        public static readonly Piece piece_t1_s4 = new Piece(t1, 4);

        public static readonly Piece piece_t1_s2 = new Piece(t1, 2);
        public static readonly Piece piece_t2_s2 = new Piece(t2, 2);
        public static readonly Piece piece_t3_s2 = new Piece(t3, 2);
        public static readonly Piece piece_t4_s2 = new Piece(t4, 2);

        protected void DoTimes(int times, Action<int> func) {
            for (int i = 0 ; i < times ; i++)
                func(i);
        }
    }
}