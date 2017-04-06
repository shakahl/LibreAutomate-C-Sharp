function t str&st

int h m
h=t/3600; t%3600
m=t/60; t%60
st.format("%i:%0i:%0i" h m t)
