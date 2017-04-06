 /
function[c]# param TYPE&a TYPE&b

 Callback function for q_sort.
 Called multiple times while sorting.
 Must compare a and b.

 param - param of q_sort.
   Can be declared as pointer or reference of any type. Then you can pass address of a variable of that type to q_sort as param.
 TYPE - replace it with the type of your array elements.
 a, b - two array elements.

 Return:
   -1 - a must be placed before b.
   1 - a must be placed after b.
   0 - there is no difference.

 Tip: to compare strings, you can use StrCompare, StrCompareN or StrCompareEx. For numbers use operators < > etc.


 <add your code here>
