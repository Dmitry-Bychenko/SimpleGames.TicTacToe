
namespace SimpleGames.TicTacToe {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Mark
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum Mark : byte {
    /// <summary>
    /// No Mark, empty
    /// </summary>
    None = 0,
    /// <summary>
    /// Cross
    /// </summary>
    Cross = 1,
    /// <summary>
    /// Nought
    /// </summary>
    Nought = 2
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Mark Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class MarkHelper {
    #region Public

    /// <summary>
    /// To Char
    /// </summary>
    public static char ToChar(this Mark mark) {
      return mark switch {
        Mark.Cross => 'X',
        Mark.Nought => 'O',
        Mark.None => '.',
        _ => '?'
      };
    }

    /// <summary>
    /// To Char
    /// </summary>
    public static char ToChar(this Mark mark, char empty) {
      return mark switch {
        Mark.Cross => 'X',
        Mark.Nought => 'O',
        Mark.None => empty,
        _ => '?'
      };
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Game outcome
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum GameOutcome {
    /// <summary>
    /// No outcome, the game keeps on
    /// </summary>
    None = 0,
    /// <summary>
    /// First Player is the winner
    /// </summary>
    FirstWin = 1,
    /// <summary>
    /// Second Player is the winner
    /// </summary>
    SecondWin = 2,
    /// <summary>
    /// Draw
    /// </summary>
    Draw = 3,
    /// <summary>
    /// No winner, since the position is illegal one
    /// </summary>
    Illegal = 4,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Game Outcome Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class GameOutcomeExtensions {
    #region Public

    /// <summary>
    /// Choose the best for the 1st 
    /// </summary>
    public static GameOutcome BestForFirst(this GameOutcome left, GameOutcome right) {
      if (left == right)
        return left;

      if (left == GameOutcome.FirstWin)
        return left;
      else if (right == GameOutcome.FirstWin)
        return right;
      else if (left == GameOutcome.Draw)
        return left;
      else if (right == GameOutcome.Draw)
        return right;
      else if (left == GameOutcome.None)
        return left;
      else if (right == GameOutcome.None)
        return right;

      return left;
    }

    /// <summary>
    /// Choose the best for the 2nd
    /// </summary>
    public static GameOutcome BestForSecond(this GameOutcome left, GameOutcome right) {
      if (left == right)
        return left;

      if (left == GameOutcome.SecondWin)
        return left;
      else if (right == GameOutcome.SecondWin)
        return right;
      else if (left == GameOutcome.Draw)
        return left;
      else if (right == GameOutcome.Draw)
        return right;
      else if (left == GameOutcome.None)
        return left;
      else if (right == GameOutcome.None)
        return right;

      return left;
    }

    /// <summary>
    /// Reverse
    /// </summary>
    public static GameOutcome Reverse(this GameOutcome value) =>
        value == GameOutcome.FirstWin ? GameOutcome.SecondWin
      : value == GameOutcome.SecondWin ? GameOutcome.FirstWin
      : value;

    #endregion Public
  }

}
