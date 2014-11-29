module Common

open System

type EMail = { From : string; Subject : string; Date : DateTime }

let MailAddress = "MY_EMAIL"
let Password = "MY_PASSWORD"
let Subject = "Daily spam"
let MailBox = "[Gmail]/All Mail"
let RegexPattern = "(^|VS: |Vs: |vs: |RE: |Re: |re: |VS:|Vs:|vs:|RE:|Re:|re:)Daily(| |)spam($|.$)"