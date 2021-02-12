using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleGames.TicTacToe {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Tic Tac Toe Strategy 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class TicTacToeStrategy {
    #region Private Data

    // Expected outcome 
    private static Dictionary<TicTacToePosition, GameOutcome> s_Outcomes;

    #endregion Private Data

    #region Algorithm

    private static void CoreUpdate() {
      s_Outcomes = TicTacToePosition
        .AllLegalPositions()
        .ToDictionary(p => p, p => GameOutcome.None);

      var data = s_Outcomes
        .Keys
        .OrderByDescending(key => key.MarkCount);

      foreach (var position in data) {
        if (position.Outcome != GameOutcome.None) {
          s_Outcomes[position] = position.Outcome;

          continue;
        }

        var onMove = position.WhoIsOnMove;

        GameOutcome bestOutcome = onMove == Mark.Cross
          ? GameOutcome.SecondWin
          : GameOutcome.FirstWin;

        foreach (var next in position.AvailablePositions()) {
          GameOutcome outcome = s_Outcomes[next];

          bestOutcome = onMove == Mark.Cross
            ? bestOutcome.BestForFirst(outcome)
            : bestOutcome.BestForSecond(outcome);
        }

        s_Outcomes[position] = bestOutcome;
      }
    }

    #endregion Algorithm

    #region Create

    static TicTacToeStrategy() {
      CoreUpdate();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Expected Winner
    /// </summary>
    public static GameOutcome ExpectedWinner(this TicTacToePosition position) {
      if (position is null)
        return GameOutcome.Illegal;

      return s_Outcomes.TryGetValue(position, out var result)
        ? result
        : GameOutcome.Illegal;
    }

    /// <summary>
    /// Move Expectation
    /// </summary>
    public static GameOutcome MoveExpectation(this TicTacToePosition position, int line, int column) {
      if (position is null)
        return GameOutcome.Illegal;

      if (position[line, column] != Mark.None)
        return GameOutcome.Illegal;
      else if (line < 0 || line > 2 || column < 0 || column > 2)
        return GameOutcome.Illegal;

      return ExpectedWinner(position.MakeMove(line, column));
    }

    /// <summary>
    /// Move Expectation
    /// </summary>
    public static GameOutcome MoveExpectation(this TicTacToePosition position, string cell) {
      if (cell is null)
        return GameOutcome.Illegal;

      var match = Regex.Match(cell, @"^\s*(?<rank>[A-Ca-c])\s*(?<file>[1-3])\s*$");

      if (!match.Success)
        return GameOutcome.Illegal;

      return MoveExpectation(position,
                             '3' - match.Groups["file"].Value[0],
                             match.Groups["rank"].Value.ToUpper()[0] - 'A');
    }

    /// <summary>
    /// Move Expectation
    /// </summary>
    public static GameOutcome MoveExpectation(this TicTacToePosition position, int index) {
      if (position is null)
        return GameOutcome.Illegal;

      if (position[index] != Mark.None)
        return GameOutcome.Illegal;
      else if (index < 1 || index > 9)
        return GameOutcome.Illegal;

      return ExpectedWinner(position.MakeMove(index));
    }

    /// <summary>
    /// Move quality
    ///   -1 worst move
    ///    0 illegal move
    ///   +1 best move
    /// </summary>
    public static int MoveQuality(this TicTacToePosition position, int line, int column) {
      var actual = MoveExpectation(position, line, column);

      if (actual == GameOutcome.Illegal)
        return 0;

      var best = ExpectedWinner(position);

      if (best == actual)
        return 1;
      else
        return -1;
    }

    /// <summary>
    /// Move quality
    ///   -1 worst move
    ///    0 illegal move
    ///   +1 best move
    /// </summary>
    public static int MoveQuality(this TicTacToePosition position, int index) {
      var actual = MoveExpectation(position, index);

      if (actual == GameOutcome.Illegal)
        return 0;

      var best = ExpectedWinner(position);

      if (best == actual)
        return 1;
      else
        return -1;
    }

    /// <summary>
    /// Move quality
    ///   -1 worst move
    ///    0 illegal move
    ///   +1 best move
    /// </summary>
    public static int MoveQuality(this TicTacToePosition position, string cell) {
      var match = Regex.Match(cell, @"^\s*(?<rank>[A-Ca-c])\s*(?<file>[1-3])\s*$");

      if (!match.Success)
        return 0;

      return MoveQuality(position,
                         '3' - match.Groups["file"].Value[0],
                         match.Groups["rank"].Value.ToUpper()[0] - 'A');
    }

    /// <summary>
    /// Best Moves (all winning moves or, if the position drawish, all draw moves)
    /// </summary>
    public static IEnumerable<(int line, int column, int index, string cell)> BestMoves(this TicTacToePosition position) {
      if (position is null)
        yield break;

      foreach (var move in position.AvailableMoves())
        if (MoveQuality(position, move.index) == 1)
          yield return move;
    }

    #endregion Public
  }

}
