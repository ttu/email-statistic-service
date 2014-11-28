// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#I @"c:\temp\MailProcessor\packages\Newtonsoft.Json.6.0.5\lib\net45\"
#r @"Newtonsoft.Json.dll"
#r @"c:\temp\MailProcessor\packages\S22.Imap.3.6.0.0\lib\net40\S22.Imap.dll"
//#r @"C:\temp\MailProcessor\packages\FSharp.Charting.0.90.7\lib\net40\FSharp.Charting.dll"

#load "Common.fs"
#load @"../packages/FSharp.Charting.0.90.7/FSharp.Charting.fsx"
#load "MailProcessor.fs"
//#load "MailReader.fs"

open System
open FSharp.Charting
//open MailReader
open MailProcessor

//MailReader.writeValidFilesToAFile(@"c:\temp\emails.json")

let proc = MailProcessor.Processor()
let items = proc.GetItems(__SOURCE_DIRECTORY__ + "\\emails.json")

// Define your library scripting code here

//proc.TotalMailsByYear(items) |> Chart.Line
let totalMails = proc.TotalMailsBySender(items)

for e,k in totalMails do
    printfn "%A" e

for e,k in totalMails do
    printfn "%A" k

//let highData = [ for x in 1.0 .. 100.0 -> (x, 3000.0 + x ** 2.0) ]
//highData |> Chart.Line

//let prices =
//  [ 26.24,25.80,26.22,25.95; 26.40,26.18,26.26,26.20
//    26.37,26.04,26.11,26.08; 26.78,26.15,26.60,26.16
//    26.86,26.51,26.69,26.58; 26.95,26.50,26.91,26.55
//    27.06,26.50,26.64,26.77; 26.86,26.43,26.53,26.59
//    27.10,26.52,26.78,26.59; 27.21,26.99,27.13,27.06
//    27.37,26.91,26.97,27.21; 27.07,26.60,27.05,27.02
//    27.33,26.95,27.04,26.96; 27.27,26.95,27.21,27.23
//    27.81,27.07,27.76,27.25; 27.94,27.29,27.93,27.50
//    28.26,27.91,28.19,27.97; 28.34,28.05,28.10,28.28
//    28.34,27.79,27.80,28.20; 27.84,27.51,27.70,27.77 ]

//Chart.Candlestick(prices) |> Chart.WithYAxis(Max = 29.0, Min = 25.0)