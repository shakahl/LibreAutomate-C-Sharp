Preview topic (F5) :mac "SHFB refresh topic previewer"
-
Focus code editor :SHFB_focus_editor
Set layout :mac "SHFB set layout" GetToolbarOwner(TriggerWindow)
-
MAML Reference :HtmlHelp 0 "C:\Program Files (x86)\EWSoftware\Sandcastle Help File Builder\Help\SandcastleMAMLGuide.chm" HH_DISPLAY_TOPIC 0
-
BLOCK
alert... :sub.SurroundWithAttributesMenu "<alert class=''@@''>[]  <para>##</para>[]</alert>[]" "note[]tip[]important"
code :SHFB_insert "<code language=''cs''><![CDATA[[]##]]></code>[]"
list... :sub.SurroundWithAttributesMenu "<list class=''@@''>[]  <listItem><para>##</para></listItem>[]  <listItem><para></para></listItem>[]  <listItem><para></para></listItem>[]  <listItem><para></para></listItem>[]  <listItem><para></para></listItem>[]  <listItem><para></para></listItem>[]  <listItem><para></para></listItem>[]  <listItem><para></para></listItem>[]</list>[]" "nobullet[]bullet[]ordered"
para :SHFB_insert "para" 2
table... :sub.table
-
INLINE
application (bold) :SHFB_insert "application"
[32]       Same style... :SHFB_insert sub.Menu("ui[]unmanagedCodeEntityReference[]>Other[]database[]hardware[]system[]userInput")
codeInline (d. blue) :SHFB_insert "codeInline"
[32]       Same style... :SHFB_insert sub.Menu("command[]>    command...[]system[]replaceable[]<[]computerOutputInline[]environmentVariable")
newTerm (italic) :SHFB_insert "newTerm"
[32]       Same style... :SHFB_insert sub.Menu("errorInline[]localUri[]>Other[]fictitiousUri[]foreignPhrase[]math[]phrase[]placeholder[]quoteInline")
literal (brown) :SHFB_insert "literal"
legacyX... :SHFB_insert sub.Menu("legacyBold[]legacyItalic[]legacyUnderline")
Other... :SHFB_insert sub.Menu("subscript[]superscript")
-
&#160; :"&#160;"
-
ANYWHERE
image block :SHFB_insert "<mediaLink>[]<caption>Optional caption</caption>[]<image xlink:href=''TopicId##''/>[]</mediaLink>[]" 5
image inline :SHFB_insert "<mediaLinkInline><image xlink:href=''TopicId##''/></mediaLinkInline>" 5
image info... :mes "mediaLink has some options, read in MAML Reference.[][]Or use the 'Entity References' pane (drag-drop or copy-paste), but it does not show new images until restarting SHFB."
-
link info... :mes "For in-page link (#address) and external link (http: etc) use the SHFB toolbar.[]For other-topic link and code-reference link use the 'Entity References' pane.[][]Explicit text for code-reference link: <codeEntityReference linkText=''...''>.[][]Previewer bug: requires some character (eg .) between <link .../> and </para>."
-
markup block :SHFB_insert "markup" 2
markup inline :SHFB_insert "markup"
-
token block :SHFB_insert "<token>##</token>[]"
token inline :SHFB_insert "<token>##</token>"
-
TOPIC
 introduction :SHFB_insert "<introduction>[]<para>##</para>[]</introduction>[]"
outline... :if('O'=mes("This will insert <autoOutline />.[][]Place text cursor in[]<introduction></introduction> or <content></content>.[][]More info in MAML Reference." "" "OCi")) SHFB_insert "<autoOutline />[]" 1
section :SHFB_insert "<section address=''optionalAddress''>[]  <title>Title##</title>[]  <content>[]    <para></para>[]  </content>[]</section>[]" 5
section+sections :SHFB_insert "<section address=''optionalAddress''>[]  <title>Title##</title>[]  <content>[]    <para></para>[]  </content>[]  []  <sections>[]    <section address=''optionalAddress''>[]      <title>Sub1</title>[]      <content>[]        <para></para>[]      </content>[]    </section>[]    []    <section address=''optionalAddress''>[]      <title>Sub2</title>[]      <content>[]        <para></para>[]      </content>[]    </section>[]    []    <section address=''optionalAddress''>[]      <title>Sub3</title>[]      <content>[]        <para></para>[]      </content>[]    </section>[]    []    <section address=''optionalAddress''>[]      <title>Sub4</title>[]      <content>[]        <para></para>[]      </content>[]    </section>[]    []  </sections>[]</section>[]" 5
relatedTopics :sub.relatedTopics


#sub Menu
function~ $items
int i=ShowMenu(items 0 0 2)-1
if(i<0) ret
_s.getl(items i)
ret _s


#sub SurroundWithAttributesMenu
function ~s $attributes
str attr=sub.Menu(attributes); if(!attr.len) ret
s.findreplace("@@" attr 4)
SHFB_insert s 2


#sub table
str s; int f
sel ShowMenu("1 table[]-[]2 title[]3 header[]4 row[]5 cell")
	case 1 s="<table>[]  <title>Optional title</title>[]  <tableHeader>[]    <row>[]      <entry><para>##</para></entry>[]      <entry><para></para></entry>[]    </row>[]  </tableHeader>[]  <row>[]    <entry><para></para></entry>[]    <entry><para></para></entry>[]  </row>[]  <row>[]    <entry><para></para></entry>[]    <entry><para></para></entry>[]  </row>[]  <row>[]    <entry><para></para></entry>[]    <entry><para></para></entry>[]  </row>[]  <row>[]    <entry><para></para></entry>[]    <entry><para></para></entry>[]  </row>[]  <row>[]    <entry><para></para></entry>[]    <entry><para></para></entry>[]  </row>[]  <row>[]    <entry><para></para></entry>[]    <entry><para></para></entry>[]  </row>[]  <row>[]    <entry><para></para></entry>[]    <entry><para></para></entry>[]  </row>[]  <row>[]    <entry><para></para></entry>[]    <entry><para></para></entry>[]  </row>[]</table>[]"; f=1
	case 2 s="title"
	case 3 s="<tableHeader><row>[]<entry><para>##</para></entry>[]</row></tableHeader>"
	case 4 s="<row>[]<entry><para>##</para></entry>[]</row>"
	case 5 s="<entry><para>##</para></entry>"
	case else ret
SHFB_insert s f


#sub relatedTopics m
_s=
     <relatedTopics>
       <!-- One or more of the following:
            - A local link
            - An external link
            - A code entity reference
;
       <link xlink:href="Other Topic's ID"/>
       <link xlink:href="Other Topic's ID">Link inner text</link>
;
       <externalLink>
           <linkText>Link text</linkText>
           <linkAlternateText>Optional alternate link text</linkAlternateText>
           <linkUri>URI</linkUri>
       </externalLink>
;
       <codeEntityReference>API member ID</codeEntityReference>
;
       Examples:
;
       <link xlink:href="00e97994-e9e6-46e0-b420-5be86b2f8270" />
       <link xlink:href="00e97994-e9e6-46e0-b420-5be86b2f8278">Some other topic</link>
;
       <externalLink>
           <linkText>SHFB on GitHub</linkText>
           <linkAlternateText>Go to GitHub</linkAlternateText>
           <linkUri>https://GitHub.com/EWSoftware/SHFB</linkUri>
       </externalLink>
;
       <codeEntityReference>T:TestDoc.TestClass</codeEntityReference>
       <codeEntityReference>P:TestDoc.TestClass.SomeProperty</codeEntityReference>
       <codeEntityReference>M:TestDoc.TestClass.#ctor</codeEntityReference>
       <codeEntityReference>M:TestDoc.TestClass.#ctor(System.String,System.Int32)</codeEntityReference>
       <codeEntityReference>M:TestDoc.TestClass.ToString</codeEntityReference>
       <codeEntityReference>M:TestDoc.TestClass.FirstMethod</codeEntityReference>
       <codeEntityReference>M:TestDoc.TestClass.SecondMethod(System.Int32,System.String)</codeEntityReference>
       -->
     </relatedTopics>
;
paste _s
