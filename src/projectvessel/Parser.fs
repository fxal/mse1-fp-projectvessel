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

    let tryParseAssessmentRoom (arg: string) valueConstructor =
        let (worked, arg') =
            match arg with
            | "Tech" -> (true, Domain.TechAss)
            | "Perfection" -> (true, Domain.PerfectionAss)
            | "Thread" -> (true, Domain.ThreadAss)
            | "Damage" -> (true, Domain.DamageAss)
            | _ -> (false, Domain.Hyperspace)

        if worked then valueConstructor arg' else ParseFailed

    match parts with
    | [ verb ] when safeEquals verb (nameof Domain.EradicatePlanet) -> ConfirmEradication
    | [ verb ] when safeEquals verb (nameof Domain.SelfDestruct) -> SelfDestruct
    | [ verb ] when safeEquals verb HelpLabel -> Help
    | [ verb; arg ] when safeEquals verb (nameof Domain.Visit) -> tryParseAssessmentRoom arg (fun value -> Visit value)
    | [ verb ] when safeEquals verb (nameof Domain.LogOff) -> LogOff
    | _ -> ParseFailed
