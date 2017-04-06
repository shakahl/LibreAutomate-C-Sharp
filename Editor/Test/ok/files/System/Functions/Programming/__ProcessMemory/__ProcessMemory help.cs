 Allocates, writes and reads memory in context of other process.

 Usually used with control messages where lParam of SendMessage is pointer or string.
 Need it because if the control belongs to other process, you cannot simply pass pointer/string, because the memory address is invalid in that process.

 Note that some structures, such as LVITEM, are different in 64-bit processes. Strings, pointers, HWND, LPARAM, xxx_PTR etc are 64-bit (long).

 See also: <__SharedMemory help>

 <open "GetListViewItemText">Example</open>
