module Domain

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
      EradicatedPlanets: Planet List }

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
      EradicatedPlanets = [] }


let update (msg: Message) (model: State): State =
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
