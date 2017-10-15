' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

Imports Windows.ApplicationModel.Background
Imports Windows.UI.Xaml.Shapes
Imports ZegarSloneczny

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page

    ' setup
    Dim m_SetDiameter = 0.7    ' ile wysokosci idzie na cień
    Dim m_SetPustyLuk As Double = 30    ' 30 stopni pustego kąta

    Dim m_GnomonFi = 6      ' srednica gnomonu (kolka)

    Dim m_LukGodziny, m_PustyLuk As Double
    Dim m_RootX, m_RootY, m_Diameter As Integer
    Dim m_CienId As Integer
    Private Timer1 As DispatcherTimer

    Private Shared Sub SetDefaults()
        Dim iTmp As Integer
        Dim dTmp As Double

        iTmp = App.GetSettingsInt("orientation", 10)    ' poza zakresem: jak nie ma, to ustaw
        If iTmp < 0 Or iTmp > 1 Then App.SetSettingsInt("orientation", 0)

        iTmp = App.GetSettingsInt("digits", 10)
        If iTmp < 0 Or iTmp > 2 Then App.SetSettingsInt("digits", 0)

        iTmp = App.GetSettingsInt("dusk", 10)
        If iTmp < 0 Or iTmp > 3 Then App.SetSettingsInt("dusk", 0)

        dTmp = App.GetSettingsDouble("latitude", -100)
        If dTmp < -90 Or dTmp > 90 Then App.SetSettingsDouble("latitude", 50 + 4 / 60)

        dTmp = App.GetSettingsDouble("longitude", -100)
        If dTmp < -90 Or dTmp > 90 Then App.SetSettingsDouble("longitude", 19 + 56 / 60)

    End Sub
    Private Shared Sub TriggersUnregister()

        For Each oTask In BackgroundTaskRegistration.AllTasks
            If oTask.Value.Name = "ZegarSlonecznyTimer" Then oTask.Value.Unregister(True)
            If oTask.Value.Name = "ZegarSlonecznyServicing" Then oTask.Value.Unregister(True)
        Next

        BackgroundExecutionManager.RemoveAccess()
    End Sub

    Private Shared Async Sub TriggersRegister()

        TriggersUnregister()

        Dim oBAS As BackgroundAccessStatus
        oBAS = Await BackgroundExecutionManager.RequestAccessAsync()

        If oBAS = BackgroundAccessStatus.AlwaysAllowed Or oBAS = BackgroundAccessStatus.AllowedSubjectToSystemPolicy Then

            ' https://docs.microsoft.com/en-us/windows/uwp/launch-resume/create-And-register-an-inproc-background-task
            Dim builder As BackgroundTaskBuilder = New BackgroundTaskBuilder
            Dim oRet As BackgroundTaskRegistration

            builder.SetTrigger(New TimeTrigger(60, False))
            builder.Name = "ZegarSlonecznyTimer"
            oRet = builder.Register()

            builder.SetTrigger(New SystemTrigger(SystemTriggerType.ServicingComplete, True))
            builder.Name = "ZegarSlonecznyServicing"
            oRet = builder.Register()

        End If

    End Sub
    Private Sub Form_Loaded(sender As Object, e As RoutedEventArgs) Handles layoutRoot.Loaded
        ' Private Sub Sundial_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        m_CienId = 0


        SetDefaults()

        Timer1 = New DispatcherTimer
        Timer1.Interval = TimeSpan.FromSeconds(1)
        AddHandler Timer1.Tick, AddressOf Timer1_Tick
        Timer1.Start()

        TriggersRegister()

    End Sub
    Private Sub Timer1_Tick()
        Timer1.Interval = TimeSpan.FromMinutes(2)
        RysujCien()
    End Sub

    Private Sub Form_Resized(sender As Object, e As SizeChangedEventArgs) Handles layoutRoot.SizeChanged
        m_RootY = 0 ' sundial.ActualHeight / m_SetRootY ' /10
        m_RootX = sundial.ActualWidth / 2   'm_SetRootX
        m_LukGodziny = (180 - 2 * m_SetPustyLuk) / 12      ' 180 stopni, - pusty luk po obu stronach
        m_PustyLuk = Math.PI / (180 / m_SetPustyLuk)       ' na radiany
        m_LukGodziny = Math.PI / (180 / m_LukGodziny)

        m_Diameter = Math.Min(sundial.ActualHeight, sundial.ActualWidth) * m_SetDiameter

        sundial.Children.Clear()
        m_CienId = 0
        rysujTarcze()
        RysujCien()

    End Sub

    Private Sub rysujTarcze()
        'Dim x, y As Integer
        'x = layoutRoot.ActualWidth
        'y = layoutRoot.ActualHeight

        Dim bNoc As Boolean
        bNoc = (GodzinaSloneczna() < 0)

        ' gnomon
        Dim oGnomon As Ellipse
        oGnomon = New Ellipse With {
        .Width = m_GnomonFi,
            .Height = m_GnomonFi,
            .Margin = New Thickness(m_RootX - m_GnomonFi / 2, m_RootY - m_GnomonFi / 2, 0, 0),
            .HorizontalAlignment = HorizontalAlignment.Left,
            .VerticalAlignment = VerticalAlignment.Top
        }
        If bNoc Then
            oGnomon.Fill = New SolidColorBrush(Windows.UI.Colors.White)
        Else
            oGnomon.Fill = New SolidColorBrush(Windows.UI.Colors.Black)
        End If
        sundial.Children.Add(oGnomon)

        Dim oBrush As Brush
        If bNoc Then
            oBrush = New SolidColorBrush(Windows.UI.Colors.White)
        Else
            oBrush = New SolidColorBrush(Windows.UI.Colors.Black)
        End If

        Dim oLine As Line
        Dim nx, ny, nx1, ny1 As Integer
        For i As Integer = 0 To 12

            ' rysuj kreske godzinową
            ny = Math.Sin(m_PustyLuk + i * m_LukGodziny) * m_Diameter
            nx = Math.Cos(m_PustyLuk + i * m_LukGodziny) * m_Diameter
            ny1 = Math.Sin(m_PustyLuk + i * m_LukGodziny) * m_Diameter * 1.1 ' * (m_Diameter + 5)
            nx1 = Math.Cos(m_PustyLuk + i * m_LukGodziny) * m_Diameter * 1.1

            oLine = New Line With {
                .Stroke = oBrush,
                .StrokeThickness = 2, ' PKAR: 2
                .X1 = nx + m_RootX,
                .Y1 = ny + m_RootY,
                .X2 = nx1 + m_RootX,
                .Y2 = ny1 + m_RootY
            }

            sundial.Children.Add(oLine)

            ' podpisz wedle m_Digits = 0 (iiii), 1 (iv), 2 (4)
            Dim m_Horiz As Boolean
            If App.GetSettingsInt("orientation") = "1" Then
                m_Horiz = True
            Else
                m_Horiz = False
            End If

            '            If Not ((i = 0 And m_Horiz) Or (i = 12 And Not m_Horiz)) Then
            If Not (i = 0 Or i = 12) Then
                ny = Math.Sin(m_PustyLuk + i * m_LukGodziny) * m_Diameter * 1.1 ' * (m_Diameter + 5)
                nx = Math.Cos(m_PustyLuk + i * m_LukGodziny) * m_Diameter * 1.1
                Dim oTB As TextBlock
                oTB = New TextBlock With {
                    .Text = GetOpis(i, m_Horiz),
                    .Margin = New Thickness(m_RootX + nx - 5, m_RootY + ny, 0, 0),
                    .HorizontalAlignment = HorizontalAlignment.Left,
                    .VerticalAlignment = VerticalAlignment.Top,
                    .TextAlignment = TextAlignment.Center,
                    .Foreground = oBrush
                }
                '.FontWeight = Windows.UI.Text.FontWeights.Bold,

                sundial.Children.Add(oTB)
            End If

            ' rysuj kreske polgodzinna
            If i < 12 Then
                ny = Math.Sin(m_LukGodziny / 2 + m_PustyLuk + i * m_LukGodziny) * m_Diameter
                nx = Math.Cos(m_LukGodziny / 2 + m_PustyLuk + i * m_LukGodziny) * m_Diameter
                ny1 = Math.Sin(m_LukGodziny / 2 + m_PustyLuk + i * m_LukGodziny) * m_Diameter * 1.05 ' (m_Diameter + 2)
                nx1 = Math.Cos(m_LukGodziny / 2 + m_PustyLuk + i * m_LukGodziny) * m_Diameter * 1.05

                oLine = New Line With {
                    .Stroke = oBrush,
                    .StrokeThickness = 2, ' PKAR: 2
                    .X1 = nx + m_RootX,
                    .Y1 = ny + m_RootY,
                    .X2 = nx1 + m_RootX,
                    .Y2 = ny1 + m_RootY
                }

                sundial.Children.Add(oLine)
            End If
        Next


    End Sub
    Private Sub RysujCien()
        ' cien
        Dim oLine As Line
        Dim hr As Double
        Dim bNoc As Boolean

        hr = GodzinaSloneczna()

        If hr < 0 Then
            bNoc = True
            hr = Math.Abs(hr)
        Else
            bNoc = False
        End If

        If hr > 12 Then Exit Sub
        If App.GetSettingsInt("orientation") = "0" Then hr = 12 - hr

        If m_CienId <> 0 Then
            sundial.Children.RemoveAt(m_CienId)
        End If

        For Each oItem As FrameworkElement In sundial.Children
            If oItem.Name = "Cien" Then sundial.Children.Remove(oItem)
        Next

        Dim nx, ny As Integer
        ' ewentualnie dla nocy (godziny MINUS) zmieniac na czarną tarcze z biała kreską?
        ' ale wtedy i skala powinna byc nie czarna a biala

        ny = Math.Sin(m_PustyLuk + Math.Abs(hr) * m_LukGodziny) * m_Diameter * 1.01
        nx = Math.Cos(m_PustyLuk + Math.Abs(hr) * m_LukGodziny) * m_Diameter * 1.01

        oLine = New Line With {
            .Stroke = New SolidColorBrush(Windows.UI.Colors.DarkGray),
            .StrokeThickness = 2, ' PKAR: 2
            .Name = "Cien",
            .X1 = m_RootX,
            .Y1 = m_RootY,
            .X2 = nx + m_RootX,
            .Y2 = ny + m_RootY
        }

        If bNoc Then
            layoutRoot.Background = New SolidColorBrush(Windows.UI.Colors.Black)
            oLine.Stroke = New SolidColorBrush(Windows.UI.Colors.LightGray)
        Else
            layoutRoot.Background = New SolidColorBrush(Windows.UI.Colors.White)
        End If

        ' m_CienId = layoutRoot.Children.Add(oLine)
        m_CienId = sundial.Children.Count
        sundial.Children.Add(oLine)

    End Sub

    Private Sub bSetup_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(Setup))
    End Sub

    Private Sub bInfo_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(InfoAbout))
    End Sub

    Private Sub bTodayInfo_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(TodayInfo))
    End Sub

    'Function GodzinaSlonecznaNew()
    '    ' po nowemu
    '    ' http://www.koch-tcm.ch/usa/local-to-solar-time-calculator/index.php

    '    Dim varX, varEOT, varLC, hr As Double
    '    'Dim oTz As TimeZone
    '    'oTz = TimeZone.CurrentTimeZone

    '    hr = DateTime.UtcNow.Hour + DateTime.UtcNow.Minute / 60     ' dzisiaj czesc doby
    '    varX = ((360 * (DateTime.UtcNow.DayOfYear - 1)) / 365.242) * Math.PI / 180
    '    varEOT = 0.258 * Math.Cos(varX) - 7.416 * Math.Sin(varX) - 3.648 * Math.Cos(2 * varX) - 9.228 * Math.Sin(2 * varX)
    '    ' varLC = (15 * varZ - GetLongit()) / 15  ' timezone offset (hours) 
    '    'varLC = GetLongit() / 15  ' przesuniecie UTC jest wyzej
    '    '        GodzinaSlonecznaNew = hr + (varEOT / 60) - varLC - TimeZoneInfo.IsDaylightSavingTime
    '    GodzinaSlonecznaNew = hr + (varEOT / 60) - varLC

    'End Function
    Shared Function GetOpis(ByVal iHr As Integer, ByVal bHoriz As Boolean)
        ' = 0 (iiii), 1 (iv), 2 (4)

        Dim iDigits As Integer
        iDigits = App.GetSettingsInt("digits")
        If Not bHoriz Then iHr = 12 - iHr

        GetOpis = " "
        If iHr = 0 Then Exit Function

        GetOpis = iHr
        If iDigits = 2 Then Exit Function

        If iHr = 0 Then Exit Function

        Select Case iHr
            Case 1
                GetOpis = "I"
            Case 2
                GetOpis = "II"
            Case 3
                GetOpis = "III"
            Case 4
                If iDigits = 1 Then
                    GetOpis = "IV"
                Else
                    GetOpis = "IIII"
                End If
            Case 5
                GetOpis = "V"
            Case 6
                GetOpis = "VI"
            Case 7
                GetOpis = "VII"
            Case 8
                GetOpis = "VIII"
            Case 9
                If iDigits = 1 Then
                    GetOpis = "IX"
                Else
                    GetOpis = "VIIII"
                End If
            Case 10
                GetOpis = "X"
            Case 11
                GetOpis = "XI"
            Case 12
                GetOpis = "XII"
        End Select

    End Function
    Shared Function GodzinaSloneczna() As Double
        Dim hr As Double
        Dim dDzien As Double
        Dim WschodZachod As WschodZachodHelp

        WschodZachod = New WschodZachodHelp(DateTime.UtcNow,
             App.GetSettingsDouble("latitude"), App.GetSettingsDouble("longitude"), App.GetSettingsInt("dusk"))

        dDzien = WschodZachod.GetZachod() - WschodZachod.GetWschod()

        ' hr = Now.Hour + Now.Minute / 60     ' dzisiaj czesc doby
        hr = DateTime.UtcNow.Hour + DateTime.UtcNow.Minute / 60     ' dzisiaj czesc doby
        If hr > WschodZachod.GetZachod() Or hr < WschodZachod.GetWschod() Then
            ' godziny nocne
            dDzien = 24 - dDzien
            GodzinaSloneczna = -12 * (hr - WschodZachod.GetZachod()) / (dDzien)  ' PKAR: MINUS
            Exit Function
        Else
            GodzinaSloneczna = 12 * (hr - WschodZachod.GetWschod()) / (dDzien)
        End If

    End Function
End Class

Public Class WschodZachodHelp
    Dim m_Date As Date
    Dim m_Lat, m_Long As Double
    Dim m_ZegarTyp As Integer

    Sub New(ByVal dData As DateTime, ByVal dLat As Integer, ByVal dLong As Integer, ByVal dTyp As Integer)
        m_Date = dData
        m_ZegarTyp = dTyp
        If dTyp < 0 Or dTyp > 3 Then m_ZegarTyp = 0
        m_Lat = dLat
        m_Long = dLong
    End Sub

    Private Function GetTypZegara() As Single
        ' algorytmy zegarowe wedlug
        ' http://cybermoon.w.interia.pl/wiedza/algorithms/wschody_slonca.html
        Select Case m_ZegarTyp
            Case 1  ' cywilny civil (srodek tarczy 6° ponizej horyzontu)
                Return -6
            Case 2  ' morski (żeglarski) nautical
                Return -12
            Case 3  ' astronomiczny astronomical
                Return -18
            Case Else   'zach? normal (refrakcja)
                Return -0.833
        End Select

    End Function
    Public Function GetWschod() As Double
        Return (System.Math.PI - (ZegarE() + 0.017453293 * m_Long + 1 * System.Math.Acos(ZegarC()))) * 57.29577951 / 15
    End Function
    Public Function GetPoludnie() As Double
        Return (System.Math.PI - (ZegarE() + 0.017453293 * m_Long + 0 * System.Math.Acos(ZegarC()))) * 57.29577951 / 15
    End Function

    Public Function GetZachod() As Double
        Return (System.Math.PI - (ZegarE() + 0.017453293 * m_Long - 1 * System.Math.Acos(ZegarC()))) * 57.29577951 / 15
    End Function

    Private Function ZegarJ() As Double
        Return 367 * m_Date.Year -
            CInt(7 * (m_Date.Year + CInt((m_Date.Month + 9) / 12)) / 4) +
            CInt(275 * m_Date.Month / 9) +
            m_Date.Day - 730531.5
    End Function
    Private Function ZegarCent() As Double
        Return (ZegarJ() / 36525)
    End Function
    Private Function ZegarL() As Double
        Return (4.8949504201433 + 628.331969753199 * ZegarCent()) Mod 6.28318530718
    End Function
    Private Function ZegarG() As Double
        Return (6.2400408 + 628.3019501 * ZegarCent()) Mod 6.28318530718
    End Function
    Private Function ZegarO() As Double
        Return 0.409093 - (0.0002269 * ZegarCent())
    End Function

    Private Function ZegarF() As Double
        Return 0.033423 * Math.Sin(ZegarG()) + 0.00034907 * Math.Sin(2 * ZegarG())
    End Function
    Private Function ZegarE() As Double
        Return 0.0430398 * Math.Sin(2 * (ZegarL() + ZegarF())) - 0.00092502 * Math.Sin(4 * (ZegarL() + ZegarF())) - ZegarF()
    End Function

    Private Function ZegarA() As Double
        Return Math.Asin(System.Math.Sin(ZegarO()) * Math.Sin(ZegarL() + ZegarF()))
    End Function
    Private Function ZegarC() As Double
        Return (Math.Sin(0.017453293 * GetTypZegara()) - Math.Sin(0.017453293 * m_Lat) * System.Math.Sin(ZegarA())) /
        (Math.Cos(0.017453293 * m_Lat) * Math.Cos(ZegarA()))
    End Function

End Class
