module Parser

open System

let safeEquals (it: string) (theOther: string) =
    String.Equals(it, theOther, StringComparison.OrdinalIgnoreCase)

[<Literal>]
let HelpLabel = "Help"

let (|ConfirmEradication|SelfDestruct|Visit|LeaveHyperspace|Calibrate|Help|ParseFailed|) (input: string) =
    let tryParseInt (arg: string) valueConstructor =
        let (worked, arg') = Int32.TryParse arg
        if worked then valueConstructor arg' else ParseFailed

    let parts = input.Split(' ') |> List.ofArray

    let tryParseAssessmentRoom (arg: string) valueConstructor =
        let (worked, arg') =
            match arg with
            | "Tech" -> (true, Domain.TechAss)
            | "Perfection" -> (true, Domain.PerfectionAss)
            | "Threat" -> (true, Domain.ThreatAss)
            | "Damage" -> (true, Domain.DamageAss)
            | _ -> (false, Domain.Hyperspace)

        if worked then valueConstructor arg' else ParseFailed

    match parts with
    | [ verb ] when safeEquals verb (nameof Domain.ConfirmEradication) -> ConfirmEradication
    | [ verb ] when safeEquals verb (nameof Domain.SelfDestruct) -> SelfDestruct
    | [ verb ] when safeEquals verb HelpLabel -> Help
    | [ verb; arg ] when safeEquals verb (nameof Domain.Visit) -> tryParseAssessmentRoom arg (fun value -> Visit value)
    | [ verb; arg ] when safeEquals verb (nameof Domain.Calibrate) -> tryParseInt arg (fun value -> Calibrate value)
    | [ verb ] when safeEquals verb (nameof Domain.LeaveHyperspace) -> LeaveHyperspace
    | _ -> ParseFailed
