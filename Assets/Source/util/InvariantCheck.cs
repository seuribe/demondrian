using System;
using com.perroelectrico.demondrian.core;

namespace com.perroelectrico.demondrian.util {

    public abstract class ObjectCheck<T> : IDisposable {
        readonly protected T obj;
        public ObjectCheck(T obj) {
            this.obj = obj;
        }

        protected abstract string Message();
        protected abstract bool DoCheck();
        public void Check() {
            if (!DoCheck())
                throw new InvalidBoardStateException(Message());
        }

        public void Dispose() {
            Check();
        }
    }

    public class BoardPiecesCountCheck : ObjectCheck<Board> {
        readonly int expected;

        public BoardPiecesCountCheck(Board board, int change) : base(board) {
            this.expected = board.Pieces.Count + change;
        }

        protected override string Message() {
            return string.Format("Number of pieces expected: {0}, current: {1}", expected, obj.Pieces.Count);
        }

        protected override bool DoCheck() {
            return expected == obj.Pieces.Count;
        }
    }


    public class GameBoardUnchanged : ObjectCheck<Game> {

        private readonly Board oldBoard;
        public GameBoardUnchanged(Game game) : base(game) {
            this.oldBoard = game.Board.Clone();
        }

        protected override string Message() {
            return string.Format("Game Board has changed");
        }

        protected override bool DoCheck() {
            return obj.Board.Matches(oldBoard);
        }
    }
}