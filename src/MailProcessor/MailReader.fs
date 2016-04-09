module MailReader

    open System
    open System.IO
    open System.Net
    open System.Text.RegularExpressions
    open S22.Imap
    open Newtonsoft.Json
    open Common

    let emailWriter =
            MailboxProcessor<Mail.MailMessage * JsonTextWriter>.Start(fun mailbox ->
                let serializer = new JsonSerializer();
                async {
                    while true do
                        let! mail, stream = mailbox.Receive()
                        match mail.Headers.AllKeys |> Seq.exists ((=) "Date") with // some emails don't have a Date header!
                        | true ->
                            let mailToSave =
                                { From = mail.From.Address
                                  Subject = mail.Subject
                                  Date = System.DateTime.Parse mail.Headers.["Date"] }

                            serializer.Serialize(stream, mailToSave)
                        | false -> printfn "found an email that has no date header, ignoring"
                })

    let writeMails (items : List<EMail>, path : string) =
        use fs = File.Open(path, FileMode.Create)
        use sw = new StreamWriter(fs)
        use jw = new JsonTextWriter(sw)

        let serializer = new JsonSerializer();
        serializer.Serialize(jw, items)
        true

    let getMails (condition : SearchCondition) =
         let client = new ImapClient("imap.gmail.com", 993, Common.MailAddress, Common.Password, AuthMethod.Login, ssl = true)
         client.Search(condition, Common.MailBox)
                        |> Seq.choose(fun id ->
                            try
                                Some (client.GetMessage(id, FetchOptions.HeadersOnly, true, Common.MailBox))
                            with
                                _ ->
                                    printfn "unable to download mail id %A" id
                                    None)

    let downloadAllMails =
        let condition = SearchCondition.Subject(Common.Subject)
        getMails(condition)

    let getMailId (mail : EMail) =
        let condition = SearchCondition.Subject(mail.Subject).And(SearchCondition.From(mail.From)).And(SearchCondition.SentOn(mail.Date))
        getMails(condition)
            |> Seq.filter (fun x -> x.Headers.["Date"].Equals mail.Date)
            |> Seq.head
            |> (fun x -> x.Headers.["Message-ID"])

    let IsMatch (input : string) : bool =
        let result = Regex(Common.RegexPattern).IsMatch(input)

        match result with
        | true -> result
        | false -> 
            printfn "%A" input
            false

    let (|IsSubjectMatch|) (input : string) =
        IsMatch(input)

    let validatedMails (items : seq<Mail.MailMessage>) =
        items
            |> Seq.choose(fun mail ->
                match mail.Subject with
                | IsSubjectMatch true -> Some mail
                | IsSubjectMatch false -> None)

    let mailMessagesToEMail (items : seq<Mail.MailMessage>) =
        items
            |> Seq.map(fun mail ->
                let mailToSave =
                    { From = mail.From.Address
                      Subject = mail.Subject
                      Date = System.DateTime.Parse mail.Headers.["Date"] }
                mailToSave)

    let writeValidFilesToAFile (path : string) =
        downloadAllMails
            |> validatedMails
            |> mailMessagesToEMail
            |> Seq.toList
            |>  (fun x -> writeMails(x, path))

    let downloadMailsAfterDate (lastMail : DateTime) =
        // Why subject amd sentsince only returns mails from that specific date?
        //let condition = SearchCondition.Subject(Common.Subject).And(SearchCondition.SentSince(lastMail))
        let condition = SearchCondition.SentSince(lastMail)
        getMails(condition)
            |> validatedMails
            |> Seq.filter(fun x -> System.DateTime.Parse x.Headers.["Date"] > lastMail)
            |> mailMessagesToEMail
            |> Seq.toList

    let updateAndWriteAfterLastDate(oldItems : List<Common.EMail>, lastDate : DateTime) = 
        let newMails = downloadMailsAfterDate(lastDate)
        let newCollection = List.append oldItems newMails
        let success = writeMails(newCollection, __SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")
        newCollection

    let writeMailsAsCSV(items : List<Common.EMail>) =
        let textData = 
            items
                |> List.map(fun x -> String.Format("{0}, {1}, {2}, {3}", x.From, x.Date.ToString(), x.Date.DayOfWeek, x.Date.Hour))
                |> String.concat "\n"
            
        use fs = File.Open(__SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.csv", FileMode.Create)
        use sw = new StreamWriter(fs)
        sw.WriteLine(textData)
        true