out 1

 atend in_main_thread2

#compile "__CTestThread"
 CTestThread x
CTestThread- x y
x=y

 out x
 x=8

 out _error.description
 act "hhhhhhhhhhhhhh"
 err out _error.description
