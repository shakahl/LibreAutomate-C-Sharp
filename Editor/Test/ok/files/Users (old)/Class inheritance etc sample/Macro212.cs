class Class1 a b
class Class2 Class1'c d ;;main class; a is member
class Class3 :Class1'c d ;;main class; derived from Class1 (inheritance)

Class2 x
x.FuncA
 or directly through member variable
x.c.FuncA1 ;;only if FuncA1 is public (set in Properties) and c is public (in class declaration without -)
 or use inheritance
Class3 y
y.FuncA1
