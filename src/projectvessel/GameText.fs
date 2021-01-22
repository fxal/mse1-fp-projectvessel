module projectvessel.GameText

open CsvReader
open Types

let atPlanet (planet: Planet) =
    i18nWithParameters
        None
        "planet.info"
        [ string planet.PopulationCount
          planet.PopulationName
          planet.Description
          string planet.KSRLevel ]

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

let enteringTechAss (ksrLevel: int) =
    i18nWithParameters None "techassessment.welcome" [ string ksrLevel ]
    + "\n"
    + i18nNoParameters "techassessment.explainkardashian"
    + "\n Type 'LeaveHyperspace' or 'Calibrate [number]'"

let enteringDamageAss =
    i18nWithParameters None "damageassessment.welcome" []
    + "\n"
    + i18nNoParameters "damageassessment.explain"
    + "\n Type 'LeaveHyperspace' or 'Calibrate [number]'"

let enteringThreatAss =
    i18nNoParameters "threatassessment.welcome"
    + "\n Type 'LeaveHyperspace'"

let enteringPerfectionAss =
    i18nNoParameters "perfectionassessment.welcome"
    + "\n Type 'LeaveHyperspace'"
