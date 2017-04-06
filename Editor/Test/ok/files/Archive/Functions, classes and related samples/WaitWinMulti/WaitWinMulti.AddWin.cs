function# $name [$_class] [`exeOrOwner] [flags] [matchindex]

 Adds a top-level window definition to be used by WaitX functions.
 Returns 1-based index of the added window.

 All arguments are as with <help "::/Functions/IDP_WIN.html">win</help>, which will be periodically called while waiting.
 Does not support win flags 64, 128, 0x100, 0x8000.


WWMITEM& r=m_a[]
r.txt=name
r.cls=_class
r.exeOrOwner=exeOrOwner
r.flags=flags~(64|128|0x100|0x8000)
r.matchindex=matchindex

ret m_a.len
