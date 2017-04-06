See maybe possible bugs because RRun::tx is byte. In many places it is set by compiler as UDT.
   The best would be to not use tx/stx at run time.
   Or review all, add ASSERT_TX_R, etc. For UDT use 255.
   Is RRun::stx really needed? Maybe instead just use tx I_INT when pointer.

shutdown 4/5: maybe call in same thread, because now on Vista QM does not receive PBT_APMSUSPEND.

Options: 'Cleanup' button. Deletes abandoned files, settings, etc.

Create chart of QM orders to see waves.
