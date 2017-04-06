type n8 int'x[9]
n8 x
x[2]=5
mac "TestFunc10" "" &x
TestFunc10(&x)
wait 2;; note: without a wait command, gives "Exception (RT) in TestFunc10"
out x[2]
end

  output:
70254368
5
70254368
5
5
