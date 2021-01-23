module Domain

open System.Threading
open Types
open CsvReader

type Room =
    | Start
    | Hyperspace
    | AtPlanet
    | VictoryRoom
    | Assessment
    | TechAss
    | ThreatAss
    | DamageAss
    | PerfectionAss

type State =
    { KSRLevel: int
      DamageThreshold: int
      DamageReceived: int
      Offset: int
      DamageDetected: bool
      CurrRoom: Room
      AllPlanets: Map<string, Planet>
      CurrPlanet: int
      EradicatedPlanets: Planet List
      EradicatedLifeforms: uint32 // 0 to 4.294.967.295
      mutable StarvedTimer: Timer }

type Message =
    | ConfirmEradication
    | Visit of Room
    | Calibrate of int
    | SelfDestruct
    | LeaveHyperspace

let init planetMap (): State =
    { KSRLevel = 2
      DamageThreshold = 100000
      DamageReceived = 1000
      Offset = 0
      DamageDetected = false
      CurrRoom = Start
      AllPlanets = planetMap
      CurrPlanet = 1
      EradicatedPlanets = []
      EradicatedLifeforms = 0u
      StarvedTimer = null }


// some state condition checks if entering a room is allowed ...
let eradicationAllowed (model: State) = model.CurrRoom = AtPlanet

let visitAssessmentAllowed (model: State) = model.CurrRoom = Hyperspace

let calibrationAllowed (model: State) =
    model.CurrRoom = DamageAss
    || model.CurrRoom = TechAss

let leaveHyperspaceAllowed (model: State) =
    model.CurrRoom = Start
    || model.CurrRoom = PerfectionAss
    || model.CurrRoom = ThreatAss
    || model.CurrRoom = DamageAss
    || model.CurrRoom = TechAss

let selfDestructAllowed (model: State) =
    let currentPlanet =
        model.AllPlanets.[string model.CurrPlanet]

    model.CurrRoom = AtPlanet
    && model.DamageDetected = true
    && model.KSRLevel > (currentPlanet.KSRLevel - model.Offset)


let checkInput (model: State) (isAllowedCondition: State -> bool) (updatedModel: State) =
    match (isAllowedCondition model) with
    | true -> updatedModel
    | false ->
        printfn "%s" (i18nNoParameters "nopermission")
        model


let starve (model: State) =
    model.StarvedTimer.Dispose()
    printfn "%s" (i18nNoParameters "starve")

let update (msg: Message) (model: State): State =
    
    match model.StarvedTimer with
    | null -> ()
    | _ -> model.StarvedTimer.Dispose()

    model.StarvedTimer <-
        new Timer(TimerCallback(fun _ -> starve { model with CurrRoom = VictoryRoom }), null, 60000, 0)
    
    match msg with
    | ConfirmEradication ->
        checkInput
            model
            eradicationAllowed
            { model with
                  EradicatedPlanets =
                      model.AllPlanets.[string model.CurrPlanet]
                      :: model.EradicatedPlanets
                  EradicatedLifeforms =
                      model.EradicatedLifeforms
                      + uint32
                          model.AllPlanets.[string model.CurrPlanet]
                              .PopulationCount
                  CurrPlanet =
                      if model.CurrPlanet > model.AllPlanets.Count
                      then 1
                      else model.CurrPlanet + 1
                  CurrRoom = Hyperspace }

    | Visit ass ->
        if visitAssessmentAllowed model then
            match ass with
            | DamageAss -> { model with CurrRoom = DamageAss }
            | PerfectionAss -> { model with CurrRoom = PerfectionAss }
            | TechAss -> { model with CurrRoom = TechAss }
            | ThreatAss -> { model with CurrRoom = ThreatAss }
            | _ -> model
        else
            model
    | Calibrate numInput ->
        if calibrationAllowed model then
            match model.CurrRoom with
            | TechAss -> { model with Offset = numInput }
            | DamageAss ->
                { model with
                      DamageThreshold = numInput }
            | _ -> model
        else
            printfn "%s" (i18nNoParameters "nopermission")
            model
    | SelfDestruct -> checkInput model selfDestructAllowed { model with CurrRoom = VictoryRoom }
    | LeaveHyperspace ->
        checkInput
            model
            leaveHyperspaceAllowed
            { model with
                  CurrRoom = AtPlanet
                  DamageDetected = model.DamageThreshold < model.DamageReceived }
