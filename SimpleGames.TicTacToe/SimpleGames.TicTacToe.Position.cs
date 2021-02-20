using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SimpleGames.TicTacToe {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Tic Tac Toe Position
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class TicTacToePosition 
    : IEquatable<TicTacToePosition>, 
      ISerializable {

    #region Private Data

    private readonly Mark[] m_Marks = new Mark[9];

    #endregion Private Data

    #region Create

    // Standard Constructor
    private TicTacToePosition() {
    }

    // [De]Serialization Constructor
    private TicTacToePosition(SerializationInfo info, StreamingContext context) {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      TicTacToePosition copy = Parse(info.GetString("field"));

      for (int i = 0; i < copy.m_Marks.Length; ++i)
        m_Marks[i] = copy.m_Marks[i];
    }

    // Clone
    private TicTacToePosition Clone() {
      TicTacToePosition result = new TicTacToePosition();

      for (int i = m_Marks.Length - 1; i >= 0; --i)
        result.m_Marks[i] = m_Marks[i];

      return result;
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out TicTacToePosition result) {
      result = null;

      if (string.IsNullOrWhiteSpace(value))
        return false;

      var raw = value
        .Where(c => !char.IsWhiteSpace(c))
        .Select(c =>
             (c == 'x' || c == 'X' || c == '1' || c == '+') ? 1
           : (c == 'O' || c == 'o' || c == '0') ? 2
           : (c == '.' || c == '_' || c == '-') ? 0
           : -1)
        .Take(10)
        .ToArray();

      if (raw.Length != 9)
        return false;

      if (raw.Any(item => item < 0))
        return false;

      result = new TicTacToePosition();

      for (int i = 0; i < raw.Length; ++i)
        result.m_Marks[i] =
            raw[i] == 1 ? Mark.Cross
          : raw[i] == 2 ? Mark.Nought
          : Mark.None;

      return true;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static TicTacToePosition Parse(string value) => TryParse(value, out var result)
      ? result
      : throw new FormatException("Not a tic-tac-toe position");

    /// <summary>
    /// Build from moves
    /// </summary>
    public static bool TryBuild(IEnumerable<TicTacToeLocation> moves, out TicTacToePosition result) {
      result = null;

      if (moves is null)
        return false;

      result = Empty;

      foreach (TicTacToeLocation move in moves) {
        if (!result.IsLegalMove(move, true)) {
          result = null;

          return false;
        };

        result.m_Marks[move.Index - 1] = result.WhoIsOnMove;
      }

      return true;
    }

    /// <summary>
    /// Build from moves
    /// </summary>
    public static bool TryBuild(out TicTacToePosition result, params TicTacToeLocation[] moves) =>
      TryBuild(moves, out result);

    /// <summary>
    /// Build from moves
    /// </summary>
    public static TicTacToePosition Build(IEnumerable<TicTacToeLocation> moves) {
      if (moves is null)
        throw new ArgumentNullException(nameof(moves));

      TicTacToePosition result = Empty;

      foreach (TicTacToeLocation move in moves) {
        if (!result.IsLegalMove(move, true))
          throw new ArgumentException($"Move {move} is illegal", nameof(moves));

        result.m_Marks[move.Index - 1] = result.WhoIsOnMove;
      }

      return result;
    }

    /// <summary>
    /// Build from moves
    /// </summary>
    public static TicTacToePosition Build(params TicTacToeLocation[] moves) =>
      Build(moves as IEnumerable<TicTacToeLocation>);

    /// <summary>
    /// Set Position
    /// </summary>
    /// <param name="result">Position or null if arguments are invalid</param>
    /// <param name="crosses">Crosses</param>
    /// <param name="naughts">Noughts</param>
    /// <returns>true, if position valid, false otherwise</returns>
    public static bool TrySet(out TicTacToePosition result, 
                                  IEnumerable<TicTacToeLocation> crosses,
                                  IEnumerable<TicTacToeLocation> naughts) {
      result = null;

      if (crosses is null)
        return false;
      if (naughts is null)
        return false;

      result = new TicTacToePosition();

      HashSet<TicTacToeLocation> completed = new HashSet<TicTacToeLocation>();

      foreach (var loc in crosses) {
        if (!completed.Add(loc)) {
          result = null;

          return false;
        }

        result.m_Marks[loc.Index - 1] = Mark.Cross;
      }

      foreach (var loc in naughts) {
        if (!completed.Add(loc)) {
          result = null;

          return false;
        }

        result.m_Marks[loc.Index - 1] = Mark.Nought;
      }

      return true;
    }

    /// <summary>
    /// Set Position
    /// </summary>
    /// <param name="crosses">Crosses</param>
    /// <param name="naughts">Noughts</param>
    /// <returns>Position</returns>
    /// <exception cref="FormatException">When set is invalid</exception>
    public static TicTacToePosition Set(IEnumerable<TicTacToeLocation> crosses,
                                        IEnumerable<TicTacToeLocation> naughts) {
      if (crosses is null)
        throw new ArgumentNullException(nameof(crosses));
      if (naughts is null)
        throw new ArgumentNullException(nameof(naughts));

      if (TrySet(out var result, crosses, naughts))
        return result;
      else
        throw new FormatException("Invalid position");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Empty (Starting) Postion
    /// </summary>
    public static TicTacToePosition Empty { get; } = new TicTacToePosition();

    /// <summary>
    /// Entire game Tree (Breadth First Search)
    /// </summary>
    public static IEnumerable<TicTacToePosition> AllLegalPositions() {
      HashSet<TicTacToePosition> agenda = new HashSet<TicTacToePosition>() { Empty };

      while (agenda.Count > 0) {
        HashSet<TicTacToePosition> next = new HashSet<TicTacToePosition>();

        foreach (var parent in agenda) {
          yield return parent;

          foreach (var child in parent.AvailablePositions())
            next.Add(child);
        }

        agenda = next;
      }
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return string.Join(Environment.NewLine,
        string.Concat(m_Marks.Skip(0).Take(3).Select(m => m.ToChar())),
        string.Concat(m_Marks.Skip(3).Take(3).Select(m => m.ToChar())),
        string.Concat(m_Marks.Skip(6).Take(3).Select(m => m.ToChar()))
     );
    }

    /// <summary>
    /// Draw Position
    /// </summary>
    public string DrawPosition() {
      return string.Join(Environment.NewLine,
        "3 | " + string.Concat(m_Marks.Skip(0).Take(3).Select(m => m.ToChar(' '))) + " ",
        "2 | " + string.Concat(m_Marks.Skip(3).Take(3).Select(m => m.ToChar(' '))) + " ",
        "1 | " + string.Concat(m_Marks.Skip(6).Take(3).Select(m => m.ToChar(' '))) + " ",
        "   -----",
        "    abc "
      );
    }

    /// <summary>
    /// Make Move
    /// </summary>
    public TicTacToePosition MakeMove(TicTacToeLocation move) {
      if (move is null)
        throw new ArgumentNullException(nameof(move));

      if (m_Marks[move.Index - 1] != Mark.None)
        throw new InvalidOperationException("Illegal move");
      if (Outcome != GameOutcome.None)
        throw new InvalidOperationException("The game is over");

      TicTacToePosition result = Clone();

      result.m_Marks[move.Index - 1] = WhoIsOnMove;

      return result;
    }

    /// <summary>
    /// Is Move Legal
    /// </summary>
    public bool IsLegalMove(TicTacToeLocation move, bool checkForOutcome) {
      if (move is null)
        return false;
      if (m_Marks[move.Index - 1] != Mark.None)
        return false;
      if (checkForOutcome && Outcome != GameOutcome.None)
        return false;

      return true;
    }

    /// <summary>
    /// Is Move Legal
    /// </summary>
    public bool IsLegalMove(TicTacToeLocation move) => IsLegalMove(move, true);

    /// <summary>
    /// Make Moves
    /// </summary>
    public TicTacToePosition MakeMoves(IEnumerable<TicTacToeLocation> moves, bool checkForOutcome) {
      if (moves is null)
        throw new ArgumentNullException(nameof(moves));

      TicTacToePosition result = Clone();

      foreach (TicTacToeLocation move in moves) {
        if (!result.IsLegalMove(move, checkForOutcome))
          throw new InvalidOperationException($"Move {move} is illegal");

        result.m_Marks[move.Index - 1] = result.WhoIsOnMove;
      }

      return result;
    }

    /// <summary>
    /// Make Moves
    /// </summary>
    public TicTacToePosition MakeMoves(bool checkForOutcome, params TicTacToeLocation[] moves) =>
      MakeMoves(moves, checkForOutcome);

    /// <summary>
    /// Make Moves
    /// </summary>
    public TicTacToePosition MakeMoves(params TicTacToeLocation[] moves) =>
      MakeMoves(moves, true);

    /// <summary>
    /// Who is On Move (Crosses or Naugts)
    /// </summary>
    public Mark WhoIsOnMove {
      get {
        if (Outcome != GameOutcome.None)
          return Mark.None;

        int c = 0;
        int n = 0;

        foreach (var mark in m_Marks)
          if (mark == Mark.Cross)
            c += 1;
          else if (mark == Mark.Nought)
            n += 1;

        if (c == n)
          return Mark.Cross;
        else if (c == n + 1 && c + n < m_Marks.Length)
          return Mark.Nought;
        else
          return Mark.None;
      }
    }

    /// <summary>
    /// Winner (if any)
    /// </summary>
    public Mark Winner {
      get {
        var outcome = Outcome;

        return
            outcome == GameOutcome.FirstWin ? Mark.Cross
          : outcome == GameOutcome.SecondWin ? Mark.Nought
          : Mark.None;
      }
    }

    /// <summary>
    /// Outcome
    /// </summary>
    public GameOutcome Outcome {
      get {
        int cs = 0;
        int ns = 0;

        foreach (var mark in m_Marks)
          if (mark == Mark.Cross)
            cs += 1;
          else if (mark == Mark.Nought)
            ns += 1;

        if (cs != ns && cs != ns + 1)
          return GameOutcome.Illegal;

        bool cWin = Lines.Any(line => line.All(m => m == Mark.Cross));
        bool nWin = Lines.Any(line => line.All(m => m == Mark.Nought));

        if (cWin && nWin)
          return GameOutcome.Illegal;
        else if (cWin)
          return GameOutcome.FirstWin;
        else if (nWin)
          return GameOutcome.SecondWin;
        else if (cs + ns == 9)
          return GameOutcome.Draw;

        return GameOutcome.None;
      }
    }

    /// <summary>
    /// Is Legal
    /// </summary>
    public bool IsLegal => Outcome != GameOutcome.Illegal;

    /// <summary>
    /// Move number (-1 for illegal position), 1-based 
    /// </summary>
    public int MoveNumber {
      get {
        if (Outcome == GameOutcome.Illegal)
          return -1;

        return m_Marks.Count(mark => mark == Mark.Cross) + 1;
      }
    }

    /// <summary>
    /// Board
    /// </summary>
    /// <param name="line">Line</param>
    /// <param name="column">Column</param>
    /// <returns>Mark</returns>
    public Mark this[TicTacToeLocation move] {
      get => move is null
        ? Mark.None
        : m_Marks[move.Index - 1];
    }
  
    /// <summary>
    /// NUmber of crosses or naughts
    /// </summary>
    public int MarkCount => m_Marks.Count(item => item != Mark.None);

    /// <summary>
    /// Horizontals, Verticals, Diagonals
    /// </summary>
    public IEnumerable<Mark[]> Lines {
      get {
        yield return new Mark[] { m_Marks[0], m_Marks[1], m_Marks[2] };
        yield return new Mark[] { m_Marks[3], m_Marks[4], m_Marks[5] };
        yield return new Mark[] { m_Marks[6], m_Marks[7], m_Marks[8] };

        yield return new Mark[] { m_Marks[0], m_Marks[3], m_Marks[6] };
        yield return new Mark[] { m_Marks[1], m_Marks[4], m_Marks[7] };
        yield return new Mark[] { m_Marks[2], m_Marks[5], m_Marks[8] };

        yield return new Mark[] { m_Marks[0], m_Marks[4], m_Marks[8] };
        yield return new Mark[] { m_Marks[2], m_Marks[4], m_Marks[6] };
      }
    }

    /// <summary>
    /// Available Positions
    /// </summary>
    public IEnumerable<TicTacToePosition> AvailablePositions() {
      Mark mark = WhoIsOnMove;

      if (mark == Mark.None)
        yield break;

      for (int i = 0; i < m_Marks.Length; ++i)
        if (m_Marks[i] == Mark.None) {
          TicTacToePosition result = Clone();

          result.m_Marks[i] = mark;

          yield return result;
        }
    }

    /// <summary>
    /// Available Moves
    /// </summary>
    public IEnumerable<TicTacToeLocation> AvailableMoves() {
      for (int index = 0; index < m_Marks.Length; ++index) {
        if (m_Marks[index] != Mark.None)
          continue;

        yield return new TicTacToeLocation(index + 1);
      }
    }

    /// <summary>
    /// Parent Positions
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TicTacToePosition> ParentPositions() {
      if (!IsLegal)
        yield break;

      int c = 0;
      int n = 0;

      foreach (var mark in m_Marks)
        if (mark == Mark.Cross)
          c += 1;
        else if (mark == Mark.Nought)
          n += 1;

      if (n + c == 0)
        yield break;

      Mark markToRemove = c > n ? Mark.Cross : Mark.Nought;

      for (int i = 0; i < m_Marks.Length; ++i) {
        if (m_Marks[i] == markToRemove) {
          TicTacToePosition result = Clone();

          result.m_Marks[i] = Mark.None;

          if (result.Outcome == GameOutcome.None)
            yield return result;
        }
      }
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(TicTacToePosition left, TicTacToePosition right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (left is null || right is null)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(TicTacToePosition left, TicTacToePosition right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (left is not null || right is not null)
        return false;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<TicTacToePosition>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(TicTacToePosition other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      return Enumerable.SequenceEqual(m_Marks, other.m_Marks);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object other) => Equals(other as TicTacToePosition);

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() {
      int result = 0;

      foreach (Mark mark in m_Marks)
        result = result * 4 + (int)mark;

      return result;
    }

    #endregion IEquatable<TicTacToePosition>

    #region ISerializable

    /// <summary>
    /// Serializable Data
    /// </summary>
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      string field = string.Join(Environment.NewLine,
        string.Concat(m_Marks.Skip(0).Take(3).Select(m => m.ToChar())),
        string.Concat(m_Marks.Skip(3).Take(3).Select(m => m.ToChar())),
        string.Concat(m_Marks.Skip(6).Take(3).Select(m => m.ToChar()))
      );

      info.AddValue("field", field);
    }

    #endregion ISerializable
  }

}
