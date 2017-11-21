using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlienExplorer.Model;

namespace AlienExplorerTests
{
    [TestClass]
    public class PlayerStateMachineTests
    {
        protected static readonly float EPSILON = 0.01f;
        private static readonly float SUBSTATE_PERIOD = 0.05f;
        private static readonly int HURT_PERIOD_MULT = 10;
        
        [TestMethod]
        public void SetStateTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 0,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            float[ ] freeSpace = new float[ ] { 0, 0, 0, 0 };
            float[ ] move = new float[ ] { 0, 0 };
            float deltaSeconds = 0;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.SetState(PlayerStateType.Hurt);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 12);
        }

        [TestMethod]
        public void ChangeStateZeroTimeTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 1,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 2, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 1, 0 };
            float deltaSeconds = 0;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.SetState(PlayerStateType.Walk);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 1);
        }

        [TestMethod]
        public void ChangeStateHalfStepTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 1,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 2, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 1, 0 };
            float deltaSeconds = SUBSTATE_PERIOD * 0.5f;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.SetState(PlayerStateType.Walk);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 1);
        }

        [TestMethod]
        public void ChangeStateOneStepTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 1,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 2, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 1, 0 };
            float deltaSeconds = SUBSTATE_PERIOD + EPSILON;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.SetState(PlayerStateType.Walk);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 2);
        }

        [TestMethod]
        public void ChangeStateOneAndHalfStepTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 1,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 2, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 1, 0 };
            float deltaSeconds = SUBSTATE_PERIOD * 1.5f;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.SetState(PlayerStateType.Walk);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 2);
        }

        [TestMethod]
        public void ChangeStateThreeStepsTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 1,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 2, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 1, 0 };
            float deltaSeconds = SUBSTATE_PERIOD * 3;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.SetState(PlayerStateType.Walk);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 2);
        }

        [TestMethod]
        public void ChangeStateThreeStepsHurtTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 1,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 2, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 1, 0 };
            float deltaSeconds = SUBSTATE_PERIOD * 3;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.SetState(PlayerStateType.Hurt);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 12);
        }

        [TestMethod]
        public void ChangeStateHurtCooldownTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 1,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 2, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 1, 0 };
            float deltaSeconds = SUBSTATE_PERIOD * HURT_PERIOD_MULT + EPSILON;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.SetState(PlayerStateType.Hurt);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 1);
        }

        [TestMethod]
        public void ChangeStateCycleTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 8,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 2, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 1, 0 };
            float deltaSeconds = SUBSTATE_PERIOD + EPSILON;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.SetState(PlayerStateType.Walk);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 9);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 1);
        }

        [TestMethod]
        public void ChangeStateNewStateTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 0,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 2, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 1, 0 };
            float deltaSeconds = 0;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 1);
        }

        [TestMethod]
        public void ChangeStateJumpTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 0,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 0, 2 };
            // dX, dY
            float[ ] move = new float[ ] { 0, 0 };
            float deltaSeconds = 0;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 10);
        }

        [TestMethod]
        public void ChangeStateJumpAndMoveTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 0,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 0, 2 };
            // dX, dY
            float[ ] move = new float[ ] { 1, 0 };
            float deltaSeconds = 0;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 10);
        }

        [TestMethod]
        public void ChangeStateStandTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 1,
                SizeX = 1,
                SizeY = 2,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 0, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 0, 0 };
            float deltaSeconds = 0;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.SetState(PlayerStateType.Walk);
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 0);
        }

        [TestMethod]
        public void ChangeStateDuckTest()
        {
            PlayerObject player = new PlayerObject()
            {
                State = 1,
                SizeX = 1,
                SizeY = 1.5f,
                SizeYstandart = 2,
                SizeYsmall = 1.5f
            };
            // слева, сверху, справа, снизу
            float[ ] freeSpace = new float[ ] { 0, 0, 0, 0 };
            // dX, dY
            float[ ] move = new float[ ] { 0, 0 };
            float deltaSeconds = 0;
            PlayerStateMachine machine = new PlayerStateMachine();
            machine.ChangeState(player, freeSpace, move, deltaSeconds);
            Assert.AreEqual(player.State, 11);
        }
    }
}
