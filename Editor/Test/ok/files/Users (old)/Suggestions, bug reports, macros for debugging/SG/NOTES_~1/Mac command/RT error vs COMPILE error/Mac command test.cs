TestFunc6
out "Called ''TestFunc6''"
mac "TestFunc7"
out "Called ''mac ''TestFunc7''''"
mac TestFunc8						;;*
out "Called ''mac TestFunc7''"

  output:
 Waited 2, Funciton 6
 Called "TestFunc6"
 Called "mac "TestFunc7""
 Waited 4, Function 7
 Waited 6, Function 8
 Error (RT) in Mac command test: item not found

 Comment:
 *This "mac TestFunc8" command DOES run TestFunc8, but then gives RT Error.

 I feel it should give instead a compile time error.
 	Else novices may confuse it with using "mac "TestFunc7""
