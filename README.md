# Tic Tac Toe engine
- Position
- Strategy
- Analysis

I use this library as a touch stone when I teach game theory. Some typical examples of usage:

## Strategy:

### Code:

```c#
    TicTacToePosition board = TicTacToePosition.Build("a1", "a2", "a3", "b3");

    string report = string.Join(Environment.NewLine,
       board.DrawPosition(),
       "",
      $"Expected outcome: {position.ExpectedWinner()}",
       "",
      $"Best moves: {string.Join(", ", board.BestMoves().AsEnumerable())}"
    );

    Console.WriteLine(report);
```

### Outcome:

```
    3 | XO  
    2 | O   
    1 | X   
       -----
        abc 
    
    Expected outcome: FirstWin
    
    Best moves: b2, c1
```

## Analysis:  

### Code:

```c#
      // Let's have a look at winning statistics:

      string report = string.Join(Environment.NewLine, TicTacToePosition
        .AllLegalPositions()
        .GroupBy(position => position.ExpectedWinner())
        .OrderBy(group => group.Key)
        .Select(group => $"{group.Key,-9} : {group.Count(),4}"));     
```

### Outcome:

```
    FirstWin  : 2936
    SecondWin : 1474
    Draw      : 1068
```
