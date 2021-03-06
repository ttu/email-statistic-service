﻿module MailProcessor

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
            |> (fun x -> x.Date)

    member this.DaysSinceFirstMail(items : List<EMail>) : int =
        items
            |> List.head
            |> (fun x -> (int)(DateTime.Now.Date - x.Date).TotalDays)

    member this.DaysThatHaveSentMails(items : List<EMail>) =
        items
            |> List.map(fun x -> x.Date.Date)
            |> Seq.distinct
            |> Seq.length

    member this.MessagesPerDay(items : List<EMail>) : int = 
        let days = this.DaysThatHaveSentMails(items)
        items.Length / days

    member this.MasseagesLast24h(items : List<EMail>) : int = 
        items
            |> Seq.filter(fun x -> x.Date.Date > DateTime.Now.AddDays(-1.0))
            |> Seq.length

    member this.MessagesToday(items : List<EMail>) : int = 
        items
            |> Seq.filter(fun x -> x.Date.Date = DateTime.Now.Date)
            |> Seq.length

     member this.SendersAt(items : List<EMail>, filterFunc) = 
        items
            |> Seq.filter(filterFunc)
            |> Seq.groupBy(fun x -> x.From)
            |> Seq.map(fun (name, mails) -> (name, Seq.length mails))
            |> Seq.sortBy snd
            |> Seq.toList
            |> List.rev

    member this.SendersToday(items : List<EMail>) = 
        this.SendersAt(items, fun x -> x.Date.Date = DateTime.Now.Date)
         
    member this.SendersLast24h(items : List<EMail>) = 
        this.SendersAt(items, fun x -> x.Date.Date > DateTime.Now.AddDays(-1.0))

    member this.TopSenderToday(items : List<EMail>) : string * int = 
        items
            |> this.SendersToday
            |> (fun x -> match List.length x with
                    | 0 -> "", 0
                    | _ -> x.Head
                )

    member this.LastMailDate(items : List<EMail>) : DateTime =
        items
            |> List.sortBy(fun x -> x.Date)
            |> List.rev
            |> List.head
            |> (fun x -> x.Date)

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
            |> Seq.map(fun (name, mails) -> (name, Seq.groupBy (fun x-> x.Date.Year) mails))
            |> Seq.map(fun (name, mailsByYear) -> (name, Seq.map (fun (year, mails) -> (year, Seq.length mails)) mailsByYear))
            |> Seq.toList
            |> List.rev

    // Group by year|month
    member this.TotalMailsBySenderByMonths(items : List<EMail>) =
        items
            |> Seq.groupBy(fun x -> x.From)
            |> Seq.map(fun (name, mails) -> (name, Seq.groupBy (fun x -> x.Date.Year.ToString() + "|" + x.Date.Month.ToString()) mails))
            |> Seq.map(fun (name, mailsByYear) -> (name, Seq.map (fun (year, mails) -> (year, Seq.length mails)) mailsByYear))
            |> Seq.toList
            |> List.rev

    // Year, Weekday, Total mails
    member this.MailsByWeekdays(items : List<EMail>) =
        items
            |> Seq.groupBy(fun x -> x.Date.Year)
            |> Seq.map(fun (year, mails) -> (year, Seq.sortBy (fun (x,y) -> x) (Seq.groupBy (fun x -> x.Date.DayOfWeek) mails)))
            |> Seq.map(fun (year, mailsByWeekday) -> (year, Seq.map (fun (day, mails) -> (day, Seq.length mails)) mailsByWeekday))
            |> Seq.toList

    // Year, Weekday, Percent from that year's mails
    member this.MailsByWeekdaysPercent(items : List<EMail>) =
        items
            |> Seq.groupBy(fun x -> x.Date.Year)
            |> Seq.map(fun (year, mails) -> (year, Seq.sortBy (fun (x,y) -> x) (Seq.groupBy (fun x -> x.Date.DayOfWeek) mails)))
            |> Seq.map(fun (year, mailsByWeekday) -> (year, Seq.map (fun (day, mails) -> (day, (float(Seq.length mails) / Seq.reduce (fun sum next -> sum + next) (Seq.map (fun (day, mails) -> float(Seq.length mails)) mailsByWeekday) ))) mailsByWeekday))
            |> Seq.toList

     member this.TotalMailsBySenderByYear(items : List<EMail>, year : int) =
        items
            |> Seq.filter(fun x -> x.Date.Year = year)
            |> Seq.toList
            |> this.TotalMailsBySender

     member this.TotalMailsByYear(items : List<EMail>) =
        items
            |> Seq.groupBy(fun x -> x.Date.Year)
            |> Seq.map(fun (key, value) -> (key, Seq.length value))
            |> Seq.toList

     member this.TotalMailsByYearAndMonth(items : List<EMail>) =
        items
            |> Seq.groupBy(fun x -> x.Date.Year)
            |> Seq.map(fun (year, mails) -> (year, Seq.sortBy (fun (x, y) -> x) (Seq.groupBy (fun x -> x.Date.Month) mails)))
            |> Seq.map(fun (year, monthlyMails) -> (year, Seq.map(fun (month, mails) -> (month, Seq.length mails)) monthlyMails))
            |> Seq.toList

    member this.YearsThatHaveData(items : List<EMail>) =
        items
            |> Seq.map(fun x -> x.Date)
            |> Seq.map(fun x -> x.Year)
            |> Seq.distinct