 /DialogPrint
function hDlg showdlg

if(!g_psd9.lStructSize)
	g_psd9.lStructSize=sizeof(g_psd9)
	 default margins
	g_psd9.rtMargin.left=1500
	g_psd9.rtMargin.top=1500
	g_psd9.rtMargin.right=1500
	g_psd9.rtMargin.bottom=1500

if(showdlg)
	g_psd9.hwndOwner=hDlg
	g_psd9.Flags=PSD_MARGINS
	PageSetupDlg(&g_psd9)
