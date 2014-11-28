module PipelineTests

open NUnit.Framework
open System.Threading

// Sequence can process items when it gets those
// List will first process all and then continue to next action

module SeqPipeliner = 
    let getDataSlow =
        let items = [1..10]
        items 
            |> Seq.map(fun id ->
                            printfn "s: get %A" id
                            Thread.Sleep(200)
                            id)                               
                            
    let transformDataSlow (items : seq<int>) =
        items 
            |> Seq.map(fun id ->
                            printfn "s: transform %A" id
                            Thread.Sleep(200)
                            id * 10)              

module ListPipeliner = 
    let getDataSlow =
        let items = [1..10]
        // Because of List this is evaluated immediately
        items 
            |> List.map(fun id ->
                            printfn "l: get %A" id
                            Thread.Sleep(200)
                            id)                               
                            
    let transformDataSlow (items : List<int>) =
        items 
            |> List.map(fun id ->
                            printfn "l: transform %A" id
                            Thread.Sleep(200)
                            id * 10)              

[<Test>]
let ``Pipeline tests 1`` () =
    let items = SeqPipeliner.getDataSlow
    let lastItem = Seq.last items

    Assert.AreEqual(lastItem,10)

[<Test>]
let ``Seq Pipeline tests`` () =
    let items = SeqPipeliner.getDataSlow
                    |> SeqPipeliner.transformDataSlow

    let lastItem = Seq.last items

    Assert.AreEqual(lastItem,100)

[<Test>]
let ``List Pipeline tests`` () =
    let items = ListPipeliner.getDataSlow
                    |> ListPipeliner.transformDataSlow

    let lastItem = Seq.last items

    Assert.AreEqual(lastItem,100)

[<Test>]
let ``Mixed Pipeline tests`` () =
    let items = SeqPipeliner.getDataSlow
                    |> Seq.toList // This will force to "collect" all from seq getDataSlow
                    |> ListPipeliner.transformDataSlow

    let lastItem = Seq.last items

    Assert.AreEqual(lastItem,100)


