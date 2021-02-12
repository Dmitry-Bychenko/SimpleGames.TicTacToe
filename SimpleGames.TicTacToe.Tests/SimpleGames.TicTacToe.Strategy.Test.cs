using SimpleGames.TicTacToe;

using System;
using System.Collections.Generic;
using System.Linq;


using Microsoft.VisualStudio.TestTools.UnitTesting;

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
  }

}
