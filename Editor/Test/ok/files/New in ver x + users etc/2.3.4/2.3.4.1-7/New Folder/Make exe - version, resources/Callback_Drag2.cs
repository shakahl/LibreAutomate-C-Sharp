 /
function# button param

 Callback function for Drag.

 button - mouse button: 0 while dragging, 1 left up, 2 right up.
 param - param passed to Drag().

 Return:
   If button 0 (mouse move), return cursor handle, or 0 to not change cursor, or 1-3 to use standard cursors: 1 move, 2 copy, 3 no operation.
   Else (mouse button up), can return any value. Drag() returns it.


if(!button) ret param
