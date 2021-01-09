module CsvReader

open FSharp.Data

// Csv reader for i18n (key, value files providing text)
// open CsvReader, then use with i18n.["key"] anywhere and get a string

type csvloader = CsvProvider<Schema = "key (string), value (string)", HasHeaders=false>
let i18n = csvloader.Load("../../../../../data/i18n.csv").Rows
            |> Seq.map(fun row -> row.Key, row.Value)
            |> Map.ofSeq

