out
int w1=child("" "Internet Explorer_Server" win("" "IEFrame"))
MSHTML.IHTMLDocument2 d=htm(w1)

 ARRAY(MSHTML.IHTMLDocument2) a
 ARRAY(str) ap
 EH_GetFrameDocuments d a ap
 
 int i
 for i 0 a.len
	 out a[i].url
	 out ap[i]



Q &q
 MSHTML.IHTMLFramesCollection2 f=d.frames
 int n=f.length


IOleContainer oc=+d
IUnknown u
IEnumUnknown eu
oc.EnumObjects(OLECONTF_EMBEDDINGS &eu)
int n
eu.Next(1 &u &n)

Q &qq
outq
out n
