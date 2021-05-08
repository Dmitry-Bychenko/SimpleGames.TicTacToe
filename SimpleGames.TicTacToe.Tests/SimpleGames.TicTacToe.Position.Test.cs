
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleGames.TicTacToe.Tests {

  [TestClass]
  public sealed class PositionTests {
    [TestMethod]
    public void OutcomeTest() {
      TicTacToePosition board = TicTacToePosition.Build(
        "a1", "a2", "a3", "b2", "b1", "c1", "b3", "c3", "c2");

      Assert.IsTrue(board.Outcome == GameOutcome.Draw);
    }

  }

}
