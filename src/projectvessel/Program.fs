open CsvReader

[<EntryPoint>]
let main argv =
    let allPlanets = CsvReader.planets
    let initialState = Domain.init allPlanets ()

    printfn "%s\r\n" (i18nNoParameters "gametitle")
    printfn "%s\r\n" (i18nNoParameters "lore")
    printfn "%s" (i18nNoParameters "welcometext")
    printf "> "

    Repl.loop initialState
    0 // return an integer exit code
