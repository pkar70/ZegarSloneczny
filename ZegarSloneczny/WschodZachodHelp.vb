Public Class WschodZachodHelp
    Dim m_Date As Date
    Dim m_Lat, m_Long As Double
    Dim m_ZegarTyp As Integer

    Sub New()
        m_Date = DateTime.UtcNow
        m_ZegarTyp = App.GetSettingsInt("dusk")
        If m_ZegarTyp < 0 Or m_ZegarTyp > 3 Then m_ZegarTyp = 0
        m_Lat = App.GetSettingsDouble("latitude")
        m_Long = App.GetSettingsDouble("longitude")
    End Sub

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

    Public Function ConvertSun2Civil(oTime As TimeSpan) As TimeSpan
        Dim dHr As Double = oTime.Hours + oTime.Minutes / 60

        Dim dDzien As Double = GetZachod() - GetWschod()
        If dHr > 11 Then dDzien = 24 - dDzien

        Dim dSunHrLen As Double = dDzien / 12

        Dim dCivilHr As Double

        If oTime.Hours < 12 Then
            ' dzienne
            dCivilHr = GetWschod() + dSunHrLen * dHr
        Else
            ' nocne
            dCivilHr = GetZachod() + dSunHrLen * (dHr - 12)
            If dCivilHr > 23 Then dCivilHr = dCivilHr - 24
        End If

        Dim dOffset As Double = -TimeZoneInfo.Local.GetUtcOffset(m_Date).TotalHours
        dCivilHr = dCivilHr - dOffset
        If dCivilHr < 0 Then dCivilHr = 24 - dCivilHr
        If dCivilHr > 24 Then dCivilHr = dCivilHr - 24

        Return New TimeSpan(Math.Floor(dCivilHr),
            (dCivilHr - Math.Floor(dCivilHr)) * 60, 0)

    End Function

    Public Function ConvertCivil2Sun(oTime As TimeSpan) As TimeSpan
        ' oTime.Hr > 12 to noc dla slonecznej

        Dim hr As Double = oTime.Hours + oTime.Minutes / 60     ' dzisiaj czesc doby

        Dim dOffset As Double = -TimeZoneInfo.Local.GetUtcOffset(m_Date).TotalHours
        hr = hr + dOffset
        If hr < 0 Then hr = 24 - hr
        If hr > 24 Then hr = hr - 24

        Dim dDzien As Double = GetZachod() - GetWschod()
        Dim dSunTime As Double

        If hr > GetZachod() Or hr < GetWschod() Then
            ' godziny nocne
            dDzien = 24 - dDzien
            If hr > GetZachod() Then
                dSunTime = -12 * (hr - GetZachod()) / (dDzien)  ' PKAR: MINUS
            Else ' przed switem, hr=0..wschod
                dSunTime = -12 + 12 * (GetWschod() - hr) / (dDzien)  ' PKAR: MINUS
            End If
            dSunTime = 12 - dSunTime ' bo dSunTime jest MINUS, więc plus - a noc jest >12
        Else
            dSunTime = 12 * (hr - GetWschod()) / (dDzien)
        End If

        Return New TimeSpan(Math.Floor(dSunTime),
            (dSunTime - Math.Floor(dSunTime)) * 60, 0)
    End Function

End Class
