str so sn st si sf
if(mes("Please select line with OLD str variables." "" "OC")!='O') ret
so.getsel
if(mes("Please select line with NEW str variables." "" "OC")!='O') ret
sn.getsel
st.getwintext(id(2210 _hwndqm))

int n=DE_UpdateVariables(st so sn)
if(n)
	st.setwintext(id(2210 _hwndqm))
	mes "Replaced %i variables.[][]Now delete lines with old variables." "" "i" n
