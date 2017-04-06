//___________________________________________________________________

STDMETHODIMP ##::QueryInterface(REFIID iid, void ** ppv)
{
	ZZ("##::QueryInterface");
	*ppv=0;
	if(iid == IID_IUnknown || iid == xxx) *ppv = this;
	if(*ppv) { AddRef(); return S_OK; }
	return E_NOINTERFACE;
}
//___________________________________________________________________

STDMETHODIMP_(ULONG) ##::AddRef()
{
	ZZ("##::AddRef");
	return ++m_cRef;
}
//___________________________________________________________________

STDMETHODIMP_(ULONG) ##::Release()
{
	ZZ("##::Release");
	int ret=--m_cRef;
	if(!ret) delete this;
	return ret;
}
//___________________________________________________________________
//___________________________________________________________________

