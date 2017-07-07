using NUnit.Framework;

using SimpleJSON;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestJSONSerialization : TestConstants {

        [Test]
        public void TestPieceSerialization() {
            string tstr = @"{ ""index"": 0}";
            var json = JSON.Parse(tstr);
            var t = PieceType.FromJSON(json);
            Assert.AreEqual(0, t.index);

            for (int i = 0 ; i < 100 ; i++) {
                var tt = PieceType.Get(i);
                var tjson = tt.ToJSON();
                var tout = PieceType.FromJSON(tjson);
                Assert.AreEqual(i, tt.index, "initial type has correct index");
                Assert.AreEqual(i, tout.index, "deserialized type has correct index");
                Assert.AreEqual(tt.index, tout.index, "both have correct index");
                Assert.IsTrue(tt.Matches(PieceType.FromJSON(tjson)), "serialize -> deserialize results in same type");
            }

            for (int i = 0 ; i < 10 ; i++) {
                for (int size = 1 ; size <= 16 ; size *= 2) {
                    var p = new Piece(PieceType.Get(i), size);
                    Assert.IsTrue(p.Matches(Piece.FromJSON(p.ToJSON())), "piece matches its serialized -> deserialized");
                }
            }
        }
    }
}