Public Class ERAControl
    Inherits Windows.Forms.UserControl

    Protected myErrorProvider As New ErrorProvider

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'ERAControl
        '
        Me.Name = "ERAControl"
        Me.ResumeLayout(False)

    End Sub

    Public Sub New()
        MyBase.New()
    End Sub
    Private components As System.ComponentModel.IContainer

    'Public frde_Parent As frmReportDefinitionEditor
    Public Overloads ReadOnly Property ParentForm() As frmReportDefinitionEditor
        Get
            Return MyBase.ParentForm
        End Get
    End Property

    Private m_EraNode As ERANode

    Public Property EraNode() As ERANode
        Get
            Return m_EraNode
        End Get
        Set(ByVal value As ERANode)
            m_EraNode = value
            LoadData()
        End Set
    End Property

    Public Overridable Sub ShowErrors()
    End Sub

    Friend Overridable Sub SaveChangesToNode()
        'This is where the changes get saved to the ERANode, not to the XML document
    End Sub

    Friend Overridable Sub LoadData()
        'This is where the eraNode data is read into the controls on the form
    End Sub

    Private Sub ERAControl_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'ShowErrors() <--moved to loaddata, as the eranode hasn't been set yet
    End Sub
End Class

