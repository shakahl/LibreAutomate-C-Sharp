 When you copy something in an application, it usually stores data in clipboard in several formats.
 This macro shows what formats are currently in the clipboard.

out

int f; str s
OpenClipboard 0
rep CountClipboardFormats
	f=EnumClipboardFormats(f)
	s.fix(GetClipboardFormatName(f s s.all(100)))
	out "%i %s" f s
CloseClipboard

out "<>The numbers without names are <link ''http://www.google.com/search?q=site:microsoft.com Clipboard Formats''>standard clipboard formats</link>."
