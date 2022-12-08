# Word Counting
## Overview
My solution consists of both an F# and C# solution in the respective directories:
* `fsharp_WordCount`, and
* `csharp_WordCount`

Each directory consists of one .NET solution, a Console Application `ConsoleApp` where the word counting code has been implemented, and `WordCountTests` implementing a small suite of unit tests for each implementation.

## Requirements
The solutions have been developed using .NET, specifically .NET SDK version 6.0.110, and has been tested on a system running Ubuntu 22.04.

## Example Usage
Each of the solutions have the same command-line interface, taking one filepath argument. A solution can be ran from their directory using:
```
> dotnet run --project ConsoleApp FILEPATH
```

## Testing
Both solutions have been tested using the `MsTest` framework.

The tests associated with each solution can be ran from their directory with:
```
> dotnet test
```
