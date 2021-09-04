v0.0.4

## Breaking changes

**elm** and **elmFinder**:
- Removed functions **elm.find**, **elm.wait**, **elm.printAll**, **elm.Find**, **elm.Wait**. Now use **elmFinder.Find** etc with the new syntax.
- Now *role* parameter cannot contain path. Instead use the new path syntax.
- The *prop* parameter type now is **Strings**, and separator is `|` (was `\0`). If substrings contain `|`, use `new("substring", "substring")`.
- Removed property **elmFinder.NavigFailed**.
- Renamed **elm.Navigate** parameter *secondsToWait* to *waitS*.

Now functions **Find** and **Wait** of all finders (**wndFinder**, **wndChildFinder**, **elmFiner**, **uiimageFinder**) return the found object, not bool. Functions **Exists** return bool.

Removed **wndFinder** conversion from string.

From **wnd**, **elm** and **uiimage** removed operator + that was used to throw **NotFoundException**. Instead use 'find' function overloads with parameter *waitS*.

**DStringList** renamed to **Strings**.

String extension methods **FindAny**, **FindNot**, **FindLastAny** and **FindLastNot** now have parameter *range* instead of *startOfRange* and *endOfRange*.


## Other changes
Many changes in **elm** and **elmFinder** classes and in the "Find UI element" dialog. The **Find** function can find through intermediate elements (path) and can wait and throw exception if not found.

Replaced syntax of finding UI elements (**elm**). Now instead of code `var e = elm.find(w, ...)` use code `var e = w[...].Find()` or `var e = w[...][...][...].Find()`.
