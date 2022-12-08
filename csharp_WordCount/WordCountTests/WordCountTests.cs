using System;
using System.IO;
using System.Security;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ConsoleApp;

namespace WordCountTests;

// Mock FileManager able to create StreamReaders compatible with WordCountsFromFile
public class StringFileManager : IFileManager {
    private string fileContent;

    public StringFileManager(string fileContent) {
        this.fileContent = fileContent;
    }
    
    public StreamReader OpenText(string path) {
        // Simply keep the stream in memory, this way no actual file I/O is required to run any tests
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(fileContent);
        writer.Flush();
        stream.Position = 0;
        return new StreamReader(stream);
    }
}

// Mock FileManager for throwing exceptions to test for proper file handling
public class ExceptionFileManager : IFileManager {
    private Exception exception;
    
    public ExceptionFileManager(Exception exception) {
        this.exception = exception;
    }
    
    public StreamReader OpenText(string path) {
        throw exception;
    }
}

[TestClass]
public class WordCountTests {
    private void TestFromString(string fileContent, IDictionary<string, int> expectedCounts) {
        IFileManager stringFileManager = new StringFileManager(fileContent);
        var counter = new WordCounter();
        var wordCounts = counter.WordCountsFromFile(String.Empty, stringFileManager);
        CollectionAssert.AreEqual((ICollection)expectedCounts, (ICollection)wordCounts);
    }

    [TestMethod]
    public void TestSingleWord() {
        IDictionary<string, int> expected = new Dictionary<string, int>() {{"something", 1}};
        TestFromString("something", expected);
    }

    [TestMethod]
    public void TestSingleString() {
        IDictionary<string, int> expected = new Dictionary<string, int>() {
            {"Go", 1},
            {"do", 2},
            {"that", 2},
            {"thing", 1},
            {"you", 1},
            {"so", 1},
            {"well", 1},
        };
        TestFromString("Go do that thing that you do so well\n", expected);
    }

    [TestMethod]
    public void TestMultipleStrings() {
        IDictionary<string, int> expected = new Dictionary<string, int>() {
            {"Go", 3},
            {"do", 6},
            {"that", 6},
            {"thing", 3},
            {"you", 3},
            {"so", 3},
            {"well", 3},
        };
        TestFromString("Go do that thing that you do so well\nGo do that thing that you do so well\nGo do that thing that you do so well\n", expected);
    }

    [TestMethod]
    public void TestEmptyStrings() {
        IDictionary<string, int> expected = new Dictionary<string, int>();
        TestFromString("", expected);
        TestFromString("\n", expected);
        TestFromString("\r\n", expected);
        TestFromString("          ", expected);
        TestFromString("   \n\r\n", expected);
    }
}

[TestClass]
public class ProgramManagerTests {
    // Ensure that the program performs exception handling correctly - i.e. correct status code is returned

    [TestMethod]
    public void TestExceptionHandling() {
        List<Exception> exceptions = new List<Exception>() {
            new UnauthorizedAccessException(),
            new ArgumentException(),
            new ArgumentNullException(),
            new PathTooLongException(),
            new FileNotFoundException(),
            new NotSupportedException(),
        };

        foreach (var exception in exceptions) {
            IFileManager exceptionFileManager = new ExceptionFileManager(exception);
            var programManager = new ProgramManager(exceptionFileManager);
            Tuple<int, string> result = programManager.Run("some path");
            Assert.AreEqual(result.Item1, 1);
        }
    }

    [TestMethod]
    public void TestSuccessfulWordCount() {
        IFileManager stringFileManager = new StringFileManager("some string");
        var programManager = new ProgramManager(stringFileManager);
        Tuple<int, string> result = programManager.Run("some path");
        Assert.AreEqual(result.Item1, 0);
    }
}
