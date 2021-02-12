using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleGames.TicTacToe {
  
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Tic Tac Toe Move
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class TicTacToeLocation : IEquatable<TicTacToeLocation>, IComparable<TicTacToeLocation> {
    #region Create

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
    /// Index [1..3]
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
    public static implicit operator TicTacToeLocation(string cell) => new TicTacToeLocation(cell);

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
  }

}
