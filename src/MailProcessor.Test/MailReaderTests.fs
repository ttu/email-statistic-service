module MailReaderTests

open System
open NUnit.Framework
open MailReader
open MailProcessor

let path = __SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json"

[<Test>]
[<Ignore>]
let ``writeValidFilesToAFile`` () =
    let success = MailReader.writeValidFilesToAFile(path)
    Assert.IsTrue(success)

[<Test>]
[<Ignore>]
let ``Get mails after date`` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(path)
    let lastDate = processor.LastMailDate(items)

    let newMails = MailReader.downloadMailsAfterDate(lastDate)

    let count = Seq.length newMails

    Assert.IsTrue(Seq.length items > 0)

[<Test>]
[<Ignore>]
let ``Update mails after last`` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(path)
    let lastDate = processor.LastMailDate(items)

    let newMails = MailReader.downloadMailsAfterDate(lastDate)

    let count = Seq.length newMails

    let success = MailReader.writeMails(List.append items newMails, path)

    Assert.IsTrue(success)

[<Test>]
[<Ignore>]
let ``updateAndWriteAfterLastDate`` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(path)
    let lastDate = processor.LastMailDate(items)

    let newMails = MailReader.updateAndWriteAfterLastDate(items, lastDate)

    Assert.IsTrue(items.Length <= newMails.Length)

[<Test>]
[<Ignore>]
let ``Get mail id`` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(path)
   
    let id = MailReader.getMailId(items.Head)

    Assert.IsTrue(id <> "")

[<Test>]
let ``IsSubjectMatch`` () =
    Assert.IsTrue(MailReader.IsMatch("RE: Daily spam"))
    Assert.IsTrue(MailReader.IsMatch("Re: Daily spam"))
    Assert.IsTrue(MailReader.IsMatch("VS: Re: Daily spam"))
    Assert.IsTrue(MailReader.IsMatch("VS: RE: Daily spam"))
    Assert.IsTrue(MailReader.IsMatch("VS: RE: Daily spam."))
    Assert.IsTrue(MailReader.IsMatch("Daily spam"))
    Assert.IsTrue(MailReader.IsMatch("Daily spam."))
    Assert.IsTrue(MailReader.IsMatch("Re: VS: VS: VS: VS: VS: VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS: VS:VS:Daily spam."))
    Assert.IsTrue(MailReader.IsMatch("Re: VS: VS: VS: VS: VS: VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS: VS:vs:Daily spam."))
    Assert.IsTrue(MailReader.IsMatch("Re: VS: VS: VS: VS: VS: VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS: VS:re:Daily spam."))
    Assert.IsTrue(MailReader.IsMatch("Re: VS: VS: VS: VS: VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS:VS: VS:VS:Dailyspam."))
    Assert.IsFalse(MailReader.IsMatch("Daily Spam"))

   