There are only 2 simple variable scopes - local and member.
Variables of other scopes are created at run time and assigned to a local reference variable:
int& v=var_global(name)
int& v=var_thread(name)
int& v=var_window(name hwnd)
Also somehow allow the user to create custom scopes. Eg var_registry(name etc).
