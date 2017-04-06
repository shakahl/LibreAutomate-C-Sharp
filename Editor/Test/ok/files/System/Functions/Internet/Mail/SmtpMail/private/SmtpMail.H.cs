function MailBee.Message&m $name [$value]

sel name 1
	case "To": m.ToAddr=value
	case "Cc": m.CCAddr=value
	case "Bcc": m.BCCAddr=value
	case "Subject": m.Subject=value
	case "From": m.FromAddr=value
	case "Reply-To": m.ReplyToAddr=value
	case "X-Mailer": m.XMailer=value
	case "Message-ID": m.MessageId=value
	case "Date": m.Date=value
	case "Return-Path": m.ReturnPath=value
	case else goto g1

if(m.MinimalRebuild)
	 g1
	if(getopt(nargs)=2) m.RemoveHeader(name)
	else m.AddHeader(name value)
