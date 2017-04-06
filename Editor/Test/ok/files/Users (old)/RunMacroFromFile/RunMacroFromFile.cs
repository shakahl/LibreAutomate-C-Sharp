 /
function $qml_file $macro_name

 Runs a macro from other QM macro list file. Does not check
 whether it is actually macro. That is, gets first item that
 matches macro_name. It can be even folder.
 Two macros cannot run simultaneously. If this function is
 called from a macro, the extracted macro runs when the
 caller macro ends. If called from a function, runs
 simultaneously.

 EXAMPLE
 RunMacroFromFile("test.qml" "Macro2")


str s.GetMacroFromFile(qml_file macro_name)
RunTextAsMacro s
err+ end _error
