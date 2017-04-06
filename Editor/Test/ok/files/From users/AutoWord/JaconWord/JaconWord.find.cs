function# VARIANT'ftxt [VARIANT'rtxt] [flags]

jaw_f_rng=jaw_doc.Range
jaw_f_rng.Find.ClearFormatting()
if jaw_f_rng.Find.Execute(ftxt)
	jaw_f_rng.Select
	if flags&JAW_REPLACE
		jaw_f_rng.Text=rtxt
		jaw_rng.Select()
	ret 1
ret

