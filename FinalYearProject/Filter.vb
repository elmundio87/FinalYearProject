Imports System.Drawing.Graphics

Public MustInherit Class Filter
    Public MustOverride Function filter(ByVal col As System.Drawing.Color)
    Public MustOverride Sub Flush()

End Class