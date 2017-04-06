 /
m_dir.expandpath(iif(_portable "%portable%\Data\User\HelpIndex" "$AppData$\GinDi\Quick Macros\HelpIndex"))

m_mw_help=CreateStringMap(5)
m_mw_tools=CreateStringMap(5)
m_mw_func=CreateStringMap(5)
m_mw_tips=CreateStringMap(5)

_s=
 a
 about
 an
 as
 at
 be
 by
 how
 I
 in
 is
 it
 of
 on
 or
 that
 the
 to
 was
 what
 when
 where
 who
 will
 with
;
 without
 you
 us
 can
 cannot
 and
 also
 ar
 not
 other
 thi
 more
 must
 some
 do
 doe
 did
 etc
 ha
 have
 which
 should
 than
 then
 thei
 instead
 most
 no
 them
 there
 becaus
 here
 into
 such
 these
 unless
 want

m_mstop=CreateStringMap(1)
m_mstop.AddList(_s "")
 The first part are Google stop-words, except some.
 The second part is from most frequently used words in QM help.
