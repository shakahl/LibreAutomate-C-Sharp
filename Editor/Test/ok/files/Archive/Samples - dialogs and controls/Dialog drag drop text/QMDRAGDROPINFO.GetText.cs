function! str&s [flags] ;;flags: 1 drag source must not delete text

 Extracts dropped text.
 Returns 1 if successful, 0 if failed.


s.fix(0)

int k=GetMod
sel(k) case [0,2] case else this.effect=DROPEFFECT_NONE; ret
if(flags&1) k=2

int i
for(i 0 this.formats.len) if(this.formats[i].cfFormat=CF_UNICODETEXT) break
if(i=this.formats.len) ret

#opt nowarnings 1
STGMEDIUM sm
this.dataObj.GetData(&this.formats[i] &sm); err ret

int gs=GlobalSize(sm.hGlobal)-2; if(gs<=0) ret
s.all(gs 2)
byte* m=GlobalLock(sm.hGlobal); if(!m) ret
memcpy s m s.len
GlobalUnlock sm.hGlobal
ReleaseStgMedium(&sm)
s.ansi

sel k
	case 0 this.effect&DROPEFFECT_MOVE
	case 2 this.effect&DROPEFFECT_COPY

ret 1
