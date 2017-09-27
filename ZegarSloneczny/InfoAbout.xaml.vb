' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.System
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class InfoAbout
    Inherits Page

    Private Sub bAboutOk_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Async Sub bRateIt_Click(sender As Object, e As RoutedEventArgs)

        Dim sUri As New Uri("ms-windows-store://review/?PFN=" & Package.Current.Id.FamilyName)
        Await Launcher.LaunchUriAsync(sUri)

    End Sub

    Private Sub bFeedback_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
