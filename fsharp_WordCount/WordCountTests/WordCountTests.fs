namespace fsharp_WordCount.Tests

open System
open System.IO
open System.Security
open Microsoft.VisualStudio.TestTools.UnitTesting

open WordCount


[<TestClass>]
type WordCountsTests () =

    let seqEquals expected result = Seq.compareWith Operators.compare expected result = 0

    [<TestMethod>]
    member this.TestSingleWord () = 
        let expected = seq {("something", 1)}
        let result = wordCounts (seq {"something\n"})
        Assert.IsTrue(seqEquals expected result)

    [<TestMethod>]
    member this.TestSingleString () = 
        let expected = seq {
            ("Go", 1);
            ("do", 2);
            ("that", 2);
            ("thing", 1);
            ("you", 1);
            ("so", 1);
            ("well", 1);
        }
        let result = wordCounts (seq {"Go do that thing that you do so well\n"})
        Assert.IsTrue(seqEquals expected result)

    [<TestMethod>]
    member this.TestMultipleStrings () = 
        let expected = seq {
            ("Go", 3);
            ("do", 6);
            ("that", 6);
            ("thing", 3);
            ("you", 3);
            ("so", 3);
            ("well", 3);
        }
        let result = wordCounts (seq {
            "Go do that thing that you do so well\n";
            "Go do that thing that you do so well\n";
            "Go do that thing that you do so well\n";
        })
        Assert.IsTrue(seqEquals expected result)

    [<TestMethod>]
    member this.TestEmptyStrings () = 
        let expected = Seq.empty
        let result = wordCounts (seq {""; "\n"; "\r\n"; "     "; "  \n\r\n"})
        Assert.IsTrue(seqEquals expected result)


[<TestClass>]
type FileWordCountsTests () =

    let fileWordCountsAlwaysRaise ex =
        // mocking function replacing File.ReadLines - to instead always throw a given exception
        let exFunc = (fun _path -> raise ex)
        let (status_code, _) = fileWordCounts exFunc "some path"
        Assert.AreEqual(status_code, 1)

    [<TestMethod>]
    member this.TestExceptionHandling () =
        // Ensure that an exception possibly thrown by File.ReadLines are properly handled
        let possibleExceptions: seq<Exception> = seq {
            new ArgumentException();
            new ArgumentNullException();
            new FileNotFoundException();
            new IOException();
            new PathTooLongException();
            new SecurityException();
            new UnauthorizedAccessException();
        }
        Seq.iter fileWordCountsAlwaysRaise possibleExceptions

    [<TestMethod>]
    member this.TestSuccessfulWordCount () =
        // Ensure that status code is correctly set when file is read correctly
        let successFunc = (fun (path: string) -> seq {path})
        let (status_code, _) = fileWordCounts successFunc "some path"
        Assert.AreEqual(status_code, 0)
