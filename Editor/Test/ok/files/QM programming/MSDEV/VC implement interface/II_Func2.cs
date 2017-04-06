 /
function str&s str&sdecl
if(!s.len) end

AC_Prepare &s

s.findreplace("([]" "(")
s.findreplace("( []" "(")
s.findreplace(",[]" ",")
s.findreplace(", []" ",")
s.findreplace(" = 0;[]" ";")

 int i k n
 rep
	 i=findc(s '(' i+1); if(i<0) break
	 k=findcr(s 32 i-2)+1
	 if(!memcmp(s+k "STDMETHOD" 9)) k=i+1; i=findc(s ')' k); if(i<0) break
	 ss.geta(s k i-k); ss+" "; n+1
	 i=findc(s ';' i); if(i<0) break
 ss.rtrim; ss+"[]"
CH_Compact s &sdecl
sdecl.findreplace("( " "(" 8)
sdecl.findreplace("; " ";" 8)
sdecl.findreplace("virtual HRESULT STDMETHODCALLTYPE" "[9]STDMETHODIMP")
sdecl.replacerx("\b__RPC__\w+ +")

str body=
 
 	{
 		return E_NOTIMPL;
 	}
 
sdecl.findreplace(";" body)

str unk=
;
 #pragma region virtual_impl
 	STD_IUNKNOWN_METHODS(IInterfaceName)
;
 
 
sdecl-unk
sdecl+"[]#pragma endregion[]"

out sdecl
 end
