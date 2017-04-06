out
int w1=act(win("Quick Macros " "QM_Editor"))
rep 20
	#region Recorded 2014.07.22 09:22:05
	'Cn             ;; Ctrl+N
	'Cp             ;; Ctrl+P
	int w2=wait(9 win("Properties -" "#32770"))
	'TY             ;; Tab Enter
	int w3=wait(7 win("Quick Macros" "#32770"))
	'Y              ;; Enter
	int w5=wait(20 win("Properties (Local Computer)" "WindowsForms10.Window.8.app.0.21d1674"))
	0.5
	'Z              ;; Esc
	act w2
	'Z              ;; Esc
	act w1
	key CF4
	#endregion

err+
