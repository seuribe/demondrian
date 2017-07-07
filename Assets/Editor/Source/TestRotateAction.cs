using System;
using NUnit.Framework;

namespace com.perroelectrico.demondrian.core.test {
    [TestFixture]
    public class TestRotateAction {

        [Test]
        public void RotateToSelfGeneratesNoOps() {
            foreach (Orientation orientation in Enum.GetValues(typeof(Orientation))) {
                var actions = RotateAction.GenerateActions(orientation, orientation);
                Assert.IsEmpty(actions, "rotation 0 generates no actions");
            }
        }

        [Test]
        public void Rotate1StepGenerates1Action() {
            var actions = RotateAction.GenerateActions(Orientation.Top, Orientation.Right);
            Assert.AreEqual(1, actions.Length, "rotation 1 generates 1 action");
            Assert.AreEqual(RotateAction.Right, actions[0], "action is Right");
            actions = RotateAction.GenerateActions(Orientation.Top, Orientation.Left);
            Assert.AreEqual(1, actions.Length, "rotation 3 generates 1 action");
            Assert.AreEqual(RotateAction.Left, actions[0], "action[0] is Left");
            actions = RotateAction.GenerateActions(Orientation.Left, Orientation.Bottom);
            Assert.AreEqual(1, actions.Length, "rotation -1 generates 1 action");
            Assert.AreEqual(RotateAction.Left, actions[0], "action[0] is Left");
        }

        [Test]
        public void Rotate2StepsGenerates2Actions() {
            var actions = RotateAction.GenerateActions(Orientation.Top, Orientation.Bottom);
            Assert.AreEqual(2, actions.Length, "rotation 2 generates 2 actions");
            Assert.AreEqual(actions[0], actions[1],"both actions are the same");
            actions = RotateAction.GenerateActions(Orientation.Left, Orientation.Right);
            Assert.AreEqual(2, actions.Length, "rotation 2 generates 2 actions");
            Assert.AreEqual(actions[0], actions[1],"both actions are the same");
        }
    }
}