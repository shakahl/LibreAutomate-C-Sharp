SYSTEMTIME st
GetLocalTime &st
out "%02i:%02i:%02i.%03i" st.wHour st.wMinute st.wSecond st.wMilliseconds
