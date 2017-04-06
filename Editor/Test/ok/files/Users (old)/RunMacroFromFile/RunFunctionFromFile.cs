 /
function# $qml_file $function_name [~a1] [~a2] [~a3] [~a4]

 Runs executable function from other QM macro list file.
 Does not check whether it is actually function. That is,
 gets first item that matches function_name. It can be even
 folder.
 The extracted function runs asynchronously. This function
 returns thread handle. If caller wants to wait until the
 function ends, it can use the handle with the wait function.

 EXAMPLE
 wait 0 H RunFunctionFromFile("test.qml" "Function2")


str s.GetMacroFromFile(qml_file function_name)
ret RunTextAsFunction(s a1 a2 a3 a4)
err+ end _error
