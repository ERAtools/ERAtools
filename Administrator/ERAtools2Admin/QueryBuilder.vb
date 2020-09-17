Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.GeoDatabase
Imports ESRI.ArcGIS.GeoDatabaseUI

Public Class QueryBuilder

    Private m_WhereClauseContainer As TextBox
    Dim pQueryPropertyPage As IQueryPropertyPage

    Public Sub ShowQueryBuilder(ByVal pTable As ITable, ByRef WhereClauseContainer As TextBox)
        Me.Show()

        pQueryPropertyPage = New QueryPropertyPage
        Dim pPropertyPageSiteConfig As IPropertyPageSiteConfig = New ComPropertyPageSite
        Dim pPropertyPage As IPropertyPage = pQueryPropertyPage 'QI
        pQueryPropertyPage.Table = pTable
        pQueryPropertyPage.ExpressionLabel = "Select where:"
        pQueryPropertyPage.Expression = WhereClauseContainer.Text

        'Pass the hWnd of the frame to the pagesite  
        pPropertyPageSiteConfig.hWnd = Me.Handle

        'Set the page reference on the propertypagesite  
        pPropertyPageSiteConfig.Page = pPropertyPage
        'Set the page site  
        pPropertyPage.SetPageSite(pPropertyPageSiteConfig)

        m_WhereClauseContainer = WhereClauseContainer

    End Sub


    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        m_WhereClauseContainer.Text = pQueryPropertyPage.QueryFilter.WhereClause
        Me.Close()
        Me.Dispose(True)
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
        Me.Dispose(True)
    End Sub

    Public Sub Trash()
        If Me.Visible Then
            Me.Close()
        End If
        Me.Dispose(True)
    End Sub
End Class