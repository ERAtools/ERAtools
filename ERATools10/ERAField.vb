Imports ESRI.ArcGIS.Geodatabase
Imports System.Reflection
Imports System.CodeDom.Compiler
Imports System.Text.RegularExpressions
Imports System.Math
Imports System.Web.Script.Serialization
Imports System.Collections.Generic

''' <summary>
''' Handles options set at the field level, returns data for individual fields for individual records.
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class EraField
    Inherits EraNode

    Private m_ParentLayer As EraLayerTable

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ERAField" /> class.
    ''' </summary>
    ''' <param name="ParentLayer">The parent layer.</param>
    Public Sub New(ByVal ParentLayer As EraLayerTable)
        m_ParentLayer = ParentLayer
    End Sub

    ''' <summary>
    ''' Gets or sets the field alias or "user-friendly" name of a field.
    ''' </summary>
    ''' <value>The field alias, or "user-friendly" name.</value>
    Public Property FieldAlias As String
        Get
            Dim fa As String = GetAttributeValue("alias", False)
            If fa.Length < 1 Then
                fa = Me.Name
            End If
            Return fa
        End Get
        Set(ByVal value As String)
            If Me.m_XmlElement Is Nothing Then
                Me.m_XmlElement = m_ParentLayer.m_OutputXmlDoc.CreateElement("FIELD")
                Me.m_XmlElement.SetAttribute("name", Me.Name)
            End If
            Me.m_XmlElement.SetAttribute("alias", value)
        End Set
    End Property

    ''' <summary>
    ''' Gets a value indicating whether this <see cref="ERAField" /> is link.
    ''' </summary>
    ''' <value><c>true</c> if link; otherwise, <c>false</c>.</value>
    Public ReadOnly Property Link As Boolean
        Get
            Return GetAttributeValue("link", False).Equals("true", StringComparison.CurrentCultureIgnoreCase)
        End Get
    End Property


    ''' <summary>
    ''' Gets the name of another field to use for the generating the URL (whereas the value for the current field is only used for the content of the link).
    ''' </summary>
    ''' <value>The name of another field which will be used to generate a URL.</value>
    Public ReadOnly Property UseForURL As String
        Get
            Return GetAttributeValue("useforurl", False)
            'Return ParseFieldName(GetAttributeValue("useforurl", False))
        End Get
    End Property

    ''' <summary>
    ''' Gets the prefix used in creating URLs. Only applicable if Link is true.
    ''' </summary>
    ''' <value>The prefix used in creating a URL.</value>
    Public ReadOnly Property Prefix As String
        Get
            Return GetAttributeValue("prefix", False)
        End Get
    End Property

    ''' <summary>
    ''' Gets the suffix used in creating URLs. Only applicable if Link is true.
    ''' </summary>
    ''' <value>The suffix used in creating a URL.</value>
    Public ReadOnly Property Suffix As String
        Get
            Return GetAttributeValue("suffix", False)
        End Get
    End Property

    ''' <summary>
    ''' Gets the summary function or GroupBy tag.
    ''' </summary>
    ''' <value>The summary function or GroupBy tag for the field.</value>
    Public ReadOnly Property Summary As String
        Get
            Return Nz(GetAttributeValue("summary", False), "GroupBy")
        End Get
    End Property

    Public ReadOnly Property Format As String
        Get
            Return Nz(GetAttributeValue("format", False), "")
        End Get
    End Property

    ''' <summary>
    ''' Sets the properties of an instance of ERAfield based on the XML element passed by reference.
    ''' </summary>
    ''' <param name="EraXmlElement">The XML element containing all of the properties to be used when creating an instance of this class.</param>
    Public Overrides Sub SetXmlNode(ByRef EraXmlElement As System.Xml.XmlElement)
        m_XmlElement = EraXmlElement
        Me.Name = GetAttributeValue("name", True)
        'If Me.m_ParentLayer.Table.FindField(Me.Name) = -1 Then
        '    Throw New EraException("The field " & Me.Name & " does not belong to the " & Me.m_ParentLayer.TagName.ToLower & " " & Me.m_ParentLayer.LayerID)
        'End If
    End Sub

    ''' <summary>
    ''' Appends the field output element to the referenced ParentNode element. The field output element becomes the header for an output data column.
    ''' </summary>
    ''' <param name="ParentNode">The parent FIELDS node to which this FIELD element will be added.</param>
    Public Sub XmlFieldResults(ByRef ParentNode As Xml.XmlElement)
        Try
            Dim ThisNode As Xml.XmlElement = ParentNode.OwnerDocument.CreateElement("FIELD")
            ThisNode.SetAttribute("name", Me.ElementName)
            ThisNode.SetAttribute("alias", Me.FieldAlias)
            ParentNode.AppendChild(ThisNode)
        Catch ex As Exception
            AddError(ex, "ERAField.XmlFieldResults")
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Appends the value output element for this field to the referenced ParentNode element. 
    ''' </summary>
    ''' <param name="ParentNode">The parent XML element to which the results element will be appended.</param>
    ''' <param name="pRow">The pointer to an Irow that contains the data.</param>
    Public Sub XmlRecordResults(ByRef ParentNode As Xml.XmlElement, ByRef pRow As ESRI.ArcGIS.Geodatabase.IRow)
        Try
            'Dim idxField As Integer

            Dim ThisNode As Xml.XmlElement = ParentNode.OwnerDocument.CreateElement(Me.ElementName)

            If Link Then
                ThisNode.SetAttribute("link", "true")
                If Me.UseForURL <> "" Then
                    'idxField = pRow.Fields.FindField(Me.UseForURL)
                    If FieldIndex(Me.UseForURL) >= 0 Then
                        'todo: with this change, links to coded values will no longer be used. the name property is used instead. need to document this
                        'ThisNode.SetAttribute("useforurl", Me.Prefix & Trim(Nz(pRow.Value(idxField))) & Me.Suffix)
                        ThisNode.SetAttribute("useforurl", Me.Prefix & Me.GetFieldValue(pRow, Me.UseForURL) & Me.Suffix)
                    Else

                        Throw New EraException("The 'useforurl' field was not found")
                        'idxField = pRow.Fields.FindField(FieldName)
                        'If idxField >= 0 Then
                        '    ThisNode.SetAttribute("useforurl", Me.Prefix & Trim(Nz(pRow.Value(idxField))) & Me.Suffix)
                        'End If
                    End If
                Else
                    'idxField = pRow.Fields.FindField(FieldName)
                    If Me.FieldIndex >= 0 Then
                        'ThisNode.SetAttribute("useforurl", Me.Prefix & Trim(Nz(pRow.Value(idxField))) & Me.Suffix)
                        ThisNode.SetAttribute("useforurl", Me.Prefix & Me.GetFieldValue(pRow) & Me.Suffix)
                    End If
                End If
            End If

            ThisNode.InnerText = GetFieldValue(pRow)

            ParentNode.AppendChild(ThisNode)
        Catch ex As EraException
            AddError(ex, "ERAField.XmlRecordResults")
            Throw
        Catch ex As Exception
            AddError(ex, "ERAField.XmlRecordResults")
            Throw
        End Try
    End Sub

    Public Function FieldIndex(Optional ByVal ForFieldName As String = "") As Integer
        If ForFieldName = "" Then ForFieldName = FieldName
        Return Me.m_ParentLayer.Table.Fields.FindField(ForFieldName)
        'Dim idxField As Integer = Me.m_ParentLayer.Table.Fields.FindField(ForFieldName)
        'If idxField < 0 Then
        '    'Throw New EraException("The field " & ForFieldName & " does not exist in the dataset.")

        'Else
        '    Return idxField
        'End If
    End Function

    Public Function pField(Optional ByVal ForFieldName As String = "") As IField
        Try
            If FieldIndex(ForFieldName) >= 0 Then
                Return Me.m_ParentLayer.Table.Fields.Field(FieldIndex(ForFieldName))
            Else
                Return Nothing
            End If
        Catch ex As EraException
            Throw
        End Try
    End Function

    Public Function GetCodedValueDomain(Optional ByVal ForFieldName As String = "") As ICodedValueDomain
        Dim m_pField As IField = pField(ForFieldName)
        If m_pField Is Nothing Then Return Nothing
        Dim pDomain As IDomain = m_pField.Domain
        If pDomain IsNot Nothing Then
            If pDomain.Type = esriDomainType.esriDTCodedValue Then
                Return pDomain
            End If
        End If
        Return Nothing
    End Function

    Public Function GetFieldValue(ByVal pRow As IRow, Optional ByVal ForFieldName As String = "") As String
        If ForFieldName = "" Then ForFieldName = FieldName

        Dim objValue As Object
        Dim tempFieldIndex As Integer = Me.m_ParentLayer.Table.Fields.FindField(ForFieldName)
        'We're not calling the FieldIndex function because we don't want it to throw an exception yet

        If tempFieldIndex >= 0 Then
            objValue = pRow.Value(tempFieldIndex)
            Dim pCodedValueDomain As ICodedValueDomain = GetCodedValueDomain(ForFieldName)
            If Nz(objValue, "<nulltest>") <> "<nulltest>" Then
                If pCodedValueDomain IsNot Nothing Then
                    For i As Integer = 0 To pCodedValueDomain.CodeCount - 1
                        If pCodedValueDomain.Value(i) = objValue Then
                            objValue = pCodedValueDomain.Name(i)
                            Exit For
                        End If
                    Next
                End If
            End If
        Else
            'calculated field? look for bracketed field names and replace as need be, then evaluate
            Dim matches As MatchCollection = Regex.Matches(ForFieldName, "\[(.+?)\]")
            If matches.Count = 0 Then
                Throw New Exception("The field '" & ForFieldName & "' does not exist in the dataset.")
            End If
            For i As Integer = 0 To matches.Count - 1
                Dim objValueSub As Object = GetFieldValue(pRow, matches(i).Groups(1).Value)
                ForFieldName = ForFieldName.Replace(matches(i).Value, objValueSub)
            Next i
            objValue = Evaluate(ForFieldName)

        End If

        If Format <> "" Then
            If Format.StartsWith("replace:", StringComparison.CurrentCultureIgnoreCase) Then
                'Custom string replacement. Format string can contain any number of find/replace pairs
                'as an array in JSON notation, e.g. replace:[{'0':'In'},{'1':'Out'}]
                Dim js As New JavaScriptSerializer
                Dim dict As Dictionary(Of String, String) = js.Deserialize(Of Dictionary(Of String, String))(Format.Replace("replace:", ""))
                Dim strValue As String = objValue
                For Each k As String In dict.Keys
                    strValue = strValue.Replace(k, dict(k))
                Next
                Return strValue
            Else
                Dim strValue As String = String.Format("{0:" & Format & "}", objValue)
                'Quarter format is not supported by format string natively, and will be copied to the result string directly
                If strValue.Contains("u") And IsDate(objValue) Then
                    'calculate the quarter via the month
                    Dim d As Date = CDate(objValue)
                    Dim m As Integer = d.Month
                    Dim q As Integer = 4
                    Dim incrementYear As Boolean = False
                    If m >= 10 Then
                        q = 1
                        incrementYear = True ' Fiscal year 2012 starts in October 2011, so increment the year
                    ElseIf m < 4 Then
                        q = 2
                    ElseIf m < 7 Then
                        q = 3
                    End If
                    strValue = strValue.Replace("u", "Q" & q)
                    If incrementYear Then
                        If Format.Contains("yyyy") Then
                            strValue = strValue.Replace(d.Year, d.Year + 1)
                        ElseIf Format.Contains("yy") Then
                            ''just replace last two digits of year
                            strValue = strValue.Replace(CStr(d.Year).Substring(2), CInt(CStr(d.Year).Substring(2) + 1))
                        End If
                    End If
                ElseIf strValue.Contains("q") And IsDate(objValue) Then
                    'calculate the quarter via the month
                    Dim m As Integer = CDate(objValue).Month
                    Dim q As Integer = 4
                    If m < 4 Then
                        q = 1
                    ElseIf m < 7 Then
                        q = 2
                    ElseIf m < 10 Then
                        q = 3
                    End If
                    strValue = strValue.Replace("q", "Q" & q)
                End If
                Return strValue
            End If
        End If
        Return Trim(Nz(objValue))
    End Function


    Function Evaluate(ByVal Expression As String) As Object
        If Expression.Length > 0 Then
            'Replace each parameter in the calculation expression with the correct values
            'Dim MatchStr As String = "{(\d+)}"
            'Dim oMatches As MatchCollection = Regex.Matches(Expression, MatchStr)
            'If oMatches.Count > 0 Then
            '    Dim DistinctCount As Integer = (From m In oMatches _
            '                         Select m.Value).Distinct.Count
            '    If DistinctCount = Args.Length Then
            '        For i As Integer = 0 To Args.Length - 1
            '            Expression = Expression.Replace("{" & i & "}", Args(i))
            '        Next
            '    Else
            '        Throw New ArgumentException("Invalid number of parameters passed")
            '    End If
            'End If

            Dim FuncName As String = "Eval" & Guid.NewGuid.ToString("N")
            Dim FuncString As String = "Imports System.Math" & vbCrLf & _
                                       "Namespace EvaluatorLibrary" & vbCrLf & _
                                       "  Class Evaluators" & vbCrLf & _
                                       "    Public Shared Function " & FuncName & "() As Double" & vbCrLf & _
                                       "      Return " & Expression & vbCrLf & _
                                       "    End Function" & vbCrLf & _
                                       "  End Class" & vbCrLf & _
                                       "End Namespace"

            'Tell the compiler what language was used
            Dim CodeProvider As CodeDomProvider = CodeDomProvider.CreateProvider("VB")

            'Set up our compiler options...
            Dim CompilerOptions As New CompilerParameters()
            With CompilerOptions
                .ReferencedAssemblies.Add("System.dll")
                .GenerateInMemory = True
                .TreatWarningsAsErrors = True
            End With

            'Compile the code that is to be evaluated
            Dim Results As CompilerResults = _
                CodeProvider.CompileAssemblyFromSource(CompilerOptions, FuncString)

            'Check there were no errors...
            If Results.Errors.Count > 0 Then
            Else
                'Run the code and return the value...
                Dim dynamicType As Type = Results.CompiledAssembly.GetType("EvaluatorLibrary.Evaluators")
                Dim methodInfo As MethodInfo = dynamicType.GetMethod(FuncName)
                Return methodInfo.Invoke(Nothing, Nothing)
            End If
        End If
        Return 0
    End Function

    ''' <summary>
    ''' Replaces the record results with data from a child table. Used only for generating output from in-line tables with summarizations.
    ''' </summary>
    ''' <param name="ThisNode">A reference to the XML element that will be passed the data.</param>
    ''' <param name="pRow">The pointer to an Irow that contains the data.</param>
    Public Sub ReplaceXmlRecordResults(ByRef ThisNode As Xml.XmlElement, ByRef pRow As ESRI.ArcGIS.Geodatabase.IRow)
        Try
            Dim idxField As Integer

            If Link Then
                ThisNode.SetAttribute("link", "true")
            Else
                ThisNode.SetAttribute("link", "false")
            End If

            If Me.Link = True Then
                If Me.UseForURL <> "" Then
                    idxField = pRow.Fields.FindField(Me.UseForURL)
                    If idxField >= 0 Then
                        ThisNode.SetAttribute("useforurl", Me.Prefix & Trim(Nz(pRow.Value(idxField))) & Me.Suffix)
                    Else 'UseForURL failed, so default to the Field Name
                        idxField = pRow.Fields.FindField(FieldName)
                        If idxField >= 0 Then
                            ThisNode.SetAttribute("useforurl", Me.Prefix & Trim(Nz(pRow.Value(idxField))) & Me.Suffix)
                        End If
                    End If
                Else
                    idxField = pRow.Fields.FindField(FieldName)
                    If idxField >= 0 Then
                        ThisNode.SetAttribute("useforurl", Me.Prefix & Trim(Nz(pRow.Value(idxField))) & Me.Suffix)
                    End If
                End If
            End If

            idxField = pRow.Fields.FindField(FieldName)
            If idxField < 0 Then
                Throw New EraException("The field " & Me.Name & " does not exist in the dataset.")
            Else
                ThisNode.InnerText = Trim(Nz(pRow.Value(idxField)))
            End If

        Catch ex As Exception
            AddError(ex, "ERAField.ReplaceXmlRecordResults")
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Gets the name of the element.
    ''' </summary>
    ''' <value>The name of the element.</value>
    Public ReadOnly Property ElementName As String
        Get
            If Summary.Equals("GroupBy", StringComparison.CurrentCultureIgnoreCase) Then
                Return Xml.XmlConvert.EncodeLocalName(m_ParentLayer.Name & "_" & FieldName).Replace(".", "_")
            Else
                Return Xml.XmlConvert.EncodeLocalName(m_ParentLayer.Name & "_" & Summary & "_" & FieldName).Replace(".", "_")
            End If
        End Get
    End Property

    ''' <summary>
    ''' Gets the name of the field as it exists in the underlying data.
    ''' The name frequently includes the layer name (layername.fieldname).
    ''' </summary>
    ''' <value>The name of the field.</value>
    Public ReadOnly Property FieldName As String
        Get
            Return Me.Name
            'Return ParseFieldName(Me.Name)
        End Get
    End Property

    ''The ParseFieldName function was causing problems with SDE data, and it doesn't appear to be necessary
    '''' <summary>
    '''' Parses the name of the field by splitting on "." and returing the last element in the list
    '''' </summary>
    '''' <param name="strFieldname">The name of the field, excluding the table name.</param>
    '''' <returns></returns>
    'Private Function ParseFieldName(ByVal strFieldname As String) As String
    '    'Split strFieldName on . and return the last element
    '    'Dim arParts() As String = Split(strFieldname, ".")
    '    'ParseFieldName = arParts(UBound(arParts))

    '    'Simpler method
    '    Return strFieldname.Substring(strFieldname.LastIndexOf(".") + 1)
    'End Function

    ''' <summary>
    ''' Gets the name of the tag.
    ''' </summary>
    ''' <value>The name of the tag. Always returns the value "FIELD".</value>
    Public Overrides ReadOnly Property TagName As String
        Get
            Return "FIELD"
        End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether this instance is flagged for a weighted average analysis on indexed data or raw data.
    ''' </summary>
    ''' <value>
    ''' <c>true</c> if this instance is flagged for weighted average using indexed data; otherwise, <c>false</c>.
    ''' </value>
    Public ReadOnly Property isIndexed As Boolean
        Get
            Return Not Me.GetAttributeValue("wavg_type", False).Equals("rawdata", StringComparison.CurrentCultureIgnoreCase)
        End Get
    End Property
End Class
