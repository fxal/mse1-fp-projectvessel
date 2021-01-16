module CsvReader

open FSharp.Data

// Csv reader for i18n (key, value files providing text)
// Two functions are exposed:
// - i18nNoParameters (i18nkey : string) : string
//   usage: i18nNoParameters "myi18nKey"
//
// - i18nWithParameters (i18nMap : Map<string, string> option) (i18nkey : string) (replacements : string list) : string
//   usage: i18nWithParameters (Some map|None) "myi18nKey" [ "string list of replacements" ]
//   If map is None: fallback i18n map is used
//   Replacements are replaced in order of occurrence in list
//   Occurrences are expected to be in original i18n value like this: {int}
//   So a i18n value like "this is a {0} test {1}" called with two replacements "DINGS", "DANGS" will result in:
//    "this is a DINGS test DANGS"

let noValueFoundForKeyMessage = "No value found for key"

type csvloader = CsvProvider<Schema = "key (string), value (string)", HasHeaders=false>
let i18n = csvloader.Load("../../../../../data/i18n.csv").Rows
            |> Seq.map(fun row -> (row.Key, row.Value))
            |> Map.ofSeq

let rec private reci18nWithParametersHelper (input : string) (replacements : string list) (index : int) =
    match replacements with
    | [] -> input
    | r::replacements ->
        reci18nWithParametersHelper (input.Replace("{" + string index + "}", r)) replacements (index + 1)

let private i18nWithMapAndParameters (i18nMap : Map<string, string>) (i18nkey : string) (replacements : string list) =
    let value =
        try
            i18nMap.[i18nkey]
        with
        | :? System.Collections.Generic.KeyNotFoundException -> noValueFoundForKeyMessage

    match replacements with
    | [] -> value
    | r::replacements ->
        reci18nWithParametersHelper (value.Replace("{0}", r)) replacements 1

let i18nWithParameters (i18nMap : Map<string, string> option) (i18nkey : string) (replacements : string list) : string =
    match i18nMap with
    | None -> i18nWithMapAndParameters i18n i18nkey replacements
    | Some map -> i18nWithMapAndParameters map i18nkey replacements
        
let i18nNoParameters (i18nkey : string) =
    try
        i18n.[i18nkey]
    with
    | :? System.Collections.Generic.KeyNotFoundException -> noValueFoundForKeyMessage
