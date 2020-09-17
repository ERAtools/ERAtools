Imports Microsoft.VisualBasic.ApplicationServices

<System.Runtime.InteropServices.ComVisible(False)> _
Public NotInheritable Class AboutDialog

    Private Sub AboutDialog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = String.Format("About {0}", My.ThisAddIn.Name)
        ' Initialize all of the text displayed on the About Box.
        Me.LabelProductName.Text = My.ThisAddIn.Name
        Me.LabelVersion.Text = String.Format("Version {0}", My.ThisAddIn.Version)
        Me.txtCopyright.Text = String.Format("Published {0}", My.ThisAddIn.Date)
        Me.txtCompany.Text = My.ThisAddIn.Company
        Me.TextBoxDescription.Text = My.ThisAddIn.Description
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

End Class
