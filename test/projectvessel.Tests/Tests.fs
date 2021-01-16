module projectvessel.Tests

open Xunit
open FsCheck
open CsvReader

[<Fact>]
let ``That the laws of reality still apply`` () =
    Assert.True(1 = 1)

[<Fact>]
let ``That incrementing twice on an initialized counter yields 2`` () =
    let initialState = Domain.init ()

    let actual =
        Domain.update Domain.Increment initialState
        |> Domain.update Domain.Increment

    let expected = 2

    Assert.Equal(expected, actual)

// not the best helper function for property based tests
// because itselve has testable behavior
let getInverse (message : Domain.Message) =
    match message with
    | Domain.Increment -> Domain.Decrement
    | Domain.Decrement -> Domain.Increment
    | Domain.IncrementBy x -> Domain.DecrementBy x
    | Domain.DecrementBy x -> Domain.IncrementBy x

[<Fact>]
let ``That applying the inverse of counter event yields the initial state`` () =
    let prop (message : Domain.Message) =
        let initialState = Domain.init ()

        let actual =
            initialState
            |> Domain.update message
            |> Domain.update (getInverse message)

        actual = initialState

    Check.QuickThrowOnFailure prop

[<Fact>]
let ``That reading the testing csv file yields a csv with one data row`` () =
    let csv = csvloader.Load("../../../../../data/i18n_test.csv")

    let testi18n = csv.Rows |> Seq.map(fun row -> row.Key, row.Value) |> Map.ofSeq
    Assert.Equal("testcontent", testi18n.["testname"])
 
    for row in csv.Filter(fun row -> row.Key = "testname").Rows do
        Assert.Equal("testname", row.Key)
        Assert.Equal("testcontent", row.Value)

    Assert.Equal(2, csv.NumberOfColumns) 
