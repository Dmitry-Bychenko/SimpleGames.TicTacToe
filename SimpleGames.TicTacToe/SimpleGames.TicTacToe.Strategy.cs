using System;
using System.Collections.Generic;
using System.Linq;

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
    public static GameOutcome MoveExpectation(this TicTacToePosition position, TicTacToeLocation move) {
      if (position is null)
        return GameOutcome.Illegal;
      if (move is null)
        return GameOutcome.Illegal;

      return ExpectedWinner(position.MakeMove(move));
    }

    /// <summary>
    /// Move quality
    ///   -1 worst move
    ///    0 illegal move
    ///   +1 best move
    /// </summary>
    public static int MoveQuality(this TicTacToePosition position, TicTacToeLocation move) {
      if (position is null)
        return 0;
      if (move is null)
        return 0;

      var actual = MoveExpectation(position, move);

      if (actual == GameOutcome.Illegal)
        return 0;

      var best = ExpectedWinner(position);

      if (best == actual)
        return 1;
      else
        return -1;
    }
        
    /// <summary>
    /// Best Moves (all winning moves or, if the position drawish, all draw moves)
    /// </summary>
    public static TicTacToeLocation[] BestMoves(this TicTacToePosition position) {
      if (position is null)
        return Array.Empty<TicTacToeLocation>();

      return position
        .AvailableMoves()
        .Where(move => MoveQuality(position, move) == 1)
        .OrderBy(move => move)
        .ToArray();
    }

    /// <summary>
    /// Move Degree, winning, loosing etc.
    /// </summary>
    public static MoveExpectancy MoveDegree(this TicTacToePosition position, TicTacToeLocation move) {
      if (position is null)
        return MoveExpectancy.Illegal;
      if (move is null)
        return MoveExpectancy.Illegal;

      if (!position.IsLegalMove(move, true))
        return MoveExpectancy.Illegal;

      var expectation = MoveExpectation(position, move);

      if (expectation == GameOutcome.Illegal)
        return MoveExpectancy.Illegal;

      if (position.WhoIsOnMove == Mark.Cross) {
        if (expectation == GameOutcome.FirstWin)
          return MoveExpectancy.Win;
        else if (expectation == GameOutcome.Draw)
          return MoveExpectancy.Draw;
        else if (expectation == GameOutcome.SecondWin)
          return MoveExpectancy.Lose;
      }
      else {
        if (expectation == GameOutcome.FirstWin)
          return MoveExpectancy.Lose;
        else if (expectation == GameOutcome.Draw)
          return MoveExpectancy.Draw;
        else if (expectation == GameOutcome.SecondWin)
          return MoveExpectancy.Win;
      }

      return MoveExpectancy.Illegal;
    }

    #endregion Public
  }

}
