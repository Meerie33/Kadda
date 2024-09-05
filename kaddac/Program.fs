open System

printfn "Hello from F#"
let input = Console.ReadLine()

if input = "ok" then 
    printfn "no errors"
else
    printfn $"ERROR: Unexpected {input}, Syntax error"