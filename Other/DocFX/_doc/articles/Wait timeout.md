---
uid: wait_timeout
---

# Wait timeout

Most 'wait for' functions have a *secondsTimeout* parameter. It is the maximal time to wait, seconds. If 0, waits infinitely. If > 0, after that time interval throws **TimeoutException**. If < 0, after that time interval returns the default value of the return type (false, null, 0, default).

###### Examples
```csharp
//wait for Notepad window
var w = AWnd.Wait(0, true, "* Notepad");
Print(w);

//wait for Notepad window max 5 seconds. Then throw exception.
var w = AWnd.Wait(5, true, "* Notepad");
Print(w);

//wait for Notepad window max 5 seconds. Then exit.
var w = AWnd.Wait(-5, true, "* Notepad");
if(w.Is0) { Print("timeout"); return; }
Print(w);

//wait for hotkey max 5 seconds. Then exit.
if(!AKeyb.WaitForHotkey(-5, "Ctrl+Shift+K")) return;
Print("hotkey");
```
