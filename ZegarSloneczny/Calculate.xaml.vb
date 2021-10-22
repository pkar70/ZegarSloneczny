' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class Calculate
    Inherits Page

    Private moWschodZachod As WschodZachodHelp
    Private mbInChange As Boolean = False

    Private Sub uiOk_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.GoBack()
    End Sub

    Private Sub uiCivilTime_Changed(sender As Object, e As TimePickerValueChangedEventArgs)
        If mbInChange Then Exit Sub
        mbInChange = True

        uiSunTime.Time = moWschodZachod.ConvertCivil2Sun(uiCivilTime.Time)

        mbInChange = False
    End Sub

    Private Sub uiSunTime_Changed(sender As Object, e As TimePickerValueChangedEventArgs)
        If mbInChange Then Exit Sub
        mbInChange = True

        uiCivilTime.Time = moWschodZachod.ConvertSun2Civil(uiSunTime.Time)

        mbInChange = False
    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        moWschodZachod = New WschodZachodHelp
    End Sub
End Class
