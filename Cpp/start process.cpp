#include "stdafx.h"
#include "cpp.h"

namespace outproc
{

//EXPORT HRESULT Cpp_StartProcess(STR exeFile, STR args, STR workingDir, STR environment, out BSTR& sResult)
//{
//	HRESULT R;
//	Cpp_Acc aAgent;
//	if(R = InjectDllAndGetAgent(GetShellWindow(), out aAgent.acc)) {
//		return R;
//	}
//
//	InProcCall c;
//	auto p = (MarshalParams_StartProcess*)c.AllocParams(&aAgent, InProcAction::IPA_StartProcess, sizeof(MarshalParams_StartProcess));
//	p->...;
//	if(R = c.Call()) return R;
//	sResult = c.DetachResultBSTR();
//	return 0;
//}

} //namespace outproc
