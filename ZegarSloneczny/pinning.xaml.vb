﻿' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.UI.StartScreen
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class Pinning
    Inherits Page

    Private Sub uiPinPage_Loaded(sender As Object, e As RoutedEventArgs)
        uiPinSunDef.IsChecked = App.GetSettingsBool("uiPinSunDef")
        uiPinSunOn.IsOn = App.GetSettingsBool("uiPinSunOn")
        uiPinSunDef.IsEnabled = App.GetSettingsBool("uiPinSunOn")
        uiPinSunBut.IsEnabled = Not SecondaryTile.Exists("SunDialSun")

        uiPinAnaDef.IsChecked = App.GetSettingsBool("uiPinAnaDef")
        uiPinAnaOn.IsOn = App.GetSettingsBool("uiPinAnaOn")
        uiPinAnaDef.IsEnabled = App.GetSettingsBool("uiPinAnaOn")
        uiPinAnaSun.IsOn = App.GetSettingsBool("uiPinAnaSun")
        uiPinAnaBut.IsEnabled = Not SecondaryTile.Exists("SunDialAna")

        uiPinDigDef.IsChecked = App.GetSettingsBool("uiPinDigDef")
        uiPinDigOn.IsOn = App.GetSettingsBool("uiPinDigOn")
        uiPinDigDef.IsEnabled = App.GetSettingsBool("uiPinDigOn")
        uiPinDigSun.IsOn = App.GetSettingsBool("uiPinDigSun")
        uiPinDig24.IsOn = App.GetSettingsBool("uiPinDig24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1)
        uiPinDigBut.IsEnabled = Not SecondaryTile.Exists("SunDialDig")

        uiPinSSgDef.IsChecked = App.GetSettingsBool("uiPinSSgDef")
        uiPinSSgOn.IsOn = App.GetSettingsBool("uiPinSSgOn")
        uiPinSSgDef.IsEnabled = App.GetSettingsBool("uiPinSSgOn")
        uiPinSSgSun.IsOn = App.GetSettingsBool("uiPinSSgSun")
        uiPinSSg24.IsOn = App.GetSettingsBool("uiPinSSg24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1)
        uiPinSsgBut.IsEnabled = Not SecondaryTile.Exists("SunDialSSg")

        uiPinBinDef.IsChecked = App.GetSettingsBool("uiPinBinDef")
        uiPinBinOn.IsOn = App.GetSettingsBool("uiPinBinOn")
        uiPinBinDef.IsEnabled = App.GetSettingsBool("uiPinBinOn")
        uiPinBinSun.IsOn = App.GetSettingsBool("uiPinBinSun")
        uiPinBin24.IsOn = App.GetSettingsBool("uiPinBin24", Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") > -1)
        uiPinBinBut.IsEnabled = Not SecondaryTile.Exists("SunDialBin")

    End Sub

    Private Sub bPinOK_Click(sender As Object, e As RoutedEventArgs)
        App.SetSettingsBool("uiPinSunDef", uiPinSunDef.IsChecked)
        App.SetSettingsBool("uiPinSunOn", uiPinSunOn.IsOn)

        App.SetSettingsBool("uiPinAnaDef", uiPinAnaDef.IsChecked)
        App.SetSettingsBool("uiPinAnaOn", uiPinAnaOn.IsOn)
        App.SetSettingsBool("uiPinAnaSun", uiPinAnaSun.IsOn)

        App.SetSettingsBool("uiPinDigDef", uiPinAnaDef.IsChecked)
        App.SetSettingsBool("uiPinDigOn", uiPinAnaOn.IsOn)
        App.SetSettingsBool("uiPinDigSun", uiPinAnaSun.IsOn)
        App.SetSettingsBool("uiPinDig24", uiPinDig24.IsOn)

        App.SetSettingsBool("uiPinSSgDef", uiPinAnaDef.IsChecked)
        App.SetSettingsBool("uiPinSSgOn", uiPinAnaOn.IsOn)
        App.SetSettingsBool("uiPinSSgSun", uiPinAnaSun.IsOn)
        App.SetSettingsBool("uiPinSSg24", uiPinSSg24.IsOn)

        App.SetSettingsBool("uiPinBinDef", uiPinAnaDef.IsChecked)
        App.SetSettingsBool("uiPinBinOn", uiPinAnaOn.IsOn)
        App.SetSettingsBool("uiPinBinSun", uiPinAnaSun.IsOn)
        App.SetSettingsBool("uiPinBin24", uiPinBin24.IsOn)

        Me.Frame.Navigate(GetType(Setup))
    End Sub

    Private Sub uiSundial_Click(sender As Object, e As RoutedEventArgs)
    End Sub

    Private Sub uiAnalog_Click(sender As Object, e As RoutedEventArgs)
    End Sub

    Private Async Function AddSecTile(sName As String) As Task(Of Boolean)
        Dim oSTile = New SecondaryTile(sName, sName, sName, New Uri("ms-appx:///pic/" & sName & ".png"), TileSize.Square150x150)
        Dim isPinned = Await oSTile.RequestCreateAsync()
        Return isPinned
    End Function
    Private Async Sub uiDigital_Click(sender As Object, e As RoutedEventArgs)

        If Await AddSecTile("SunDialDig") Then uiPinDigBut.IsEnabled = False
        App.SecTileUpdateDig()

    End Sub

    Private Sub uiSevSeg_Click(sender As Object, e As RoutedEventArgs)
    End Sub

    Private Sub uiBinary_Click(sender As Object, e As RoutedEventArgs)
    End Sub

    Private Sub uiSunInc_Change(sender As Object, e As RoutedEventArgs) Handles uiPinSunOn.Toggled
        Try
            uiPinSunDef.IsEnabled = uiPinSunOn.IsOn
            If Not uiPinSunDef.IsEnabled And uiPinSunDef.IsChecked Then WybierzPierwszyEnabled
        Catch ex As Exception
            ' moze jeszcze nie byc czegos
        End Try
    End Sub

    Private Sub uiAnaInc_Change(sender As Object, e As RoutedEventArgs) Handles uiPinAnaOn.Toggled
        Try
            uiPinAnaDef.IsEnabled = uiPinAnaOn.IsOn
            If Not uiPinAnaDef.IsEnabled And uiPinAnaDef.IsChecked Then WybierzPierwszyEnabled
        Catch ex As Exception
            ' moze jeszcze nie byc czegos
        End Try
    End Sub

    Private Sub uiDigInc_Change(sender As Object, e As RoutedEventArgs) Handles uiPinDigOn.Toggled
        Try
            uiPinDigDef.IsEnabled = uiPinDigOn.IsOn
            If Not uiPinDigDef.IsEnabled And uiPinDigDef.IsChecked Then WybierzPierwszyEnabled
        Catch ex As Exception
            ' moze jeszcze nie byc czegos
        End Try
    End Sub

    Private Sub uiSSgInc_Change(sender As Object, e As RoutedEventArgs) Handles uiPinSSgOn.Toggled
        Try
            uiPinSSgDef.IsEnabled = uiPinSSgOn.IsOn
            If Not uiPinSSgDef.IsEnabled And uiPinSSgDef.IsChecked Then WybierzPierwszyEnabled
        Catch ex As Exception
            ' moze jeszcze nie byc czegos
        End Try
    End Sub

    Private Sub uiBinInc_Change(sender As Object, e As RoutedEventArgs) Handles uiPinBinOn.Toggled
        Try
            uiPinBinDef.IsEnabled = uiPinBinOn.IsOn
            If Not uiPinBinDef.IsEnabled And uiPinBinDef.IsChecked Then WybierzPierwszyEnabled
        Catch ex As Exception
            ' moze jeszcze nie byc czegos
        End Try
    End Sub

    Private Sub WybierzPierwszyEnabled()
        Try
            If uiPinSunDef.IsEnabled Then
                uiPinSunDef.IsChecked = True
                Exit Sub
            End If

            If uiPinAnaDef.IsEnabled Then
                uiPinAnaDef.IsChecked = True
                Exit Sub
            End If
            If uiPinDigDef.IsEnabled Then
                uiPinDigDef.IsChecked = True
                Exit Sub
            End If
            If uiPinSSgDef.IsEnabled Then
                uiPinSSgDef.IsChecked = True
                Exit Sub
            End If
            If uiPinBinDef.IsEnabled Then
                uiPinBinDef.IsChecked = True
                Exit Sub
            End If

            ' czyli wszystkie są zablokowane, włączamy domyslny
            uiPinAnaDef.IsEnabled = True
            uiPinAnaDef.IsChecked = True
        Catch ex As Exception
            ' moze jeszcze nie byc czegos
        End Try
    End Sub
End Class