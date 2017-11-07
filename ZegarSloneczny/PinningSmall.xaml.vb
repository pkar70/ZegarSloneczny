' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
Imports Windows.UI.StartScreen

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class PinningSmall
    Inherits Page
    Private Sub uiPinPage_Loaded(sender As Object, e As RoutedEventArgs)
        uiPinSunBut.IsEnabled = Not SecondaryTile.Exists("SunDialSun")

        uiPinAnaBut.IsEnabled = Not SecondaryTile.Exists("SunDialAna")

        uiPinDig24.IsOn = App.GetSettingsBool("uiPinDig24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1)
        uiPinDigBut.IsEnabled = Not SecondaryTile.Exists("SunDialDig")

        uiPinSSg24.IsOn = App.GetSettingsBool("uiPinSSg24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1)
        uiPinSsgBut.IsEnabled = Not SecondaryTile.Exists("SunDialSSg")

        uiPinBin24.IsOn = App.GetSettingsBool("uiPinBin24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1)
        uiPinBinBut.IsEnabled = Not SecondaryTile.Exists("SunDialBin")

    End Sub

    Private Sub SaveSettings()
        ' dużo na False, bo mogą byc gdzies w kodzie wykorzystywane - to z pozniejszej wersji, o PrimaryTile
        App.SetSettingsInt("uiCurrentClock", 0)

        App.SetSettingsBool("uiPinSunDef", False)
        App.SetSettingsBool("uiPinSunOn", False)

        App.SetSettingsBool("uiPinAnaDef", False)
        App.SetSettingsBool("uiPinAnaOn", False)
        App.SetSettingsBool("uiPinAnaSun", False)

        App.SetSettingsBool("uiPinDigDef", False)
        App.SetSettingsBool("uiPinDigOn", False)
        App.SetSettingsBool("uiPinDigSun", False)
        App.SetSettingsBool("uiPinDig24", uiPinDig24.IsOn)

        App.SetSettingsBool("uiPinSSgDef", False)
        App.SetSettingsBool("uiPinSSgOn", False)
        App.SetSettingsBool("uiPinSSgSun", False)
        App.SetSettingsBool("uiPinSSg24", uiPinSSg24.IsOn)

        App.SetSettingsBool("uiPinBinDef", False)
        App.SetSettingsBool("uiPinBinOn", False)
        App.SetSettingsBool("uiPinBinSun", False)
        App.SetSettingsBool("uiPinBin24", uiPinBin24.IsOn)

    End Sub

    Private Sub bPinOK_Click(sender As Object, e As RoutedEventArgs)
        SaveSettings()
        Me.Frame.Navigate(GetType(Setup))
    End Sub

    Private Async Sub uiSundial_Click(sender As Object, e As RoutedEventArgs)
        If Await AddSecTile("SunDialSun") Then
            uiPinSunBut.IsEnabled = False
            'App.EnsureSunTarczaExist(True) ' to pierwsza instrukcja w w SecTileUpdate
            App.SecTileUpdateSun(False)
        End If
    End Sub

    Private Async Sub uiAnalog_Click(sender As Object, e As RoutedEventArgs)
        If Await AddSecTile("SunDialAna") Then
            uiPinAnaBut.IsEnabled = False
            'App.EnsureAnaTarczaExist(True)
            App.SecTileUpdateAna(False)
        End If
    End Sub

    Private Async Function AddSecTile(sName As String) As Task(Of Boolean)
        SaveSettings()  ' uaktualnij zmienne, bo bedzie je wykorzystywal rysujac Tiles
        Dim oSTile = New SecondaryTile(sName, sName, sName, New Uri("ms-appx:///pic/" & sName & ".png"), TileSize.Square150x150)
        Dim isPinned = Await oSTile.RequestCreateAsync()
        Return isPinned
    End Function
    Private Async Sub uiDigital_Click(sender As Object, e As RoutedEventArgs)
        If Await AddSecTile("SunDialDig") Then uiPinDigBut.IsEnabled = False
        App.SecTileUpdateDig(False)
    End Sub

    Private Async Sub uiSevSeg_Click(sender As Object, e As RoutedEventArgs)
        If Await AddSecTile("SunDialSsg") Then uiPinSsgBut.IsEnabled = False
        App.SecTileUpdateSsg(False)
    End Sub

    Private Async Sub uiBinary_Click(sender As Object, e As RoutedEventArgs)
        If Await AddSecTile("SunDialBin") Then uiPinBinBut.IsEnabled = False
        App.SecTileUpdateBin(False)
    End Sub


End Class
