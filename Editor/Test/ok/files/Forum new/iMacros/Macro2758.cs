 assume the iMacros macro is:

 VERSION  BUILD=7500718 RECORDER=FX
 TAB T=1
 URL GOTO=http://demo.imacros.net/Automate/Filter
 TAG POS=1 TYPE=INPUT:TEXT FORM=NAME:form1 ATTR=NAME:textfield CONTENT="Image Filter ON (No images downloaded from server)"

 and you want to insert QM code before TAG POS


IDispatch iim._create("iMacros")
int status=iim.iimOpen("")
str macro

macro=
 VERSION  BUILD=7500718 RECORDER=FX
 TAB T=1
 URL GOTO=http://demo.imacros.net/Automate/Filter
status=iim.iimPlayCode(macro)

mes "press ok to next"

macro=
 TAG POS=1 TYPE=INPUT:TEXT FORM=NAME:form1 ATTR=NAME:textfield CONTENT="Image Filter ON (No images downloaded from server)"
status=iim.iimPlayCode(macro)
