module MailReaderTests

open NUnit.Framework
open MailReader
open MailProcessor

[<Test>]
let ``writeValidFilesToAFile`` () =
    let success = MailReader.writeValidFilesToAFile(__SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")
    Assert.IsTrue(success)

[<Test>]
let ``Get mails after date`` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(__SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")
    let lastDate = processor.LastMailDate(items)

    let newMails = MailReader.downloadMailsAfterDate(lastDate)

    let count = Seq.length newMails

    Assert.IsTrue(Seq.length items > 0)

[<Test>]
let ``Update mails after last`` () =
    let processor = new MailProcessor.Processor()
    let items = processor.GetItems(__SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")
    let lastDate = processor.LastMailDate(items)

    let newMails = MailReader.downloadMailsAfterDate(lastDate)

    let count = Seq.length newMails

    let success = MailReader.writeMails(List.append items newMails, __SOURCE_DIRECTORY__ + @"\..\MailProcessor\emails.json")

    Assert.IsTrue(success)

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
    Assert.IsFalse(MailReader.IsMatch("Daily Spam"))
   