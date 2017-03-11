Option Explicit On
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Globalization
Imports VB = Microsoft.VisualBasic
Imports System.Web
Imports HomeSeerAPI

Friend Module Web_UI_Util


    ' HTTP constants
    Public Const HTML_StartHead As String = "<head>" & vbCrLf
    Public Const HTML_EndHead As String = "</head>" & vbCrLf
    Public Const HTML_StartPage As String = "<html>" & vbCrLf
    Public Const HTML_EndPage As String = "</html>" & vbCrLf
    Public Const HTML_StartBody As String = "<body>" & vbCrLf
    Public Const HTML_EndBody As String = "</body>" & vbCrLf
    'Public Const HTML_StartForm As String = "<form method=""post"">" & vbCrLf
    Public Const HTML_EndForm As String = "</form>" & vbCrLf
    Public Const HTML_NewLine As String = "<br>" & vbCrLf
    'Public Const HTML_StartPara As String = "<p>"
    'Public Const HTML_EndPara As String = "</p>" & vbCrLf
    'Public Const HTML_Line As String = "<hr noshade>"
    Public Const HTML_Line As String = "<hr>"
    Public Const HTML_EndTable As String = "</table>" & vbCrLf
    Public Const HTML_EndTableAlign As String = "</table></div>" & vbCrLf
    'Public Const HTML_StartRow As String = "<tr>"
    Public Const HTML_EndRow As String = "</tr>" & vbCrLf
    Public Const HTML_EndCell As String = "</td>"
    'Public Const ALIGN_RIGHT As Integer = 1 ' for cell alignment in tables
    'Public Const ALIGN_LEFT As Integer = 2
    'Public Const ALIGN_CENTER As Integer = 3
    'Public Const ALIGN_TOP As Integer = 4
    'Public Const ALIGN_BOTTOM As Integer = 5
    'Public Const ALIGN_MIDDLE As Integer = 6
    Public Const HTML_StartBold As String = "<b>"
    Public Const HTML_EndBold As String = "</b>"
    Public Const HTML_StartItalic As String = "<i>"
    Public Const HTML_EndItalic As String = "</i>"
    Public Const HTML_StartHead2 As String = "<h2>"
    Public Const HTML_EndHead2 As String = "</h2>"
    Public Const HTML_StartHead3 As String = "<h3>"
    Public Const HTML_EndHead3 As String = "</h3>"
    Public Const HTML_StartHead4 As String = "<h4>"
    Public Const HTML_EndHead4 As String = "</h4>"
    Public Const HTML_StartTitle As String = "<title>"
    Public Const HTML_EndTitle As String = "</title>" & vbCrLf
    Public Const HTML_EndFont As String = "</font>"

    Public Const COLOR_WHITE As String = "#FFFFFF"
    Public Const COLOR_KEWARE As String = "#0080C0"
    Public Const COLOR_RED As String = "#FF0000"
    Public Const COLOR_BLACK As String = "#000000"
    Public Const COLOR_NAVY As String = "#000080"
    Public Const COLOR_LT_BLUE As String = "#D9F2FF"
    Public Const COLOR_LT_GRAY As String = "#E1E1E1"
    Public Const COLOR_LT_PINK As String = "#FFB6C1"
    Public Const COLOR_ORANGE As String = "#D58000"
    Public Const COLOR_GREEN As String = "#008000"
    Public Const COLOR_PURPLE As String = "#4B088A"

    Friend Structure EventWebControlInfo
        Public Decoded As Boolean
        Public EventTriggerGroupID As Integer
        Public GroupID As Integer
        Public EvRef As Integer
        Public TriggerORActionID As Integer
        Public Name_or_ID As String
        Public Additional As String
    End Structure

    <Serializable()> _
    Public Enum HTML_Align
        RIGHT = 1
        LEFT = 2
        CENTER = 3
        JUSTIFY = 4
    End Enum

    <Serializable()> _
    Public Enum HTML_VertAlign
        TOP = 1
        MIDDLE = 2
        BOTTOM = 3
        BASELINE = 4
    End Enum

    <Serializable()> _
    Public Enum HTML_TableAlign
        RIGHT = 1
        LEFT = 2
        INHERIT = 3
    End Enum

    Friend gMultiConfigResult As String = ""

#Region "    HTML Public Functions     "

    ' =====================================================================================================
    '                                  START OF HTML FUNCTIONS
    ' =====================================================================================================

#If 0 Then
    Private Function AddNavLinkPlugin(ByRef url As String, ByRef label As String, _
                                 Optional ByVal bSelected As Boolean = False, _
                                 Optional ByVal AltText As String = "") As Object
        Dim st As String = ""
        On Error Resume Next

        If AltText Is Nothing Then
            AltText = " "
        End If
        '<input type="button" class="linkrowbutton" value="Webring" onClick="location.href='/misc/webring.asp'">
        If bSelected Then
            st = "<input type=""button"" class=""functionrowbuttonselected ui-corner-all"" value=""" & label & """ alt=""" & AltText & """ onClick=""location.href='" & url & "'"" onmouseover=""this.className='functionrowbutton ui-corner-all';"" onmouseout=""this.className='functionrowbuttonselected ui-corner-all';"">"
        Else
            st = "<input type=""button"" class=""functionrowbutton ui-corner-all"" value=""" & label & """ alt=""" & AltText & """ onClick=""location.href='" & url & "'"" onmouseover=""this.className='functionrowbuttonselected ui-corner-all';"" onmouseout=""this.className='functionrowbutton ui-corner-all';"">"
        End If

        Return st
    End Function
#End If
    Friend Function AddNavTransLink(ByRef url As String, ByRef label As String, _
                                     Optional ByVal bSelected As Boolean = False, _
                                     Optional ByVal AltText As String = "") As String
        Dim st As String = ""
        On Error Resume Next

        If AltText Is Nothing Then
            AltText = " "
        End If
        '<input type="button" class="linkrowbutton" value="Webring" onClick="location.href='/misc/webring.asp'">
        If bSelected Then
            st = "<input type=""button"" class=""buttontrans ui-corner-all"" value=""" & label & """ alt=""" & AltText & """ onClick=""location.href='" & url & "'"" onmouseover=""this.className='buttontrans ui-corner-all';"" onmouseout=""this.className='buttontrans ui-corner-all';"">"
        Else
            st = "<input type=""button"" class=""buttontrans ui-corner-all"" value=""" & label & """ alt=""" & AltText & """ onClick=""location.href='" & url & "'"" onmouseover=""this.className='buttontrans ui-corner-all';"" onmouseout=""this.className='buttontrans ui-corner-all';"">"
        End If

        Return st
    End Function

    Public Function HTML_Graphic(ByVal file As String, ByVal width As Integer, ByVal height As Integer) As String
        Dim st As String = ""
        If width = 0 Then
            st = "<img src=""" & file & """ />"
        Else
            st = "<img src=""" & file & """ height=""" & height.ToString & """" & "width=""" & width.ToString & """ />"
        End If
        Return st
    End Function

    Public Function AddHidden(ByRef Name As String, ByRef Value As String) As Object
        Dim st As String = ""
        On Error Resume Next

        st = "<input type=""hidden"" name=""" & Name & """ value=""" & Value & """>"
        Return st
    End Function

    Public Function AddHiddenWithID(ByRef Name As String, ByRef Value As String) As Object
        Dim st As String = ""
        On Error Resume Next

        st = "<input type=""hidden"" ID=""" & Name & """ name=""" & Name & """ value=""" & Value & """>"
        Return st
    End Function

    Public Function HTML_StartFont(ByVal Color As String) As String
        Return HTML_Font(Color, "", "")
    End Function

    Public Function HTML_Font(Optional ByVal color As String = "", Optional ByVal Style As String = "", Optional ByVal ClassName As String = "") As String
        Dim st As String = "<font "
        On Error Resume Next

        If color IsNot Nothing AndAlso Not String.IsNullOrEmpty(color.Trim) Then
            st &= "color='" & color.Trim & "' "
        End If
        If ClassName IsNot Nothing AndAlso Not String.IsNullOrEmpty(ClassName.Trim) Then
            st &= "class='" & ClassName.Trim & "' "
        End If
        If Style IsNot Nothing AndAlso Not String.IsNullOrEmpty(Style.Trim) Then
            st &= "style='" & Style.Trim & "' "
        End If
        st &= ">"
        Return st
    End Function

    Public Function HTML_StartCell(ByRef Class_name As String, _
                                   ByRef colspan As Short, _
                                   Optional ByVal align As HTML_Align = 0, _
                                   Optional ByVal nowrap As Boolean = False, _
                                   Optional ByVal RowHeight As Integer = 0, _
                                   Optional ByVal Style As String = "", _
                                   Optional ByVal VertAlign As HTML_VertAlign = 0) As String
        'Dim st As String = ""
        'Dim stalign As String = ""
        'Dim wrap As String = ""
        'Dim rheight As String = ""
        Dim s As String = "<td "
        Dim TotalStyle As String = ""

        On Error Resume Next

        'If RowHeight > 0 Then
        'rheight = " height=""" & RowHeight.ToString & """"
        'End If
        If Class_name IsNot Nothing AndAlso Not String.IsNullOrEmpty(Class_name) Then
            s &= "class='" & Class_name & "' "
        End If

        If Style IsNot Nothing AndAlso Not String.IsNullOrEmpty(Style.Trim) Then
            TotalStyle = Style.Trim
        End If

        If nowrap Then
            'wrap = " nowrap"
            If Not String.IsNullOrEmpty(TotalStyle.Trim) Then
                If TotalStyle.Trim.EndsWith(";") Then
                    ' Nothing to do.
                Else
                    TotalStyle &= ";"
                End If
            End If
            TotalStyle &= " white-space: nowrap;"
        End If

        If RowHeight > 0 Then
            If Not String.IsNullOrEmpty(TotalStyle.Trim) Then
                If TotalStyle.Trim.EndsWith(";") Then
                    ' Nothing to do.
                Else
                    TotalStyle &= ";"
                End If
            End If
            TotalStyle &= " height:" & RowHeight.ToString & "px;"
        End If

        If [Enum].IsDefined(GetType(HTML_Align), align) Then
            s &= "align='"
            Select Case align
                Case HTML_Align.LEFT
                    s &= "left"
                Case HTML_Align.CENTER
                    s &= "center"
                Case HTML_Align.RIGHT
                    s &= "right"
                Case HTML_Align.JUSTIFY
                    s &= "justify"
            End Select
            s &= "' "
        End If

        If [Enum].IsDefined(GetType(HTML_VertAlign), VertAlign) Then
            s &= "valign='"
            Select Case VertAlign
                Case HTML_VertAlign.TOP
                    s &= "top"
                Case HTML_VertAlign.MIDDLE
                    s &= "middle"
                Case HTML_VertAlign.BOTTOM
                    s &= "bottom"
                Case HTML_VertAlign.BASELINE
                    s &= "baseline"
            End Select
            s &= "' "
        End If

        If colspan > 0 Then
            s &= " colspan='" & colspan.ToString & "' "
        End If

        If Not String.IsNullOrEmpty(TotalStyle) Then
            s &= " style='" & TotalStyle & "' "
        End If

        s &= ">"

        Return s

    End Function

    Public Function AddNBSP(ByVal amount As Short) As String
        Dim s As New StringBuilder
        Dim x As Short

        If amount < 1 Then
            Return ""
        End If
        For x = 1 To amount
            s.Append("&nbsp;")
        Next
        Return s.ToString

    End Function


    'Public Const HTML_StartRow As String = "<tr>"
    Public Function HTML_StartRow(Optional ByVal ClassName As String = "", _
                                  Optional ByVal Style As String = "", _
                                  Optional ByVal Align As HTML_Align = 0, _
                                  Optional ByVal BackColor As String = "", _
                                  Optional ByVal VertAlign As HTML_VertAlign = 0) As String

        Dim s As String = "<tr "

        On Error Resume Next

        If ClassName IsNot Nothing AndAlso Not String.IsNullOrEmpty(ClassName.Trim) Then
            s &= "class='" & ClassName & "' "
        End If

        If Style IsNot Nothing AndAlso Not String.IsNullOrEmpty(Style) Then
            s &= "style='" & Style & "' "
        End If

        If [Enum].IsDefined(GetType(HTML_Align), Align) Then
            s &= "align='"
            Select Case Align
                Case HTML_Align.LEFT
                    s &= "left"
                Case HTML_Align.CENTER
                    s &= "center"
                Case HTML_Align.RIGHT
                    s &= "right"
                Case HTML_Align.JUSTIFY
                    s &= "justify"
            End Select
            s &= "' "
        End If

        If BackColor IsNot Nothing AndAlso Not String.IsNullOrEmpty(BackColor.Trim) Then
            s &= "bgcolor='" & BackColor & "' "
        End If

        If [Enum].IsDefined(GetType(HTML_VertAlign), VertAlign) Then
            s &= "valign='"
            Select Case VertAlign
                Case HTML_VertAlign.TOP
                    s &= "top"
                Case HTML_VertAlign.MIDDLE
                    s &= "middle"
                Case HTML_VertAlign.BOTTOM
                    s &= "bottom"
                Case HTML_VertAlign.BASELINE
                    s &= "baseline"
            End Select
            s &= "' "
        End If

        s &= ">"

        Return s

    End Function

    Public Function HTML_StartTable(ByVal border As Integer, _
                                    Optional ByRef CellSpacing As Short = -1, _
                                    Optional ByRef TableWidthPercent As Short = -1, _
                                    Optional ByVal Style As String = "", _
                                    Optional ByVal ClassName As String = "", _
                                    Optional ByVal Align As HTML_TableAlign = 0, _
                                    Optional ByVal CellPadding As Short = -1, _
                                    Optional ByVal TableWidthPixels As Integer = -1) As String

        Dim s As String = "<table "
        Dim TotalStyle As String = ""

        On Error Resume Next

        If ClassName IsNot Nothing AndAlso Not String.IsNullOrEmpty(ClassName) Then
            s &= "class='" & ClassName & "' "
        End If

        If Style IsNot Nothing AndAlso Not String.IsNullOrEmpty(Style.Trim) Then
            TotalStyle = Style.Trim
        End If


        If [Enum].IsDefined(GetType(HTML_TableAlign), Align) Then
            If Not String.IsNullOrEmpty(TotalStyle.Trim) Then
                If TotalStyle.Trim.EndsWith(";") Then
                    ' Nothing to do.
                Else
                    TotalStyle &= ";"
                End If
            End If
            TotalStyle &= " float:"
            Select Case Align
                Case HTML_TableAlign.LEFT
                    TotalStyle &= "left"
                Case HTML_TableAlign.RIGHT
                    TotalStyle &= "right"
                Case HTML_TableAlign.INHERIT
                    TotalStyle &= "inherit"
            End Select
            TotalStyle &= ";"
        End If

        If CellPadding >= 0 Then
            s &= "cellpadding='" & CellPadding.ToString & "' "
        End If

        If CellSpacing >= 0 Then
            s &= "cellspacing='" & CellSpacing.ToString & "' "
        End If

        If TableWidthPixels >= 0 Then
            If TableWidthPercent >= 0 Then
                s &= "width='" & TableWidthPercent.ToString & "%' "
            Else
                s &= "width='" & TableWidthPixels.ToString & "' "
            End If
        Else
            If TableWidthPercent >= 0 Then
                s &= "width='" & TableWidthPercent.ToString & "%' "
            End If
        End If

        If Not String.IsNullOrEmpty(TotalStyle.Trim) Then
            s &= "style='" & TotalStyle & "'"
        End If

        s &= ">"

        Return s

    End Function

    Public Function AddLink(ByRef ref As String, _
                            ByRef label As String, _
                            Optional ByRef image As Object = Nothing, _
                            Optional ByRef Width As Integer = 0, _
                            Optional ByRef Height As Integer = 0, _
                            Optional ByVal ClassName As String = "") As String
        Dim sReturn As String = ""
        Dim sClass As String = ""
        On Error Resume Next

        If ClassName <> "" Then
            sClass = " class=""" & ClassName & """ "
        End If

        If IsNothing(image) Then
            sReturn = "<a " & sClass & "href=""" & ref & """>" & label & "</a>" & vbCrLf
        Else
            If Width = 0 Then
                sReturn = "<a " & sClass & "href=""" & ref & """>" & label & "<img src=""" & image & """ border=""0""></a>" & vbCrLf
            Else
                sReturn = "<a " & sClass & "href=""" & ref & """>" & label & "<img src=""" & image & """ width=""" & Width.ToString & """ height=""" & Height.ToString & """ border=""0""></a>" & vbCrLf
            End If
        End If
        AddLink = sReturn
    End Function

    Public Function AddNavLink(ByRef ref As String, ByRef label As String, _
                               Optional ByVal bSelected As Boolean = False, _
                               Optional ByVal AltText As String = "", _
                               Optional ByVal target As String = "") As Object
        Dim st As String = ""
        On Error Resume Next

        If AltText Is Nothing Then
            AltText = " "
        End If
        '<input type="button" class="linkrowbutton" value="Webring" onClick="location.href='/misc/webring.asp'">
        If bSelected Then
            If target <> "" Then
                st = "<input type=""button"" class=""linkrowbuttonselected"" value=""" & label & """ alt=""" & AltText & """ onClick=""window.open('" & ref & "','" & target & "')" & """ onmouseover=""this.className='linkrowbutton';"" onmouseout=""this.className='linkrowbuttonselected';"">"
            Else
                st = "<input type=""button"" class=""linkrowbuttonselected"" value=""" & label & """ alt=""" & AltText & """ onClick=""location.href='" & ref & "'"" onmouseover=""this.className='linkrowbutton';"" onmouseout=""this.className='linkrowbuttonselected';"">"
            End If
        Else
            If target <> "" Then
                st = "<input type=""button"" class=""linkrowbutton"" value=""" & label & """ alt=""" & AltText & """ onClick=""window.open('" & ref & "','" & target & "')" & """ onmouseover=""this.className='linkrowbuttonselected';"" onmouseout=""this.className='linkrowbutton';"">"
            Else
                st = "<input type=""button"" class=""linkrowbutton"" value=""" & label & """ alt=""" & AltText & """ onClick=""location.href='" & ref & "'"" onmouseover=""this.className='linkrowbuttonselected';"" onmouseout=""this.className='linkrowbutton';"">"
            End If
        End If

        AddNavLink = st
    End Function

    Public Function AddNavLinkPlugin(ByRef ref As String, ByRef label As String, _
       Optional ByVal bSelected As Boolean = False, _
       Optional ByVal AltText As String = "") As Object
        Dim st As String = ""
        On Error Resume Next

        If AltText Is Nothing Then
            AltText = " "
        End If
        '<input type="button" class="linkrowbutton" value="Webring" onClick="location.href='/misc/webring.asp'">
        If bSelected Then
            st = "<input type=""button"" class=""linkrowbuttonselectedplugin"" value=""" & label & """ alt=""" & AltText & """ onClick=""location.href='" & ref & "'"" onmouseover=""this.className='linkrowbuttonplugin';"" onmouseout=""this.className='linkrowbuttonselectedplugin';"">"
        Else
            st = "<input type=""button"" class=""linkrowbuttonplugin"" value=""" & label & """ alt=""" & AltText & """ onClick=""location.href='" & ref & "'"" onmouseover=""this.className='linkrowbuttonselectedplugin';"" onmouseout=""this.className='linkrowbuttonplugin';"">"
        End If

        AddNavLinkPlugin = st
    End Function

    Public Function AddFuncLink(ByRef ref As String, ByRef label As String, Optional ByVal bSelected As Boolean = False) As Object
        Dim st As String = ""
        On Error Resume Next
        '<input type="button" class="linkrowbutton" value="Webring" onClick="location.href='/misc/webring.asp'">
        If bSelected Then
            st = "<input type=""button"" class=""functionrowbuttonselected"" value=""" & label & """ onClick=""location.href='" & ref & "'""  onmouseover=""this.className='functionrowbutton';"" onmouseout=""this.className='functionrowbuttonselected';"">"
        Else
            st = "<input type=""button"" class=""functionrowbutton"" value=""" & label & """ onClick=""location.href='" & ref & "'"" onmouseover=""this.className='functionrowbuttonselected';"" onmouseout=""this.className='functionrowbutton';"">"
        End If

        AddFuncLink = st
    End Function

    Public Function AddSWPB(Optional ByVal wWidth As Short = 250, Optional ByVal wHeight As Short = 150, Optional ByVal func_name As String = "openwc") As String
        Dim s As New StringBuilder

        s.Append("<script language=""JavaScript"">" & vbCrLf)
        s.Append("function " & func_name & "(parms)" & vbCrLf)
        s.Append("{" & vbCrLf)
        s.Append("var w = " & wWidth.ToString & ", h = " & wHeight.ToString & ";" & vbCrLf)
        s.Append("  w += 32;" & vbCrLf)
        s.Append("  h += 96;" & vbCrLf)
        s.Append("  wleft = (screen.width - w) / 2;" & vbCrLf)
        s.Append("  wtop = (screen.height - h) / 2;" & vbCrLf)
        s.Append("  var win = window.open('swpb?' + parms," & vbCrLf)
        s.Append("        '_blank'," & vbCrLf)
        s.Append("        'width=' + w + ', height=' + h + ', ' +" & vbCrLf)
        s.Append("        'left=' + wleft + ', top=' + wtop + ', ' +" & vbCrLf)
        s.Append("        'location=no, menubar=no,titlebar=no, ' +" & vbCrLf)
        s.Append("        'status=no, toolbar=no, scrollbars=no, resizable=no');" & vbCrLf)
        s.Append("  win.resizeTo(w, h);" & vbCrLf)
        s.Append("  win.moveTo(wleft, wtop);" & vbCrLf)
        s.Append("  win.focus();" & vbCrLf)
        s.Append("}" & vbCrLf)
        s.Append("</script>" & vbCrLf)

#If 0 Then
        s.Append("<script language=""JavaScript"">" & vbCrLf)
        s.Append("function " & func_name & "(parms) {" & vbCrLf)
        s.Append("var w = 800, h = 600;" & vbCrLf)
        s.Append("if (document.all) {" & vbCrLf)
        s.Append("   w = document.body.clientWidth;" & vbCrLf)
        s.Append("   h = document.body.clientHeight;" & vbCrLf)
        s.Append("}" & vbCrLf)
        s.Append("else if (document.layers) {" & vbCrLf)
        s.Append("   w = window.innerWidth;" & vbCrLf)
        s.Append("   h = window.innerHeight;" & vbCrLf)
        s.Append("}" & vbCrLf)
        s.Append("var popW = " & wWidth.ToString & ", popH = " & wHeight.ToString & ";" & vbCrLf)
        s.Append("var leftPos = (w-popW)/2, topPos = (h-popH)/2;" & vbCrLf)
        's.Append("wp = window.open(""swpb?"" + parms, ""_blank"", ""width=" & ww.ToString & ",height=" & wh.ToString & ",location=no,menubar=no,resizable=yes,scrollbars=no,status=no,titlebar=no,toolbar=no"");" & vbCrLf)
        s.Append("wp = window.open('swpb?' + parms, '_blank', 'width=" & wWidth.ToString & ",height=" & wHeight.ToString & ",top='+topPos+',left='+leftPos+',location=no,menubar=no,resizable=no,scrollbars=no,status=no,titlebar=no,toolbar=no');" & vbCrLf)
        s.Append("if (wp != null)      wp.focus();  }" & vbCrLf)
        s.Append("</script>" & vbCrLf)
#End If
        AddSWPB = s.ToString

    End Function

    Public Function FormCheckBox(ByRef label As String, ByRef name As String, ByRef Value As String, _
      ByRef checked As Boolean, Optional ByRef onChange As Boolean = False) As String
        Dim st As String = ""
        Dim chk As String = ""
        On Error Resume Next

        If checked Then
            chk = " checked "
        Else
            chk = ""
        End If
        If onChange Then
            st = "<input class=""formcheckbox"" type=""checkbox""" & chk & " name=""" & name & """ value=""" & Value & """ onClick=""submit();"" > " & label & vbCrLf
        Else
            st = "<input class=""formcheckbox"" type=""checkbox""" & chk & " name=""" & name & """ value=""" & Value & """ > " & label & vbCrLf
        End If
        Return st
    End Function

    Public Function FormCheckBoxEx(ByRef label As String, ByRef name As String, ByRef Value As String, _
     ByRef checked As Boolean, Optional ByRef PrevNNL As Boolean = False, _
     Optional ByRef NNL As Boolean = False, Optional ByRef ColSpan As Integer = 0) As Object
        Dim st As New StringBuilder
        Dim chk As String = ""
        On Error Resume Next

        If ColSpan < 1 Then ColSpan = 1

        If PrevNNL Then
            st.Append(HTML_StartCell("", ColSpan, HTML_Align.LEFT, True))
        Else
            'st.Append(HTML_StartTable(0) & HTML_StartCell("", 1, ALIGN_LEFT, True))
            st.Append(HTML_EndRow & HTML_StartRow() & HTML_StartCell("", ColSpan, HTML_Align.LEFT, True))
        End If

        If checked Then
            chk = " checked "
        Else
            chk = ""
        End If

        st.Append("<input class=""FormCheckBoxEx"" type=""checkbox""" & chk & " name=""" & name & """ value=""" & Value & """ > " & label & vbCrLf)

        If NNL Then
            st.Append(HTML_EndCell)
        Else
            'st.Append(HTML_EndCell & HTML_EndTable)
            st.Append(HTML_EndCell & HTML_EndRow)
        End If

        FormCheckBoxEx = st.ToString
    End Function

    Public Function FormRadio(ByRef label As String, ByRef name As String, ByRef Value As String, _
      ByRef checked As Boolean, Optional ByRef OnChange As Boolean = False) As Object
        Dim st As String = ""
        Dim chk As String = ""
        On Error Resume Next

        If checked Then
            chk = " checked "
        Else
            chk = ""
        End If
        If OnChange Then
            st = "<input class=""formradio"" type=""radio""" & chk & " name=""" & name & """ value=""" & Value & """ onClick=""submit();"" > " & label & vbCrLf
        Else
            st = "<input class=""formradio"" type=""radio""" & chk & " name=""" & name & """ value=""" & Value & """> " & label & vbCrLf
        End If

        FormRadio = st
    End Function

    Public Function FormRadioEx(ByRef label As String, ByRef name As String, _
                                ByRef Value As String, ByRef checked As Boolean, _
                                Optional ByRef PrevNNL As Boolean = False, _
                                Optional ByRef NNL As Boolean = False, _
                                Optional ByRef ColSpan As Integer = 0) As Object
        Dim st As String = ""
        Dim chk As String = ""
        On Error Resume Next

        If ColSpan < 1 Then ColSpan = 1

        If PrevNNL Then
            st = HTML_StartCell("", ColSpan, HTML_Align.LEFT, True)
        Else
            st = HTML_StartTable(0) & HTML_StartCell("", ColSpan, HTML_Align.LEFT, True)
        End If

        If checked Then
            chk = " checked "
        Else
            chk = ""
        End If
        st = st & "<input class=""FormRadioEx"" type=""radio""" & chk & " name=""" & name & """ value=""" & Value & """> " & label & vbCrLf

        If NNL Then
            st = st & HTML_EndCell
        Else
            st = st & HTML_EndCell & HTML_EndTable
        End If

        FormRadioEx = st
    End Function

    ' rjh 2.2.0.2
    Public Function FormLinkButton(ByRef URL As String, _
                                   ByRef label As String, _
                                   Optional ByVal Width As Integer = -1, _
                                   Optional ByVal Height As Integer = -1) As String
        Dim st As String = ""
        Dim more As String = ""
        If StringIsNullOrEmpty(URL) Then URL = ""
        If StringIsNullOrEmpty(label) Then label = ""

        On Error Resume Next

        st = "<input type=""button"" "
        If Width > 0 Or Height > 0 Then
            more = ""
            st &= "style="""
            If Width > 0 Then
                st &= "width:" & Width.ToString & "px"
                more = ";"
            End If
            If Height > 0 Then
                st &= more & "height:" & Height.ToString
            End If
            st &= """ "
        End If
        st &= "class=""formbutton"" value=""" & label & """ "
        st &= "onClick=""location.href='" & URL & "'"" "
        st &= " onmouseover=""this.className='formbutton';"" onmouseout=""this.className='formbutton';"">"
        'If Width > 0 Then
        '    Return "<input type=""button"" style=""width:" & Width.ToString & """ class=""formbutton"" value=""" & label & """ onClick=""location.href='" & URL & "'""  onmouseover=""this.className='formbutton';"" onmouseout=""this.className='formbutton';"">"
        'Else
        '    Return "<input type=""button"" class=""formbutton"" value=""" & label & """ onClick=""location.href='" & URL & "'""  onmouseover=""this.className='formbutton';"" onmouseout=""this.className='formbutton';"">"
        'End If
        Return st
    End Function

    <Serializable()> _
    Public Class AddStyle

        Public Function Build() As String
            Dim s As New Collections.Generic.List(Of String)
            Dim GotSomething As Boolean = False

            If vBackgroundColor <> ColorName._Unspecified_ Then
                If vBackgroundColor = ColorName._HexCode_Provided_ Then
                    If Not StringIsNullOrEmpty(vBackgroundColorHex) Then
                        GotSomething = True
                        s.Add("background-color:" & vBackgroundColorHex)
                    End If
                Else
                    GotSomething = True
                    s.Add("background-color:" & vBackgroundColor.ToString)
                End If
            End If

            If Not StringIsNullOrEmpty(vBackgroundImage) Then
                GotSomething = True
                s.Add("background-image:url(""" & vBackgroundImage & """)")
            End If

            If vBackgroundRepeat <> Background_Repeat_enum._Unspecified_ Then
                GotSomething = True
                Select Case vBackgroundRepeat
                    Case Background_Repeat_enum.Repeat_X
                        s.Add("background-repeat:repeat-x")
                    Case Background_Repeat_enum.Repeat_Y
                        s.Add("background-repeat:repeat-y")
                    Case Background_Repeat_enum.Repeat_XY
                        s.Add("background-repeat:repeat-xy")
                End Select
            End If

            If vBackgroundPosition <> Background_Position_enum._Unspecified_ Then
                GotSomething = True
                Select Case vBackgroundPosition
                    Case Background_Position_enum._x_y_percent_specified_
                        s.Add("background-position: " & vBackgroundPositionX.ToString & "% " & vBackgroundPositionY.ToString & "%")
                    Case Background_Position_enum._xpos_ypos_specified_
                        s.Add("background-position: " & vBackgroundPositionX.ToString & "px " & vBackgroundPositionY.ToString & "px")
                    Case Else
                        s.Add("background-position:" & vBackgroundPosition.ToString.Replace("_", " "))
                End Select
            End If

            If Not StringIsNullOrEmpty(vFontFamily) Then
                GotSomething = True
                s.Add("font-family:" & vFontFamily)
            End If

            If vFontSize <> Font_Size_enum._Unspecified_ Then
                If vFontSize = Font_Size_enum.aaa_value_specified_aaa Then
                    If Not StringIsNullOrEmpty(vFontSizeLength) Then
                        GotSomething = True
                        s.Add("font-size:" & vFontSizeLength)
                    End If
                ElseIf vFontSize = Font_Size_enum.aaa_percent_specified_aaa Then
                    If vFontSizePercent > 0 And vFontSizePercent < 101 Then
                        GotSomething = True
                        s.Add("font-size:" & vFontSizePercent.ToString)
                    End If
                Else
                    GotSomething = True
                    s.Add("font-size:" & vFontSize.ToString.Replace("_", "-"))
                End If
            End If

            If vFontStyle <> Font_Style_enum._Unspecified_ Then
                GotSomething = True
                s.Add("font-style:" & vFontStyle.ToString)
            End If

            If vFontVariant <> Font_Variant_enum._Unspecified_ Then
                GotSomething = True
                s.Add("font-variant:" & vFontVariant.ToString.Replace("_", "-"))
            End If

            If vFontWeight <> Font_Weight_enum._Unspecified_ Then
                If vFontWeight = Font_Weight_enum.aaa_100_900_Specified_aaa Then
                    If vFontWeight100900 > 99 And vFontWeight100900 < 901 Then
                        GotSomething = True
                        s.Add("font-weight:" & vFontWeight100900.ToString)
                    End If
                Else
                    GotSomething = True
                    s.Add("font-weight:" & vFontWeight.ToString)
                End If
            End If

            If vTextColor <> ColorName._Unspecified_ Then
                If vTextColor = ColorName._HexCode_Provided_ Then
                    If Not StringIsNullOrEmpty(vTextColorHex) Then
                        GotSomething = True
                        s.Add("color:" & vTextColorHex)
                    End If
                Else
                    GotSomething = True
                    s.Add("color:" & vTextColor.ToString)
                End If
            End If

            If vTextAlign <> Text_Align_enum._Unspecified_ Then
                GotSomething = True
                s.Add("text-align:" & vTextAlign.ToString)
            End If

            If vTextDecoration <> Text_Decoration_enum._Unspecified_ Then
                GotSomething = True
                s.Add("text-decoration:" & vTextDecoration.ToString.Replace("_", "-"))
            End If

            'If vTextIndent <> Text_Indent_enum._Unspecified_ Then

            'End If

            If vTextWhiteSpace <> White_Space_enum._Unspecified_ Then
                GotSomething = True
                s.Add("text-decoration:" & vTextDecoration.ToString.Replace("_", "-"))
            End If

            If vTextWordSpacing <> Word_Spacing_enum._Unspecified_ Then
                If vTextWordSpacing = Word_Spacing_enum.aaa_length_specified_aaa Then

                Else
                    GotSomething = True
                    s.Add("word-spacing:" & vTextWordSpacing.ToString)
                End If
            End If


            If GotSomething Then
                Return "Style='" & Join(s.ToArray, ";") & "'"
            Else
                Return ""
            End If

        End Function

        Private vBackgroundColor As ColorName = ColorName._Unspecified_
        Private vBackgroundColorHex As String = ""
        Public WriteOnly Property Background_Color As ColorName
            Set(ByVal value As ColorName)
                vBackgroundColor = value
            End Set
        End Property
        Public WriteOnly Property Background_Color_Hex As String
            Set(ByVal value As String)
                If StringIsNullOrEmpty(value) Then
                    vBackgroundColorHex = ""
                Else
                    If Not value.Trim.StartsWith("#") Then
                        vBackgroundColorHex = ""
                        Exit Property
                    End If
                    If Not value.Trim.Length = 7 Then
                        vBackgroundColorHex = ""
                        Exit Property
                    End If
                    vBackgroundColorHex = value.Trim
                End If
            End Set
        End Property

        Private vBackgroundImage As String = ""
        Public WriteOnly Property Background_Image As String
            Set(ByVal value As String)
                If StringIsNullOrEmpty(value) Then
                    vBackgroundImage = ""
                Else
                    vBackgroundImage = value.Trim
                End If
            End Set
        End Property

        Private vBackgroundRepeat As Background_Repeat_enum = Background_Repeat_enum._Unspecified_
        Public WriteOnly Property Background_Repeat As Background_Repeat_enum
            Set(ByVal value As Background_Repeat_enum)
                vBackgroundRepeat = value
            End Set
        End Property

        Private vBackgroundPosition As Background_Position_enum = Background_Position_enum._Unspecified_
        Private vBackgroundPositionX As Integer
        Private vBackgroundPositionY As Integer
        Public WriteOnly Property Background_Position As Background_Position_enum
            Set(ByVal value As Background_Position_enum)
                vBackgroundPosition = value
            End Set
        End Property
        Public Sub Background_Position_Percent(ByVal xPercent As Integer, ByVal yPercent As Integer)
            If xPercent < 1 Then Exit Sub
            If xPercent > 100 Then Exit Sub
            If yPercent < 1 Then Exit Sub
            If yPercent > 100 Then Exit Sub
            vBackgroundPositionX = xPercent
            vBackgroundPositionY = yPercent
        End Sub
        Public Sub Background_Position_Pixels(ByVal xPosition As Integer, ByVal yPosition As Integer)
            If xPosition < 0 Then Exit Sub
            If yPosition < 0 Then Exit Sub
            vBackgroundPositionX = xPosition
            vBackgroundPositionY = yPosition
        End Sub

        Private vFontFamily As String = ""
        Public WriteOnly Property Font_Family As String
            Set(ByVal value As String)
                If StringIsNullOrEmpty(value) Then
                    vFontFamily = ""
                Else
                    vFontFamily = value.Trim
                    If vFontFamily.Contains(" ") Then
                        If Not vFontFamily.Contains("'") And Not vFontFamily.Contains("""") Then
                            vFontFamily = """" & vFontFamily & """"
                        ElseIf vFontFamily.Contains("'") Then
                            vFontFamily = vFontFamily.Replace("'", """")
                        End If
                    End If
                End If
            End Set
        End Property

        Private vFontSize As Font_Size_enum = Font_Size_enum._Unspecified_
        Private vFontSizeLength As String = "12px"
        Private vFontSizePercent As Short = 100

        Public WriteOnly Property Font_Size As Font_Size_enum
            Set(ByVal value As Font_Size_enum)
                vFontSize = value
            End Set
        End Property
        Public WriteOnly Property Font_Size_Length As String
            Set(ByVal value As String)
                If StringIsNullOrEmpty(value) Then
                    vFontSizeLength = ""
                Else
                    vFontSizeLength = value.Trim
                End If
            End Set
        End Property
        Public WriteOnly Property Font_Size_Percent As Short
            Set(ByVal value As Short)
                If value < 1 Then Exit Property
                If value > 100 Then Exit Property
                vFontSizePercent = value
            End Set
        End Property

        Private vFontStyle As Font_Style_enum = Font_Style_enum._Unspecified_
        Public WriteOnly Property Font_Style As Font_Style_enum
            Set(ByVal value As Font_Style_enum)
                vFontStyle = value
            End Set
        End Property

        Private vFontVariant As Font_Variant_enum = Font_Variant_enum._Unspecified_
        Public WriteOnly Property Font_Variant As Font_Variant_enum
            Set(ByVal value As Font_Variant_enum)
                vFontVariant = value
            End Set
        End Property

        Private vFontWeight As Font_Weight_enum = Font_Weight_enum._Unspecified_
        Private vFontWeight100900 As Short = 400
        Public WriteOnly Property Font_Weight As Font_Weight_enum
            Set(ByVal value As Font_Weight_enum)
                vFontWeight = value
            End Set
        End Property
        Public WriteOnly Property Font_Weight_Specified As Short
            Set(ByVal value As Short)
                If value < 100 Then Exit Property
                If value > 900 Then Exit Property
                If value Mod 100 <> 0 Then Exit Property
                vFontWeight100900 = value
            End Set
        End Property

        Private vTextColor As ColorName = ColorName._Unspecified_
        Private vTextColorHex As String = ""
        Public WriteOnly Property Text_Color As ColorName
            Set(ByVal value As ColorName)
                vTextColor = value
            End Set
        End Property
        Public WriteOnly Property Text_Color_Hex As String
            Set(ByVal value As String)
                If StringIsNullOrEmpty(value) Then
                    vTextColorHex = ""
                Else
                    If Not value.Trim.StartsWith("#") Then
                        vTextColorHex = ""
                        Exit Property
                    End If
                    If Not value.Trim.Length = 7 Then
                        vTextColorHex = ""
                        Exit Property
                    End If
                    vTextColorHex = value.Trim
                End If
            End Set
        End Property


        Private vTextAlign As Text_Align_enum = Text_Align_enum._Unspecified_
        Public WriteOnly Property Text_Align As Text_Align_enum
            Set(ByVal value As Text_Align_enum)
                vTextAlign = value
            End Set
        End Property

        Private vTextDecoration As Text_Decoration_enum = Text_Decoration_enum._Unspecified_
        Public WriteOnly Property Text_Decoration As Text_Decoration_enum
            Set(ByVal value As Text_Decoration_enum)
                vTextDecoration = value
            End Set
        End Property

        'Private vTextIndent As Text_Indent_enum = Text_Indent_enum._Unspecified_
        'Private vTextIndentLength As String = ""
        'Private vTextIndentPercent As Short = 100
        'Public WriteOnly Property Text_Indent As Text_Indent_enum
        '    Set(ByVal value As Text_Indent_enum)
        '        vTextIndent = value
        '    End Set
        'End Property
        'Public WriteOnly Property Text_Indent_Length As String
        '    Set(ByVal value As String)
        '        If StringIsNullOrEmpty(value) Then
        '            vTextIndentLength = ""
        '        Else
        '            vTextIndentLength = value.Trim
        '        End If
        '    End Set
        'End Property
        'Public WriteOnly Property Text_Indent_Percent As Short
        '    Set(ByVal value As Short)
        '        If value < 1 Then Exit Property
        '        If value > 100 Then Exit Property
        '        vTextIndentPercent = value
        '    End Set
        'End Property

        Private vTextWhiteSpace As White_Space_enum = White_Space_enum._Unspecified_
        Public WriteOnly Property Text_White_Space As White_Space_enum
            Set(ByVal value As White_Space_enum)
                vTextWhiteSpace = value
            End Set
        End Property

        Private vTextWordSpacing As Word_Spacing_enum = Word_Spacing_enum._Unspecified_
        Public WriteOnly Property Text_Word_Spacing As Word_Spacing_enum
            Set(ByVal value As Word_Spacing_enum)
                vTextWordSpacing = value
            End Set
        End Property

        Public Enum ColorName
            _Unspecified_
            _HexCode_Provided_
            AliceBlue
            AntiqueWhite
            Aqua
            Aquamarine
            Azure
            Beige
            Bisque
            Black
            BlanchedAlmond
            Blue
            BlueViolet
            Brown
            BurlyWood
            CadetBlue
            Chartreuse
            Chocolate
            Coral
            CornflowerBlue
            Cornsilk
            Crimson
            Cyan
            DarkBlue
            DarkCyan
            DarkGoldenRod
            DarkGray
            DarkGrey
            DarkGreen
            DarkKhaki
            DarkMagenta
            DarkOliveGreen
            Darkorange
            DarkOrchid
            DarkRed
            DarkSalmon
            DarkSeaGreen
            DarkSlateBlue
            DarkSlateGray
            DarkSlateGrey
            DarkTurquoise
            DarkViolet
            DeepPink
            DeepSkyBlue
            DimGray
            DimGrey
            DodgerBlue
            FireBrick
            FloralWhite
            ForestGreen
            Fuchsia
            Gainsboro
            GhostWhite
            Gold
            GoldenRod
            Gray
            Grey
            Green
            GreenYellow
            HoneyDew
            HotPink
            IndianRed
            Indigo
            Ivory
            Khaki
            Lavender
            LavenderBlush
            LawnGreen
            LemonChiffon
            LightBlue
            LightCoral
            LightCyan
            LightGoldenRodYellow
            LightGray
            LightGrey
            LightGreen
            LightPink
            LightSalmon
            LightSeaGreen
            LightSkyBlue
            LightSlateGray
            LightSlateGrey
            LightSteelBlue
            LightYellow
            Lime
            LimeGreen
            Linen
            Magenta
            Maroon
            MediumAquaMarine
            MediumBlue
            MediumOrchid
            MediumPurple
            MediumSeaGreen
            MediumSlateBlue
            MediumSpringGreen
            MediumTurquoise
            MediumVioletRed
            MidnightBlue
            MintCream
            MistyRose
            Moccasin
            NavajoWhite
            Navy
            OldLace
            Olive
            OliveDrab
            Orange
            OrangeRed
            Orchid
            PaleGoldenRod
            PaleGreen
            PaleTurquoise
            PaleVioletRed
            PapayaWhip
            PeachPuff
            Peru
            Pink
            Plum
            PowderBlue
            Purple
            Red
            RosyBrown
            RoyalBlue
            SaddleBrown
            Salmon
            SandyBrown
            SeaGreen
            SeaShell
            Sienna
            Silver
            SkyBlue
            SlateBlue
            SlateGray
            SlateGrey
            Snow
            SpringGreen
            SteelBlue
            Tan
            Teal
            Thistle
            Tomato
            Turquoise
            Violet
            Wheat
            White
            WhiteSmoke
            Yellow
            YellowGreen
        End Enum

        Public Enum Background_Repeat_enum
            _Unspecified_
            Repeat_X
            Repeat_Y
            Repeat_XY
        End Enum

        Public Enum Background_Position_enum
            _Unspecified_
            _x_y_percent_specified_
            _xpos_ypos_specified_
            inherit
            left
            top
            left_center
            left_bottom
            right_top
            right_center
            right_bottom
            center_top
            center_center
            center_bottom
        End Enum

        Public Enum Font_Size_enum
            _Unspecified_
            aaa_value_specified_aaa
            xx_small
            small
            medium
            large
            x_large
            xx_large
            smaller
            larger
            aaa_percent_specified_aaa
            inherit
        End Enum

        Public Enum Font_Style_enum
            _Unspecified_
            normal
            italic
            oblique
            inherit
        End Enum

        Public Enum Font_Variant_enum
            _Unspecified_
            normal
            small_caps
            inherit
        End Enum

        Public Enum Font_Weight_enum
            _Unspecified_
            normal
            bold
            bolder
            lighter
            aaa_100_900_Specified_aaa
        End Enum

        Public Enum Text_Align_enum
            _Unspecified_
            left
            right
            center
            justify
            inherit
        End Enum

        Public Enum Text_Decoration_enum
            _Unspecified_
            none
            underline
            overline
            line_through
            blink
            inherit
        End Enum

        Public Enum Text_Indent_enum
            _Unspecified_
            aaa_length_specified_aaa
            aaa_percent_specified_aaa
            inherit
        End Enum

        Public Enum White_Space_enum
            _Unspecified_
            normal
            nowrap
            pre
            pre_line
            pre_wrap
            inherit
        End Enum

        Public Enum Word_Spacing_enum
            _Unspecified_
            normal
            aaa_length_specified_aaa
            inherit
        End Enum

    End Class


    Public Function HTML_WrapSpan(ByVal id As String, ByVal wraptext As String, Optional ByVal bdisplay As Boolean = False, _
       Optional ByVal WithTable As Boolean = False) As String
        Dim st As New StringBuilder
        On Error Resume Next

        If WithTable Then
            st.Append("<td>" & vbCrLf)
        End If
        If bdisplay Then
            st.Append("<span  id=""" & id & """>" & vbCrLf)
        Else
            st.Append("<span style=""display:none;"" id=""" & id & """>" & vbCrLf)
        End If
        st.Append(wraptext)
        st.Append("</span>" & vbCrLf)
        If WithTable Then
            st.Append("</td>")
        End If

        Return st.ToString

    End Function

    Public Function FormConfButton(ByRef Name As String, ByRef id As String, ByRef Source As String, _
     ByRef alt_title As String, Optional ByVal sel As Boolean = False, _
     Optional ByVal bHeight As Short = 33, Optional ByVal bWidth As Short = 100) As String
        Dim bup As New StringBuilder

        bup.Append("<input type=""image"" name=""")
        bup.Append(Name)
        bup.Append(""" border=""0"" id=""")
        bup.Append(id)
        bup.Append(""" src=""")
        If sel Then
            bup.Append(Source & "s.gif")
        Else
            bup.Append(Source & "n.gif")
        End If
        bup.Append(""" height=""" & bHeight.ToString & """ width=""" & bWidth.ToString & """ alt=""")
        bup.Append(alt_title)
        bup.Append(""" fp-style=""fp-btn: Glass Tab 1; fp-font-color-hover: #0000FF; ")
        bup.Append("fp-font-color-press: #FF0000; fp-transparent: 1"" fp-title=""")
        bup.Append(alt_title)
        bup.Append("" & vbCrLf)
        bup.Append("onmouseover = ""swapImg(1,0,/*id*/'" & id & "',/*url*/'" & Source & "h.gif')""" & vbCrLf)
        If sel Then
            bup.Append("onmouseout = ""swapImg(0,0,/*id*/'" & id & "',/*url*/'" & Source & "s.gif')""" & vbCrLf)
        Else
            bup.Append("onmouseout = ""swapImg(0,0,/*id*/'" & id & "',/*url*/'" & Source & "n.gif')""" & vbCrLf)
        End If
        If sel Then
            bup.Append("onmousedown = ""swapImg(1,0,/*id*/'" & id & "',/*url*/'" & Source & "n.gif')""" & vbCrLf)
        Else
            bup.Append("onmousedown = ""swapImg(1,0,/*id*/'" & id & "',/*url*/'" & Source & "s.gif')""" & vbCrLf)
        End If
        bup.Append("onmouseup=""swapImg(0,0,/*id*/'" & id & "',/*url*/'" & Source & "h.gif')"">" & vbCrLf)

        FormConfButton = bup.ToString

    End Function

#End Region

    


End Module
