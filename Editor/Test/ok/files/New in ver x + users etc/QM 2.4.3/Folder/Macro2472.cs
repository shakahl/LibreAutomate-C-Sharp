str url="https://onedrive.live.com/download.aspx?cid=346C476CB5B7FA65&resid=346C476CB5B7FA65%21343&canary=Pt85%2B6D%2FSTiOPYymJzsNktgUH3Xz8vbqD2V9bj4p4Fc%3D5"
IntGetFile url _s
Http h.Connect("onedrive.live.com" "support@quickmacros.com" "slapta1" 443)
h.Get("download.aspx?cid=346C476CB5B7FA65&resid=346C476CB5B7FA65%21343&canary=Pt85%2B6D%2FSTiOPYymJzsNktgUH3Xz8vbqD2V9bj4p4Fc%3D5" _s)
out _s
