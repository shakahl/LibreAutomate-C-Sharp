 /
function ~text

 Outputs string that can be captured with dbgview.exe.
 You can find dbgview on the Internet.

 EXAMPLES
 outd "some text"
 outd 5
 outd 5.55
 int x(3) y(30)
 outd F"{x} {y}"
 outd _s.format("%i %i" x y)


if(empty(text)) text="[10]"
OutputDebugStringW @text
