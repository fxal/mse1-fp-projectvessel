module Repl

open System
open Domain
open Parser

type Message =
    | DomainMessage of Domain.Message
    | HelpRequested
    | NotParsable of string

type State = Domain.State

let testPlanet: Domain.Planet =
    { Name = "Cononis"
      PopulationName = "Canicuties"
      PopulationCount = 399324
      KSRLevel = 30
      Description = "They are super intelligent" }

let read (input: string) =
    match input with
    | ConfirmEradication -> Domain.EradicatePlanet testPlanet |> DomainMessage
    | SelfDestruct -> Domain.SelfDestruct |> DomainMessage
    | Visit room -> Domain.Visit room |> DomainMessage
    | LogOff -> Domain.LogOff |> DomainMessage
    | Help -> HelpRequested
    | ParseFailed -> NotParsable input



open Microsoft.FSharp.Reflection

// TODO: change (as not all commands should be listed)
let createHelpText (): string =
    FSharpType.GetUnionCases typeof<Domain.Message>
    |> Array.map (fun case -> case.Name)
    |> Array.fold (fun prev curr -> prev + " " + curr) ""
    |> (fun s -> s.Trim() |> sprintf "Known commands are: %s")




let evaluate (update: Domain.Message -> State -> State) (state: State) (msg: Message) =
    match msg with
    | DomainMessage msg ->
        let newState = update msg state

        let message =
            sprintf "The message was %A. New state is %A" msg newState

        (newState, message)
    | HelpRequested ->
        let message = createHelpText ()
        (state, message)
    | NotParsable originalInput ->
        let message =
            sprintf
                """"%s" was not parsable. %s"""
                originalInput
                "You can get information about known commands by typing \"Help\""

        (state, message)

let print (state: State, msg: string) =
    printfn "%s\n" msg
    printf "> "

    state

let rec loop (state: State) =
    Console.ReadLine()
    |> read
    |> evaluate Domain.update state
    |> print
    |> loop
