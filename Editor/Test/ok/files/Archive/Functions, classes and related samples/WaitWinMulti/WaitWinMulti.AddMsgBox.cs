function# $name $staticText [`exeOrOwner] [flags] [matchindex]

 Adds a standard message box window definition to be used by WaitX functions.
 Returns 1-based index of the added window.

 staticText - text in the window, in the Static control. To compare, uses matchw, case insensitive.

 All other arguments are as with <help "::/Functions/IDP_WIN.html">win</help>, which will be periodically called while waiting.
 Does not support win flags 64, 128, 0x100, 0x8000.


WWMITEM& r=m_a[]
r.func=3
r.txt=name
r.cls=staticText
r.exeOrOwner=exeOrOwner
r.flags=flags~(64|128|0x100|0x8000)
r.matchindex=matchindex

ret m_a.len
