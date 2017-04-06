
 Gets current statement info.

 caller - 0 current function, 1 caller, 2 caller's caller, and so on.
 statOffset - 0 current statement, 1 next, -1, previous, etc. If out of range, gets nearest (first or last) statement. Note that some statements, eg declarations, are not used at run time. To see used statements, run the macro in debug mode.
 statement - receives the statement. Can be 0 if not needed (faster).
 itemId - receives QM item id. Can be 0 if not needed.
 flags: 1 include indirect callers. Direct caller is macro or function that called current function by name.

 On success, returns offset of the statement in code.
 On failure, returns:
 -1 there is no caller at the specified depth.
 -2 the QM item is deleted or encrypted. Gets only itemId.

 Note that the results may be incorrect if you changed code after compiling it.
 Note that the caller's statement not necessary begins with current function's name. For example, it may be "var=CurFunc(1)" or "ret CurFunc(1)" or "AnotherFunc CurFunc(1) 2".
 Unavailable in exe.
 Added in QM 2.3.1.

 example
_i=0
_i=1
OutPrevStatement "Warning"
