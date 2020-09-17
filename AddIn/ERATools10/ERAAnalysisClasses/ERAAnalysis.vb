Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry

''' <summary>
''' Abstract class from which all ERAtools analysis classes inherit.
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public MustInherit Class ERAAnalysis
    Inherits EraLayerTable

    Private m_FC_SpatRef As ISpatialReference

    ''' <summary>
    ''' Private pointer to the user-digitized or selected point, line or polygon
    ''' </summary>
    Private m_AnalysisShape As IGeometry

    ''' <summary>
    ''' Private pointer to the optional buffer of user-specified size around the AnalysisShape, as a "donut" that excludes the AnalysisShape if the AnalysisShape is a polygon
    ''' </summary>
    Private m_BufferShape As IPolygon


    ''' <summary>
    ''' Private pointer to a union ot the analysis and buffer shapes. If the AnalysisShape is a polygon, and a buffer has been specified, then this is the union of the AnalysisShape and BufferShape. If the AnalysisShape is a point or line, then this is equivalent to the BufferShape. If it is a point or a line and no buffer was supplied, this will be equivalent to just the analysis shape
    ''' </summary>
    Private m_AnalysisPlusBufferShape As IGeometry

    ''' <summary>
    ''' Gets or sets the analysis shape (the user-digitized or selected point, line or polygon).
    ''' </summary>
    ''' <value>The analysis shape (the user-digitized or selected point, line or polygon).</value>
    Public Property AnalysisShape() As IGeometry
        Get
            Return m_AnalysisShape
        End Get
        Set(ByVal value As IGeometry)
            If value Is Nothing Then
                m_AnalysisShape = Nothing
            Else
                m_AnalysisShape = DirectCast(value, ESRI.ArcGIS.esriSystem.IClone).Clone
                If m_FC_SpatRef IsNot Nothing Then m_AnalysisShape.Project(m_FC_SpatRef)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the optional buffer of user-specified size around the AnalysisShape, as a "donut" that excludes the AnalysisShape if the AnalysisShape is a polygon.
    ''' </summary>
    ''' <value>The buffer shape.</value>
    Public Property BufferShape() As IGeometry
        Get
            Return m_BufferShape
        End Get
        Set(ByVal value As IGeometry)
            If value Is Nothing Then
                m_BufferShape = Nothing
            Else
                m_BufferShape = DirectCast(value, ESRI.ArcGIS.esriSystem.IClone).Clone
                If m_FC_SpatRef IsNot Nothing Then m_BufferShape.Project(m_FC_SpatRef)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the pointer to a union ot the analysis and buffer shapes. If the AnalysisShape is a polygon, and a buffer has been specified, then this is the union of the AnalysisShape and BufferShape. If the AnalysisShape is a point or line, then this is equivalent to the BufferShape. If it is a point or a line and no buffer was supplied, this will be equivalent to just the analysis shape
    ''' </summary>
    ''' <value>The analysis plus buffer shape.</value>
    Public Property AnalysisPlusBufferShape() As IGeometry
        Get
            Return m_AnalysisPlusBufferShape
        End Get
        Set(ByVal value As IGeometry)
            If value Is Nothing Then
                m_AnalysisPlusBufferShape = Nothing
            Else
                m_AnalysisPlusBufferShape = DirectCast(value, ESRI.ArcGIS.esriSystem.IClone).Clone
                If m_FC_SpatRef IsNot Nothing Then m_AnalysisPlusBufferShape.Project(m_FC_SpatRef)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets the type of the analysis.
    ''' </summary>
    ''' <value>The type of the analysis.</value>
    Public ReadOnly Property AnalysisType() As String
        Get
            Return GetAttributeValue("analysistype", True)
        End Get
    End Property


    ''' <summary>
    ''' Sets the properties of an instance of an analysis class based on the XML element passed by reference.
    ''' </summary>
    ''' <param name="EraXmlElement">The XML element containing all of the properties to be used when creating an instance of this class.</param>
    Public Overrides Sub SetXmlNode(ByRef EraXmlElement As Xml.XmlElement)
        Try
            m_XmlElement = EraXmlElement
            Me.Name = GetAttributeValue("name", True)
            Dim pGeoDataSet As IGeoDataset = FeatureClass
            m_FC_SpatRef = pGeoDataSet.SpatialReference

            OpenTablesAndFields()
            m_SortFieldsNode = EraXmlElement.SelectSingleNode("SORT_FIELDS")
        Catch ex As EraException
            AddError(ex, "ERAAnalysis.SetXmlNode")
            Logging.Log.Error("XML layer tag resulting in error: " & m_XmlElement.OuterXml)
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Writes the XML results.
    ''' </summary>
    Public MustOverride Sub WriteXmlResults()

    ''' <summary>
    ''' Creates the LAYER XML element and prepares an instance of an inherited class for writing XML results.   This Sub MUST be called as the first line of WriteXmlResults in inherited classes,
    ''' otherwise m_outputXmlDoc and m_OutputXmlElement, needed for WriteXmlResults, will be nothing
    ''' </summary>
    ''' <param name="ParentNode">A pointer to the parent ISSUE XML element.</param>
    Protected Friend Sub WriteXmlHeader(ByRef ParentNode As Xml.XmlElement)
        m_OutputXmlDoc = ParentNode.OwnerDocument
        m_OutputXmlElement = m_OutputXmlDoc.CreateElement("LAYER", "")

        ParentNode.AppendChild(m_OutputXmlElement)

        'Add name and type to RootNode
        m_OutputXmlElement.SetAttribute("name", Me.Name)
        m_OutputXmlElement.SetAttribute("type", Me.AnalysisType)
    End Sub

    ''' <summary>
    ''' Gets the name of the tag.
    ''' </summary>
    ''' <value>The name of the tag.</value>
    Public Overrides ReadOnly Property TagName() As String
        Get
            Return "LAYER"
        End Get
    End Property
End Class