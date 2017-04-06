RECENT

Error "expected 0 arguments" when calling a #sub from a toolbar, eg toolbar VS2015. OK after restarting QM.

On shutdown PC (or logoff), not saved open items etc.
   Was maybe 2 times. Cannot reproduce.

OLD

Exception in MessageLoop (DispatchMessage):
   It was in clipboard_copy_triggers thread, which terminated after 2 exceptions.
   Then exception while compiling clipboard_copy_triggers (ShowDialog).
   This was 2 times (2009.3.3 and 15). Everything were exactly the same. Second time, QM was Debug config, first time don't know.

When QM restarts using shutdown -2, sometimes (rarely) fails to init network. This happened when using php. Possibly fixed.


VERY OLD

menu: sometimes doesn't paste. Usually in browser edit control. Seen this long long ago.

Sometimes an attached toolbar causes to fail to activate that window.
  When QM is launched after Visual C++, if you activate Visual C++, toolbar is not added. But when you switch to other window (eg QM), toolbar is added at that time, and Visual C++ remains active.
  Seen this long long ago.

Sometimes a toolbar is invisible (transparent or prev window's background).
  Often with Dreamweaver (old ver). Sometimes with Notepad.
  It seems that only attached borderless toolbars.
  Usually when owner is activated after long time.
  It seems that it no longer happens after adding more RAM. It was on XP with 256 MB.
