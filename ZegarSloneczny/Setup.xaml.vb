' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
Imports Windows.Devices.Geolocation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class Setup
    Inherits Page

    Private Sub bSettOK_Click(sender As Object, e As RoutedEventArgs)

        App.SetSettingsInt("orientation", cbOrient.SelectedIndex)
        App.SetSettingsInt("digits", cbDigits.SelectedIndex)
        App.SetSettingsInt("dusk", cbType.SelectedIndex)

        Dim dTmp As Double
        If Double.TryParse(ebLat.Text, dTmp) Then
            If dTmp >= -90 And dTmp <= 90 Then App.SetSettingsDouble("latitude", dTmp)
        End If

        If Double.TryParse(ebLong.Text, dTmp) Then
            If dTmp >= -180 And dTmp <= 180 Then App.SetSettingsDouble("longitude", dTmp)
        End If

        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub Setup_Loaded(sender As Object, e As RoutedEventArgs)
        cbOrient.SelectedIndex = App.GetSettingsInt("orientation")
        cbDigits.SelectedIndex = App.GetSettingsInt("digits")
        cbType.SelectedIndex = App.GetSettingsInt("dusk")
        ebLat.Text = App.GetSettingsDouble("latitude").ToString
        If ebLat.Text.Length > 8 Then ebLat.Text = ebLat.Text.Substring(0, 8)
        ebLong.Text = App.GetSettingsDouble("longitude").ToString
        If ebLong.Text.Length > 8 Then ebLong.Text = ebLong.Text.Substring(0, 8)
    End Sub

    Private Async Sub bGetGPS_Click(sender As Object, e As RoutedEventArgs)

        bGetGPS.IsEnabled = False

        Dim oDevGPS As Geolocator
        oDevGPS = New Geolocator()

        ' od Anniversary (10.0.14393.0)
        If Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Devices.Geolocation.Geolocator.AllowFallbackToConsentlessPositions") Then
            oDevGPS.AllowFallbackToConsentlessPositions()
        Else
            Dim rVal As GeolocationAccessStatus = Await Geolocator.RequestAccessAsync()
            If rVal <> GeolocationAccessStatus.Allowed Then
                ' GeolocationAccessStatus.Denied , GeolocationAccessStatus.Unspecified
                Exit Sub
            End If
        End If

        Dim oPos As Geoposition
        Try
            oPos = Await oDevGPS.GetGeopositionAsync()
        Catch ex As Exception
            bGetGPS.IsEnabled = True
            Exit Sub
        End Try

        Dim sTmp As String
        sTmp = oPos.Coordinate.Point.Position.Latitude
        ebLat.Text = sTmp
        If sTmp.Length > 8 Then ebLat.Text = sTmp.Substring(0, 8)

        sTmp = oPos.Coordinate.Point.Position.Longitude
        ebLong.Text = sTmp
        If sTmp.Length > 8 Then ebLong.Text = sTmp.Substring(0, 8)

        bGetGPS.IsEnabled = True

    End Sub

    Private Sub bSetupPin_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(Pinning))
    End Sub
End Class
