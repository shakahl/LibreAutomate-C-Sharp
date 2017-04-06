 In Internet Explorer, Gmail message view, move mouse on Download link and run this macro.
 It extracts sender name and message labels.

Acc a alab afrom
a=acc(mouse) ;;Download link
str s=a.Name
if(s!"Download") mes- "Mouse pointer must be on Download link." "" "x"

a.Navigate("parent3") ;;table containing Download link
rep ;;search for beginning of message (there is no separate parent object for a message)
	a.Navigate("previous"); err mes- "Failed." "" "x"
	a.Role(s)
	if(s="GROUPING") break ;;the down arrow in the top-right corner of the message
	 note: I don't know, maybe message body also sometimes contains GROUPING. Then need more testing here.

a.Navigate("next first3 child3" afrom) ;;navigate from the down arrow to sender name
str from=afrom.Name

a.Navigate("parent child") ;;navigate from the down arrow to subject
str subject=a.Name

ARRAY(str) k ;;array for labels
a.Navigate("next") ;;skip Inbox table
rep ;;for each label table
	a.Navigate("next")
	a.Role(s)
	if(s!"TABLE") break
	a.Navigate("child7 first" alab) ;;get label
	k[]=alab.Name


out "-- From --"
out from
out "-- Subject --"
out subject
out "-- Label(s) --"
int i
for i 0 k.len
	out k[i]

 Now you have sender, subject and labels.
 Mouse pointer is on Download link...
