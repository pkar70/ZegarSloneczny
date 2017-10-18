
Imports Windows.Data.Xml.Dom
Imports Windows.Storage
Imports Windows.UI.Notifications
Imports Windows.UI.StartScreen
''' <summary>
''' Provides application-specific behavior to supplement the default Application class.
''' </summary>
NotInheritable Class App
    Inherits Application

    ''' <summary>
    ''' Invoked when the application is launched normally by the end user.  Other entry points
    ''' will be used when the application is launched to open a specific file, to display
    ''' search results, and so forth.
    ''' </summary>
    ''' <param name="e">Details about the launch request and process.</param>
    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
        Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)

        ' Do not repeat app initialization when the Window already has content,
        ' just ensure that the window is active

        If rootFrame Is Nothing Then
            ' Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = New Frame()

            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed

            If e.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' TODO: Load state from previously suspended application
            End If
            ' Place the frame in the current Window
            Window.Current.Content = rootFrame
        End If

        If e.PrelaunchActivated = False Then
            If rootFrame.Content Is Nothing Then
                ' When the navigation stack isn't restored navigate to the first page,
                ' configuring the new page by passing required information as a navigation
                ' parameter
                rootFrame.Navigate(GetType(MainPage), e.Arguments)
            End If

            ' Ensure the current window is active
            If e.TileActivatedInfo IsNot Nothing Then
                ' przelaczenie na nastepny to exit; nie ma zegara do pokazania w maintile, continue
                If UstawTarczeCommon() Then Me.Exit()
            End If
            Window.Current.Activate()
        End If
    End Sub

    ''' <summary>
    ''' Invoked when Navigation to a certain page fails
    ''' </summary>
    ''' <param name="sender">The Frame which failed navigation</param>
    ''' <param name="e">Details about the navigation failure</param>
    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub

    ''' <summary>
    ''' Invoked when application execution is being suspended.  Application state is saved
    ''' without knowing whether the application will be terminated or resumed with the contents
    ''' of memory still intact.
    ''' </summary>
    ''' <param name="sender">The source of the suspend request.</param>
    ''' <param name="e">Details about the suspend request.</param>
    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        ' TODO: Save application state and stop any background activity
        deferral.Complete()
    End Sub

    Public Shared Function GetSettingsInt(sName As String, Optional iDefault As Integer = 0) As Integer
        Dim sTmp As Integer

        sTmp = iDefault

        If ApplicationData.Current.RoamingSettings.Values.ContainsKey(sName) Then
            sTmp = CInt(ApplicationData.Current.RoamingSettings.Values(sName).ToString)
        End If
        If ApplicationData.Current.LocalSettings.Values.ContainsKey(sName) Then
            sTmp = CInt(ApplicationData.Current.LocalSettings.Values(sName).ToString)
        End If

        Return sTmp

    End Function

    Public Shared Sub SetSettingsInt(sName As String, sValue As Integer, Optional bRoam As Boolean = False)
        If bRoam Then ApplicationData.Current.RoamingSettings.Values(sName) = sValue.ToString
        ApplicationData.Current.LocalSettings.Values(sName) = sValue.ToString
    End Sub
    Public Shared Sub SetSettingsDouble(sName As String, sValue As Double, Optional bRoam As Boolean = False)
        If bRoam Then ApplicationData.Current.RoamingSettings.Values(sName) = sValue.ToString
        ApplicationData.Current.LocalSettings.Values(sName) = sValue.ToString
    End Sub

    Public Shared Sub SetSettingsString(sName As String, sValue As String, Optional bRoam As Boolean = False)
        If bRoam Then ApplicationData.Current.RoamingSettings.Values(sName) = sValue
        ApplicationData.Current.LocalSettings.Values(sName) = sValue
    End Sub
    Public Shared Function GetSettingsString(sName As String, Optional sDefault As String = "") As String
        Dim sTmp As String

        sTmp = sDefault

        If ApplicationData.Current.RoamingSettings.Values.ContainsKey(sName) Then
            sTmp = ApplicationData.Current.RoamingSettings.Values(sName).ToString
        End If
        If ApplicationData.Current.LocalSettings.Values.ContainsKey(sName) Then
            sTmp = ApplicationData.Current.LocalSettings.Values(sName).ToString
        End If

        Return sTmp

    End Function

    Public Shared Function GetSettingsDouble(sName As String, Optional sDefault As Double = 0) As Double
        Dim sTmp As Double

        sTmp = sDefault

        If ApplicationData.Current.RoamingSettings.Values.ContainsKey(sName) Then
            sTmp = CDbl(ApplicationData.Current.RoamingSettings.Values(sName).ToString)
        End If
        If ApplicationData.Current.LocalSettings.Values.ContainsKey(sName) Then
            sTmp = CDbl(ApplicationData.Current.LocalSettings.Values(sName).ToString)
        End If

        Return sTmp

    End Function
    Public Shared Function GetSettingsBool(sName As String, Optional iDefault As Boolean = False) As Boolean
        Dim sTmp As Boolean

        sTmp = iDefault

        If ApplicationData.Current.RoamingSettings.Values.ContainsKey(sName) Then
            sTmp = CBool(ApplicationData.Current.RoamingSettings.Values(sName).ToString)
        End If
        If ApplicationData.Current.LocalSettings.Values.ContainsKey(sName) Then
            sTmp = CBool(ApplicationData.Current.LocalSettings.Values(sName).ToString)
        End If

        Return sTmp

    End Function
    Public Shared Sub SetSettingsBool(sName As String, sValue As Boolean, Optional bRoam As Boolean = False)
        If bRoam Then ApplicationData.Current.RoamingSettings.Values(sName) = sValue.ToString
        ApplicationData.Current.LocalSettings.Values(sName) = sValue.ToString
    End Sub

    Public Shared Function GetUpdaterClearQueue(sName As String, bCommon As Boolean, sXml As String) As TileUpdater
        Dim oXml As New XmlDocument
        oXml.LoadXml(sXml)
        Dim oTile = New Windows.UI.Notifications.TileNotification(oXml)

        Dim oTUPS As TileUpdater
        If sName = "" Or bCommon Then
            oTUPS = TileUpdateManager.CreateTileUpdaterForApplication
        Else
            oTUPS = TileUpdateManager.CreateTileUpdaterForSecondaryTile(sName)
        End If

        oTUPS.Clear()
        For i = oTUPS.GetScheduledTileNotifications.Count - 1 To 0 Step -1
            oTUPS.RemoveFromSchedule(oTUPS.GetScheduledTileNotifications(i))
        Next

        oTUPS.Update(oTile)

        Return oTUPS
    End Function

    Private Shared Function SecTileUpdateDigTarcza(iHr As Integer, iMin As Integer, b24 As Boolean) As String
        Dim sTmp As String
        If Not b24 Then If iHr > 12 Then iHr = iHr - 12
        Dim sHr = iHr.ToString & ":" & iMin.ToString("d2")

        sTmp = "<tile><visual>"

        sTmp = sTmp & "<binding template ='TileSmall' branding='none' hint-textStacking='center'>"
        sTmp = sTmp & "<text hint-style='title' hint-align='center'>" & sHr & "</text>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template ='TileMedium' branding='none' hint-textStacking='center'>"
        sTmp = sTmp & "<text hint-style='headerSubtle' hint-align='center'>" & sHr & "</text>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template ='TileWide' branding='none' hint-textStacking='center'>"
        sTmp = sTmp & "<text hint-style='headerSubtle' hint-align='center'>" & sHr & "</text>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template ='TileLarge' branding='none' hint-textStacking='center'>"
        sTmp = sTmp & "<text hint-style='headerSubtle' hint-align='center'>" & sHr & "</text>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "</visual></tile>"

        SecTileUpdateDigTarcza = sTmp
    End Function

    Public Shared Sub SecTileUpdateDig(bCommon As Boolean)
        Dim sXml As String
        Dim oDate As Date = Date.Now
        oDate.AddSeconds(-oDate.Second) ' wycofaj na poczatek minuty

        sXml = SecTileUpdateDigTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinDig24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))

        Dim oTUPS As TileUpdater = GetUpdaterClearQueue("SunDialDig", bCommon, sXml)
        Dim oXml As New XmlDocument

        For i = 1 To 90    ' 1.5h, a timer jest co godzine
            oDate = oDate.AddMinutes(1)
            sXml = SecTileUpdateDigTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinDig24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
            oXml.LoadXml(sXml)
            Dim oSchST = New ScheduledTileNotification(oXml, oDate)
            If i = 90 Then oSchST.ExpirationTime = oDate.AddMinutes(2)  ' wygas, nawet jak nie bedzie aktualizacji
            oTUPS.AddToSchedule(oSchST)
        Next

    End Sub
    Public Shared Sub SecTileUpdateSun(bCommon As Boolean)
        Dim sXml As String
        Dim oDate As Date = Date.Now
        oDate.AddSeconds(-oDate.Second) ' wycofaj na poczatek minuty

        'sXml = SecTileUpdateAnaTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinAna24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))

        'Dim oTUPS As TileUpdater = GetUpdaterClearQueue("SunDialDig", bCommon, sXml)
        'Dim oXml As New XmlDocument

        'For i = 1 To 90    ' 1.5h, a timer jest co godzine
        '    oDate = oDate.AddMinutes(1)
        '    sXml = SecTileUpdateDigTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinDig24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
        '    oXml.LoadXml(sXml)
        '    Dim oSchST = New ScheduledTileNotification(oXml, oDate)
        '    If i = 90 Then oSchST.ExpirationTime = oDate.AddMinutes(2)  ' wygas, nawet jak nie bedzie aktualizacji
        '    oTUPS.AddToSchedule(oSchST)
        'Next

    End Sub

    Private Shared Async Function EnsureAnaTarczaExist(iHr As Integer, iMin As Integer) As Task(Of String)
        Dim sPic = "pic\a\" & iHr.ToString("d2") & iMin.ToString("d2") & ".png"

        ' check if file exist
        Dim oSF = Windows.ApplicationModel.Package.Current.InstalledLocation
        If Await oSF.TryGetItemAsync(sPic) IsNot Nothing Then Return sPic

        ' jesli nie, zrob tarcze
        Return sPic
    End Function
    Private Shared Async Function SecTileUpdateAnaTarcza(iHr As Integer, iMin As Integer) As Task(Of String)
        Dim sTmp, sPic As String

        sPic = Await EnsureAnaTarczaExist(iHr, iMin)
        sTmp = "<tile><visual>"

        sTmp = sTmp & "<binding template ='TileSmall' branding='none'>"
        sTmp = sTmp & "<image hint-align='center' src='" & sPic & "'/>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template ='TileMedium' branding='none'>"
        sTmp = sTmp & "<image hint-align='center' src='" & sPic & "'/>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template ='TileWide' branding='none'>"
        sTmp = sTmp & "<image hint-align='center' src='" & sPic & "'/>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template ='TileLarge' branding='none'>"
        sTmp = sTmp & "<image hint-align='center' src='" & sPic & "'/>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "</visual></tile>"

        Return sTmp
    End Function

    Public Shared Async Sub SecTileUpdateAna(bCommon As Boolean)
        Dim sXml As String
        Dim oDate As Date = Date.Now
        oDate.AddSeconds(-oDate.Second) ' wycofaj na poczatek minuty

        sXml = Await SecTileUpdateAnaTarcza(oDate.Hour, oDate.Minute)

        Dim oTUPS As TileUpdater = GetUpdaterClearQueue("SunDialAna", bCommon, sXml)
        Dim oXml As New XmlDocument

        For i = 1 To 90    ' 1.5h, a timer jest co godzine
            oDate = oDate.AddMinutes(1)
            sXml = Await SecTileUpdateAnaTarcza(oDate.Hour, oDate.Minute)
            oXml.LoadXml(sXml)
            Dim oSchST = New ScheduledTileNotification(oXml, oDate)
            If i = 90 Then oSchST.ExpirationTime = oDate.AddMinutes(2)  ' wygas, nawet jak nie bedzie aktualizacji
            oTUPS.AddToSchedule(oSchST)
        Next

    End Sub
    Private Shared Function SecTileUpdateBinTarcza(iHr As Integer, iMin As Integer, b24 As Boolean) As String
        Dim sTmpH, sTmpM, sPic, sTmp As String
        If Not b24 Then If iHr > 12 Then iHr = iHr - 12

        Dim sHr = iHr.ToString("d2")

        sTmpH = "<subgroup hint-weight='40'><image src='pic\b\"
        If b24 Then
            sTmpH = sTmpH & "2"
        Else
            sTmpH = sTmpH & "1"
        End If
        sTmpH = sTmpH & "b" & sHr.Substring(0, 1) & "b.png'  hint-removeMargin='true'/></subgroup>"
        sTmpH = sTmpH & "<subgroup hint-weight='40'><image src='pic\b\4b" & sHr.Substring(1, 1) & "b.png' hint-removeMargin='true'/></subgroup>"

        sHr = iMin.ToString("d2")
        sTmpM = "<subgroup hint-weight='40'><image src='pic\b\3b" & sHr.Substring(0, 1) & "b.png' hint-removeMargin='true'/></subgroup>"
        sTmpM = sTmpM & "<subgroup hint-weight='40'><image src='pic\b\4b" & sHr.Substring(1, 1) & "b.png' hint-removeMargin='true'/></subgroup>"

        sPic = "<group>" & sTmpH & sTmpM & "</group>"   ' wersja Small nie ma przerw pomiedzy

        sTmp = "<tile><visual>"
        sTmp = sTmp & "<binding template='TileSmall' branding='none' hint-textStacking='center' >"
        sTmp = sTmp & sPic
        sTmp = sTmp & "</binding>"

        sPic = "<group>" & "<subgroup hint-weight='2'><image src='pic\b\Spacer2.png' /></subgroup>" &
            sTmpH & "<subgroup hint-weight='2'><image src='pic\b\Spacer2.png' /></subgroup>" &
            sTmpM & "<subgroup hint-weight='2'><image src='pic\b\Spacer2.png' /></subgroup></group>"

        sTmp = sTmp & "<binding template='TileMedium' branding='none' hint-textStacking='center'>"
        sTmp = sTmp & sPic
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template='TileWide' branding='none' hint-textStacking='center'>"
        sTmp = sTmp & sPic
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template='TileLarge' branding='none' hint-textStacking='center'>"
        sTmp = sTmp & sPic
        sTmp = sTmp & "</binding>"


        sTmp = sTmp & "</visual></tile>"

        SecTileUpdateBinTarcza = sTmp
    End Function
    Public Shared Sub SecTileUpdateBin(bCommon As Boolean)
        Dim sXml As String
        Dim oDate As Date = Date.Now
        oDate.AddSeconds(-oDate.Second) ' wycofaj na poczatek minuty

        sXml = SecTileUpdateBinTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinBin24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
        Dim oTUPS As TileUpdater = GetUpdaterClearQueue("SunDialBin", bCommon, sXml)
        Dim oXml As New XmlDocument

        For i = 1 To 90    ' 1.5h, a timer jest co godzine
            oDate = oDate.AddMinutes(1)
            sXml = SecTileUpdateBinTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinBin24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
            oXml.LoadXml(sXml)
            Dim oSchST = New ScheduledTileNotification(oXml, oDate)
            If i = 90 Then oSchST.ExpirationTime = oDate.AddMinutes(2)  ' wygas, nawet jak nie bedzie aktualizacji
            oTUPS.AddToSchedule(oSchST)
        Next

    End Sub
    Private Shared Function SecTileUpdateSsgTarcza(iHr As Integer, iMin As Integer, b24 As Boolean) As String
        Dim sTmpH, sTmpM, sPic, sTmp As String
        If Not b24 Then If iHr > 12 Then iHr = iHr - 12

        Dim sHr = iHr.ToString("d2")
        If sHr.Substring(0, 1) = "0" Then
            sTmpH = "<subgroup hint-weight='40'><image src='pic\7\Null.png' /></subgroup>" ' pusty, a nie zero!
        Else
            sTmpH = "<subgroup hint-weight='40'><image src='pic\7\" & sHr.Substring(0, 1) & ".png' /></subgroup>"
        End If
        sTmpH = sTmpH & "<subgroup hint-weight='40'><image src='pic\7\" & sHr.Substring(1, 1) & ".png' /></subgroup>"

        sHr = iMin.ToString("d2")
        sTmpM = "<subgroup hint-weight='40'><image src='pic\7\" & sHr.Substring(0, 1) & ".png' /></subgroup>"
        sTmpM = sTmpM & "<subgroup hint-weight='40'><image src='pic\7\" & sHr.Substring(1, 1) & ".png' /></subgroup>"

        sPic = "<group>" & sTmpH & sTmpM & "</group>"   ' wersja Small nie ma przerw pomiedzy

        sTmp = "<tile><visual>"
        sTmp = sTmp & "<binding template='TileSmall' branding='none' hint-textStacking='center' >"
        sTmp = sTmp & sPic
        sTmp = sTmp & "</binding>"

        sPic = "<group>" & "<subgroup hint-weight='2'><image src='pic\b\Spacer2.png' /></subgroup>" &
            sTmpH & "<subgroup hint-weight='2'><image src='pic\b\Spacer2.png' /></subgroup>" &
            sTmpM & "<subgroup hint-weight='2'><image src='pic\b\Spacer2.png' /></subgroup></group>"

        sTmp = sTmp & "<binding template='TileMedium' branding='none' hint-textStacking='center'>"
        sTmp = sTmp & sPic
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template='TileWide' branding='none' hint-textStacking='center'>"
        sTmp = sTmp & sPic
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template='TileLarge' branding='none' hint-textStacking='center'>"
        sTmp = sTmp & sPic
        sTmp = sTmp & "</binding>"


        sTmp = sTmp & "</visual></tile>"

        SecTileUpdateSsgTarcza = sTmp
    End Function
    Public Shared Sub SecTileUpdateSsg(bCommon As Boolean)

        Dim sXml As String
        Dim oDate As Date = Date.Now
        oDate.AddSeconds(-oDate.Second) ' wycofaj na poczatek minuty

        sXml = SecTileUpdateSsgTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinSsg24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
        Dim oTUPS As TileUpdater = GetUpdaterClearQueue("SunDialSsg", bCommon, sXml)
        Dim oXml As New XmlDocument

        For i = 1 To 90    ' 1.5h, a timer jest co godzine
            oDate = oDate.AddMinutes(1)
            sXml = SecTileUpdateSsgTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinSsg24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
            oXml.LoadXml(sXml)
            Dim oSchST = New ScheduledTileNotification(oXml, oDate)
            If i = 90 Then oSchST.ExpirationTime = oDate.AddMinutes(2)  ' wygas, nawet jak nie bedzie aktualizacji
            oTUPS.AddToSchedule(oSchST)
        Next

    End Sub
    Public Shared Sub SecTileUpdate()

        If SecondaryTile.Exists("SunDialSun") Then SecTileUpdateSun(False)
        If SecondaryTile.Exists("SunDialAna") Then SecTileUpdateAna(False)
        If SecondaryTile.Exists("SunDialDig") Then SecTileUpdateDig(False)

        If SecondaryTile.Exists("SunDialSSg") Then SecTileUpdateSsg(False)
        If SecondaryTile.Exists("SunDialBin") Then SecTileUpdateBin(False)

        'if analogicznie do glownej tile
    End Sub

    Public Shared Sub PriTileUpdate()
        ' rysowanie main Tile - z timer

        ' if not exist tile exit sub
        ' https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/tiles-and-notifications-primary-tile-apis
        'ale to od CreatorsUpdate, Aska tego nie ma.
        'Dim entry = Await Package.Current.GetAppListEntriesAsync()
        'Dim isPinned = Await StartScreenManager.GetDefault().ContainsAppListEntryAsync(entry)
        'If Not isPinned Then Exit Sub

        ' rysujemy defaultowy zegarek, od teraz przelacznik zaczyna od zera
        If App.GetSettingsBool("uiPinSunDef") Then
            SecTileUpdateSun(True)
            SetSettingsInt("uiCurrentClock", 1)
        End If
        If App.GetSettingsBool("uiPinAnaDef") Then
            SecTileUpdateAna(True)
            SetSettingsInt("uiCurrentClock", 2)
        End If
        If App.GetSettingsBool("uiPinDigDef") Then
            SecTileUpdateDig(True)
            SetSettingsInt("uiCurrentClock", 3)
        End If
        If App.GetSettingsBool("uiPinSSgDef") Then
            SecTileUpdateSsg(True)
            SetSettingsInt("uiCurrentClock", 4)
        End If
        If App.GetSettingsBool("uiPinBinDef") Then
            SecTileUpdateBin(True)
            SetSettingsInt("uiCurrentClock", 5)
        End If

    End Sub
    Private Shared Sub DelNearestUpdate(iSec As Integer)
        Dim oTUPS = TileUpdateManager.CreateTileUpdaterForApplication()
        Dim oDate As Date = Date.Now
        oDate.AddSeconds(iSec)

        For i = oTUPS.GetScheduledTileNotifications.Count - 1 To 0 Step -1
            If oTUPS.GetScheduledTileNotifications.Item(i).DeliveryTime < oDate Then
                oTUPS.RemoveFromSchedule(oTUPS.GetScheduledTileNotifications(i))
            End If
        Next

    End Sub
    Private Shared Function GetNextClockId(iCurrentClock As Integer) As Integer

        ' kontrola ostatniego czasu zmiany
        Dim iSecNow, iSecOld As Integer
        iSecOld = GetSettingsInt("LastChangeTile")
        iSecNow = CInt(Date.Now.ToString("HHmmss"))
        '  jesli dawno, to reset - idziemy od nowa (ale HMS dzisiaj moze byc < HMS wczoraj)
        If Math.Abs(iSecNow - iSecOld) > 10 Then iCurrentClock = 0
        ' DZIWNOSC: to ponizsze na sledzeniu przeskakuje do End Function
        ' ale pulapka linijka nizej - wskakuje. Zakaz sledzenia? :)
        SetSettingsInt("LastChangeTile", iSecNow)

        iCurrentClock = iCurrentClock + 1
        If iCurrentClock > 5 Then iCurrentClock = 1

        If iCurrentClock < 2 Then
            If App.GetSettingsBool("uiPinSunOn") And Not App.GetSettingsBool("uiPinSunDef") Then Return 1
        End If

        If iCurrentClock < 3 Then
            If App.GetSettingsBool("uiPinAnaOn") And Not App.GetSettingsBool("uiPinAnaDef") Then Return 2
        End If

        If iCurrentClock < 4 Then
            If App.GetSettingsBool("uiPinDigOn") And Not App.GetSettingsBool("uiPinDigDef") Then Return 3
        End If

        If iCurrentClock < 5 Then
            If App.GetSettingsBool("uiPinSSgOn") And Not App.GetSettingsBool("uiPinSSgDef") Then Return 4
        End If

        If iCurrentClock < 6 Then
            If App.GetSettingsBool("uiPinBinOn") And Not App.GetSettingsBool("uiPinBinDef") Then Return 5
        End If

        Return 0    ' błędne ustawienia najwyrazniej, ale po prostu nic nie daj

    End Function

    Private Shared Function UstawTarczeCommon() As Boolean
        ' obsługa kliknięcia na main Tile

        Dim iClock As Integer = GetSettingsInt("uiCurrentClock")
        iClock = GetNextClockId(iClock)
        If iClock = 0 Then Return False
        SetSettingsInt("uiCurrentClock", iClock)

        Dim sXml As String
        Dim oDate As Date = Date.Now

        Select Case iClock
            Case 1  ' sundial
                Return False    ' tego jeszcze nie umiemy
            Case 2  ' analog
                Return False   ' tego jeszcze nie umiemy
            Case 3  ' digital
                sXml = SecTileUpdateDigTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinDig24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
            Case 4  ' 7seg
                sXml = SecTileUpdateSsgTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinSsg24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
            Case 5  ' bin
                sXml = SecTileUpdateBinTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinBin24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
            Case Else
                Return False     ' dziwacznosc, bo tak byc nie powinno
        End Select

        Dim oXml As New XmlDocument
        oXml.LoadXml(sXml)

        Dim oTile = New Windows.UI.Notifications.TileNotification(oXml)
        Dim oTUPS = TileUpdateManager.CreateTileUpdaterForApplication

        DelNearestUpdate(30)    ' usun aktualizacje tile jesli jest < 30 sekund do niej

        oTUPS.Update(oTile)

        Return True
    End Function

    Protected Overrides Sub OnBackgroundActivated(args As BackgroundActivatedEventArgs)
        SecTileUpdate()
        PriTileUpdate()
    End Sub
End Class
