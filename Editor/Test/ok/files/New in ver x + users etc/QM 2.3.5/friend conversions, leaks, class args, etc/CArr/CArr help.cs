 This macro will be displayed when you click this class name in code editor and press F1.
 Add class help here. Add code examples below EXAMPLES.

 EXAMPLES

#compile "__CArr"
 CArr x

 x.Test

 x.m_a="one[]two"
 
  ARRAY(BSTR) b=x.m_a
  ARRAY(str) b=x.m_a
 ARRAY(BSTR) b=x
  ARRAY(str) b=x
 out b

ARRAY(CArr) a.create(2)
VARIANT v=a
a=v
