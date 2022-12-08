module WordCount

open System
open System.IO

// Count word occurances across multiple strings
let wordCounts (lines: seq<string>) =
    lines 
    |> Seq.fold (fun acc line -> Seq.append acc (line.Split(' ', StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries))) Seq.empty<string>
    |> Seq.countBy id

// Having readLines as argument allows for file reading to be abstracted away
// to any function, making it possible to test exception handling too.
let fileWordCounts (readLines: string -> seq<string>) (path: string) =
    try
        (0, readLines path 
            |> wordCounts
            |> Seq.map (fun (name, count) -> (string count) + ": " + name)
            |> String.concat "\n")
    with ex ->
        (1, "An error occurred while processing file " + path + "\nGot message: " + ex.Message)

[<EntryPoint>]
let main args =
    match args with
    | [|path|] ->
        let (status_code, message) = fileWordCounts File.ReadLines path
        printfn "%s" message
        status_code
    | _ -> 
        printfn "Error: Expected one argument: 'FILEPATH'"
        1
