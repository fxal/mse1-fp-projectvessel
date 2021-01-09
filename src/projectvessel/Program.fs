open CsvReader
[<EntryPoint>]
let main argv =
    printfn "%s\n%s" i18n.["welcometext.line1"] i18n.["welcometext.line2"]
    printf "> "

    let initialState = Domain.init ()
    Repl.loop initialState
    0 // return an integer exit code
