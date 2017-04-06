 #compile mup_api
ref mup "mup_api"

int h=mup.mupInit
atend mup.mupRelease h
mup.mupAddValIdent(h &mup_id)

 mup.mupSetExpr(h "4000000000>1")
mup.mupSetExpr(h "-(1+2)")
 out mup.mupError
int j=mup.mupEval(h)
if(mup.mupError) out mup.mupGetErrorMsg
	
out j
