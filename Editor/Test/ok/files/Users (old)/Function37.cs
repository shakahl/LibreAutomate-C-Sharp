DATE+ g_macro_started_date
DATE d.getclock
d=d-g_macro_started_date
str s1(g_macro_started_date) s2(d)
mes- "started %s, ran %s.[][]Press Yes to end." "" "YN" s1 s2
shutdown -6 0 "replace this with real macro name"
