str s=
 use Win32::Clipboard;
 $CLIP = Win32::Clipboard();
 print "Clipboard contains: ", $CLIP->Get(), "\n";
 $CLIP->Set("some text to copy into the clipboard");
 $CLIP->WaitForChange();
 print "Clipboard has changed!\n";

PerlExec s
out PerlEval("$CLIP->Get()")


 PerlExec2 "print 'hello world'"
 out PerlEval("print 'hello world'")

 pls, not pl
