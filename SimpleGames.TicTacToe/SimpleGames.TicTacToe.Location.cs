using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace SimpleGames.TicTacToe {
  
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Tic Tac Toe Move
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class TicTacToeLocation 
    : IEquatable<TicTacToeLocation>, 
      IComparable<TicTacToeLocation>,
      ISerializable {

    #region Create

    private TicTacToeLocation(SerializationInfo info, StreamingContext context) {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      int index = info.GetInt32("info");

      if (index < 1 || index > 9)
        throw new ArgumentOutOfRangeException(nameof(index), "Index must be within 1..9");

      Index = index;
    }

    /// <summary>
    /// From Index [1..9]
    /// </summary>
    public TicTacToeLocation(int index) {
      if (index < 1 || index > 9)
        throw new ArgumentOutOfRangeException(nameof(index), "Index must be within 1..9");

      Index = index;
    }

    /// <summary>
    /// From Rank [1..3] and File [1..3]
    /// </summary>
    public TicTacToeLocation(int rank, int file) {
      if (rank < 1 || rank > 3)
        throw new ArgumentNullException(nameof(rank));
      if (file < 1 || file > 3)
        throw new ArgumentNullException(nameof(file));

      Index = 3 * (rank - 1) + file;
    }

    /// <summary>
    /// From Rank [1..3] and File [1..3]
    /// </summary>
    public TicTacToeLocation((int rank, int file) location) {
      if (location.rank < 1 || location.rank > 3)
        throw new ArgumentNullException(nameof(location), "location.rank is out of range");
      if (location.file < 1 || location.file > 3)
        throw new ArgumentNullException(nameof(location), "location.file is out of range");

      Index = 3 * (location.rank - 1) + location.file;
    }

    /// <summary>
    /// From Cell [a1..a3], [b1..b3], [c1..c3]
    /// </summary>
    public TicTacToeLocation(string cell) {
      if (cell is null)
        throw new ArgumentNullException(nameof(cell));

      var match = Regex.Match(cell, @"^\s*(?<rank>[A-Ca-c])\s*(?<file>[1-3])\s*$");

      if (!match.Success)
        throw new ArgumentException("Incorrect Syntax for cell; a1..c3 expected", nameof(cell));

      Index = ('3' - match.Groups["file"].Value[0]) * 3 +
               match.Groups["rank"].Value.ToUpper()[0] - 'A' + 1;
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out TicTacToeLocation result) {
      result = null;

      if (string.IsNullOrWhiteSpace(value))
        return false;

      if (Regex.IsMatch(value, @"^\s*(?<rank>[A-Ca-c])\s*(?<file>[1-3])\s*$")) {
        result = new TicTacToeLocation(value);

        return true;
      }

      if (int.TryParse(value, out int index) && index >= 1 && index <= 9) {
        result = new TicTacToeLocation(index);

        return true;
      }

      var match = Regex.Match(value, @"^\s*(?<rank>[1-3])\s*[:_,;=~]?\s*(?<file>[1-3])\s*$");

      if (match.Success) {
        result = new TicTacToeLocation(int.Parse(match.Groups["rank"].Value), int.Parse(match.Groups["file"].Value));

        return true;
      }

      return false;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static TicTacToeLocation Parse(string value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      if (TryParse(value, out var result))
        return result;

      throw new FormatException("Invalid Tic-Tac-Toe location.");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(TicTacToeLocation left, TicTacToeLocation right) {
      if (ReferenceEquals(left, right))
        return 0;
      if (left is null)
        return -1;
      if (right is null)
        return 1;

      return string.Compare(left.Cell, right.Cell, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Index [1..9]
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Rank [1..3] (Row)
    /// </summary>
    public int Rank => (Index - 1) / 3 + 1;

    /// <summary>
    /// File [1..3] (Column)
    /// </summary>
    public int File => (Index - 1) % 3 + 1;

    /// <summary>
    /// Cell a1..a3, b1..b3, c1..c3
    /// </summary>
    public string Cell => $"{(char)('a' + (Index - 1) % 3)}{3 - (Index - 1) / 3}";

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => Cell;

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals 
    /// </summary>
    public  static bool operator == (TicTacToeLocation left, TicTacToeLocation right) {
      if (ReferenceEquals(left, right))
        return true;
      if (left is null || right is null)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals 
    /// </summary>
    public static bool operator !=(TicTacToeLocation left, TicTacToeLocation right) {
      if (ReferenceEquals(left, right))
        return false;
      if (left is null || right is null)
        return true;

      return !left.Equals(right);
    }

    /// <summary>
    /// From cell 
    /// </summary>
    public static implicit operator TicTacToeLocation(string cell) => Parse(cell);

    /// <summary>
    /// From index 
    /// </summary>
    public static implicit operator TicTacToeLocation(int index) => new TicTacToeLocation(index);

    /// <summary>
    /// From Rank and File 
    /// </summary>
    public static implicit operator TicTacToeLocation((int rank, int file) position) => new TicTacToeLocation(position);

    #endregion Operators

    #region IEquatable<TicTacToeMove>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(TicTacToeLocation other) => (other is not null) && (other.Index == Index);

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => obj is TicTacToeLocation other && Equals(other);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Index;

    #endregion IEquatable<TicTacToeMove>

    #region IComparable<TicTacToeLocation>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(TicTacToeLocation other) => Compare(this, other);

    #endregion IComparable<TicTacToeLocation>

    #region ISerializable

    /// <summary>
    /// Serializable Data
    /// </summary>
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      info.AddValue("index", Index);
    }

    #endregion ISerializable
  }

}
