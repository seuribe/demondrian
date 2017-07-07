using SimpleJSON;

namespace com.perroelectrico.demondrian.core {

    /// <summary>
    /// An action that affects the pieces on a board
    /// </summary>
    public abstract class BoardAction {
        public abstract void Undo(Board board);
        public abstract void Do(Board board);

        public abstract JSONClass ToJSON();
        public static BoardAction FromJSON(JSONNode json) {
            switch (json["type"].ToString()) {
                case "rotate":
                    return RotateAction.FromJSON(json);
                case "compact":
                    return CompactAction.FromJSON(json);
                case "remove":
                    return RemoveAction.FromJSON(json);
                case "fall":
                    return FallAction.FromJSON(json);
                case "new":
                    return NewPieceAction.FromJSON(json);
            }
            return null;
        }

        public override string ToString() {
            return "Action: " + ToJSON().ToString();
        }
    }

    public class OrientateAction : BoardAction {
        public readonly Orientation orientation;

        public static OrientateAction OrientateTop = new OrientateAction(Orientation.Top);
        public static OrientateAction OrientateBottom = new OrientateAction(Orientation.Bottom);
        public static OrientateAction OrientateLeft = new OrientateAction(Orientation.Left);
        public static OrientateAction OrientateRight = new OrientateAction(Orientation.Right);

        public static OrientateAction Get(Orientation o) {
            switch (o) {
                case Orientation.Top: return OrientateTop;
                case Orientation.Bottom: return OrientateBottom;
                case Orientation.Left: return OrientateLeft;
                case Orientation.Right: return OrientateRight;
            }
            return null;
        }

        private OrientateAction(Orientation orientation) {
            this.orientation = orientation;
        }

        public override void Do(Board board) {
            board.RotateTo(orientation);
        }

        public override void Undo(Board board) {
            board.RotateTo(orientation);
        }

        public override JSONClass ToJSON() {
            var json = new JSONClass();
            json["type"] = "orientate";
            json["o"] = (int)orientation;
            return json;
        }

        public static OrientateAction FromJSON(JSONClass json) {
            return Get((Orientation)json["o"].AsInt);
        }

        public override int GetHashCode() {
            return orientation.GetHashCode();
        }

        public override bool Equals(object obj) {
            return obj != null && (obj is OrientateAction) &&
                (obj as OrientateAction).orientation == orientation;
        }
    }

    public class RotateAction : BoardAction {
        public RotationDir dir;

        public static RotateAction Left = new RotateAction { dir = RotationDir.Left };
        public static RotateAction Right = new RotateAction { dir = RotationDir.Right };

        private static RotateAction[][] RotateActions = new RotateAction[][]{
            new RotateAction[]{},
            new RotateAction[]{Right},
            new RotateAction[]{Right, Right},
            new RotateAction[]{Left}
        };

        public override void Do(Board board) {
            if (dir == RotationDir.Left)
                board.RotateLeft();
            else
                board.RotateRight();
        }

        public override void Undo(Board board) {
            if (dir == RotationDir.Right)
                board.RotateLeft();
            else
                board.RotateRight();
        }

        public override JSONClass ToJSON() {
            var json = new JSONClass();
            json["type"] = "rotate";
            json["right"] = (dir == RotationDir.Right);
            return json;
        }

        public static RotateAction FromJSON(JSONClass json) {
            return new RotateAction {
                dir = json["right"].AsBool ? RotationDir.Right : RotationDir.Left
            };
        }

        public static RotateAction[] GenerateActions(Orientation from, Orientation to) {
            return RotateActions[((int)to - (int)from + 4) % 4];
        }
    }

    public class CompactAction : BoardAction {
        public Coord coord;
        public Piece bigPiece;
        public PiecesSquare removed;

        public CompactAction(Coord coord, Piece bigPiece, PiecesSquare removed) {
            this.coord = coord;
            this.bigPiece = bigPiece;
            this.removed = removed;
        }

        public override void Do(Board board) {
            int smallSize = bigPiece.size / 2;

            board.Remove(coord);
            board.Remove(coord.Move(smallSize, 0));
            board.Remove(coord.Move(0, smallSize));
            board.Remove(coord.Move(smallSize, smallSize));
            board.Set(coord, bigPiece);
        }

        public override void Undo(Board board) {
            int smallSize = bigPiece.size / 2;

            board.Remove(bigPiece);
            board.Set(coord, removed.p00);
            board.Set(coord.Move(smallSize, 0), removed.p10);
            board.Set(coord.Move(0, smallSize), removed.p01);
            board.Set(coord.Move(smallSize, smallSize), removed.p11);
        }

        public override JSONClass ToJSON() {
            var json = new JSONClass();
            json["type"] = "compact";
            json["coord"] = coord.ToJSON();
            json["bigPiece"] = bigPiece.ToJSON();
            json["removed"] = removed.ToJSON();
            return json;
        }

        public static CompactAction FromJSON(JSONClass json) {
            return new CompactAction(
                Coord.FromJSON(json["coord"]),
                Piece.FromJSON(json["bigPiece"]),
                PiecesSquare.FromJSON(json["removed"])
            );
        }
    }

    public class RemoveAction : BoardAction {
        public readonly Coord coord;
        public readonly Piece removed;

        public RemoveAction(Piece removed, Coord c) {
            System.Diagnostics.Debug.Assert(removed != null, "piece cannot be null in RemoveAction!");
            this.coord = c;
            this.removed = removed;
        }

        public override void Do(Board board) {
            board.Remove(coord);
        }

        public override void Undo(Board board) {
            board.Set(coord, removed);
        }

        public override JSONClass ToJSON() {
            var json = new JSONClass();
            json["type"] = "remove";
            json["coord"] = coord.ToJSON();
            json["removed"] = removed.ToJSON();
            return json;
        }
        public static RemoveAction FromJSON(JSONClass json) {
            return new RemoveAction(Piece.FromJSON(json["removed"]), Coord.FromJSON(json["coord"]));
        }
    }

    public class FallAction : BoardAction {
        public readonly int rows;
        public readonly Coord coord;
        public readonly Piece piece;

        public FallAction(Piece piece, Coord c, int rows) {
            this.piece = piece;
            this.coord = c;
            this.rows = rows;
        }

        public override void Do(Board board) {
            board.Remove(coord);
            board.Set(coord.Move(0, -rows), piece);
        }

        public override void Undo(Board board) {
            var fallenCoord = board.Where(piece);
            board.Remove(piece);
            board.Set(fallenCoord.Move(0, rows), piece);
        }

        public override JSONClass ToJSON() {
            var json = new JSONClass();
            json["type"] = "fall";
            json["rows"] = rows;
            json["coord"] = coord.ToJSON();
            json["piece"] = piece.ToJSON();
            return json;
        }

        public static FallAction FromJSON(JSONClass json) {
            return new FallAction(Piece.FromJSON(json["piece"]),
                    Coord.FromJSON(json["coord"]),
                    json["rows"].AsInt);
        }
    }

    public class NewPieceAction : BoardAction {
        public Coord coord;
        public Piece newPiece;

        public override void Do(Board board) {
            board.Set(coord, newPiece);
        }

        public override void Undo(Board board) {
            board.Remove(coord);
        }

        public override JSONClass ToJSON() {
            var json = new JSONClass();
            json["type"] = "new";
            json["coord"] = coord.ToJSON();
            json["newPiece"] = newPiece.ToJSON();
            return json;
        }
        public static NewPieceAction FromJSON(JSONClass json) {
            var action = new NewPieceAction();
            action.coord = Coord.FromJSON(json["coord"]);
            action.newPiece = Piece.FromJSON(json["newPiece"]);
            return action;
        }
    }

}