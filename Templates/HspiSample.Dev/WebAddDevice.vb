Imports System.Text
Imports System.Web
Imports Scheduler

Public Class WebPageAddDevice
    Inherits clsPageBuilder

    Public Sub New(ByVal pagename As String)
        MyBase.New(pagename)
    End Sub

    Public Overrides Function postBackProc(page As String, data As String, user As String, userRights As Integer) As String
        Dim parts As Collections.Specialized.NameValueCollection
        parts = HttpUtility.ParseQueryString(data)

        Return MyBase.postBackProc(page, data, user, userRights)
    End Function



    ' build and return the actual page
    Public Function GetPagePlugin(ByVal pageName As String, ByVal user As String, ByVal userRights As Integer, ByVal queryString As String) As String
        Dim stb As New StringBuilder

        Try
            Me.reset()

            stb.Append("This is the add device config")

            Return stb.ToString
        Catch ex As Exception
            'WriteMon("Error", "Building page: " & ex.Message)
            Return "error"
        End Try
    End Function


End Class
