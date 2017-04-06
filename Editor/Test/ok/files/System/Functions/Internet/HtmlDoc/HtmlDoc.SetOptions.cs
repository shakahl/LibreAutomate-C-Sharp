function flags ;;flags: 1 no scripts, 2 with web browser control

 Sets options to be used with other functions.
 Call this function before InitX.

 flags:
   1 - don't execute scripts.
       This also adds flag 2.
   2 - use hidden web browser control.
       Usually it works better. For example, after InitFromWeb, GetLinks gets full URLs, not "about:relativeURL".
       Without web browser control, HtmlDoc functions don't work well with some pages, because the document object is windowless. Even may show Internet Explorer.

 Added in: QM 2.3.3.


m_flags=flags
