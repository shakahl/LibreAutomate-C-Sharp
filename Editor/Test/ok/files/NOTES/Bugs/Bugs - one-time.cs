NEWEST

Unexpected assertion "bad array index", probably in ArrMT<DLLFUNCTION>, now unsure. Then I did drag and drop QM items in System.

Region lines/boxes disappeared.

Recording menu commands does not work in QM window. Now works, probably after restarting Windows. Win10.

QM shell menu items don't work. Correctly added, but do nothing when clicked. Win10.

Process triggers stopped working. Recovered after restarting QM.

On QM exit: exception access violation in callgp dtor at: call callgp+8.

____________________________________

Mouse trigger for NextWindow stopped working.
   2010 july 12.
   Did not work in any window.
   Prev window trigger worked.
   Fixed after reloading file.

After returning from screen saver (not secure), top-level window name and class strings in the status bar were garbage.

Error in FF caused QM to hang for about 10 s.

HtmlDoc class: Sometimes HtmlDoc becomes black and is not recognized as having member functions. This happens even with Htm when it is typed in a HtmlDoc member function.
   One of reasons probably is that HtmlDoc class is declared on demand, to avoid loading MSHTML typelib. It should be declared automatically when typing, and now it is true, but then it was false.
   Don't remember exactly, but it happened also after restarting QM and then adding one or several member functions (possibly by cloning).

"unspecified error" thrown by AutoPassword in "qm forum admin login".
   Both edit fields were already populated.
   Worked well before maybe 1 hour in the same session.
   Started work only after restarting QM.
   The same again after 0.5 year.

Drag-drop did not work.
   Did not work with editor.
   There was no QMVISTAD window. Cursor was NO.
   Before that, worked with toolbar.
   Did not work until QM restarted.

Stopped working many commands. Eg out, act, key. But mes worked.
   It happened two times. One or both times it happened after Ftp upload.
   To debug, debug_kwt function runs every 1 s (tim) and checks keyword tables.

When compiling for exe (/exe), clipboard copy triggers an unrelated thread ended with error "unavailable in exe".
   The exe was simple. Just called FindTrayIcon without arguments (error).
   Here is the whole output:
      Error in Macro1287:  expected 2 arguments, not 0.
      Cannot make exe. Error 4, failed to compile.
      Error in MessageLoop_debug:  unavailable in exe.
      Error (RT) in clipboard_copy_triggers:  noncompiled function

After launching G: from a QM menu, QM hung for ~30 s, then error msgbox: qmvista.exe - server execution failed. Next time OK. Windows 7.

Exception in RssNotifier.
   Code: IXmlNode n=xml.Path("rss/channel/item/link")
   Were 2 exceptions:
      Error (RT) in RssNotifier:  Exception 0xC0000005. Access violation. Cannot access memory at 0x0. In  at 0x0 (0x0+0x0).
      Exception in destructors of local variables of RssNotifier.
   Conditions: nothing special.

Error (probably RT, don't remember) "type mismatch" in HtmlDoc.InitFromText, statement d3=+d.
   Works well after restarting QM.
   Before that, QM was recently restarted, and I worked with ExcelSheet. But later could not reproduce.

Error in web: cannot find or connect to a compatible web browser window.
   It was in dialog, on WM_INITDIALOG. The web browser control was initialized by assigning "" to its variable.
   Possible reasons: 1. "about:blank" was not loaded (why)? 2. ?

At QM startup: RT error in SpamFilter: "dialog definition not found or invalid".

___________________________________________________________

 VERY OLD

After trying to activate maxthon (was qm) using mm, mouse clicks almost stopped working in all apps. Needed to terminate qm and maxthon.

Scrolling in OE stopped working. Needed to disable/enable qm.

On Vista (don't remember 32 or 64 bit), QM crashed on menu Tools -> Type Libraries. Possibly fixed, don't remember.

When Notepad is restored from minimized, does not add toolbar.

Acc.Navigate: if with next is used too big number, works endlessly.
