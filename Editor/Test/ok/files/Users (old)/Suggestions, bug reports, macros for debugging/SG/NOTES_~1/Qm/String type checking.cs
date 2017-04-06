 Type checking of str' when calling function - potential disaster without compile time error
 ===========================================================================================
str s="abc"
TF3(&s +"def")

 output:
 string is abc
 integer is 19174056
 string2 is (null)

 Note: shouldnt there be type checking for str type when calling a function?

 In this example, intention is to pass "abc" as a string argument, but say we forget to insert
 an integer argument.
 
 Result: integer i in TF3 gets assgined a monstrous number
 which could have catastrophic results if programmer does not check for error.

 (Also: how can a programmer check for such an error? ie how to check if a required integer is not being passed as a string?)


 My situation:
 Had a function needing many arguments, i added one more in the middle.
 Forgot to insert extra argument in one of my calls. Result was function was taking
 forever to complete. 
 So i thought this can be disasterous if i somehow make an ommision error!