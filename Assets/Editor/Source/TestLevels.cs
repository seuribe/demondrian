using com.perroelectrico.demondrian.level;
using NUnit.Framework;
using UnityEngine;
using SimpleJSON;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestLevels {

        [Test]
        public void TestLevelLoading() {

            var assetName = "Levels/levels.basic";
            var lm = LevelRepository.Instance;
            var asset = Resources.Load<TextAsset>(assetName);
            Assert.NotNull(asset, "asset could be loaded from " + assetName);
            var text = asset.text;
            Assert.NotNull(text, "asset contents not null");
            lm.Load(text);
            Assert.AreEqual(1, lm.Count, "level manager loaded 1 level");
            var level = lm.Levels[0];
            Assert.LessOrEqual(1, level.Count, "level has at least 1 puzzle");

            var game = level[0].NewGame();
            Assert.NotNull(game, "game not null after initialization from puzzle");
            Assert.NotNull(game.Board, "game board not null");
            Assert.AreEqual(2, game.Board.Size, "loaded game has 2x2 board");
            Assert.IsTrue(game.Board.IsFull(), "board is full");
        }

        [Test]
        public void TestBoardLoadingWithPointers() {
            var jsonStr = @"
            {
				""size"" : 4,
				""pointers"" : [""0/2->0:2"", ""0/2->2:2"", ""0/2->2:0"", ""0/1->0:0"", ""0/1->1:0"", ""0/1->0:1"", ""1/1->1:1""]
			}";
            var board = Board.FromJSON(JSON.Parse(jsonStr));
            Assert.NotNull(board, "board was parsed and is not null");
            Assert.AreEqual(4, board.Size, "board size is correct");
            var piece = board.Get(new Coord(0, 0));
            Assert.NotNull(piece, "there is a piece @ 0,0");
            Assert.AreEqual(1, piece.size, "piece @ 0,0 size 1");
            piece = board.Get(new Coord(0, 2));
            Assert.NotNull(piece, "there is a piece @ 0,2");
            Assert.AreEqual(2, piece.size, "piece @ 0,0 size 2");
            Assert.IsTrue(piece.type.Matches(PieceType.Get(0)), "piece @ 0,0 type 0");
        }

    }
}