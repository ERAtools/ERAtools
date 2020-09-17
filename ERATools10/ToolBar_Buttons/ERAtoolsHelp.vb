Imports System.Runtime.InteropServices
Imports System.Drawing
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.ArcMapUI
Imports System.Windows.Forms

Public NotInheritable Class ERAtoolsHelp
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Protected Overrides Sub OnClick()
        Dim AssemblyLocation As String = System.Reflection.Assembly.GetExecutingAssembly.Location
        Dim fi As New IO.FileInfo(AssemblyLocation)
        Dim filename As String = IO.Path.Combine(fi.DirectoryName, "ERAtoolsHelp.pdf")
        Try
            If IO.File.Exists(filename) Then
                'Shell(filename, AppWinStyle.NormalFocus)
                Process.Start(filename)
            Else
                MessageBox.Show("Unable to display help. Help file ERAtoolsHelp.pdf not found in expected path (" & fi.DirectoryName & ")", "Help File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Error displaying help: " & ex.Message, "Error Displaying Help", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class

Public NotInheritable Class ERAtoolsAbout
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Protected Overrides Sub OnClick()
        Dim ab As New AboutDialog
        ab.ShowDialog()
    End Sub
End Class


