 /
function# [ARRAY(int)&handles] [threadid]

 Gets top-level windows of current or specified thread.
 Returns handle of first window.

 handles - variable for window handles. Can be 0.
 threadid - thread id. Default: 0 - current thread.


type ___GTW hwnd ARRAY(int)*a

if(!threadid) threadid=GetCurrentThreadId
___GTW d.a=&handles

EnumThreadWindows(threadid &sub.EnumProc &d)
if(&handles and handles.len) ret handles[0]
ret d.hwnd


#sub EnumProc
function# hwnd ___GTW&d

if(d.a)
	ARRAY(int)& a=d.a
	a[]=hwnd
	ret 1
else d.hwnd=hwnd
