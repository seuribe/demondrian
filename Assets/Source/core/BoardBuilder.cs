using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleJSON;

namespace com.perroelectrico.demondrian.core {

    /// <summary>
    /// Builds boards from different types of representations
    /// </summary>
    public class BoardBuilder {

        public Board FromTypeGrid(string str) {
            var grid = StringToTypeGrid(str);
            return FromTypeGrid(grid);
        }

        static T[,] ToGrid<T>(IEnumerable<T> values, int chunkSize) {
            var numChunks = values.Count() / chunkSize;
            var ret = new T[numChunks, chunkSize];
            for (int i = 0 ; i < numChunks ; i++) {
                for (int j = 0 ; j < chunkSize ; j++) {
                    ret[i,j] = values.ElementAt((i * chunkSize) + j);
                }
            }
            return ret;
        }

        public static int[,] StringToTypeGrid(string str) {
            var boardDef = str.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Trim();
            int size = (int)Math.Sqrt(boardDef.Length);
            if (size * size != boardDef.Length)
                Board.ThrowInvalidOp("Invalid board definition string '{0}'", str);

            var typeList = boardDef.Select(
                    c => int.Parse(char.ToString(c)) );

            var grid = ToGrid(typeList, size);

            return grid;
        }

        public Board FromTypeGrid(int[,] typeGrid) {
            Board board = new Board(typeGrid.GetLength(0));
            board.AllCoords.ToList<Coord>().ForEach(
                    coord => {
                        var typeIndex = typeGrid[board.Size - coord.row - 1, coord.col];
                        var piece = new Piece(PieceType.Get(typeIndex), 1);
                        board.Set(coord, piece);
                    }
            );
            new BoardCompactor(board).Compact();
            return board;
        }

        public Board CloneScaled(Board board, int scale) {
            var clone = new Board(board.Size * scale);
            foreach (var p_c in board.Pieces) {
                var coord = p_c.Value;
                var piece = p_c.Key;
                var scaledCoord = new Coord(coord.col * 2, coord.row * 2);
                var scaledPiece = new Piece(piece.type, piece.size * 2);
                clone.Set(scaledCoord, scaledPiece);
            }
            return clone;
        }


        public Board FromJSONPointers(JSONArray pointers, int size) {
            Board board = new Board(size);
            string pattern = @"(\d+)/(\d+)->(\d+):(\d+)";
            for (int i = 0 ; i < pointers.Count ; i++) {
                string ptr = pointers[i];
                MatchCollection mc = Regex.Matches(ptr, pattern);
                if (mc.Count == 1) {
                    var groups = mc[0].Groups;
                    var type = PieceType.Get(int.Parse(groups[1].Value));
                    var pSize = int.Parse(groups[2].Value);
                    var piece = new Piece(type, pSize);
                    var x = int.Parse(groups[3].Value);
                    var y = int.Parse(groups[4].Value);
                    var coord = new Coord(x, y);
                    board.Set(coord, piece);
                }
            }
            return board;
        }

        public Board FromJSONRows(JSONArray rows) {
            int size = rows.Count;
            Board board = new Board(size);
            for (int row = 0 ; row < size ; row++) {
                var pcs = rows[(size - row - 1)].AsArray;
                for (int col = 0 ; col < size ; col++) {
                    var piece = Piece.FromJSON(pcs[col]);
                    // because I travel from 0,0 right and up, it's guaranteed that the first time I find a piece
                    // it will be in it's lower-left coordinate
                    if (!board.ContainsPiece(piece))
                        board.Set(new Coord(col, row), piece);
                }
            }
            return board;
        }
    }
}