str s ts
s="abcdefg"
out "left 0  %s" ts.left(s 0)
out "right 0 %s" ts.right(s 0)
out "get 0   %s" ts.get(s 0)
out "geta 0  %s" ts.geta(s 0)
out "-negatives:"
out "left -  %s" ts.left(s -2)
out "right - %s" ts.right(s -2)
out "get -   %s" ts.get(s -2)
out "geta -  %s" ts.geta(s -2)

 Note: inconsistancy between right, left, & get/geta

 Comment: 

 I feel, if nc argument is supplied, that no part of the string should be got when nc<=0.
 Otherwise we have to check everytime we call this function, that the variable is not <=0
 Some calculations leave the value below or at 0 so that we wouldnt want to get any part
 of the string in such a case

 In QM documentation:
 "left copies nc characters from beginning of ss.
 right copies nc characters from end of ss.
 get... If nc omitted, then copies all right part of ss."
 
 It does not state that if nc is <=0 that it will get all of the string



 Same issue with findcr, which finds value starting from end of string if from<0
s="abc def ghi jkl"
int i x(-3)
i=findcr(s 32 x)
out i
 Here searching from right to left, it returns 11, though x through a calculation may
 have been <0, ie start searching left from Before the beginning of the string,
 which logically should return "not found"