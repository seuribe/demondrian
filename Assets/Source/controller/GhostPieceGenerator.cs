using System.Collections.Generic;
using System.Linq;
using com.perroelectrico.demondrian.core;

namespace com.perroelectrico.demondrian.controller {

    public class GhostPieceGenerator {
        readonly PieceGenerator pieceGenerator;
        readonly BoardLayout layout;
        readonly Dictionary<Coord, PieceController> ghosts = new Dictionary<Coord, PieceController>();
        readonly float removeTime;

        readonly object syncLock = new object();

        public GhostPieceGenerator(PieceGenerator pieceGenerator, BoardLayout layout, float removeTime) {
            this.pieceGenerator = pieceGenerator;
            this.layout = layout;
            this.removeTime = removeTime;
        }

        bool IsGhosted(Coord coord) {
            return ghosts.ContainsKey(coord);
        }

        public void HoverOver(Coord coord, Piece newPiece) {
            lock (syncLock) {
                UnGhost(ghosts.Keys.Where( c => !c.Matches(coord)));
                if (!IsGhosted(coord))
                    Ghost(coord, newPiece);
            }
        }

        public void ClickedOn(Coord coord) {
            lock (syncLock) {
                if (IsGhosted(coord))
                    UnGhost(coord);
            }
        }

        public void Clear() {
            lock (syncLock) {
                UnGhost(ghosts.Keys);
            }
        }

        void Ghost(Coord coord, Piece newPiece) {
            var pc = GenerateGhostController(coord, newPiece);
            pc.OnAdd();
            ghosts[coord] = pc;
        }

        void UnGhost(IEnumerable<Coord> coords) {
            // To avoid remove from the same collection, which throws an out of sync exception
            foreach (var coord in new List<Coord>(coords))
                UnGhost(coord);
        }

        void UnGhost(Coord coord) {
            var pc = ghosts[coord];
            ghosts.Remove(coord);
            pc.OnRemove();
            pieceGenerator.DestroyPiece(pc, removeTime);
        }

        PieceController GenerateGhostController(Coord coord, Piece newPiece) {
            var gameObject = pieceGenerator.NewStandardPiece(newPiece, true);
            var pc = gameObject.GetComponent<PieceController>();
            layout.PositionPieceAboveBoard(pc, coord);
            return pc;
        }

    }
}