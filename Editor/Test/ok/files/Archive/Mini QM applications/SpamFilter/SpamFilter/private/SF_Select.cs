function [lst] [i] [hlst]

MailBee.Message m
str s; SFDELETED* sd
int-- curlst cursel

if(!lst or i<0)
	 g1
	if(curlst or cursel) SetDlgItemText __sfmain 17 ""
	curlst=0; cursel=0
else if(lst!=curlst or i!=cursel)
	if(lst=1) if(i<a.len) m=a[i].m; else goto g1
	else
		sd=+SF_LvGetLparam(hlst i); if(!sd) goto g1
		m._create; m.CodepageMode=1
		if(!m.ImportFromFile(sd.sf)) goto g1
	
	s=m.BodyText; if(m.BodyFormat=1) s=m.GetPlainFromHtml(s)
	s.setwintext(id(17 __sfmain))
	CheckRadioButton __sfmain 18 19 18; SendMessage __sfmain WM_COMMAND 18 id(18 __sfmain)
	curlst=lst; cursel=i
