module projectvessel.GameText

open CsvReader
open Types
open Domain

let atPlanet (planet: Planet) (state: State) =
    i18nWithParameters
        None
        "planet.info"
        [ string planet.PopulationCount
          planet.PopulationName
          planet.Description
          string planet.KSRLevel
          if state.DamageDetected && state.KSRLevel > (planet.KSRLevel - state.Offset) then "a threat" else "no threat"
          if state.DamageDetected then "damage" else "no damage"
          if state.KSRLevel > (planet.KSRLevel - state.Offset) then "superior" else "inferior"
          if state.DamageDetected && state.KSRLevel > (planet.KSRLevel - state.Offset) then (i18nNoParameters "planet.action.selfdestruct") else (i18nNoParameters "planet.action.eradicate")]

let enteringHyperspace (planet: Planet) (eradicatedLifeforms: uint32) =
    i18nWithParameters
        None
        "hyperspace.successfullyEradicated"
        [ string planet.PopulationCount
          planet.PopulationName
          planet.Name ]
    + "\n"
    + i18nWithParameters None "hyperspace.welcome" [ string eradicatedLifeforms ]
    + "\n"
    + i18nNoParameters "hyperspace.commands"

let enteringTechAss (state: State) =
    i18nWithParameters None "techassessment.welcome" [ string state.KSRLevel ]
    + "\n"
    + i18nNoParameters "techassessment.explainkardashian"
    + i18nWithParameters None "techassessment.calibrate" [ string state.Offset ]
    + "\n Type 'LeaveHyperspace' or 'Calibrate [number]'"

let enteringDamageAss (state: State) =
    i18nNoParameters "damageassessment.welcome" 
    + "\n"
    + i18nWithParameters None "damageassessment.explain" [ string state.DamageThreshold ]
    + "\nType 'LeaveHyperspace' or 'Calibrate [number]'"

let enteringThreatAss =
    i18nNoParameters "threatassessment.welcome"
    + "\nType 'LeaveHyperspace'"

let enteringPerfectionAss =
    i18nNoParameters "perfectionassessment.welcome"
    + "\nType 'LeaveHyperspace'"
