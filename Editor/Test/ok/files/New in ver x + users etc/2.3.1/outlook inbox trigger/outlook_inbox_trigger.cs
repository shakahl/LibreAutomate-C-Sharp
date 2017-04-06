 /exe 1
typelib Outlook {00062FFF-0000-0000-C000-000000000046} 9.2

Outlook.Application a._getactive
Outlook.NameSpace ns=a.Session
Outlook.Items items=ns.GetDefaultFolder(olFolderInbox).Items
items._setevents("sub.items")

 wait until outlook exits
a._setevents("sub.a")
opt waitmsg 1
int- t_quit
rep() 1; if(t_quit) break


#sub a_Quit
function ;;Outlook._Application'a

out "outlook quit"

int- t_quit
t_quit=1


#sub items_ItemAdd
function IDispatch'Item ;;Outlook._Items'items

out "outlook item added"
sub.NewMessageInInbox Item


#sub NewMessageInInbox
function Outlook.MailItem'mi

 note: here Outlook shows a security warning message box

str email=mi.SenderEmailAddress
out email
str subject=mi.Subject
out subject
 out mi.BodyFormat
str s=mi.Body
out s

 now extract words from s and subject...

err+ out _error.description
