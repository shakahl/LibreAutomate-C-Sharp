function` `Macro [`Arg1] [`Arg2] [`Arg3] [`Arg4] [`Arg5] [`Arg6] [`Arg7] [`Arg8] [`Arg9] [`Arg10] [`Arg11] [`Arg12] [`Arg13] [`Arg14] [`Arg15] [`Arg16]

 Runs an Excel macro (Sub or Function).
 Returns the return value of the macro.

 Macro - macro name.
   It will be evaluated in the context of the active sheet.
 Arg1-Arg16 - arguments. Optional.
   Cannot be objects.
   QM 2.3.3. Added Arg5-Arg16.

 See also: <ExcelSheet.AddCode>
 Added in: QM 2.3.2.
 Errors: Excel errors


WS(1)

VARIANT vo=__VarOpt
VARIANT* p=&Macro; int i
for(i getopt(nargs) 17) p[i]=vo

ret ws.Application.Run(Macro Arg1 Arg2 Arg3 Arg4 Arg5 Arg6 Arg7 Arg8 Arg9 Arg10 Arg11 Arg12 Arg13 Arg14 Arg15 Arg16)

err+ E

 tested: fails if Macro is Range.
