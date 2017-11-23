
Imports Microsoft.Graphics.Canvas
Imports Windows.Data.Xml.Dom
Imports Windows.Foundation.Metadata
Imports Windows.Storage
Imports Windows.UI
Imports Windows.UI.Notifications
Imports Windows.UI.Popups
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
            'If e.TileActivatedInfo IsNot Nothing Then
            '    ' przelaczenie na nastepny to exit; nie ma zegara do pokazania w maintile, continue
            '    If UstawTarczeCommon() Then Me.Exit()
            'End If
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
        Dim oTile As TileNotification

        If sXml IsNot Nothing Then
            Dim oXml As New XmlDocument
            oXml.LoadXml(sXml)
            oTile = New Windows.UI.Notifications.TileNotification(oXml)
        End If

        Dim oTUPS As TileUpdater
        If sName = "" Or bCommon Then
            oTUPS = TileUpdateManager.CreateTileUpdaterForApplication
        Else
            oTUPS = TileUpdateManager.CreateTileUpdaterForSecondaryTile(sName)
        End If

        oTUPS.Clear()
        For i = oTUPS.GetScheduledTileNotifications.Count - 1 To 0 Step -1
            Try
                oTUPS.RemoveFromSchedule(oTUPS.GetScheduledTileNotifications(i))
            Catch ex As Exception
                ' healt stacktrace:
                ' ZegarSloneczny::App.GetUpdaterClearQueue
                ' System::Collections::Generic::IReadOnlyList_A__w_UI_Notifications_ScheduledTileNotification_V___Impl::Dispatcher.global::System.Collections.Generic.IReadOnlyList_Windows.UI.Notifications.ScheduledTileNotification_.get_Item
                ' System::Runtime::InteropServices::WindowsRuntime::IVectorViewSharedReferenceTypesDynamicAdapter$1_System::__Canon_.get_Item$catch$0
                ' wiec moze próba usunięcia czegoś co wlasnie weszlo jako aktualne i nie ma w kolejce
            End Try
        Next

        If sXml IsNot Nothing Then
            oTUPS.Update(oTile)
        End If
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
        oDate = oDate.AddSeconds(-oDate.Second) ' wycofaj na poczatek minuty

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
        Dim WschodZachod As WschodZachodHelp

        EnsureSunTarczaExist(False)

        WschodZachod = New WschodZachodHelp(DateTime.UtcNow,
             App.GetSettingsDouble("latitude"), App.GetSettingsDouble("longitude"), App.GetSettingsInt("dusk"))

        Dim dDzien, dNoc, dSun As Double

        dDzien = WschodZachod.GetZachod() - WschodZachod.GetWschod()
        dNoc = 24 - dDzien

        Dim d5SecD, d5SecN As Double
        d5SecD = dDzien / 12      ' dług dnia słon w h zwyklych / 12 godzin = dług godziny słon (w h zwyklych)
        d5SecD = 5 * d5SecD / 60 ' = dług. 5 minut słon. w godzinach zegarowych
        d5SecD = d5SecD * 60    ' = dług. 5 minut słon. w minutach zegarowych
        d5SecD = d5SecD * 60    ' = dług. 5 minut słon. w sekundach zegarowych

        d5SecN = dNoc / 12 * 5 * 60 ' to samo dla minut nocnych

        Dim dSecStep, dSecStep1, dHr As Double

        dHr = DateTime.UtcNow.Hour + DateTime.UtcNow.Minute / 60     ' dzisiaj czesc doby

        If dHr > WschodZachod.GetZachod() Or dHr < WschodZachod.GetWschod() Then
            ' godziny nocne

            If dHr > WschodZachod.GetZachod() Then
                dSun = 12 * (dHr - WschodZachod.GetZachod()) / dNoc
            Else ' przed switem, hr=0..wschod
                dSun = 12 - 12 * (WschodZachod.GetWschod() - dHr) / (dDzien)  ' PKAR: MINUS
            End If

            dSecStep = d5SecN
            dSecStep1 = d5SecD
        Else
            dSun = 12 * (dHr - WschodZachod.GetWschod()) / dDzien
            dSecStep = d5SecD
            dSecStep1 = d5SecN
        End If

        ' dHr = godzina zegarowa (np. 1.8567)
        ' dSun = godzina słoneczna 
        ' dSecStep, dSecStep1 = sekund zegarowych na 5 minut sloneczne teraz i po wschod/zachod

        Dim iSunHr As Integer = 0
        While iSunHr < dSun     ' wlasny Floor, bo funkcja Fix nie istnieje, a \1 też zaokrągla (w góre czasem!)
            iSunHr += 1
        End While
        If iSunHr > 0 Then iSunHr -= 1
        Dim iSunMin As Integer = 60 * (dSun - iSunHr)
        iSunMin = iSunMin - (iSunMin Mod 5)         ' ucinamy na poczatek 5 minutowego fragmentu

        Dim oTUPS As TileUpdater = GetUpdaterClearQueue("SunDialSun", bCommon, Nothing)
        Dim oXml As New XmlDocument
        Dim sXml As String

        ' krok petli, 5 minut slonecznych w przeliczeniu na sekundy ścienne

        Dim oDate As Date = Date.Now.AddMinutes(1)

        For i = 0 To 90 Step 5  ' na 90 minut zrob ikonki
            ' dHr = aktualny czas scienny
            sXml = SecTileUpdateSunTarcza(iSunHr, iSunMin)

            oXml.LoadXml(sXml)

            Dim oSchST = New ScheduledTileNotification(oXml, oDate)

            If i = 90 Then
                oSchST.ExpirationTime = oDate.AddMinutes(10)  ' wygas, nawet jak nie bedzie aktualizacji
            End If
            oTUPS.AddToSchedule(oSchST)

            oDate = oDate.AddSeconds(dSecStep)
            ' kolejne 5 minut
            iSunMin = iSunMin + 5
            If iSunMin > 59 Then
                iSunMin = 0
                iSunHr = iSunHr + 1
                If iSunHr > 11 Then ' przejscie przez wschód/zachód
                    iSunHr = 0
                    dSecStep = dSecStep1    ' zmiana dlugosci 5 minut (noc/dzien)
                End If
            End If

        Next


    End Sub

    Public Shared Async Sub EnsureAnaTarczaExist(bMsg As Boolean)

        ' check if file exist
        Dim oFolderP = Await ApplicationData.Current.LocalFolder.CreateFolderAsync("pic", CreationCollisionOption.OpenIfExists)
        Dim oFolder = Await oFolderP.CreateFolderAsync("a", CreationCollisionOption.OpenIfExists)
        If Await oFolder.TryGetItemAsync("null.png") IsNot Nothing Then Exit Sub
        If Await oFolder.TryGetItemAsync("0000.png") IsNot Nothing Then Exit Sub
        ' 1159 - wtedy wylatuje na errorze, bo w dwu watkach zaczyna tworzyc pliki

        ' sample
        ' https://social.msdn.microsoft.com/Forums/windowsapps/en-US/db4710c7-fe8e-4019-97ea-75d5300b2a7d/uwp-draw-line-shape-image-directly-on-writeablebitmap-?forum=wpdevelop
        ' dokumentacja
        ' http://microsoft.github.io/Win2D/html/Introduction.htm

        Dim oDevice = CanvasDevice.GetSharedDevice()


        ' 1. rysowanie tarczy (wspólne)
        ' 200x200 pokazuje za mało (jest crop), robimy 150 - ale z bckgrnd moze jest inaczej
        Dim oTarczaTlo = New CanvasRenderTarget(oDevice, 200, 200, 96)

        Dim ds = oTarczaTlo.CreateDrawingSession

        ds.Clear(Colors.Transparent)

        ' 0,0 jest na gorze

        Dim iGrub, iLen As Integer
        Dim ny, nx, nx1, ny1 As Integer

        Dim dLuMinutowy = 6 * Math.PI / 180
        Dim dLukGodzinny = 30 * Math.PI / 180

        For i = 1 To 60
            iGrub = 2
            iLen = 10

            If i Mod 5 = 0 Then
                iGrub = 6
                iLen = 20
            End If

            If i Mod 15 = 0 Then
                iGrub = 8
            End If

            ' tylko raz liczymy sinusa i cosinusa
            ny = Math.Sin(i * dLuMinutowy) * (95 - iLen)
            nx = Math.Cos(i * dLuMinutowy) * (95 - iLen)
            ny1 = Math.Sin(i * dLuMinutowy) * 95
            nx1 = Math.Cos(i * dLuMinutowy) * 95



            ds.DrawLine(nx + 100, ny + 100, nx1 + 100, ny1 + 100, Colors.White, iGrub)

        Next

        ds.Dispose()

        Dim oFilePicN = Await oFolder.CreateFileAsync("null.png", CreationCollisionOption.ReplaceExisting)
        Await oTarczaTlo.SaveAsync(oFilePicN.Path)

        ' 2. dodawanie wskazówek
        For iHr = 0 To 11
            For iMin = 0 To 59
                Dim oTarcza = New CanvasRenderTarget(oDevice, 200, 200, 96)
                ds = oTarcza.CreateDrawingSession
                ds.Clear(Colors.Transparent)
                ds.DrawImage(oTarczaTlo)

                ' wskazowka minutowa
                Dim iNo As Double
                iNo = iMin - 15
                iGrub = 6
                ny1 = Math.Sin(iNo * dLuMinutowy) * 82
                nx1 = Math.Cos(iNo * dLuMinutowy) * 82
                ds.DrawLine(100, 100, nx1 + 100, ny1 + 100, Colors.White, iGrub)
                ny1 = -0.24 * ny1
                nx1 = -0.24 * nx1
                ds.DrawLine(100, 100, nx1 + 100, ny1 + 100, Colors.White, iGrub)

                ' wskazowka godzinna
                iNo = ((iNo + 15) / 60) + iHr - 3
                If iNo > 12 Then iNo = iNo - 12
                iGrub = 7
                ny1 = Math.Sin(iNo * dLukGodzinny) * 65
                nx1 = Math.Cos(iNo * dLukGodzinny) * 65
                ds.DrawLine(100, 100, nx1 + 100, ny1 + 100, Colors.White, iGrub)
                ny1 = -0.31 * ny1
                nx1 = -0.31 * nx1
                ds.DrawLine(100, 100, nx1 + 100, ny1 + 100, Colors.White, iGrub)

                ds.Dispose()

                Dim oFilePic = Await oFolder.CreateFileAsync(
                        iHr.ToString("d2") & iMin.ToString("d2") & ".png", CreationCollisionOption.ReplaceExisting)
                Await oTarcza.SaveAsync(oFilePic.Path)

                oTarcza.Dispose()

            Next
        Next


    End Sub

    Public Shared Async Sub EnsureSunTarczaExist(bMsg As Boolean)

        ' check if file exist
        Dim oFolderP = Await ApplicationData.Current.LocalFolder.CreateFolderAsync("pic", CreationCollisionOption.OpenIfExists)
        Dim oFolder = Await oFolderP.CreateFolderAsync("s", CreationCollisionOption.OpenIfExists)
        If Await oFolder.TryGetItemAsync("null.png") IsNot Nothing Then Exit Sub
        If Await oFolder.TryGetItemAsync("0000.png") IsNot Nothing Then Exit Sub

        'If GetSettingsBool("inCreatingSunTarcza") Then Exit Sub
        'SetSettingsBool("inCreatingSunTarcza", True)

        Dim m_SetDiameter = 0.65    ' ile wysokosci idzie na cień
        Dim m_SetPustyLuk As Double = 45    ' 30 stopni pustego kąta
        Dim m_MoveBottom = 10

        Dim oDevice = CanvasDevice.GetSharedDevice()
        ' 1. rysowanie tarczy (część wspólna)
        Dim oTarczaTlo = New CanvasRenderTarget(oDevice, 200, 200, 96)

        Dim ds = oTarczaTlo.CreateDrawingSession
        ds.Clear(Colors.Transparent)

        ' gnomon
        ds.FillCircle(100, m_MoveBottom + 10, 4, Colors.LightGray)

        Dim m_PustyLuk As Double = Math.PI / (180 / m_SetPustyLuk)
        Dim m_LukGodziny As Double = Math.PI / (180 / ((180 - 2 * m_SetPustyLuk) / 12))
        Dim m_Diameter = 200 * m_SetDiameter

        Dim nx, ny, nx1, ny1, iGrub As Integer
        For i As Integer = 0 To 12

            ' rysuj kreske godzinową
            ny = Math.Sin(m_PustyLuk + i * m_LukGodziny) * m_Diameter
            nx = Math.Cos(m_PustyLuk + i * m_LukGodziny) * m_Diameter
            ny1 = ny * 1.08 ' * (m_Diameter + 10), normalnie m_Diameter = 200*0.65 = 130
            nx1 = nx * 1.08
            iGrub = 2
            If i Mod 3 = 0 Then iGrub = 4

            ds.DrawLine(100 + nx, m_MoveBottom + 10 + ny, 100 + nx1, m_MoveBottom + 10 + ny1, Colors.White, iGrub)

            ' rysuj kreske polgodzinna
            If i < 12 Then
                ny = Math.Sin(m_LukGodziny / 2 + m_PustyLuk + i * m_LukGodziny) * m_Diameter
                nx = Math.Cos(m_LukGodziny / 2 + m_PustyLuk + i * m_LukGodziny) * m_Diameter
                ny1 = ny * 1.04 ' (m_Diameter + 2)
                nx1 = nx * 1.04

                ds.DrawLine(100 + nx, m_MoveBottom + 10 + ny, 100 + nx1, m_MoveBottom + 10 + ny1, Colors.White, 2)
            End If
        Next

        ds.Dispose()

        Dim oFilePicN = Await oFolder.CreateFileAsync("null.png", CreationCollisionOption.ReplaceExisting)
        Await oTarczaTlo.SaveAsync(oFilePicN.Path)

        ' 2. rysowanie cienia - wersja pozioma (0 po prawej stronie)
        Dim dAngle As Double
        For iHr = 0 To 11
            For iMin = 0 To 59 Step 5   ' dokladność 5 minut - musza byc symetryczne, tj. 5 i 55 (bo odbicie lustrzane przy zmianie polozenia)
                Dim oTarcza = New CanvasRenderTarget(oDevice, 200, 200, 96)
                ds = oTarcza.CreateDrawingSession
                ds.Clear(Colors.Transparent)
                ds.DrawImage(oTarczaTlo)

                dAngle = m_PustyLuk + (iHr + iMin / 60) * m_LukGodziny
                ny1 = Math.Sin(dAngle) * m_Diameter * 1.01
                nx1 = Math.Cos(dAngle) * m_Diameter * 1.01

                ds.DrawLine(100, m_MoveBottom + 10, 100 + nx1, m_MoveBottom + 10 + ny1, Colors.LightGray, 3)

                ds.Dispose()

                Dim oFilePic = Await oFolder.CreateFileAsync(
                        iHr.ToString("d2") & iMin.ToString("d2") & ".png", CreationCollisionOption.ReplaceExisting)
                Await oTarcza.SaveAsync(oFilePic.Path)

                oTarcza.Dispose()


            Next
        Next

        'SetSettingsBool("inCreatingSunTarcza", False)

    End Sub

    Private Shared Function SecTileUpdateSunAnaTarcza(iHr As Integer, iMin As Integer, bSun As Boolean) As String
        ' zwraca XML z obrazkiem dla iHr i iMin
        Dim sTmp, sPic As String

        If iHr > 11 Then iHr = iHr - 12

        If bSun Then
            sPic = "ms-appdata:///local/pic/s/"
        Else
            sPic = "ms-appdata:///local/pic/a/"
        End If


        sPic = sPic & iHr.ToString("d2") & iMin.ToString("d2") & ".png"

        sTmp = "<tile><visual>"

        sTmp = sTmp & "<binding template ='TileSmall' branding='none'>"
        sTmp = sTmp & "<image placement='background' src='" & sPic & "'/>"
        'sTmp = sTmp & "<image hint-align='center' src='" & sPic & "'/>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template ='TileMedium' branding='none'>"
        sTmp = sTmp & "<image placement='background' src='" & sPic & "'/>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template ='TileWide' branding='none'>"
        sTmp = sTmp & "<image placement='background' src='" & sPic & "'/>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "<binding template ='TileLarge' branding='none'>"
        sTmp = sTmp & "<image placement='background' src='" & sPic & "'/>"
        sTmp = sTmp & "</binding>"

        sTmp = sTmp & "</visual></tile>"

        Return sTmp

    End Function


    Private Shared Function SecTileUpdateAnaTarcza(iHr As Integer, iMin As Integer) As String
        Return SecTileUpdateSunAnaTarcza(iHr, iMin, False)
    End Function
    Private Shared Function SecTileUpdateSunTarcza(iHr As Integer, iMin As Integer) As String
        If iHr > 11 Then iHr = iHr - 12

        If GetSettingsInt("orientation") = "0" Then
            iHr = 12 - iHr
            iMin = 60 - iMin    ' muszą byc symetryczne minuty, tj. 5 i -5
            If iMin > 0 Then iHr -= 1   ' 10:30 -> 1:30, a nie 2:30
        End If

        iMin = iMin - (iMin Mod 5)

        Return SecTileUpdateSunAnaTarcza(iHr, iMin, True)
    End Function


    Public Shared Sub SecTileUpdateAna(bCommon As Boolean)
        EnsureAnaTarczaExist(False)

        Dim sXml As String
        Dim oDate As Date = Date.Now
        oDate = oDate.AddSeconds(-oDate.Second) ' wycofaj na poczatek minuty

        sXml = SecTileUpdateAnaTarcza(oDate.Hour, oDate.Minute)

        Dim oTUPS As TileUpdater = GetUpdaterClearQueue("SunDialAna", bCommon, sXml)
        Dim oXml As New XmlDocument

        oDate = oDate.AddMinutes(1)
        For i = 1 To 90    ' 1.5h, a timer jest co godzine
            sXml = SecTileUpdateAnaTarcza(oDate.Hour, oDate.Minute)
            oXml.LoadXml(sXml)
            Dim oSchST = New ScheduledTileNotification(oXml, oDate)
            oDate = oDate.AddMinutes(1)
            ' If i = 90 Then oSchST.ExpirationTime = oDate.AddMinutes(2)  ' wygas, nawet jak nie bedzie aktualizacji
            oSchST.ExpirationTime = oDate
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
        oDate = oDate.AddSeconds(-oDate.Second) ' wycofaj na poczatek minuty

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
        oDate = oDate.AddSeconds(-oDate.Second) ' wycofaj na poczatek minuty

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

    Public Shared Async Sub PriTileUpdate()
        '' rysowanie main Tile - z timer

        '' https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/tiles-and-notifications-primary-tile-apis
        '' dopiero od CreatorsUpdate, Aska tego nie ma
        ''If ApiInformation.IsTypePresent("Windows.UI.StartScreen.StartScreenManager") Then
        ''    Dim entry = Await Package.Current.GetAppListEntriesAsync()
        ''    Dim isPinned = Await StartScreenManager.GetDefault().ContainsAppListEntryAsync(entry)
        ''    If Not isPinned Then Exit Sub
        ''End If
        '' uzasadnienie ze nie ma kontroli istnienia Tile: bo może ktos dodac pozniej

        '' rysujemy defaultowy zegarek, od teraz przelacznik zaczyna od zera
        'If App.GetSettingsBool("uiPinSunDef") Then
        '    SecTileUpdateSun(True)
        '    SetSettingsInt("uiCurrentClock", 1)
        'End If
        'If App.GetSettingsBool("uiPinAnaDef") Then
        '    SecTileUpdateAna(True)
        '    SetSettingsInt("uiCurrentClock", 2)
        'End If
        'If App.GetSettingsBool("uiPinDigDef") Then
        '    SecTileUpdateDig(True)
        '    SetSettingsInt("uiCurrentClock", 3)
        'End If
        'If App.GetSettingsBool("uiPinSSgDef") Then
        '    SecTileUpdateSsg(True)
        '    SetSettingsInt("uiCurrentClock", 4)
        'End If
        'If App.GetSettingsBool("uiPinBinDef") Then
        '    SecTileUpdateBin(True)
        '    SetSettingsInt("uiCurrentClock", 5)
        'End If

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

        'Dim iClock As Integer = GetSettingsInt("uiCurrentClock")
        'iClock = GetNextClockId(iClock)
        'If iClock = 0 Then Return False
        'SetSettingsInt("uiCurrentClock", iClock)

        'Dim sXml As String
        'Dim oDate As Date = Date.Now

        'Select Case iClock
        '    Case 1  ' sundial
        '        EnsureSunTarczaExist(False)
        '        sXml = SecTileUpdateSunTarcza(oDate.Hour, oDate.Minute)
        '    Case 2  ' analog
        '        EnsureAnaTarczaExist(False)
        '        sXml = SecTileUpdateAnaTarcza(oDate.Hour, oDate.Minute)
        '    Case 3  ' digital
        '        sXml = SecTileUpdateDigTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinDig24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
        '    Case 4  ' 7seg
        '        sXml = SecTileUpdateSsgTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinSsg24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
        '    Case 5  ' bin
        '        sXml = SecTileUpdateBinTarcza(oDate.Hour, oDate.Minute, App.GetSettingsBool("uiPinBin24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1))
        '    Case Else
        '        Return False     ' dziwacznosc, bo tak byc nie powinno
        'End Select

        'Dim oXml As New XmlDocument
        'oXml.LoadXml(sXml)

        'Dim oTile = New Windows.UI.Notifications.TileNotification(oXml)
        'Dim oTUPS = TileUpdateManager.CreateTileUpdaterForApplication

        'DelNearestUpdate(30)    ' usun aktualizacje tile jesli jest < 30 sekund do niej

        'oTUPS.Update(oTile)

        Return True
    End Function

    Protected Overrides Sub OnBackgroundActivated(args As BackgroundActivatedEventArgs)
        SecTileUpdate()
        'PriTileUpdate()
    End Sub
End Class
