int x(5)
ARRAY(int)* p._new ;;creates array in heap memory, not on stack
ARRAY(int)& y=p
y.create(2)
y[1]=6

mac "mac_t2" "" x &y ;;passes pointer to the array, let the function delete it when don't need
