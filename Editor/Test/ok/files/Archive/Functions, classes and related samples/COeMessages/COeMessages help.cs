 Enumerates Outlook Express or Windows Mail folders and messages. Loads messages.
 Works with OE database files, not with OE window.
 Works on XP, Vista and possibly later OS. Does not work with old OE versions (probably < 6).
 At first call Init, then other functions.
 Also you can call functions of ns. Documented in MSDN library, look for IStoreNamespace. Use OpenFolder to get IStoreFolder.
 Note: if there are many messages, may show 'Compact Messages' message box. This cannot be disabled. Unless you create window-triggered function that closes it, or launch another thread that waits and closes it.

 EXAMPLE

out
#compile __COeMessages
ARRAY(OEFOLDER) af
ARRAY(OEMESSAGE) am
COeMessages oe.Init
oe.GetFolders(af)
int i j
 MailBee.Message mm._create
for i 0 af.len ;;for each folder
	OEFOLDER& f=af[i]
	out "-------- FOLDER: %s --------" f.name
	oe.GetMessages(f am 1)
	for j 0 am.len ;;for each message
		OEMESSAGE& m=am[j]
		out "------------ MESSAGE: subject=''%s'', from=''%s'' --------" m.pszSubject m.pszDisplayFrom
		 out m.source
		 mm.RawBody=m.source ;;message source -> MailBee.Message, which parses the message so you can easily access its properties
		 out mm.AltBodyText

  another example
 out
 #compile __COeMessages
 ARRAY(OEFOLDER) af
 ARRAY(OEMESSAGE) am
 COeMessages oe.Init
 OEFOLDER f
 oe.FindFolder("Deleted Items" f)
 out f.name
 int i j
 MailBee.Message mm._create
 oe.GetMessages(f am 1)
 for j 0 am.len ;;for each message
	 OEMESSAGE& m=am[j]
	 out "------------ MESSAGE: subject=''%s'', from=''%s'' --------" m.pszSubject m.pszDisplayFrom
	  out m.source
	  mm.RawBody=m.source ;;message source -> MailBee.Message, which parses the message so you can easily access its properties
	  out mm.AltBodyText
