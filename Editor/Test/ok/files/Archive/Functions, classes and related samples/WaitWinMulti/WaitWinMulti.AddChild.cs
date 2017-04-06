function# _id $text [$_class] [`_window] [flags] [matchindex]

 Adds a child window definition to be used by WaitX functions.
 Returns 1-based index of the added window.

 All arguments are as with <help "::/Functions/IDP_CHILD.html">child</help>, which will be periodically called while waiting.
 If _id is 0, can be any.
 Does not support child flags 8, 32, 128, 0x100, 0x8000.


WWMITEM& r=m_a[]
r.func=1
r.id=_id
r.txt=text
r.cls=_class
r.exeOrOwner=_window
r.flags=flags~(8|32|128|0x100|0x8000)
r.matchindex=matchindex

ret m_a.len
