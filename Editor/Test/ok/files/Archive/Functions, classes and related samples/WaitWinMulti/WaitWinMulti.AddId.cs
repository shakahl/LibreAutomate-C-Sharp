function# _id [`_window] [flags]

 Adds a child window definition to be used by WaitX functions.
 Returns 1-based index of the added window.

 All arguments are as with <help "::/Functions/IDP_ID.html">id</help>, which will be periodically called while waiting.


WWMITEM& r=m_a[]
r.func=2
r.id=_id
r.exeOrOwner=_window
r.flags=flags

ret m_a.len
