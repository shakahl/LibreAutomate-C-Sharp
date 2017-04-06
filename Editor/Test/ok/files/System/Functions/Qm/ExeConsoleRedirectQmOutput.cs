 /
function [flags] ;;flags: 1 stop redirecting

 In exe redirects all QM output text to the console window of this or parent process.

 REMARKS
 Uses <help>RedirectQmOutput</help> to capture all output text (<help>out</help>, errors, etc) of current process.
 Uses <help>ExeConsoleWrite</help>.
 Does nothing if used not in exe.

 Added in: QM 2.4.1.
 See also: <ExeOutputWindow>


#if EXE
if(flags&1) RedirectQmOutput 0; ret
opt nowarningshere 1
__ExeConsoleInit
RedirectQmOutput &sub.OutRedirProc


#sub OutRedirProc
function# str&s reserved

ExeConsoleWrite s
ret 1
