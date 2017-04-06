 For Thunderbird without "Thunderbird Conversations" extension.

str s email rc

int w=win(" Thunderbird" "MozillaWindowClass")
Acc aDoc aHdr aEmail aRC
aDoc.Find(w "DOCUMENT" "" "" 0x3010 2)
 get email
aDoc.Navigate("parent prev" aHdr)
aEmail.Find(aHdr.a "mail-emailaddress" "" "a:headerName=to" 0x1014 1)
email=aEmail.WebAttribute("emailAddress")
 get RC
aRC.Find(aDoc.a "TEXT" "^\w{32}-\S+" "" 0x3012 1)
rc=aRC.Name

 out email
 out rc

err+ OnScreenDisplay "failed, try again"; ret

RegcodeToDb rc email
