
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace SimpleGames.TicTacToe.Tests {

  [TestClass]
  public sealed class StrategyTests {

    [TestMethod]
    public void BestMovesTest() {
      TicTacToePosition board = TicTacToePosition.Empty.MakeMoves(
        "a1", "a2", "a3", "b3"
      );

      var bestMoves = board.BestMoves(); // .OrderBy(move => move);

      TicTacToeLocation[] expected = new TicTacToeLocation[] {
        "b2", "c1"
      };

      Assert.IsTrue(bestMoves.SequenceEqual(expected), string.Join(" ", bestMoves.AsEnumerable()));
    }

    [TestMethod]
    public void Expectations() {
      TicTacToePosition board = TicTacToePosition.Build("a1", "a2", "a3", "b3");

      Assert.IsTrue(board.ExpectedWinner() == GameOutcome.FirstWin, board.DrawPosition());
    }

    [TestMethod]
    public void MoveExpectancy() {
      TicTacToePosition board = TicTacToePosition.Set(
        crosses: new TicTacToeLocation[] { "a3", "c1" },
        naughts: new TicTacToeLocation[] { "b2", "c3" });

      Assert.IsTrue(board.MoveDegree("a1") == TicTacToe.MoveExpectancy.Win, "Must be winning move");
      Assert.IsTrue(board.MoveDegree("a2") == TicTacToe.MoveExpectancy.Lose, "Must be losing move");
    }

    [TestMethod]
    public void MoveExpectancyIllegal() {
      TicTacToePosition board = TicTacToePosition.Set(
        crosses: new TicTacToeLocation[] { "a3", "c1" },
        naughts: new TicTacToeLocation[] { "b2", "c3" });

      Assert.IsTrue(board.MoveDegree("a3") == TicTacToe.MoveExpectancy.Illegal, "Must be illegal move");
    }
  }

}
