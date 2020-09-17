Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports System.Runtime.InteropServices

''' <summary>
''' In user documentation, this is referred to as a Generic Resource Analysis. 
''' The Generic Resource analysis lists attributes of resource features that 
''' intersect the user-specified analysis feature. If a buffer is used during 
''' the analysis run, attributes for resource features intersecting the buffer 
''' boundary will be listed separately. 
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class BufferAnalysis
    Inherits ERAAnalysis


    ''' <summary>
    ''' Writes the XML results.
    ''' </summary>
    Public Overrides Sub WriteXmlResults()
        WriteResultsSub(AnalysisShape)

        If BufferShape IsNot Nothing AndAlso Not BufferShape.IsEmpty Then
            WriteXmlHeader(m_OutputXmlElement.ParentNode)
            m_OutputXmlElement.SetAttribute("name", Me.Name & " (within buffer)")
            WriteResultsSub(BufferShape)
        End If
    End Sub

    ''' <summary>
    ''' Does the actual work of writing the XML Results. This may be called twice for each call to WriteXmlResults, once for the AnalysisShape and again for the BufferShape, if provided.
    ''' </summary>
    ''' <param name="FilterShape">The shape to use for writing results. Will be the AnalysisShape on the first call, and if BufferShape is provided, will be BufferShape on the second call.</param>
    Private Sub WriteResultsSub(ByRef FilterShape As IGeometry)
        Try
            'Initialize Counter and get first record
            Dim cntRow As Integer = 1
            Dim pFeatureCursor As IFeatureCursor = OpenFeatureCursor(FilterShape)

            'Check to see if this is a Summarization Analysis
            If Me.isSummarization Then
                Dim SumProcess As New SummarizationProcess(Me)
                SumProcess.ProcessSummarization(pFeatureCursor)
                Logging.Log.Debug("Releasing cursor for " & Me.Name)
                Marshal.ReleaseComObject(pFeatureCursor)
                Exit Sub
            End If

            Dim pRow As IFeature = pFeatureCursor.NextFeature
            If pRow Is Nothing Then
                WriteNoRecordsNode()
            Else
                'Add FIELDS and FIELD elements
                WriteFieldsNode()

                'Add RECORD elements and <field name> elements
                While pRow IsNot Nothing
                    If HasInnerJoins Then
                        For Each childtable As EraTable In m_ColTables
                            If Not childtable.HasMatchingRecords(pRow) Then
                                pRow = pFeatureCursor.NextFeature
                                Continue While
                            End If
                        Next
                    End If
                    Dim RecordNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("RECORD")
                    RecordNode.SetAttribute("rec", cntRow)

                    Dim ValuesNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("VALUES")
                    For Each NewField As EraField In m_ColFields
                        NewField.XmlRecordResults(ValuesNode, CType(pRow, IRow))
                    Next NewField
                    RecordNode.AppendChild(ValuesNode)
                    m_OutputXmlElement.AppendChild(RecordNode)

                    For Each ChildTable As EraTable In m_ColTables
                        ChildTable.WriteXmlTableResults(RecordNode, CType(pRow, IRow))
                    Next

                    Marshal.ReleaseComObject(pRow)

                    'Increment counter and get next Record from QueryResults
                    pRow = pFeatureCursor.NextFeature
                    cntRow += 1
                End While

                'Sort the Records
                If m_SortFieldsNode IsNot Nothing AndAlso m_SortFieldsNode.ChildNodes.Count > 0 Then
                    Dim sortList As New Generic.List(Of Xml.XmlElement)
                    For Each recordNode As Xml.XmlElement In m_OutputXmlElement.SelectNodes("RECORD")
                        sortList.Add(recordNode)
                    Next
                    For Each recordNode As Xml.XmlElement In sortList
                        m_OutputXmlElement.RemoveChild(recordNode)
                    Next
                    sortList.Sort(New RecordComparer(m_SortFieldsNode))
                    For Each recordNode As Xml.XmlElement In sortList
                        m_OutputXmlElement.AppendChild(recordNode)
                    Next
                End If
            End If 'pRow is nothing
            Marshal.ReleaseComObject(pFeatureCursor)
        Catch Ex As Exception
            AddError(Ex, "BufferAnalysis.WriteResultsSub")
            Throw
        End Try
    End Sub
End Class
