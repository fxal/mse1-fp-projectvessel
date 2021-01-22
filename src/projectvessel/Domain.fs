module Domain

open System.Threading
open Types

type Room =
    | Start
    | Hyperspace
    | AtPlanet
    | VictoryRoom
    | Assessment
    | TechAss
    | ThreadAss
    | DamageAss
    | PerfectionAss

type State =
    { KSRLevel: int
      DamageThreshold: int
      Offset: int
      DamageDetected: bool
      CurrRoom: Room
      AllPlanets: Map<string, Planet>
      CurrPlanet: int
      EradicatedPlanets: Planet List
      mutable StarvedTimer: Timer }

type Message =
    | ConfirmEradication
    | Visit of Room
    | SelfDestruct
    | LeaveHyperspace

let init planetMap (): State =
    { KSRLevel = 2
      DamageThreshold = 10
      Offset = 50
      DamageDetected = false
      CurrRoom = Start
      AllPlanets = planetMap
      CurrPlanet = 1
      EradicatedPlanets = []
      StarvedTimer = null }


// some state condition checks if entering a room is allowed ...
let eradicationAllowed (model: State) = model.CurrRoom = AtPlanet

let visitAllowed (model: State) = model.CurrRoom = Hyperspace

let leaveHyperspaceAllowed (model: State) =
    model.CurrRoom = Start
    || model.CurrRoom = PerfectionAss
    || model.CurrRoom = ThreadAss
    || model.CurrRoom = DamageAss
    || model.CurrRoom = TechAss

let selfDestructAllowed (model: State) =
    model.CurrRoom = Hyperspace
    && model.DamageDetected = true

let checkInput (model: State) (condition: State -> bool) (updatedModel: State) =
    match (condition model) with
    | true -> updatedModel
    | false -> model


let goToVictoryRoom (model: State) =
    model.StarvedTimer.Dispose()

    printfn
        "You Starved due to failing to follow commands. The Vessel Can now no longer fulfill it's directive and will self desctruct."

let update (msg: Message) (model: State): State =

    match model.StarvedTimer with
    | null -> ()
    | _ -> model.StarvedTimer.Dispose()

    model.StarvedTimer <- new Timer(TimerCallback(fun _ -> goToVictoryRoom model), null, 5000, 0)

    match msg with
    | ConfirmEradication ->
        checkInput
            model
            eradicationAllowed
            { model with
                  EradicatedPlanets =
                      model.AllPlanets.[string model.CurrPlanet]
                      :: model.EradicatedPlanets
                  CurrPlanet = model.CurrPlanet + 1
                  CurrRoom = Hyperspace }

    | Visit ass ->
        if visitAllowed model then
            match ass with
            | DamageAss -> { model with CurrRoom = DamageAss }
            | PerfectionAss -> { model with CurrRoom = PerfectionAss }
            | TechAss -> { model with CurrRoom = TechAss }
            | ThreadAss -> { model with CurrRoom = ThreadAss }
            | _ -> model
        else
            model
    | SelfDestruct -> checkInput model selfDestructAllowed { model with CurrRoom = VictoryRoom }
    | LeaveHyperspace -> checkInput model leaveHyperspaceAllowed { model with CurrRoom = AtPlanet }
