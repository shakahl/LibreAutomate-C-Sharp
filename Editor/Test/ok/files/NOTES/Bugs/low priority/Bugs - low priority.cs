Error in Application from 2 typelibs:  type library error: type name not unique. Application.

When renaming a function, warning "this name already exists".
   Reproduce: 1. Create a macro and convert it to function (it changes spaces in name to _). 2. Rename it. 3. Create new function and give it the old name of the first function.

No typeinfo when a variable is thread pointer, like str-* s.

Error "expected BaseType*", probably because does not unalias when calling member function:
   GdipGraphics g2.FromImage(bm2.CreateEmpty(16 16))
    Here OK:
   bm2.CreateEmpty(16 16)
   GdipGraphics g2.FromImage(bm2)

Error if single-line sel/case contains if:
	sel(1) case 1 if(1) out 1; case 2 out 2

NetSendMacro/NewItem: If macro to be replaced is opened, selects another item. If it is folder, macro text in editor is not updated until you select some other macro. Maybe fixed.

Toolbar with web browser control:
  Previously: If we get WebBrowser through _getcontrol, and then call GoBack, QM crashes. Navigate works well. In dialog works well. web "Back" works well.
  Now is opposite: web "Back" often does not work, and QM may hang, whereas GoBack works well.

Problems with web browser control on Vista (on XP OK). See Dialog48.
1. script error message box when loading google. OK if Silent property is set. Now WM_SETTEXT sets it.
2. does not draw whole page (try to scroll).

Autotext: works in the middle of a word if before is r (and there is a two-key trigger where r is first key and is not eaten).

_setevents: add one more AddRef, because in some cases the interface is released too early (eg if using local variable in dialog). There should be a macro that demonstrates it, but could not find after some time.

After hibernation, OSD window moved from right-bottom to top-left and become nontransparent.

IntGetFile bugs and problems:
   InGetUrl with progress: when long time to connect, pressing Cancel closes the dialog but does not end the macro.
   How to set timeout (user request)? Would need to do everything async.

The Setup program sometimes waits 5-10s in the 'finishing' phase.
   It started in 2014 year. I did not change the setup program at that time, but some changes were in the qm.exe part.
   Difficult to reproduce. It seems that waits only after long time of using PC. Next time does not wait.

Cannot unlock computer when locked by Remote Desktop. Even when the Remote Desktop session is finished.

In Remote Desktop, IsLoggedOn returns 3.

TCC does not close eg h file after error.
