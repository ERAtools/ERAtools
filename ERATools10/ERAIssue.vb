''' <summary>
''' Represents an ERAtools Issue XML element.
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class EraIssue
    Inherits EraNode

    ''' <summary>
    ''' Private collection (generic list) of EraAnalysis instances listed as children of this issue.
    ''' </summary>
    Private m_colAnalysis As New Generic.List(Of ERAAnalysis)

    ''' <summary>
    ''' Gets the value for the meta attribute of the ISSUE element passed to SetXmlNode. The meta attribute is used only for documentation of the issue, and is not used in processing the report.
    ''' </summary>
    ''' <value>The value for the meta attribute.</value>
    Public ReadOnly Property Meta() As String
        Get
            Return GetAttributeValue("meta", False)
        End Get
    End Property


    ''' <summary>
    ''' Sets the properties of this instance of an EraIssue node based on the XML element passed by reference.
    ''' </summary>
    ''' <param name="EraXmlElement">The XML element containing all of the properties to be used when creating an instance of this class.</param>
    Public Overrides Sub SetXmlNode(ByRef EraXmlElement As System.Xml.XmlElement)
        Dim attrib As Xml.XmlAttribute
        m_XmlElement = EraXmlElement
        Me.Name = GetAttributeValue("name", True)
        'For each child LAYER XML element, create instances of an appropriate EraAnalysis class.
        For Each LayerNode As Xml.XmlElement In EraXmlElement.SelectNodes("LAYER")
            Dim newAnalysis As ERAAnalysis
            attrib = LayerNode.GetAttributeNode("analysistype")
            If attrib Is Nothing Then
                Throw New EraException("The analysistype attribute was not set for layer tag with name=""" & LayerNode.GetAttribute("name") & """.")
            End If
            Select Case attrib.Value
                Case "weighted average"
                    newAnalysis = New WeightedAverageAnalysis
                Case "acreage" 'AKA Feature Comparison Analysis
                    newAnalysis = New FeatureComparisonAnalysis
                Case "distance"
                    newAnalysis = New DistanceAnalysis
                Case "distance and direction"
                    newAnalysis = New DistanceAndDirectionAnalysis
                Case "comparison"
                    newAnalysis = New ComparisonResourceAnalysis
                Case "buffer"
                    newAnalysis = New BufferAnalysis
                Case "pinpoint"
                    newAnalysis = New PinpointAnalysis
                Case Else
                    Throw New EraException("An unrecognized analysistype value (" & attrib.Value & ") was set for layer tag with name=""" & LayerNode.GetAttribute("name") & """.")
            End Select
            newAnalysis.SetXmlNode(LayerNode)
            m_colAnalysis.Add(newAnalysis)
        Next LayerNode
    End Sub


    ''' <summary>
    ''' Recursively calls all analyses for this issue, and appends the results to a new ISSUE element to be created by this sub and appended to the parent EVALUATION element passed by reference.
    ''' </summary>
    ''' <param name="ParentNode">A pointer to the parent EraEvaluation element to which the ISSUE element and child elements will be appended.</param>
    Public Sub WriteIssueXmlResults(ByRef ParentNode As Xml.XmlNode)
        Dim ThisNode As Xml.XmlElement
        Try
            ThisNode = ParentNode.OwnerDocument.CreateElement("ISSUE", "")
            ThisNode.SetAttribute("name", Me.Name)
            If Me.Meta.Length > 0 Then
                ThisNode.SetAttribute("description", Me.Meta)
            End If
            Logging.Log.Debug(m_colAnalysis.Count & " analyses to process")
            For Each newAnalysis As ERAAnalysis In m_colAnalysis
                newAnalysis.AnalysisShape = EraListener.Current.AnalysisShape
                newAnalysis.BufferShape = EraListener.Current.BufferShape
                newAnalysis.AnalysisPlusBufferShape = EraListener.Current.AnalysisPlusBufferShape
                newAnalysis.WriteXmlHeader(ThisNode)
                newAnalysis.WriteXmlResults()
            Next newAnalysis
            ParentNode.AppendChild(ThisNode)
        Catch ex As Exception
            AddError(ex, "ERAIssue.WriteIssueXmlResults")
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Gets the name of the tag.
    ''' </summary>
    ''' <value>The name of the tag.</value>
    Public Overrides ReadOnly Property TagName() As String
        Get
            Return "ISSUE"
        End Get
    End Property
End Class