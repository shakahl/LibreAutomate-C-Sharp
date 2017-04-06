str s rx
rx="\d(\d)\d\d\d"
s="78"
REGEXOPTIONS op.matchFlags=PCRE2_PARTIAL_SOFT
out RegexFind(s rx 0 0 0 0 op)
