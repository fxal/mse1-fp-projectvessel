open CsvReader
[<EntryPoint>]
let main argv =
    printfn "%s\r\n" (i18nNoParameters "gametitle")
    printfn "%s\r\n" (i18nNoParameters "lore")
    printfn "%s" (i18nNoParameters "welcometext")
    printf "> "

    let initialState = Domain.init ()
    Repl.loop initialState
    0 // return an integer exit code
