module Parser

open System

let safeEquals (it: string) (theOther: string) =
    String.Equals(it, theOther, StringComparison.OrdinalIgnoreCase)

[<Literal>]
let HelpLabel = "Help"

let (|ConfirmEradication|SelfDestruct|Visit|LogOff|Help|ParseFailed|) (input: string) =
    let tryParseInt (arg: string) valueConstructor =
        let (worked, arg') = Int32.TryParse arg
        if worked then valueConstructor arg' else ParseFailed

    let parts = input.Split(' ') |> List.ofArray

    match parts with
    | [ verb ] when safeEquals verb (nameof Domain.EradicatePlanet) -> ConfirmEradication
    | [ verb ] when safeEquals verb (nameof Domain.SelfDestruct) -> SelfDestruct
    | [ verb ] when safeEquals verb HelpLabel -> Help
    | [ verb; arg ] when safeEquals verb (nameof Domain.Visit) -> Visit Domain.TechAss // TODO: visit room based on string
    | [ verb ] when safeEquals verb (nameof Domain.LogOff) -> LogOff
    | _ -> ParseFailed
