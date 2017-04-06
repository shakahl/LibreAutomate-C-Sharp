 /
function hDlg $buttons

 Sets a symbol font (Marlett) for one or more buttons.
 For example, if button text is "6", it will be down-arrow.
 The button cannot have readable text.

 buttons - space-separated list of button ids.

__Font-- f.Create("Marlett" 10)
f.SetDialogFont(hDlg buttons)
