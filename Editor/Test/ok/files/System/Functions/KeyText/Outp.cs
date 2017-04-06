function ~text

 Pastes/replaces text. If first character of selected text is uppercase, first character of replacement text also will be uppercase.


text.unicode
if(text.len)
	word* w=text
	if(IsCharLowerW(w[0]))
		str ss.getsel(0 CF_UNICODETEXT)
		if(ss.len)
			word* ww=ss
			if(IsCharUpperW(ww[0]))
				w[0]=CharUpperW(+w[0])

text.setsel(CF_UNICODETEXT)
