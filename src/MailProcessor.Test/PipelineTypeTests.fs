module PipelineTypeTests

open NUnit.Framework
open System.Threading

type SeqPipeliner() = 
    member this.getDataSlow() =
        let items = [1..10]
        items 
            |> Seq.map(fun id ->
                            printfn "s:get %A" id
                            Thread.Sleep(10)
                            id)                               
                            
    member this.transformDataSlow (items : seq<int>) =
        items 
            |> Seq.map(fun id ->
                            printfn "s: transform %A" id
                            Thread.Sleep(10)
                            id * 10)              

type ListPipeliner() = 
    member this.getDataSlow() : List<int> =
        let items = [1..10]
        items 
            |> List.map(fun id ->
                            printfn "l:get %A" id
                            Thread.Sleep(10)
                            id)                               
                            
     member this.transformDataSlow (items : List<int>) : List<int> =
        items 
            |> List.map(fun id ->
                            printfn "l: transform %A" id
                            Thread.Sleep(10)
                            id * 10)

[<Test>]
let ``Seq Pipeline tests`` () =
    let items = SeqPipeliner().getDataSlow() |> SeqPipeliner().transformDataSlow
    let lastItem = Seq.last items

    Assert.AreEqual(lastItem,100)