dll "qm.exe" q_memset !*dest count
dll "qm.exe" q_memset_sse !*dest count
dll "qm.exe" q_memset_8 !*dest count
dll kernel32 RtlZeroMemory !*m n

str s.all(4000 2)

Q &q
rep(1000) q_memset s 4000
Q &qq
rep(1000) memset s 0 4000
Q &qqq
rep(1000) RtlZeroMemory s 4000
Q &qqqq
rep(1000) q_memset_sse s 4000
Q &qqqqq
rep(1000) q_memset_8 s 4000
Q &qqqqqq
outq
