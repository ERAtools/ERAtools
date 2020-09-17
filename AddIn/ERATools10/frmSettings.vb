Imports System.Windows.Forms

<System.Runtime.InteropServices.ComVisible(False)> _
Public Class frmSettings

    Private Sub frmSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If My.Settings.OutputFormatFolder = "" Then
            'pull from the registry
            Dim k As Microsoft.Win32.RegistryKey = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\ERAtools2")
            If k IsNot Nothing Then
                txtOutputFormatsFolder.Text = k.GetValue("SystemFolder", "C:\myav8\ERAtools2\System")
            Else
                txtOutputFormatsFolder.Text = "C:\myav8\ERAtools2\System"
            End If
        Else
            Me.txtOutputFormatsFolder.Text = My.Settings.OutputFormatFolder
        End If
        Me.txtScratchWorkspacePath.Text = SystemWorkspace.DatabasePath
        If My.Settings.AuthorName = "" Then
            Try
                AppDomain.CurrentDomain.SetPrincipalPolicy(Security.Principal.PrincipalPolicy.WindowsPrincipal)
                Dim p As Security.Principal.WindowsPrincipal = System.Threading.Thread.CurrentPrincipal
                Dim wi As Security.Principal.WindowsIdentity = p.Identity
                txtAuthorName.Text = wi.Name
            Catch ex As Exception
                Logging.Log.Error("Error obtaining username", ex)
                'This isn't critical
            End Try
        Else
            txtAuthorName.Text = My.Settings.AuthorName
        End If
        cbWarnReplaceOutput.Checked = My.Settings.WarnReplaceOutput
    End Sub

    Private Sub txtOutputFormatsFolder_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtOutputFormatsFolder.Validating
        If txtOutputFormatsFolder.Text = "" Then
            ep.SetError(Me.BrowseOutputFormatsButton, "You must enter a valid folder path")
            e.Cancel = True
        Else
            If Not IO.Directory.Exists(txtOutputFormatsFolder.Text) Then
                ep.SetError(Me.BrowseOutputFormatsButton, "You must enter a valid folder path")
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub txtScratchWorkspacePath_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtScratchWorkspacePath.Validating
        If txtScratchWorkspacePath.Text = "" Then
            ep.SetError(Me.BrowseSystemWorkspaceButton, "You must enter a valid filename.")
            e.Cancel = True
        Else
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(txtScratchWorkspacePath.Text)) Then
                ep.SetError(Me.BrowseSystemWorkspaceButton, "You must enter a valid writable path.")
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub txtOutputFormatsFolder_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtOutputFormatsFolder.Validated
        ep.SetError(BrowseOutputFormatsButton, "")
    End Sub

    Private Sub txtScratchWorkspacePath_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtScratchWorkspacePath.Validated
        ep.SetError(Me.BrowseSystemWorkspaceButton, "")
    End Sub

    Private Sub BrowseOutputFormatsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseOutputFormatsButton.Click
        Dim fbd As New FolderBrowserDialog
        With fbd
            .Description = "Browse to the ERAtool2 folder where XSLT files are stored."
            .ShowNewFolderButton = False
            If .ShowDialog = DialogResult.OK Then
                Me.txtOutputFormatsFolder.Text = .SelectedPath
                ep.SetError(BrowseOutputFormatsButton, "")
            End If
        End With
    End Sub

    Private Sub BrowseSystemWorkspaceButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BrowseSystemWorkspaceButton.Click
        Dim ofd As New OpenFileDialog
        With ofd

            .Title = "Browse to or create the ERAtool2 Saved Shapes geodatabase"
            .AddExtension = True
            .Filter = "Personal Geodatabase Files (*.mdb)|*.mdb"
            If IO.Directory.Exists(IO.Path.GetDirectoryName(txtOutputFormatsFolder.Text)) Then
                .InitialDirectory = IO.Path.GetDirectoryName(txtOutputFormatsFolder.Text)
            End If

            .Multiselect = False
            .ShowReadOnly = False
            .CheckPathExists = True
            .CheckFileExists = False

            If .ShowDialog = DialogResult.OK Then
                Me.txtOutputFormatsFolder.Text = .FileName
                ep.SetError(Me.BrowseSystemWorkspaceButton, "")
            End If
        End With
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        SystemWorkspace.DatabasePath = Me.txtScratchWorkspacePath.Text
        My.Settings.OutputFormatFolder = Me.txtOutputFormatsFolder.Text
        My.Settings.AuthorName = Me.txtAuthorName.Text
        My.Settings.WarnReplaceOutput = cbWarnReplaceOutput.Checked
        My.Settings.Save()
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class