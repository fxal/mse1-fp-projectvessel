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

let enteringHyperspace (planet: Planet) (eradicatedPlanets: Planet List) =
    (i18nWithParameters
        None
         "hyperspace.successfullyEradicated"
         [ string planet.PopulationCount
           planet.PopulationName
           planet.Name ])
    + "\n"
    + (i18nWithParameters None "hyperspace.welcome" [ string eradicatedPlanets.Length ])
    + "\n"
    + (i18nNoParameters "hyperspace.commands")