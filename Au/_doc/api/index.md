# Automation library

### Namespaces
- Au - main classes of this library, except triggers.
- Au.Types - types of function parameters, exceptions, extension methods, etc.
- Au.Triggers - action triggers: hotkeys, autotext, mouse, window.
- Au.Util - rarely used classes.

### .NET Assembly files
- Au.dll - contains code of the above namespaces.

### Native dll files
- Dll/64bit/AuCpp.dll - used by Au.dll in 64-bit processes.
- Dll/32bit/AuCpp.dll - used by Au.dll in 32-bit processes.
- Dll/64bit/sqlite3.dll - used by the SQLite wrapper class in 64-bit processes.
- Dll/32bit/sqlite3.dll - used by the SQLite wrapper class in 32-bit processes.

These files are in the editor folder. When you create an .exe file from a script, they are automatically copied to the .exe folder, except sqlite3.dll.

Other dll files in the editor folder are not part of the library. They are undocumented.

<hr style="margin-top: 50px"/>
<div id="disqus_thread"></div>
<script>
/**
*  RECOMMENDED CONFIGURATION VARIABLES: EDIT AND UNCOMMENT THE SECTION BELOW TO INSERT DYNAMIC VALUES FROM YOUR PLATFORM OR CMS.
*  LEARN WHY DEFINING THESE VARIABLES IS IMPORTANT: https://disqus.com/admin/universalcode/#configuration-variables*/
/*
var disqus_config = function () {
this.page.url = PAGE_URL;  // Replace PAGE_URL with your page's canonical URL variable
this.page.identifier = PAGE_IDENTIFIER; // Replace PAGE_IDENTIFIER with your page's unique identifier variable
};
*/
(function() { // DON'T EDIT BELOW THIS LINE
var d = document, s = d.createElement('script');
s.src = 'https://qm3.disqus.com/embed.js';
s.setAttribute('data-timestamp', +new Date());
(d.head || d.body).appendChild(s);
})();
</script>

