<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.lbxDownloading = New System.Windows.Forms.ListBox
        Me.lbxProcessing = New System.Windows.Forms.ListBox
        Me.lbxSending = New System.Windows.Forms.ListBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.edtDetail = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'lbxDownloading
        '
        Me.lbxDownloading.FormattingEnabled = True
        Me.lbxDownloading.Location = New System.Drawing.Point(12, 39)
        Me.lbxDownloading.Name = "lbxDownloading"
        Me.lbxDownloading.Size = New System.Drawing.Size(250, 290)
        Me.lbxDownloading.TabIndex = 0
        '
        'lbxProcessing
        '
        Me.lbxProcessing.FormattingEnabled = True
        Me.lbxProcessing.Location = New System.Drawing.Point(270, 39)
        Me.lbxProcessing.Name = "lbxProcessing"
        Me.lbxProcessing.Size = New System.Drawing.Size(250, 290)
        Me.lbxProcessing.TabIndex = 1
        '
        'lbxSending
        '
        Me.lbxSending.FormattingEnabled = True
        Me.lbxSending.Location = New System.Drawing.Point(524, 39)
        Me.lbxSending.Name = "lbxSending"
        Me.lbxSending.Size = New System.Drawing.Size(250, 290)
        Me.lbxSending.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(69, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Downloading"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(267, 23)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(59, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Processing"
        '
        'edtDetail
        '
        Me.edtDetail.Location = New System.Drawing.Point(12, 336)
        Me.edtDetail.Multiline = True
        Me.edtDetail.Name = "edtDetail"
        Me.edtDetail.Size = New System.Drawing.Size(762, 181)
        Me.edtDetail.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(521, 23)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(55, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Uploading"
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1000
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 529)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.edtDetail)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lbxSending)
        Me.Controls.Add(Me.lbxProcessing)
        Me.Controls.Add(Me.lbxDownloading)
        Me.Name = "Main"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbxDownloading As System.Windows.Forms.ListBox
    Friend WithEvents lbxProcessing As System.Windows.Forms.ListBox
    Friend WithEvents lbxSending As System.Windows.Forms.ListBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents edtDetail As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer

End Class
