module Domain
open System.Threading

type Planet =
    { Name: string
      PopulationName: string
      PopulationCount: int
      KSRLevel: int
      Description: string }

type Room =
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
      EradicatedPlanets: Planet List
      mutable StarvedTimer: Timer}

type Message =
    | EradicatePlanet of Planet
    | Visit of Room
    | SelfDestruct
    | LogOff


let init (): State =
    { KSRLevel = 2
      DamageThreshold = 10
      Offset = 50
      DamageDetected = false
      CurrRoom = AtPlanet
      EradicatedPlanets = []
      StarvedTimer = null}


let goToVictoryRoom (model: State) =
          model.StarvedTimer.Dispose()
          printfn "You Starved due to failing to follow commands. The Vessel Can now no longer fulfill it's directive and will self desctruct."

let update (msg: Message) (model: State): State =

    match model.StarvedTimer with
    | null -> ()
    | _ -> model.StarvedTimer.Dispose()

    model.StarvedTimer <- new Timer(
                             TimerCallback (fun _ -> goToVictoryRoom model), null, 5000,0)

    match msg with
    | EradicatePlanet planet ->
        { model with
              EradicatedPlanets = planet :: model.EradicatedPlanets
              CurrRoom = Hyperspace }
    | Visit ass ->
        match ass with
        | DamageAss -> { model with CurrRoom = DamageAss }
        | PerfectionAss -> { model with CurrRoom = PerfectionAss }
        | TechAss -> { model with CurrRoom = TechAss }
        | ThreadAss -> { model with CurrRoom = ThreadAss }
        | _ -> model
    | SelfDestruct -> { model with CurrRoom = VictoryRoom } // TODO: implement check if allowed
    | LogOff -> { model with CurrRoom = AtPlanet }
