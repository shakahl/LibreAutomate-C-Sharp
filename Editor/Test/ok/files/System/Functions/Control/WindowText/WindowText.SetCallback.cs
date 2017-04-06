function cbFunc [cbParam]

 Sets callback function to call when Find() or Wait() finds matching text item.
 If the callback function returns 0, Find() or Wait() returns the item. Else continues searching or waiting.

 cbFunc - address of user-defined function that begins with:
   function# WTI&t cbParam
   t - the text item. The function can evaluate its properties (t.color, t.rt etc) and return 1 if it is what you need, or return 0 to continue searching/waiting.
   cbParam - cbParam of SetCallback. Can be int, or a pointer or reference of any type.
 cbParam - some value to pass to the callback function. For example, can be address of a local variable.


m_cbFunc=cbFunc
m_cbParam=cbParam
