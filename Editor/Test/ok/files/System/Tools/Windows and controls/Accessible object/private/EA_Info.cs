function hDlg [~text]

 Shows an info control in place of the speed Static.
 Hides if text empty/

text.setwintext(id(13 hDlg))
TO_Show hDlg "13 -48" text.len!0
