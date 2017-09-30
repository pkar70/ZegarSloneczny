' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.ApplicationModel.Resources
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class TodayInfo
    Inherits Page

    Private Sub Today_Loaded(sender As Object, e As RoutedEventArgs)
        ebLong.Text = App.GetSettingsDouble("longitude").ToString.Substring(0, 8)
        ebLat.Text = App.GetSettingsDouble("latitude").ToString.Substring(0, 8)
        uiTodayData.Date = Date.Now
        Przelicz()
    End Sub

    Private Sub Przelicz()
        Dim oHelper As WschodZachodHelp
        Dim oDate As Date = New Date(uiTodayData.Date.Year, uiTodayData.Date.Month, uiTodayData.Date.Day)
        'oDate = Date.UtcNow
        oHelper = New WschodZachodHelp(oDate,
        App.GetSettingsDouble("latitude"), App.GetSettingsDouble("longitude"), App.GetSettingsInt("dusk"))

        Dim dOffset As Double
        dOffset = Date.UtcNow.Hour - Date.Now.Hour
        dOffset = dOffset + (Date.UtcNow.Minute - Date.Now.Minute) / 60

        ebWschod.Text = Hr2Txt(oHelper.GetWschod - dOffset)
        ebNoon.Text = Hr2Txt(oHelper.GetPoludnie - dOffset)
        ebZachod.Text = Hr2Txt(oHelper.GetZachod - dOffset)

        Dim dLen As Double
        dLen = oHelper.GetZachod - oHelper.GetWschod
        ebDayLen.Text = Hr2Txt(dLen)
        ebHourLen.Text = CInt(60 * (dLen / 12))

        oDate = oDate.AddDays(1)
        oHelper = New WschodZachodHelp(oDate, 'uiTodayData.Date,
        App.GetSettingsDouble("latitude"), App.GetSettingsDouble("longitude"), App.GetSettingsInt("dusk"))
        Dim dLenNext As Double
        dLenNext = oHelper.GetZachod - oHelper.GetWschod
        If dLen > dLenNext Then
            ebNextDay.Text = ResourceLoader.GetForCurrentView().GetString("resNextDayShorter")
        Else
            ebNextDay.Text = ResourceLoader.GetForCurrentView().GetString("resNextDayLonger")
        End If

    End Sub

    Private Shared Function Hr2Txt(dLen As Double) As String
        Dim sTmp As String
        Dim iInd As Integer

        sTmp = dLen.ToString
        iInd = sTmp.IndexOfAny(".,")
        If iInd > 0 Then
            iInd = CInt(sTmp.Substring(0, iInd))
        Else
            iInd = 0
        End If

        Hr2Txt = iInd.ToString & ":" & CInt((dLen - iInd) * 60).ToString

    End Function

    Private Sub bOk_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub bToday_Changed(sender As Object, e As DatePickerValueChangedEventArgs)
        Przelicz()
    End Sub
End Class
