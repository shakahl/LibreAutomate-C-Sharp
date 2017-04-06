
 Closes browser started by StartChrome() etc.

 REMARKS
 The user should not close Selenium-started browsers manually. It would not delete temporary files and driver processes. To close correctly, call this function. Or use StartChrome() etc with flag 1 to call this function automatically.


if(!m_started) ret
x._Quit; err
if(m_started>0)	___wdGlobal.cs.Call("OnQuit" m_started); err
m_started=0
