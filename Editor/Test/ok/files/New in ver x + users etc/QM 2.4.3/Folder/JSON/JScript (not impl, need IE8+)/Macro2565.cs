HRESULT result = CoCreateInstance(m_LanguageId, 0, CLSCTX_ALL, IID_IActiveScriptProperty, (LPVOID*) &m_pScriptProperty);

if (result == S_OK && m_LanguageId == CLSID_JScript)

{

VARIANT set;

set.vt = VT_I4;

set.intVal = SCRIPTLANGUAGEVERSION_5_8;

if (m_pScriptProperty->SetProperty(SCRIPTPROP_INVOKEVERSIONING, NULL, &set) != S_OK)

m_bLoadJSONLibrary = true;

}