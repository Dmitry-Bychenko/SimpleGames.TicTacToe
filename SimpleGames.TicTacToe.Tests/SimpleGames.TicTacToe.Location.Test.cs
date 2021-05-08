
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleGames.TicTacToe.Tests {

  [TestClass]
  public sealed class LocationTests {
    [TestMethod]
    public void LocationCorrespondence() {
      for (int i = 1; i <= 9; ++i) {
        TicTacToeLocation loc = new(i);

        Assert.AreEqual(loc.Rank - 1, (i - 1) / 3, "Rank");
        Assert.AreEqual(loc.File - 1, (i - 1) % 3, "Rank");

        int r = (i - 1) / 3;
        int f = (i - 1) % 3;

        Assert.AreEqual(loc.Cell, $"{(char)('a' + f)}{3 - r}");
      }
    }

    [TestMethod]
    public void CellCorrespondence() {
      int index = 0;

      for (int i = 3; i >= 1; --i) {
        for (char c = 'a'; c <= 'c'; ++c) {
          string cell = $"{c}{i}";

          index += 1;

          TicTacToeLocation loc = new(cell);

          Assert.AreEqual(loc.Index, index);
        }
      }
    }
  }

}
