module MailProcessor

open System
open System.IO
open Common
//open Nessos.FsPickler
//open Nessos.FsPickler.Json
open Newtonsoft.Json

type Processor() =

    member this.GetItems (?fullPath : string) : List<EMail> =
        let path = defaultArg fullPath (__SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")
        use textReader = new StreamReader(path)
        let json = textReader.ReadToEnd()

        JsonConvert.DeserializeObject<List<EMail>>(json)

     member this.GetAllItems() : List<EMail> =
        this.GetItems()

    member this.LastMail(items : List<EMail>) : EMail =
        items
            |> List.rev
            |> List.head

    member this.FirstMailDate(items : List<EMail>) =
        items
            |> List.head
            |> (fun x -> System.DateTime.Parse x.Date)

    member this.DaysSinceFirstMail(items : List<EMail>) : int =
        items
            |> List.head
            |> (fun x -> (int)(DateTime.Now.Date - (System.DateTime.Parse x.Date).Date).TotalDays)

    member this.DaysThatHaveSentMails(items : List<EMail>) =
        items
            |> List.map((fun x -> (System.DateTime.Parse x.Date).Date))
            |> Seq.distinct
            |> Seq.length

    member this.LastMailDate(items : List<EMail>) : DateTime =
        items
            |> List.sortBy(fun x -> System.DateTime.Parse x.Date)
            |> List.rev
            |> List.head
            |> (fun x -> System.DateTime.Parse x.Date)

     member this.TotalMailsBySender(items : List<EMail>) =
        items
            |> Seq.groupBy(fun x -> x.From)
            |> Seq.map(fun (name, mails) -> (name, Seq.length mails))
            |> Seq.sortBy snd
            |> Seq.toList
            |> List.rev

    member this.TotalMailsBySenderByYears(items : List<EMail>) =
        items
            |> Seq.groupBy(fun x -> x.From)
            |> Seq.map(fun (name, mails) -> (name, Seq.groupBy (fun x-> (System.DateTime.Parse x.Date).Year) mails))
            |> Seq.map(fun (name, mailsByYear) -> (name, Seq.map (fun (year, mails) -> (year, Seq.length mails)) mailsByYear))
            |> Seq.toList
            |> List.rev

    // Group by year|month
    member this.TotalMailsBySenderByMonths(items : List<EMail>) =
        items
            |> Seq.groupBy(fun x -> x.From)
            |> Seq.map(fun (name, mails) -> (name, Seq.groupBy (fun x -> (System.DateTime.Parse x.Date).Year.ToString() + "|" + (System.DateTime.Parse x.Date).Month.ToString()) mails))
            |> Seq.map(fun (name, mailsByYear) -> (name, Seq.map (fun (year, mails) -> (year, Seq.length mails)) mailsByYear))
            |> Seq.toList
            |> List.rev

     member this.TotalMailsBySenderByYear(items : List<EMail>, year : int) =
        items
            |> Seq.filter(fun x -> (System.DateTime.Parse x.Date).Year = year)
            |> Seq.toList
            |> this.TotalMailsBySender

     member this.TotalMailsByYear(items : List<EMail>) =
        items
            |> Seq.groupBy(fun x -> (System.DateTime.Parse x.Date).Year)
            |> Seq.map(fun (key, value) -> (key, Seq.length value))
            |> Seq.toList

    member this.YearsThatHaveData(items : List<EMail>) =
        items
            |> Seq.map(fun x -> (System.DateTime.Parse x.Date))
            |> Seq.map(fun x -> x.Year)
            |> Seq.distinct