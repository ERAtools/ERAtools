Imports ESRI.ArcGIS.Geodatabase
Imports System.Runtime.InteropServices

''' <summary>
''' The Pinpoint analysis lists all features from the analysis layer that intersect the user-specified analysis feature. The user-specified buffer is not used.
''' </summary>
<System.Runtime.InteropServices.ComVisible(False)> _
Public Class PinpointAnalysis
    Inherits ERAAnalysis

    ''' <summary>
    ''' Writes the XML results.
    ''' </summary>
    Public Overrides Sub WriteXmlResults()
        Try
            'Initialize Counter and get first record
            Dim iRow As Integer = 1
            Dim pFeatureCursor As IFeatureCursor = OpenFeatureCursor(AnalysisShape)

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
                    RecordNode.SetAttribute("rec", iRow)

                    Dim ValuesNode As Xml.XmlElement = m_OutputXmlDoc.CreateElement("VALUES")
                    For Each NewField As EraField In m_ColFields
                        NewField.XmlRecordResults(ValuesNode, CType(pRow, IRow))
                    Next NewField
                    RecordNode.AppendChild(ValuesNode)
                    m_OutputXmlElement.AppendChild(RecordNode)

                    For Each ChildTable As EraTable In m_ColTables
                        ChildTable.WriteXmlTableResults(RecordNode, CType(pRow, IRow))
                    Next

                    'Increment counter and get next Record from QueryResults
                    pRow = pFeatureCursor.NextFeature
                    iRow = iRow + 1
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
            AddError(Ex, "PinpointAnalysis.WriteXmlResults")
            Throw
        End Try
    End Sub
End Class
