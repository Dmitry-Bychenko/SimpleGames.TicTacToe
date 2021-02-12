using System;
using System.Collections.Generic;

using SimpleGames.TicTacToe;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleGames.TicTacToe.Tests {


  [TestClass]
  public sealed class GameOutcomeTests {
    [TestMethod]
    public void TestReverse() {
      HashSet<GameOutcome> hasWinner = new HashSet<GameOutcome>() {
        GameOutcome.FirstWin,
        GameOutcome.SecondWin
      };

      foreach (GameOutcome outcome in Enum.GetValues<GameOutcome>()) {
        GameOutcome reverse = outcome.Reverse();

        if (hasWinner.Contains(outcome)) {
          Assert.AreNotEqual(outcome, reverse, "Reversed Winner");

          Assert.AreEqual(outcome, reverse.Reverse(), "Reversed Winner Twice");
        }
        else
          Assert.AreEqual(outcome, reverse, "No Winner");
      }
    }
  }
}
