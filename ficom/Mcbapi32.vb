Option Explicit On

' -------------------------------------------------------------------
' This is an intial draft of an updated implementation of the 
' historic Mcbapi32.BAS file.  
' -------------------------------------------------------------------

Imports System.Text

' -------------------------------------------------------------------
' The following table shows the equivalence of data types from the old
' VB 6.0 example (MCBAPI32.BAS) to this new .NET implementation.
'
' VB6	        VB.NET	    Comments
' ________      ________    ____________________________________________
' Integer	    Short	    16 bits
' Long	        Integer	    32 bits
' N/A	        Long	    64 bits
' Variant	    N/A	Use     the new 'Object' data type
' Currency	    N/A	Use     Decimal in VB6 or Decimal or Long in VB.NET
' N/A	        Decimal	    Available* in VB6. Native in VB.NET
' String	    String	    VB.NET doesn't support fixed length strings
' -------------------------------------------------------------------

Public Class Mcbapi32
    '-------------------------------------------------------------
    ' This file contains all definition and routines required to
    ' use the EG&G ORTEC Unified MCB Interface to communicate with
    ' and control the detectors.  These functions described in
    ' detail in the "Unified MCB Interface User's Guide"
    '
    '-------------------------------------------------------------

#Region "Definitions and Constants"
    '--------------------------------------
    ' Defined Const for UMCBI status codes
    '--------------------------------------
    'return from MIO function call
    Public Const MIO_TRUE = 1
    Public Const MIO_SUCCESS = 1
    Public Const MIO_ERROR = 0

    '**************************************************
    ' Define error codes returned by MIOGetLastError()
    '**************************************************
    Public Const MIOENONE = 0           ' No error (maybe an MCB warning)
    Public Const MIOEINVALID = 1        ' Det handle or other parameter is invalid
    Public Const MIOEMCB = 2            ' MCB reported error (see nMacroErr & nMicroErr)
    Public Const MIOEIO = 3             ' Disk, Network or MCB I/O error (see nMacroErr)
    Public Const MIOEMEM = 4            ' Memory allocation error
    Public Const MIOENOTAUTH = 5        ' Authorization or Password failure
    Public Const MIOEBLOCKING = 6       ' MCBCIO call already in progress
    ' (obsolete -- not used by MCBCIO32)
    Public Const MIOEINTR = 7           ' MCBCIO call interrupted
    ' (obsolete -- not used by MCBCIO32)
    Public Const MIOENOCONTEXT = 8      ' MCBCIO call before MIOStartup() or...
    '...after MIOCleanup().
    ' Sub codes for MIOEIO (nMacroErr)
    Public Const MIOEMACCLOSED = -1     '   Detector Comm Broken, Call MIOCloseDetector()
    Public Const MIOEMACTIMEOUT = -2    '   Detector communication timeout -- Try Again
    Public Const MIOEMACCOMM = -3       '   Detector communication error -- Try Again
    Public Const MIOEMACTOOMANY = -4    '   Too Many open detectors -- close detector then try again
    Public Const MIOEMACOTHER = -5      '   Disk, OS or any other error

    '-----------------------------------
    ' MIODebug Arguments
    '-----------------------------------
    Public Const MIODEBUGNONE = 0        'stop all debugging
    Public Const MIODEBUGNORMAL = 1
    Public Const MIODEBUGDETAIL = 2
    Public Const MIODEBUGALL = 3
    Public Const MIODEBUGSTARTTIMER = -1 'MIODEBUGSTARTTIMER
    Public Const MIODEBUGSTOPLOCAL = -2  'MIODEBUGGETTRACE
    Public Const MIODEBUGSTOPCLIENT = -3 'stop client debugging
    Public Const MIODEBUGTRACKTASK = -4  'stop client debugging
    Public Const MIODEBUGSTARTTRACE = -5 'stop client debugging
    Public Const MIODEBUGSTOPTRACE = -6  'stop client debugging
    Public Const MIODEBUGNETDUMP = -7    'network status debug dump
    Public Const MIODEBUGGETLEVEL = -8   'does nothing but return level
    Public Const MIODEBUGGETTRACE = -9   'return the trace level

    '------------------------------------------------
    ' Define feature codes used with MIOIsFeature()
    '------------------------------------------------
    Public Const MIOFEAT_CONVGAIN = 0              '' software settable conversion gain 
    Public Const MIOFEAT_COARSEGAIN = 1            '' software settable coarse gain 
    Public Const MIOFEAT_FINEGAIN = 2              '' software settable fine gain 
    Public Const MIOFEAT_GAINSTAB = 3              '' gain stabilizer 
    Public Const MIOFEAT_ZEROSTAB = 4              '' zero stabilizer 
    Public Const MIOFEAT_PHAMODE = 5               '' PHA mode functions available 
    Public Const MIOFEAT_MCSMODE = 6               '' MCS mode functions available 
    Public Const MIOFEAT_LISTMODE = 7              '' list mode functions available 
    Public Const MIOFEAT_SAMPMODE = 8              '' sample mode functions available 
    Public Const MIOFEAT_DIGITALOFF = 9            '' digital Offset (SET/SHOW_OFFSET) 
    Public Const MIOFEAT_FINEOFF = 10              '' software settable fine Offset (SET/SHOW_OFFS_FINE) 
    Public Const MIOFEAT_HVPOWER = 11              '' HV power supply (SHOW_HV, ENA/DIS_HV) 
    Public Const MIOFEAT_HVENHANCED = 12           '' Enhanced HV (SET_HV, SET/SHOW_HV_POL, SHOW_HV_ACT) 
    Public Const MIOFEAT_HVRANGE = 13              '' software settable HV range (ENA_NAI, DIS_NAI) 
    Public Const MIOFEAT_AUTOPZ = 14               '' auto PZ (START_PZ_AUTO) 
    Public Const MIOFEAT_MANPZ = 15                '' software settable manual PZ (SET/SHOW_PZ) 
    Public Const MIOFEAT_CLOCK = 16                '' internal clock (SHOW_DATE/TIME, SHOW_DATE/TIME_START) 
    Public Const MIOFEAT_SAMPCHANGER = 17          '' Sample Changer support (SET/SHOW_OUTPUT, SHOW_INPUT) 
    Public Const MIOFEAT_FIELDMODE = 18            '' one-button acq (ENA/DIS/SHOW_TRIG_SPEC, MOVE) 
    Public Const MIOFEAT_NOMADIC = 19              '' nomadic (likely to move between opens) 
    Public Const MIOFEAT_APPDATA = 20              '' local app data (SET_DATA_APP, SHOW_DATA_APP) 
    Public Const MIOFEAT_SERIALNUM = 21            '' software retrievable serial number (SHOW_SNUM) 
    Public Const MIOFEAT_POWERMAN = 22             '' power management commands (CONS, ON, OFF, etc.) 
    Public Const MIOFEAT_BATTERYSTAT = 23          '' battery status support (SH_STAT_BATT) 
    Public Const MIOFEAT_AMPPOLARITY = 24          '' software settable AMP polarity (SET/SHOW_GAIN_POLAR) 
    Public Const MIOFEAT_OPTIMIZE = 25             '' support for flattop optimization (ENA/DIS_OPTI) 
    Public Const MIOFEAT_STOPPZ = 26               '' stoppable AutoPZ (STOP_PZ_AUTO cmd) 
    Public Const MIOFEAT_NETWORK = 27              '' network support (SET/SHOW_NET_ADDR) 
    Public Const MIOFEAT_MULTIDROP = 28            '' multi-drop serial support (i.e. uNomad) 
    Public Const MIOFEAT_DPMADDR = 29              '' software settable DPM address (SET_DPM_ADDR) 
    Public Const MIOFEAT_MULTIDEV = 30             '' multiple devices (i.e. 919) 
    Public Const MIOFEAT_GATEMODE = 31             '' software settable ADC gate mode (SET_GATE...) 
    Public Const MIOFEAT_DOWNLOAD = 32             '' downloadable firmware 
    Public Const MIOFEAT_THAMODE = 33              '' time histogram functions available (i.e. 9308) 
    Public Const MIOFEAT_LLD = 34                  '' software settable Lower level disc (SET_LLD) 
    Public Const MIOFEAT_ULD = 35                  '' software settable Upper level disc (SET_ULD) 
    Public Const MIOFEAT_SCAINPUT = 36             '' MCS mode SCA input available 
    Public Const MIOFEAT_TTLINPUT = 37             '' MCS mode positive TTL input available 
    Public Const MIOFEAT_NEGNIMINPUT = 38          '' MCS mode fast negative NIM input available 
    Public Const MIOFEAT_DISCINPUT = 39            '' MCS mode discriminator input available 
    Public Const MIOFEAT_DISCEDGE = 40             '' software switchable discriminator edge 
    Public Const MIOFEAT_DISCLEVEL = 41            '' software programmable discriminator level 
    Public Const MIOFEAT_SCAPROG = 42              '' software programmable SCA upper and lower threasholds 
    Public Const MIOFEAT_INPUTSELECT = 43          '' software selectable MCS mode input sources 
    Public Const MIOFEAT_STATPRESET = 44           '' statistical preset (SET/SHOW_UNCERT_PRES) 
    Public Const MIOFEAT_VARIFEAT = 45             '' features vary by input (SHOW_FEAT depends on device/segment) 
    Public Const MIOFEAT_SHUTDOWN = 46             '' software settable HV shutdown mode (SET/SHOW/VERI_SHUTDOWN) 
    Public Const MIOFEAT_SHAPECONST = 47           '' software settable shaping time constants (SET_SHAP) 
    Public Const MIOFEAT_EXPLORESHAPE = 48         '' explorable shaping time constants (LIST_SHAP) 
    Public Const MIOFEAT_ADVANCEDSHAPE = 49        '' advanced shaping time (LIST/SET/SHOW_SHAP_RISE, _SHAPE_FLAT, etc.) 
    Public Const MIOFEAT_BLR = 50                  '' software settable BLR (ENA/DIS/SHO_BLR_AUTO SET/SHO/VERI_BLR) 
    Public Const MIOFEAT_SHOWSTAT = 51             '' SHOW_STATUS command supported (returns $M record) 
    Public Const MIOFEAT_OVERPRESET = 52           '' overflow preset (ENA/DIS/SHO_OVER_PRES) 
    Public Const MIOFEAT_CLICKER = 53              '' software enabled audio clicker (ENA/DIS_CLICK) 
    Public Const MIOFEAT_THERMISTOR = 54           '' software readable thermistor (SHOW_THERM) 
    Public Const MIOFEAT_FLOATFINE = 55            '' Fine Gain is float number (SET/SHO/VERI/LIST_GAIN_FINE) 
    Public Const MIOFEAT_PUR = 56                  '' software enabled Pile-up Rej. (ENA/DIS/SHO_PUR, SET_WIDT_REJ) 
    Public Const MIOFEAT_ALPHAHV = 57              '' Alpha style HV power (SHOW_HV_CURRENT) 
    Public Const MIOFEAT_VACUUM = 58               '' software readable vacuum (SHOW_VACUUM) 
    Public Const MIOFEAT_ACQALARM = 59             '' acquisition alarms (ENA/DIS_ALARM) 
    Public Const MIOFEAT_TRIGGER = 60              '' hardware acquisition trigger (ENA/DIS_TRIG) 
    Public Const MIOFEAT_ORDINALSHAP = 61          '' ordinal shapping times (SET_SHAP 0, SET_SHAP 1, ...) 
    Public Const MIOFEAT_LISTGAINS = 62            '' querey gain ranges (LIST/VERI_GAIN_FINE, ..._COAR, ..._CONV) 
    Public Const MIOFEAT_ROUTINPUT = 63            '' routable inputs (SET/SHOW_INPUT_ROUTE) 
    Public Const MIOFEAT_EXTDWELL = 64             '' external dwell support (ENA/DIS_DWELL_EXT) 
    Public Const MIOFEAT_SUMREPLACE = 65           '' selectable SUM or REPLACE MCS modes (ENA/DIS_SUM) 
    Public Const MIOFEAT_EXTSTART = 66             '' external Start support (ENA/DIS/SHO_START_EXT) 
    Public Const MIOFEAT_LISTMCS = 67              '' explorable MCS (LIST_SOURCE, LIST_LLSCA & LIST_ULSCA) 
    Public Const MIOFEAT_MDAPRESET = 68            '' Device support the MDA preset (DSPEC & 92xII) 
    Public Const MIOFEAT_ADCTYPE = 69              '' Software settable ADC type (Matchmaker) 
    Public Const MIOFEAT_DAISY = 70                '' Has ability to Daisy chain MCBs (DART) 
    Public Const MIOFEAT_ZERODT = 71               '' Zero Dead Time Functions available (DSPec+) 
    Public Const MIOFEAT_DSPPTRIG = 72             '' DSPec+ style InSite Triggering 
    Public Const MIOFEAT_MULTIINP = 73             '' Multiple input per connection support (OCTE+E) 
    Public Const MIOFEAT_COUNTRATE = 74            '' Has Hardware Count Rate monitor (SH_CRM) 
    Public Const MIOFEAT_MULTIZDT = 75             '' Has multiple ZDT modes (SET/SHOW/LIST_MODE_ZDT) 
    Public Const MIOFEAT_MULTIMDA = 76             '' Has multi nuclide MDA preset 
    Public Const MIOFEAT_MCSRPLSUM = 77            '' Has MCS Replace then Sum mode (SET_RPLSUM) 
    Public Const MIOFEAT_MCSPRGXDWELL = 78         '' Has programmable external dwell voltage capability 
    Public Const MIOFEAT_INTDIFSHAPE = 79          '' Separate shaping time for integration & differentiation...
    ''                                              ...(LIST/SET/SHOW_SHAP_INTE, _SHAP_DIFF) (SBS-60) 
    Public Const MIOFEAT_PRGPULSE = 80             '' Programmable pulser (ENAB/DISA/SHOW_PULSER) 
    Public Const MIOFEAT_PRGVCMINTRLK = 81         '' Programmable Vacuum/HV interlock (OASIS) 
    Public Const MIOFEAT_PRGCURINTRLK = 82         '' Programmable Current/HV interlock (OASIS) 
    Public Const MIOFEAT_EXPLORESTAB = 83          '' Explorable Stabilizer (LIST_GAIN_ADJU,LIST_ZERO_ADJU) 
    Public Const MIOFEAT_PRGINPUTIMP = 84          '' Has programmable input impedance (MCS)  
    Public Const MIOFEAT_NOCUSP = 85               '' Advanced shaping feature has no CUSP (DigiDART)  
    ''                                                [MIOFEAT_NOCUSP is valid only with MIOFEAT_ADVANCEDSHAPE] 
    Public Const MIOFEAT_HVRISE = 86               '' Settable HV risetime (SET/SHOW/LIST_HV_RISE) (SBS-60) 
    Public Const MIOFEAT_LISTGATE = 87             '' Explorable ADC GATE settings (LIST_GATE, SET_GATE n) 
    Public Const MIOFEAT_MONITORS = 88             '' Monitor command support(SHOW_MONI_MAX/LABEL/VALUE) 
    Public Const MIOFEAT_SMARTDET = 89             '' Smart Detector support (SHOW_DET_SMART, SHOW_DET_SNUM, SHOW_HV_RECO, ...) 
    Public Const MIOFEAT_NUCLIDEREPORT = 90        '' Nuclide report (SET/SHOW_NUCL_COEF, SET/SHOW_ROI_NUCL, ...) 
    Public Const MIOFEAT_INTERACTDISPLAY = 91      '' Interactive Display features such as Nuclide Report 
    Public Const MIOFEAT_ADVSTOREDSPECTRA = 92     '' SH_SPEC_COUNT, SHOW_SPEC_ID and MOVE cmds like DigiDART 
    Public Const MIOFEAT_MULTIVIEWDATA = 93        '' SET/SHOW_VIEW in MCBs with DPM or PP interfaces, LIST_VIEW in all MCBs 
    Public Const MIOFEAT_RS232 = 94                '' Connected to Mcb via RS-232 (Slow) port 
    Public Const MIOFEAT_HVNOSETPOL = 95           '' ENH HV does NOT support SET_HV_POSI, SET_HV_NEGA, ENA_NAI and DIS_NAI 
    ''                                                [MIOFEAT_HVNOSETPOL is valid only when MIOFEAT_HVENHANCED is set] 
    Public Const MIOFEAT_LOWFREQREJ = 96           '' Low Frequency Rejecter (ENA/DIS/SHOW_LFR DSPEC-PRO) 
    Public Const MIOFEAT_RESENHANCER = 97          '' Resolution Enhancer (ENA/DIS/SHOW_RENHANCER, SET/SHOW_RETABLE idx,val) 
    Public Const MIOFEAT_RESENHLIST = 98           '' SET_MODE_RELIST for Res Enhancer List Mode, returns ADC,Index 
    Public Const MIOFEAT_SHOWTIMESAMP = 99         '' Readable Sample mode time per chan (SH_TIME_SAMPLE) 
    Public Const MIOFEAT_SETTIMESAMP = 100         '' Settable Sample mode time per chan (SET/LIST_TIME_SAMPLE) 
    Public Const MIOFEAT_LISTMODEDB = 101          '' List Mode data streamed and formated like DB 
    Public Const MIOFEAT_HIGHTHROUGHPUT = 102      '' Supports High Throughput mode (ENA/DIS/SHOW_COWBOY) 
    Public Const MIOFEAT_LISTMODEPRO = 103         '' List Mode data streamed and formated like DSPEC-PRO 
    Public Const MIOFEAT_ENHMANPZ = 104            '' SET/SHOW/LIST_PZ using floating point microseconds 
    Public Const MIOFEAT_SHAPEREADONLYPPG = 105    '' Risetime, flattop width and cusp not user changeable from property page <6.04.05> 
    Public Const MIOFEAT_HVREADONLYPPG = 106       '' High Voltage not user changeable from property page <6.04.05> 
    Public Const MIOFEAT_AMPGAINREADONLYPPG = 107  '' Coarse and fine gain not user changeable from property page <6.04.05> 
    Public Const MIOFEAT_PZREADONLYPPG = 108       '' PZ and flattop tilt not user changeable from property page <6.04.05> 
    Public Const MIOFEAT_LFRREADONLYPPG = 109      '' LFR not user changeable from property page <6.04.05> 
    Public Const MIOFEAT_SYNCHLISTMODE = 110       '' Synhronized List Mode utilized in list mode (Portal Monitor Style) 
    Public Const MIOFEAT_DSPECPROAUXENAB = 111     '' AUX input BNC available (DSPEC-PRO & DSPEC-50) 
    Public Const MIOFEAT_NOSETDISPLAY = 112        '' SET/SHOW_DISPLAY command NOT used to select ZDT data view <6.09.00> 
    Public Const MIOFEAT_IDREPORTS = 113           '' ID Reports available (DO_ID, SHOW_REPORT, SHOW_REPORT_LINES) <6.09.03> 
    Public Const MIOFEAT_HASNEUTRONDET = 114       '' Has Neutron Detector (SHOW_CRM 2 returns valid number) 
    Public Const MIOFEAT_LISTMODEDBE = 115         '' List Mode data streamed and formatted like DIGIBaseE  <6.09.04> 
    Public Const MIOFEAT_ADVANCEDALPHA = 116       '' Has the Alpha Aria, Duo and Ensemble feature set <6.10.00> 
    Public Const MIOFEAT_SELECTABLEHVSOURCE = 117  '' Has the DSPEC 50 style selectable HV source (SET/SHO/LIST_HV_SOURCE) <7.00.01> 
    Public Const MIOFEAT_NOISE_REJ = 118           '' Has the DSPEC 50 style Noise Rejection (SET/SHO/LIST_NOISE_REJ) <7.02.01> 
    Public Const MIOFEAT_CONTRAST = 119            '' Has adjustable display contrast (SET/SHOW_CONTRAST) <8.01.07> 
    Public Const MIOFEAT_PINGPONG = 120            '' Has SHOW_HPRDS and SHOW_VMON 3 commands for Ping-Pong acquisition <8.02.06> 

    Public Const MIOFEAT_EXTENDED = 127            '' extended feature mask available (SH_FEAT_EXT) 
    Public Const MIOFEAT_EXTENDED2 = 255           '' extended feature mask available (SH_FEAT_EXT2) 
    Public Const MIOFEAT_EXTENDED3 = 383           '' extended feature mask available (SH_FEAT_EXT3) 

    Public Const MIOLASTDET = -1

    '------------------
    ' Data structures
    '------------------
    Structure TM ' Local time structure
        Dim tm_sec As Integer
        Dim tm_min As Integer
        Dim tm_hour As Integer
        Dim tm_mday As Integer
        Dim tm_mon As Integer
        Dim tm_year As Integer
        Dim tm_wday As Integer
        Dim tm_yday As Integer
        Dim tm_isDat As Integer
    End Structure
#End Region

#Region "Documented API Function"
    '----------------------------------------------------------
    '
    '   Unified MCB Interface function declarations
    '
    '----------------------------------------------------------

    '----------------------------------------------------------
    'Function Name: MIOAddConfigItem
    '
    'Action:	    Add a detector to a named pick list. 
    'Comments:	    This function allows the application to copy information from the master detector list
    '               to a private detector pick list for use in detector selection via MIOOpenDetector().
    'Parameters:	nDet	Insertion point for new detector. Use MIOLASTDET to insert at the end of the 
    '                       list (recommended). If another is specified, the detector will be inserted into
    '                       the list at the point specified. If that position in the list is filled, the 
    '                       existing items will be shifted down in the list and the new item inserted at the
    '                       nDetth location.
    '	            lpszListName	List name string (up to 5 characters.)
    '	            nMasterDet	Index in master list of the detector information to copy. 
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error
    '               number may be retrieved by calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOAddConfigItem")> Public Shared Function MIOAddConfigItem(ByVal nDet As Integer, ByVal lpszListName As String, ByVal nMasterDet As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOCleanup
    '
    'Action:	    Terminate use of the MCBCIO DLL.
    'Comments:	    The application or DLL that links to the UMCBI library, MCBCIO32.LIB, must call this
    '               function after the last call to MCBCIO functions. It allows the MCBCIO system to 
    '               perform application related cleanup. Multiple calls to this function must be separated
    '               by calls to MIOStartup().
    'Return Value:	The return value is always TRUE (this function has no errors). This function does 
    '               not affect the error status returned by MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOCleanup")> Public Shared Function MIOCleanup() As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOClearROI
    '
    'Action:	    Clear the ROI flag for the specified channels for the detector represented by hDet.
    'Comments:	    This function clears hardware ROI flags that may be retrieved along with the “raw” 
    '               instrument data by calling MIOGetData(). No changes are made to channels that already
    '               have cleared ROI flags. Set wNumChans to MIOMAXCHANS to clear ROI flags for the maximum
    '               number of channels available (starting with wStartChan).
    'Parameters:	hDet	Supplied handle to an open detector (see MIOOpenDetector()).
    '	            wStartChan	Number of first channel (starting with 0) to change. 
    '	            wNumChans	Number of channels to change. 
    '               lpszAuth	Reserved for future expansion. Must be “”. 
    '               lpszPass	Supplied string that is compared with the stored password string for the
    '                           specified detector to determine if this function will be allowed. If the 
    '                           strings match, this function is allowed; otherwise this function fails. 
    '                           This string is ignored if the detector has not been locked with 
    '                           MIOLockDetector().
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific
    '               error number may be retrieved by calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOClearROI")> Public Shared Function MIOClearROI(ByVal hDet As Integer, ByVal sChan As Short, ByVal nChan As Short, ByVal auth As String, ByVal Pass As String) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOCloseDetector
    '
    'Action:	    Close a communication channel previously opened by MIOOpenDetector().
    'Parameters:	hDet	Supplied handle to an open detector (see MIOOpenDetector()).
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific 
    '               error number may be retrieved by calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOCloseDetector")> Public Shared Function MIOCloseDetector(ByVal hDet As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOComm
    '
    'Action:	    Send an instrument command and receive the instrument response.
    'Comments:	    This function waits for the specified instrument to become ready to accept a command, 
    '               sends the command, waits for the response to become ready, and reads the response. Only 
    '               nondestructive commands will be accepted by the detector if a password is set and 
    '               lpszPass does not match that password. Destructive commands such as CLEAR, START, or 
    '               STOP will be allowed if lpszPass matches the detector password or if no password is set.
    'Parameters:	hDet	Supplied handle to an open detector (see MIOOpenDetector()).
    '               lpszCmd	Supplied NULL terminated instrument command string.
    '               lpszAuth	Reserved for future expansion. Must be “”. 
    '               lpszPass	Supplied string that is compared with the stored password string for the 
    '                           specified detector to determine if this function will be allowed. If the 
    '                           strings match, this function is allowed; otherwise this function fails. This 
    '                           string is ignored if the detector has not been locked with MIOLockDetector().
    '               nMaxResp	Supplied size in bytes of the lpszResp buffer. Set this value to zero to 
    '                           ignore instrument responses.
    '               lpszResp    Optional caller supplied buffer to receive the NULL terminated instrument data
    '                           response, if any, to the command in lpszCmd. The handshake messages (%000000069 = OK)
    '                           are not returned here but are interpreted by the system and the error codes are 
    '                           returned by calling MIOGetLastError(). This buffer must be at least nMaxResp bytes 
    '                           long. A NULL is always appended to the end of the instrument response even if the 
    '                           response is truncated to fit in the supplied buffer. Set this parameter to NULL and
    '                           nMaxResp to zero to ignore instrument responses. (Note: In Visual Basic 6.0, this must
    '                           be a fixed-length string.)
    '               lpnRespLen	Optional Long variable to receive the actual number of bytes copied to the lpszResp
    '                           buffer, not including the terminating NULL. Set this parameter to NULL if the response 
    '                           length is not needed.
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number 
    '               may be retrieved by calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOComm")> Public Shared Function MIOComm(ByVal hDet As Integer, ByVal comm As String, ByVal auth As String, ByVal Pass As String, ByVal nbuf As Integer, ByVal buf As StringBuilder, ByRef rbuf As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOCreateConfigList
    '
    'Action:	    Create new detector pick list. If the named list exists it will be overwritten.
    'Comments:	    This function allows the application to create a private detector pick list for use in detector 
    '               selection via MIOOpenDetector().
    'Parameters:	lpszListName	List name string (up to 5 characters.)
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number 
    '               may be retrieved by calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOCreateConfigList")> Public Shared Function MIOCreateConfigList(ByVal lpszListName As String) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetAccess
    '
    'Action:	    Return the access level for a detector.
    'Comments:	    This function returns an access code that describes the permitted access to the detector represented by 
    '               hDet. The access level is determined by lpszPass.
    'Parameters:	hDet	        Supplied handle to an open detector (see MIOOpenDetector()).
    '	            lpszAuth	    Reserved for future expansion. Must be “”.
    '               lpszPass	    Supplied string that is compared with the stor'ed password string for the specified 
    '                               detector to determine if destructive access will be allowed. If the strings match, this 
    '                               function returns 3 in lpnAccess; otherwise lpnAccess is set to 1. This string is ignored 
    '                               and lpnAccess is set to 3 if the detector has not been locked with MIOLockDetector().
    '               lpszOwnerName	Optional returned NULL terminated string that is set to the detector owner's name if the 
    '                               detector is locked. (Note: In Visual Basic 6.0, this must be a fixed-length string.)
    '               nMaxOwnerName	Supplied length of lpszOwnerName buffer above. Use 0 for nMaxOwnerName and NULL for 
    '                               lpszOwnerName if name of detector owner is not needed.
    '               lpbLocked	    Optional returned BOOL that is TRUE if the detector is locked and FALSE otherwise. Use 
    '                               NULL if lock status is not needed.
    '               lpnAccess	    Integer detector access code returned from function: 
    '                               1 No destructive access is permitted. 
    '                               3 All functions are permitted. 
    '                              All other codes are reserved for future expansion. Use NULL if the access code is not needed.
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may 
    '               be retrieved by calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetAccess")> Public Shared Function MIOGetAccess(ByVal hDet As Integer, ByVal auth As String, ByVal Pass As String, ByVal lpszOwner As StringBuilder, ByVal ccOwnerMax As Integer, ByRef fLocked As Integer, ByRef iAccess As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetAppData
    '
    'Action:	    Retrieve arbitrary string previously set by MIOSetAppData().
    'Comments:	    This function returns the NULL terminated string associated with the name lpszDataName that was previously 
    '               stored for the detector represented by hDet by a call to MIOSetAppData(). See Appendix A for use of ORTEC 
    '               application data.
    'Parameters:	hDet            Supplied handle to an open detector (see MIOOpenDetector()).
    '               lpszDataName    Supplied NULL terminated printable ASCII string no longer than MIOAPPDATANAMEMAX bytes 
    '                               (including terminating NULL) that identifies the data string. The UMCBI does not distinguish
    '                               between upper case and lower case letters in the data name. SampleDescription, SAMPLEDESCRIPTION 
    '                               and sampledescription are all considered the same name. Data names should start with either an 
    '                               alphabetic or numeric character but not special characters like $, %, @, !, or \.
    '               lpszDefault     Supplied NULL terminated printable ASCII string no longer than MIOAPPDATAMAX bytes (including 
    '                               terminating NULL) that will be returned in lpszDataString if no stored string is found. This 
    '                               parameter must never be NULL.
    '               lpszDataString	Returned NULL terminated AppData string if one was found or the default string (lpszDefault) if 
    '                               no stored string is found. (Note: In Visual Basic 6.0, this must be declared as a fixed-length 
    '                               string.) 
    '               nMaxDataString  Supplied length of lpszDataString buffer above.
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved
    '               by calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetAppData")> Public Shared Function MIOGetAppData(ByVal hDet As Integer, ByVal LpszDataName As String, ByVal lpszDefault As String, ByVal lpszData As StringBuilder, ByVal ccMax As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetConfigMax
    '
    'Action:	    Retrieve the maximum detector index that may be given to MIOOpenDetector() or MIOGetConfigName().
    'Comments:	    This function retrieves the highest detector index configured for the specified detector list. Out of date detector
    '               lists (lpbOutDated == TRUE) are not returned. Instead, the master detector list is returned just as if lpszListName 
    '               was set to “”. The master detector list can never be out of date.
    'Parameters:    lpszListName    Supplied 0 to 5 character NULL terminated string that specifies which detector list to read. Use “” 
    '                               for the master list.
    '               nDetMax	        Returned maximum index number that may be used for the specified detector list.
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved by 
    '               calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetConfigMax")> Public Shared Function MIOGetConfigMax(ByVal ListName As String, ByRef MaxDet As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetConfigName
    '
    'Action:	Retrieve the description and ID for a given detector index.
    'Comments:	    This function returns the configuration name and ID associated with a given index number in a given detector list.  
    '               The index number (nDetIdx) may be from 1 through MIOGetConfigMax() in value and is used for opening a communication 
    '               channel with a detector. lpszListName specifies which detector list to read. Out-of-date detector lists (lpbOutDated 
    '               == TRUE) are not returned. Instead, the master detector list is returned just as if lpszListName was set to “” if 
    '               lpbOutDated returns TRUE. The master detector list can never be out of date.
    'Parameters:    nDetIdx	Supplied index number (starting with 1) for which data is to be retrieved. (See MIOGetConfigMax()).
    '               lpszListName	Supplied 0 to 5 character NULL terminated string specifying which detector list to read. Use “” for 
    '                               the master list.
    '               nNameMax        Number of bytes in szName.
    '               szName	        Supplied buffer that receives configuration name. The name may be up to MIODETNAMEMAX bytes long, 
    '                               including NULL. (Note: must be a fixed-length string in Visual Basic 6.0.)
    '               lpdwID	        Supplied double word to receive instrument ID.
    '               lpbOutDated	    Optional returned BOOL that is TRUE if detector list is out of date. All detector index values are
    '                               out of date if any one value is out of date.
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved 
    '               by calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetConfigName")> Public Shared Function MIOGetConfigName(ByVal nDet As Integer, ByVal List As String, ByVal ccMax As Integer, ByVal dName As StringBuilder, ByRef Id As Integer, ByRef OutDated As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetData
    '
    'Action:	    Copy instrument data into caller’s buffer.
    'Comments:	    This function copies the specified instrument data in its “raw” form (exactly as it appears in the instrument) to the 
    '               caller’s buffer.
    'Parameters:    hDet	Supplied handle to an open detector (see MIOOpenDetector())
    '               wStartChan	Number of first channel (starting with 0) to return.
    '               wNumChans	   Number of channels to return. The lpdwBuffer buffer must be large enough to hold the number of channels 
    '                              requested.
    '               lpdwBuffer	   Supplied buffer that will receive the raw channel data. Use NULL if not needed.
    '               lpwRetChans	   Optional WORD returned with the actual number of channels copied. Use NULL if not needed.
    '               lpdwDataMask   Optional returned DWORD that must be ANDed with raw channel data to remove any non-data bits.
    '               lpdwROIMask	   Optional returned DWORD that must be ANDed with raw channel data to remove any non-ROI bits. 
    '               lpszAuth       Reserved for future expansion. Must be “”.
    'Return Value:	The return value is a pointer to the requested number of channels (or fewer) of raw instrument data if successful. If 
    '               NULL is returned, the function failed and a specific error number may be retrieved by calling MIOGetLastError(). 
    '               (Note: In Visual Basic 6.0, any non-zero value indicates success.)
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetData")> Public Shared Function MIOGetData(ByVal hDet As Integer, ByVal sChan As Short, ByVal nChan As Short, ByRef Buffer As Integer, ByRef rchan As Short, ByRef datamask As Integer, ByRef roimask As Integer, ByVal auth As String) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetDetectorInfo
    '
    'Action:	    Retrieve detector information for an open detector.
    'Comments:	    This function returns information associated with a given detector. The information consists of the instrument ID
    '               number and detector description (up to MIODETDESCMAX chars including NULL). Only one copy of this information is
    '               maintained for the entire network. The returned information is intended to be used to universally identify the 
    '               associated detector in subsequent analysis reports. See page 92 for comments about instrument ID.
    'Parameters:    hDet	        Supplied handle to an open detector (see MIOOpenDetector()).
    '               lpszDesc	    Supplied buffer to receive description text. (Note: In Visual Basic 6.0, this must be a fixed-length string.) 
    '               nMaxDesc        Size of buffer to receive description text.
    '	            lpbDefaultDesc	Optionally returned TRUE if default description returned.
    '	            lpdwID	        Supplied double word to receive instrument ID.
    '	            lpbDefaultID	Optionally returned TRUE if default ID returned. 
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved by
    '               calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetDetectorInfo")> Public Shared Function MIOGetDetectorInfo(ByVal hDet As Integer, ByVal lpszDesc As StringBuilder, ByVal nMax As Integer, ByRef fDefDesc As Integer, ByRef lDetId As Integer, ByRef fDefid As Integer) As Short
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetDetLength
    '
    'Action:	    Get the total number of channels associated with detector hDet.
    'Comments:	    This function returns the total number of channels associated with a detector. This number is equivalent to the maximum
    '               conversion gain that may be set for the detector. Unpredictable results occur if this function is called with an invalid 
    '               hDet.
    'Return Value:	The return value is the number of channels associated with hDet. This function has no errors and does not affect the
    '               error status returned by MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetDetLength")> Public Shared Function MIOGetDetLength(ByVal hDet As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetLastError
    '
    'Action:	    Get the error status for the last MCBCIO function that was called by the application.
    'Comments:	    This function returns the completion status of the last MCBCIO function that was called. When an MCBCIO function returns 
    '               FALSE, this function should be called to retrieve the related error number. The error number is stored in a common 
    '               location for each application or thread that calls MIOStartup() and is overwritten by most MCBCIO functions. Therefore, 
    '               this function should be called immediately after the failed MCBCIO function is called. See hardware manual for the 
    '               individual instrument for the meaning of the Macro and Micro codes.
    'Parameters:	nMacroErr	Optional returned integer that represents the Macro error code returned from the detector if the completion 
    '                           status is MIOEMCB or a specially defined code if the completion status is MIOEIO (see definition below). Use 
    '                           NULL if the Macro error code is not needed.
    '	            nMicroErr	Optional returned integer that represents the Micro error code returned from the detector. This value is only
    '                           meaningful if the MCBCIO completion status is MIOEMCB. Use NULL if the Micro error code is not needed.
    'Return Value:	The return value indicates the error number for the last MCBCIO function called by the application as follows:
    '               MIOENONE 	Function completed successfully. 
    '               MIOEINVALID Invalid function parameter supplied. 
    '               MIOEMCB 	Detector rejected command (see nMacroErr and nMicroErr). 
    '               MIOEIO 	    Instrument communication or network I/O error. 
    '               nMacroErr 	Returned as follows: 
    '                           •	MIOEMACCLOSED 	Connection is broken. Call MIOCloseDetector(). 
    '                           •	MIOEMACTIMEOUT 	Communication time-out, try again. 
    '                           •	MIOEMACCOMM 	Other communication error, try again. 
    '                           •	MIOEMEM 	    Memory allocation error. 
    '                           •	MIOENOTAUTH 	Detector locked and lpszPass does not match. 
    '                           •	MIOENOCONTEXT 	MCBCIO function called before MIOStartup() or after MIOCleanup()
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetLastError")> Public Shared Function MIOGetLastError(ByRef MacroError As Integer, ByRef MicroError As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetRevision
    '
    'Action:	    Return the revision number for the MCB Client I/O interface software (this software).
    'Comments:	    This function may be used to insure function compatibility with future revisions of the MCB Client I/O interface software.
    'Return Value:	An integer number interpreted as two byte values. The low byte represents the minor revision and the high byte represents 
    '               the major revision. The first release of the software returns 0x0100. This function has no errors and does not affect the 
    '               error status returned by MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetRevision")> Public Shared Function MIOGetRevision() As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetStartTime
    '
    'Action:	    Retrieve the acquisition start time for the data in the currently selected detector.
    'Comments:	    This function retrieves the 4-byte integer acquisition start time for the spectral data associated with the detector, hDet.    
    '               The current time (retrieved from the same time source as the start time) will be returned in lCurrentTime. The format of the
    '               time is the format used by the Microsoft C and C++ Run-Time Library function time(). This function returns the number of 
    '               seconds since midnight (00:00:00) January 1, 1970, Universal Coordinated Time. See MIOlocaltime for more details on Visual 
    '               Basic 6.0 uses.
    'Parameters:	hDet	        Supplied handle to an open detector (see MIOOpenDetector()).
    '	            lCurrentTime	Optional current time returned in the same format as the start time. This can be used to compensate for 
    '                               differences in the system clocks between networked computers. Use NULL if the current time is not needed.
    'Return Value:	The return value is non-zero if successful. A zero return value indicates an error and the specific error number may be 
    '               retrieved by calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetStartTime")> Public Shared Function MIOGetStartTime(ByVal hDet As Integer, ByRef LTime As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOGetTypeEx
    '
    'Action:	    Retrieve the SHOW_VERSION string for the currently selected detector.
    'Comments:	    This function retrieves the 8-char hardware version string for the instrument associated with the currently selected detector. 
    '               The returned string is used to determine what hardware specific features are available for the detector. Note that an appropriate 
    '               string will be returned for all instruments, even ones without the SHOW_VERSION instrument command. Unpredictable results occur
    '               if this function is called with an invalid hDet.
    'Parameters:	hDet	    Supplied handle to an open detector (see MIOOpenDetector()).
    '               lpszType    Caller supplied buffer to receive the NULL terminated instrument data response.
    '               ccMax       Supplied size in bytes of the lpszType buffer. Set this value to zero to ignore instrument responses.
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved
    '               by calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOGetTypeEx")> Public Shared Function MIOGetTypeEx(ByVal hDet As Integer, ByVal lpszType As StringBuilder, ByVal ccMax As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOIsActive
    '
    'Action:	    Return TRUE if the detector is collecting data.
    'Comments:	    This function returns TRUE (nonzero) if the detector represented by hDet is collecting data, FALSE otherwise.
    'Parameters:	hDet	Supplied handle to an open detector (see MIOOpenDetector()).
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved by calling 
    '               MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOIsActive")> Public Shared Function MIOIsActive(ByVal hDet As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOIsDetector
    '
    'Action:	    Return TRUE if the detector handle hDet is valid (associated with an open detector).
    'Comments:	    This function returns TRUE if hDet is valid. Specifically, hDet must be a non-NULL return from MIOOpenDetector() that has not 
    '               been closed by MIOCloseDetector().
    'Parameters:	hDet	Supplied detector handle to be tested.
    'Return Value:	The return value is TRUE (nonzero) if hDet is valid, FALSE otherwise. This function has no errors and does not affect the 
    '               error status returned by MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOIsDetector")> Public Shared Function MIOIsDetector(ByVal hDet As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOIsFeature
    '
    'Action:	    Determine if the specified feature is available for a detector.
    'Comments:	    This function is used to test the features of a detector for example to determine if the detector has a software controllable 
    '               conversion gain, gain stabilizer, digital offset, etc. To make a low overhead function, the feature information will be
    '               maintained locally for all open detectors.
    'Parameters:	hDet	  Supplied handle to an open detector (see MIOOpenDetector()).
    '	            nFeature  Integer value of feature to interrogate. The list of features and their associated numbers may be found in the 
    '                         file MCBCIO32.H (for C programmers) and MCBAPI32.BAS (for VisualBasic 6.0 programmers).
    'Return Value:	The return value is TRUE if the feature is present in the detector, FALSE if the feature is unavailable.
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOIsFeature")> Public Shared Function MIOIsFeature(ByVal hDet As Integer, ByVal nFeature As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOlocaltime
    '
    'Action:	    Convert from longword time to MIOTM structure.
    'Comments:	    This function converts time from a 4-byte integer to the MIOTM structure. If the lTime parameter is zero, the function returns the 
    '               current system time. This function duplicates the C-language function localtime. It is provided to allow Visual Basic 6.0 
    '               applications to interpret the detector start time. Visual Basic 6.0 programs should declare the lpMIOtm variable using the 
    '               user-defined data type TM defined in the MCBAPI.BAS module.  (Dim lpMIOtm as TM)
    'Parameters:	lTime	  Supplied 4-byte integer time (see MIOGetStartTime()).
    '	            lpMIOtm	  User-allocated structure of integers representing time.
    'Return Value:	The return value is a longword representing the time. If lTime was set to 0, the return value is the 4-byte integer representing 
    '               the current system time, otherwise it is the same as lTime.
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOlocaltime")> Public Shared Function MIOlocaltime(ByVal LTime As Integer, ByRef tmTime As TM) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOLockDetector
    '
    'Action:	    Lock a detector.
    'Comments:	    This function password protects a detector to prevent destructive access. The detector remains locked until the MIOUnlockDetector() 
    '               function is called with the same password used to lock the detector.
    'Parameters:	hDet	        Supplied handle to an open detector (see MIOOpenDetector()).
    '	            lpszAuth	    Reserved for future expansion. Must be “”. 
    '	            lpszPass	    Supplied string that is saved and compared with the supplied password string for the specified detector to determine 
    '                               if that function will be allowed.
    '	            lpszOwnerName	Supplied string that will be saved as the owner’s name and will be returned by MIOGetAccess(). A maximum of 
    '                               MIOOWNERMAX characters will be saved, including the terminating NULL.
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved by calling
    '               MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOLockDetector")> Public Shared Function MIOLockDetector(ByVal hDet As Integer, ByVal auth As String, ByVal Pass As String, ByVal lpszOwner As String) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOOpenDetector
    '
    'Action:	    Open communication channel with an instrument.
    'Comments:	    This function prepares for command and data communication with a specific detector, either local or remote. The index 
    '               (nDetIdx) is the same ordinal number used in MIOGetConfigName().
    'Parameters:	nDetIdx	        Supplied index number that matches the index used with MIOGetConfigName().
    '	            lpszListName	Supplied 0 to 5 character string that specifies which detector list to use. Use “” for the master list. 
    '               lpszAuth        Reserved for future expansion. Must be “”.
    'Return Value:	The return value is an MCBCIO communication handle to the requested detector. The handle is used by all MCBCIO 
    '               communication functions and represents a specific stream of commands, responses, and data requests. If NULL is returned, 
    '               the function failed and a specific error number may be retrieved by calling MIOGetLastError(). MIOCloseDetector() need 
    '               not be called if NULL is returned.
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOOpenDetector")> Public Shared Function MIOOpenDetector(ByVal nDet As Integer, ByVal ListName As String, ByVal auth As String) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOOpenDetID
    '
    'Action:	    Open communication channel with an instrument given the detector's ID number. 
    'Comments:	    This function performs the same operation as MIOOpenDetector() except that it uses an instrument ID number (see 
    '               MIOGetDetectorInfo()) instead of a detector index and list name.
    'Parameters:	dwDetID	    Supplied instrument ID number corresponding to the detector to be opened.
    '	            lpszAuth	Reserved for future expansion. Must be “”.
    'Return Value:	The return value is an MCBCIO communication handle to the requested detector. This handle is identical to the one 
    '               returned by MIOOpenDetector().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOOpenDetID")> Public Shared Function MIOOpenDetID(ByVal Id As Integer, ByVal auth As String) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOSetAppData
    '
    'Action:	    Set arbitrary application specific, detector related string to be retrieved by MIOGetAppData().
    'Comments:	    This function allows the application to store strings associated with the detector for later retrieval. Strings stored with
    '               the same name for the same detector will replace previously stored strings.
    'Parameters:	hDet	         Supplied handle to an open detector (see MIOOpenDetector()).
    '	            lpszDataName	 Supplied NULL terminated printable ASCII string no longer than MIOAPPDATANAMEMAX (32) bytes (including 
    '                                terminating NULL) that identifies the data string. The UMCBI does not distinguish between upper case and 
    '                                lower case letters in the data name. SampleDescription, SAMPLEDESCRIPTION and sampledescription are all 
    '                                considered the same name. Data names should start with either an alphabetic or numeric character but not
    '                                special characters like $, %, @, !, or \.
    '	            lpszDataString	 Supplied NULL terminated printable ASCII string no longer than MIOAPPDATAMAX (128) bytes (including 
    '                                terminating NULL) to be stored.
    '	            lpszAuth	     Reserved for future expansion. Must be “”.
    '	            lpszPass	     Supplied string that is compared with the stored password string for the specified detector to determine 
    '                                if this function will be allowed. If the strings match, this function is allowed; otherwise this function 
    '                                fails. This string is ignored if the detector has not been locked with MIOLockDetector().
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved by
    '               calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOSetAppData")> Public Shared Function MIOSetAppData(ByVal hDet As Integer, ByVal LpszDataName As String, ByVal lpszData As String, ByVal auth As String, ByVal Pass As String) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOSetROI
    '
    'Action:	    Set the ROI flag(s) for the specified channels for the detector represented by hDet.
    'Comments:	    This function sets hardware ROI flags that may be retrieved along with the “raw” instrument data by calling MIOGetData().
    '               No changes are made to channels that already have set ROI flags. Set wNumChans to MIOMAXCHANS to set ROI flags for the 
    '               maximum number of channels available (starting with wStartChan).
    'Parameters:	hDet	     Supplied handle to an open detector (see MIOOpenDetector()).
    '	            wStartChan	 Number of first channel (starting with 0) to change.
    '	            wNumChans	 Number of channels to change.
    '	            lpszAuth	 Reserved for future expansion. Must be “”. 
    '	            lpszPass	 Supplied string that is compared with the stored password string for the specified detector to determine if 
    '                            this function will be allowed. If the strings match, this function is allowed; otherwise this function fails. 
    '                            This string is ignored if the detector has not been locked with MIOLockDetector().
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved by 
    '               calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOSetROI")> Public Shared Function MIOSetROI(ByVal hDet As Integer, ByVal sChan As Short, ByVal nChan As Short, ByVal auth As String, ByVal Pass As String) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOStartup
    '
    'Action:	    Initiate use of the MCBCIO32 DLL.
    'Comments:	    The application that links to the UMCBI library, MCBCIO32.LIB, must call this function before any other MCBCIO function. It 
    '               allows the MCBCIO system to perform application related initialization. Multiple calls to this function must be separated 
    '               by calls to MIOCleanup().
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a message box explains the problem. This function
    '               does not affect the error status returned by MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOStartup")> Public Shared Function MIOStartup() As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOUnlockDetector
    '
    'Action:	    Unlock a detector.
    'Comments:	    This function removes the password protection previously set by a call to MIOLockDetector().
    'Parameters:	hDet	    Supplied handle to an open detector (see MIOOpenDetector()).
    '	            lpszAuth	Reserved for future expansion. Must be “”.
    '	            lpszPass	Supplied string that must match the existing password for this function to succeed. This function always returns
    '                           successfully if the specified detector is not locked (has no password set).
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved by calling 
    '               MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOUnlockDetector")> Public Shared Function MIOUnlockDetector(ByVal hDet As Integer, ByVal auth As String, ByVal Pass As String) As Integer
    End Function
#End Region

#Region "Undocumented API Function"
    '----------------------------------------------------------------------------------
    ' Undocumented MIO functions
    '----------------------------------------------------------------------------------

    '----------------------------------------------------------
    'Function Name: MIODebug
    '
    'Action:	    Set the debug output level to be used when executing your CONNECTIONS programs.
    'Parameters:    Flag     The desired debug output level.  The following values are valid:
    '                        MIODEBUGNONE  = 0         (stop all debugging)
    '                        MIODEBUGNORMAL  = 1
    '                        MIODEBUGDETAIL  = 2
    '                        MIODEBUGALL  = 3
    '                        MIODEBUGSTARTTIMER  = -1 (MIODEBUGSTARTTIMER)
    '                        MIODEBUGSTOPLOCAL  = -2  (MIODEBUGGETTRACE)
    '                        MIODEBUGSTOPCLIENT  = -3 (stop client debugging)
    '                        MIODEBUGTRACKTASK  = -4  (stop client debugging)
    '                        MIODEBUGSTARTTRACE  = -5 (stop client debugging)
    '                        MIODEBUGSTOPTRACE  = -6  (stop client debugging)
    '                        MIODEBUGNETDUMP  = -7    (network status debug dump)
    '                        MIODEBUGGETLEVEL  = -8   (does nothing but return level)
    '                        MIODEBUGGETTRACE  = -9   (return the trace level)
    'Return Value:	The return value is the current debug level. 
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIODebug")> Public Shared Function MIODebug(ByVal Flag As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOSetDetectorInfo
    '
    'Action:	    Set the detector information for an open detector.
    'Comments:	    This function sets information associated with a given detector. The information consists of the instrument ID
    '               number and detector description (up to MIODETDESCMAX chars including NULL). Only one copy of this information is
    '               maintained for the entire network. 
    'Parameters:    hDet	        Supplied handle to an open detector (see MIOOpenDetector()).
    '	            lpszAuth	    Reserved for future expansion. Must be “”.
    '               lpszPass	    Supplied string that is saved and compared with the supplied password string for the specified detector to 
    '                               determine if that function will be allowed.
    '               lpszDetDesc	    Supplied detector description text. 
    '	            lpdwID	        Supplied instrument ID.
    'Return Value:	The return value is TRUE if successful. Otherwise, the value is FALSE and a specific error number may be retrieved by
    '               calling MIOGetLastError().
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOSetDetectorInfo")> Public Shared Function MIOSetDetectorInfo(ByVal hDet As Integer, ByVal lpszAuth As String, ByVal lpszPass As String, ByVal lpszDetDesc As String, ByVal DetId As Integer) As Integer
    End Function
    '----------------------------------------------------------
    'Function Name: MIOCompressConfigList
    '
    'Action:	    Compress the config list removing any empty items left by deletions or out-of-sequence insertions.
    'Parameters:    szListName     Name of Detector Pick list (up to 5 characters)
    'Return Value:	Returns MIO_SUCCESS if the list was created
    '----------------------------------------------------------
    <System.Runtime.InteropServices.DllImport("MCBCIO32.dll", EntryPoint:="MIOCompressConfigList")> Public Shared Function MIOCompressConfigList(ByVal lpszListName As String) As Integer
    End Function

#End Region

#Region "Shared Example Functions"
    '------------------------------------------------------
    ' This undocumented MIO function "MIOErrorString" is
    ' provided here to help interpret the MIO error codes.
    '------------------------------------------------------
    Shared Function MIOErrorString() As String
        '**************************************************
        ' This function gets the code associated with the
        '       most recent UMCBI call and returns a error
        '       string that describes the error
        '***************************************************
        Dim MioError As Integer, MacroError As Integer, MicroError As Integer
        Dim Mcbstring

        Mcbstring = "MCB Reported Error -- "

        '-----------------------
        ' Get the MIO error code
        '-----------------------
        MioError = MIOGetLastError(MacroError, MicroError)
        '---------------------------------------------------
        ' Load buffer with Error message based on codes
        ' returned from a call to a Unified MCB function
        '---------------------------------------------------
        Select Case MioError
            Case MIOENONE
                MIOErrorString = "MIO Error 0"
            Case MIOEINVALID
                MIOErrorString = "Invalid Detector Handle"
            Case MIOEMCB
                '------------------------
                ' Error returned from MCB
                '------------------------
                MIOErrorString = Mcbstring & " Macro: " & CStr(MacroError) & "  Micro: " & CStr(MicroError)
                Select Case (MacroError)
                    Case 129    ' Command Syntax
                        Select Case MicroError
                            Case 1
                                MIOErrorString = Mcbstring & "Invalid Verb"
                            Case 2
                                MIOErrorString = Mcbstring & "Invalid Noun"
                            Case 4
                                MIOErrorString = Mcbstring & "Invalid Modifier"
                            Case 8
                                MIOErrorString = Mcbstring & "Invalid Command Data"
                            Case 128 To 131
                                MIOErrorString = Mcbstring & "Invalid Data Value - " & CStr(MicroError - 127)
                            Case 132
                                MIOErrorString = Mcbstring & "Invalid Command"
                        End Select

                    Case 131    ' Execution Error
                        Select Case MicroError
                            Case 128 To 131
                                MIOErrorString = Mcbstring & "Invalid command parameter - " & CStr(MicroError - 127)
                            Case 132
                                MIOErrorString = Mcbstring & "Inumber of parameters"
                            Case 133
                                MIOErrorString = Mcbstring & "Invlaid data (besides command data)"
                            Case 134
                                MIOErrorString = Mcbstring & "Invlaid Segment number"
                            Case 135
                                MIOErrorString = Mcbstring & "Not Allowed During acquisition"
                        End Select

                End Select ' MacroError

            Case MIOEIO
                '-----------------------
                ' error in communication
                '-----------------------
                Select Case (MacroError)
                    Case MIOEMACCLOSED
                        MIOErrorString = "Detector Communcation Broken"
                    Case MIOEMACTIMEOUT
                        MIOErrorString = "Detector Communication Timeout"
                    Case MIOEMACCOMM
                        MIOErrorString = "Detector Communcation Error"
                    Case MIOEMACTOOMANY
                        MIOErrorString = "Too Many Open Detectors"
                    Case MIOEMACOTHER
                        MIOErrorString = "Disk, OS or Other Error"
                End Select
            Case MIOEMEM
                MIOErrorString = "Detector I/O Memory Allocation Error"
            Case MIOENOTAUTH
                MIOErrorString = "Authorization or Password Failure"
            Case MIOEBLOCKING
                MIOErrorString = "Detector I/O Already in Progress"
            Case MIOEINTR
                MIOErrorString = "Detector Call Interrupted"
            Case MIOENOCONTEXT
                MIOErrorString = "MCBCIO Call Before MIOStartup or after MIOCleanup"
        End Select
    End Function

    '----------------------------------------------------------
    'Function Name: MIOGetTypeEx
    '
    'Action:	    Retrieve the SHOW_VERSION string for the currently selected detector.
    'Comments:	    This function retrieves the 8-char hardware version string for the instrument associated with the currently selected detector. 
    '               The returned string is used to determine what hardware specific features are available for the detector. Note that an appropriate 
    '               string will be returned for all instruments, even ones without the SHOW_VERSION instrument command. Unpredictable results occur
    '               if this function is called with an invalid hDet.
    'Parameters:	hDet	    Supplied handle to an open detector (see MIOOpenDetector()).
    'Return Value:	C Language:	        The return value is a far pointer to the type string associated with hDet. The returned pointer is valid as 
    '                                   long as hDet is valid.
    '	            Visual Basic 6.0:	The return value is variable length string. 
    '	            Both:	            This function has no errors and does not affect the error status returned by MIOGetLastError().
    '----------------------------------------------------------
    Shared Function MIOGetType(hDet As Long) As String
        '-----------------------------6----------------
        ' Get the detector type in local string
        ' Trim trailing null are return result string
        '---------------------------------------------
        MIOGetType = "NULL-2"
        Dim Buffer As New StringBuilder(20)
        Dim l As Integer
        l = MIOGetTypeEx(hDet, Buffer, 20)
        If (l <> 0) Then
            l = InStr(1, Buffer.ToString(), Chr(0)) 'search for null terminator
            If (l > 0) Then
                MIOGetType = Mid$(Buffer.ToString(), 1, l - 1)
            Else
                MIOGetType = Buffer.ToString()
            End If
        End If
    End Function
#End Region
End Class
