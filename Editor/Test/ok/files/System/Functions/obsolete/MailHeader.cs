 /
function MailBee.Message&m $name [~value]

m.Locked=0
if(!m.MinimalRebuild) m.MinimalRebuild=-1

if(getopt(nargs)=2) m.RemoveHeader(name)
else m.AddHeader(name value)

 used by spam filter
