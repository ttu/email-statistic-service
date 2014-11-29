module MailProcessor.Test.ProcessJson

open System
open NUnit.Framework
open MailProcessor
open MailReader

[<Test>]
let ``Process items`` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(__SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")

    let count = Seq.length items
    Assert.IsTrue(count > 0)

[<Test>]
let ``Get last date`` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(__SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")
    let lastDate = processor.LastMailDate(items)

    Assert.IsTrue(lastDate.Ticks > 0L)

[<Test>]
let ``DaysSinceFirstMail `` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(__SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")

    let totalDuration = processor.DaysSinceFirstMail(items)
    let realDays = processor.DaysThatHaveSentMails(items)

    Assert.IsTrue(totalDuration > realDays)

[<Test>]
let ``YearsThatHaveData`` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(__SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")

    let years = processor.YearsThatHaveData(items)

    Assert.IsTrue(Seq.length years > 0)

[<Test>]
let ``All statistics`` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(__SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")

    let count = Seq.length items

    let totalDuration = processor.DaysSinceFirstMail(items)
    let realDays = processor.DaysThatHaveSentMails(items)

    let yearTotals = processor.TotalMailsByYear(items)

    let bySenderByYears = processor.TotalMailsBySenderByYears(items)
    let bySenderByMonths = processor.TotalMailsBySenderByMonths(items)

    let grouped = processor.TotalMailsBySender(items)
    let g2010 = processor.TotalMailsBySenderByYear(items, 2010)
    let g2011 = processor.TotalMailsBySenderByYear(items, 2011)
    let g2012 = processor.TotalMailsBySenderByYear(items, 2012)
    let g2013 = processor.TotalMailsBySenderByYear(items, 2013)
    let g2014 = processor.TotalMailsBySenderByYear(items, 2014)

    yearTotals |> List.iter  (fun (k,v) -> printfn "%A" k)

    printfn ""

    yearTotals |> List.iter  (fun (k,v) -> printfn "%A" v)

    printfn ""

    grouped |> List.iter  (fun (k,v) -> printfn "%A" k)

    printfn ""

    grouped |> List.iter  (fun (k,v) -> printfn "%A" k)

    Assert.IsTrue(true)