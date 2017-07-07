using System.Collections.Generic;

namespace com.perroelectrico.demondrian.core {

    /// <summary>
    /// Simulates the effect of a move on the board, including all it's consequences
    /// </summary>
    public class MoveSimulator {
        private readonly Board board;
        private readonly PlayLogic playLogic;
        private readonly Piece nextPiece;

        public readonly Piece oldPiece;
        public readonly Coord moveCoord;

        private ICollection<BoardAction> actions = new List<BoardAction>();
        private ICollection<Piece> obstacles = new List<Piece>();

        public bool Simulated {
            get { return obstacles.Count > 0 || actions.Count > 0; }
        }

        MoveSimulator(Board board, Coord coord, Piece nextPiece) {
            this.board = board;
            this.moveCoord = coord;
            this.oldPiece = board.Get(coord);
            this.playLogic = new PlayLogic(board);
            this.nextPiece = nextPiece;
        }

        public static MoveSimulator FromPotentialMove(Board board, PotentialMove move, Piece nextPiece) {
            var newBoard = board.Clone();
            newBoard.RotateTo(move.orientation);
            return new MoveSimulator(newBoard, move.coord, nextPiece);
        }

        public static MoveSimulator FromCoord(Board board, Coord coord, Piece nextPiece) {
            return new MoveSimulator(board.Clone(), coord, nextPiece);
        }

#region core logic
        public void Simulate() {
            if (Simulated)
                return;

            CheckForObstacles();
            if (!HasObstacles())
                GenerateActions();
        }

        void CheckForObstacles() {
            obstacles = playLogic.GetAllObstacles(moveCoord);
        }

        bool HasObstacles() {
            return obstacles.Count > 0;
        }

        public ICollection<Piece> Obstacles() {
            if (!Simulated)
                throw new InvalidGameOperationException("Move has not been simulated yet");

            return obstacles;
        }

        public Move GetMove() {
            if (!IsMovePossible())
                throw new InvalidGameOperationException("Move is not possible");

            return new Move(actions);
        }

        public bool IsMovePossible() {
            if (!Simulated)
                throw new InvalidGameOperationException("Move has not been simulated yet");

            return !HasObstacles();
        }
#endregion

#region actions
        void GenerateActions() {
            actions = new List<BoardAction>();
            GenerateOrientationAction();
            var droppables = playLogic.GetDroppablesAbove(oldPiece);
            GenerateRemoveAction();
            GenerateFallActions(droppables);
            GenerateNewPieceAction();
            GenerateCompactActions();
            // Add again for orientating correctly the board when undoing
            GenerateOrientationAction();
        }

        void AddAndExecute(BoardAction action) {
            actions.Add(action);
            action.Do(board);
        }

        void GenerateOrientationAction() {
            AddAndExecute(OrientateAction.Get(board.Orientation));
        }

        void GenerateRemoveAction() {
            AddAndExecute(new RemoveAction(oldPiece, moveCoord));
        }

        void GenerateFallActions(ICollection<Piece> droppables) {
            foreach (var dropPiece in droppables) {
                var dropCoord = board.Where(dropPiece);
                AddAndExecute(new FallAction(dropPiece, dropCoord, oldPiece.size));
            }
        }

        void GenerateNewPieceAction() {
            AddAndExecute(new NewPieceAction {
                coord = new Coord { col = moveCoord.col, row = board.Size - oldPiece.size },
                newPiece = nextPiece
            });
        }

        void GenerateCompactActions() {
            var compactor = new BoardCompactor(board);
            Coord compactableCoord;
            while ((compactableCoord = compactor.NextCompactable()).Valid)
                GenerateCompactAction(compactableCoord);
        }

        void GenerateCompactAction(Coord coord) {
            AddAndExecute(new CompactAction(
                coord,
                board.Get(coord).DoubleSize(),
                board.PiecesSquare(coord)
            ));
        }
#endregion
    }
}
