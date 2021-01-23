module Repl

open System
open Domain
open Parser

open CsvReader
open Types
open projectvessel

type Message =
    | DomainMessage of Domain.Message
    | NotParsable of string

type State = Domain.State

let testPlanet: Types.Planet =
    { ID = 1
      Name = "Cononis"
      PopulationName = "Canicuties"
      PopulationCount = 399324
      KSRLevel = 30
      Description = "They are super intelligent" }


let read (input: string) =
    match input with
    | ConfirmEradication -> Domain.ConfirmEradication |> DomainMessage
    | SelfDestruct -> Domain.SelfDestruct |> DomainMessage
    | Visit room -> Domain.Visit room |> DomainMessage
    | Calibrate numInput -> Domain.Calibrate numInput |> DomainMessage
    | LeaveHyperspace -> Domain.LeaveHyperspace |> DomainMessage
    | ParseFailed -> NotParsable input

let evaluate (update: Domain.Message -> State -> State) (state: State) (msg: Message) =
    match msg with
    | DomainMessage msg ->
        let newState = update msg state

        let message =
            match newState.CurrRoom with
            | Hyperspace -> GameText.enteringHyperspace newState.EradicatedPlanets.Head newState.EradicatedLifeforms
            | AtPlanet -> GameText.atPlanet newState.AllPlanets.[string state.CurrPlanet] newState
            | TechAss -> GameText.enteringTechAss newState
            | ThreatAss -> GameText.enteringThreatAss
            | DamageAss -> GameText.enteringDamageAss newState
            | PerfectionAss -> GameText.enteringPerfectionAss
            | VictoryRoom -> i18nWithParameters None "gameover" [ string state.EradicatedLifeforms ]
            | _ -> sprintf "The message was %A. New state is %A" msg newState

        (newState, message)
    | NotParsable originalInput ->
        let message =
            sprintf
                """"%s" was not parsable. %s"""
                originalInput
                "You can get information about known commands by reading the info text"

        (state, message)

let print (state: State, msg: string) =
    printfn "%s\n" msg
    printf "---------- \n> "

    state

let rec loop (state: State) =
    Console.ReadLine()
    |> read
    |> evaluate Domain.update state
    |> print
    |> loop
