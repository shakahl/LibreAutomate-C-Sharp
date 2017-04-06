int w=win("Global Options jEdit: General" "SunAwtDialog")
act w
 Acc a.Find(w "check box" "Sort recent file list" "" 0x1001)
 a.DoDefaultAction

Acc a.Find(w "text" "" "" 0x1004)
 a.JavaAction("caret-end")
a.JavaAction("selection-next-word")

 Java actions:	page-up, caret-forward, caret-backward, selection-down,
 selection-next-word, cut-to-clipboard, selection-previous-word, select-line,
 selection-begin-line, delete-previous-word, selection-backward,
 caret-up, caret-end-word, selection-begin-paragraph, selection-page-left,
 unselect, caret-end, copy-to-clipboard, selection-page-down, selection-forward,
 selection-end-word, caret-previous-word, set-read-only, insert-tab, beep,
 insert-break, caret-begin-word, delete-previous, notify-field-accept,
 caret-begin-paragraph, set-writable, selection-end-paragraph, page-down,
 select-paragraph, insert-content, caret-begin, selection-begin, selection-page-up,
 select-word, caret-end-line, selection-end, dump-model, selection-begin-word,
 selection-page-right, delete-next, delete-next-word, caret-end-paragraph,
 toggle-componentOrientation, selection-end-line, caret-down, select-all,
 paste-from-clipboard, reset-field-edit, caret-begin-line, default-typed,
 selection-up, caret-next-word
