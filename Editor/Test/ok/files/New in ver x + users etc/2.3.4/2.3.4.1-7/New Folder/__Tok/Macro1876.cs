out
fff "one""two"
fff (1)(2)
fff 1(2)
fff 5"two"
fff (5)"two"
fff "two"5
fff "two"(5)
 fff _hwndqm(2) ;;err
 fff _hwndqm"two" ;;err
fff fff(1)fff(2)
 fff 'a''b' ;;err
fff 1'b' ;;err
 fff 'b'2 ;;err
fff 1,2
fff 1,,2
fff , 1, 2
fff(,1,2,)
_i=fff(,1,2,)
fff @"one"
