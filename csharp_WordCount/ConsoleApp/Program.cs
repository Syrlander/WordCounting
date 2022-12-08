using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp;

/* Interface to abstract away the external filesystem dependency. */
public interface IFileManager {
    StreamReader OpenText(string filepath);
}

public class FileManager : IFileManager {
    public StreamReader OpenText(string path) {
        return File.OpenText(path);
    }
}

public class WordCounter {
    // Count word occurrances in a single line and store in given reference dict.
    public void WordCounts(string line, ref IDictionary<string, int> wordCounts) {        
        var words = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words) {
            if (wordCounts.ContainsKey(word)) {
                wordCounts[word]++;
            } else {
                wordCounts.Add(word, 1);
            }
        }
    }

    // Parse word counts from a file line-by-line
    // Uses the IFileManager interface to make it possible to test exception handling
    public IDictionary<string, int> WordCountsFromFile(string path, IFileManager fileManager) {
        IDictionary<string, int> wordCounts = new Dictionary<string, int>();

        using (StreamReader reader = fileManager.OpenText(path)) {
            string? line;
            while ((line = reader.ReadLine()) != null) {
                WordCounts(line, ref wordCounts);
            }
        }

        return wordCounts;
    }
}

public class ProgramManager {
    IFileManager fileManager;
    
    public ProgramManager() {
        fileManager = new FileManager();
    }

    public ProgramManager(IFileManager fileManager) {
        this.fileManager = fileManager;
    }

    public Tuple<int, string> Run(string path) {
        var counter = new WordCounter();

        IDictionary<string, int> wordCounts = new Dictionary<string, int>();
        try {
            wordCounts = counter.WordCountsFromFile(path, fileManager);
        } catch (Exception exception) {
            return new Tuple<int, string>(1, exception.Message);
        }

        var stringBuilder = new StringBuilder();
        foreach (KeyValuePair<string, int> countPair in wordCounts) {
            stringBuilder.Append($"{countPair.Value}: {countPair.Key}\n");
        }
        return new Tuple<int, string>(0, stringBuilder.ToString());
    }
}

class Program {
    static int Main(string[] args) {
        if (args.Length != 1) {
            Console.WriteLine("Error: Expected one commandline argument: 'FILEPATH'");
            return 1;
        }

        string path = args[0];
        Tuple<int, string> result = new ProgramManager().Run(path);
        Console.Write(result.Item2);
        return result.Item1;
    }
}
