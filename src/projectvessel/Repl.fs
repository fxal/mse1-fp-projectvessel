module Repl

open System
open Domain
open Parser

open CsvReader
open Types

type Message =
    | DomainMessage of Domain.Message
    | HelpRequested
    | NotParsable of string

type State = Domain.State

let testPlanet: Types.Planet =
    { ID = 1
      Name = "Cononis"
      PopulationName = "Canicuties"
      PopulationCount = 399324
      KSRLevel = 30
      Description = "They are super intelligent" }


let createPlanetText (planet: Planet) =
    i18nWithParameters
        None
        "planet.info"
        [ string planet.PopulationCount
          planet.PopulationName
          planet.Description
          string planet.KSRLevel ]


let read (input: string) =
    match input with
    | ConfirmEradication ->
        Domain.ConfirmEradication testPlanet
        |> DomainMessage
    | SelfDestruct -> Domain.SelfDestruct |> DomainMessage
    | Visit room -> Domain.Visit room |> DomainMessage
    | LeaveHyperspace -> Domain.LeaveHyperspace |> DomainMessage
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
            match newState.CurrRoom with
            | Hyperspace -> i18nNoParameters "planet.planet1.line1"
            | AtPlanet -> createPlanetText newState.AllPlanets.[string state.CurrPlanet]
            | _ -> sprintf "The message was %A. New state is %A" msg newState

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
