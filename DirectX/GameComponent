Imports UG = Ultima_GameEngine
Public MustInherit Class GameComponent
    Inherits Ultima_GameEngine.GameComponent
    Public Parent As Game
    Public Updated As Boolean = False
    Public Layer As Integer
    Public Permanent As Boolean = False
    Private ReferencedLists As List(Of ComponentList)
    Public Shared CreateNew As ComponentCreateHandle = ComponentCreateHandle.Default
    Public Enum ComponentCreateHandle
        [Default]
        DoNotAdd
        InstantAdd
    End Enum
    Public Sub New(ByVal g As Game, Optional ByVal ObjectLayer As Integer = 0)
        Select Case CreateNew
            Case ComponentCreateHandle.Default
                g.Add(Me, ObjectLayer)
        End Select
        Parent = g
    End Sub
    Public Overrides Sub Update(ByVal ftime As UG.GameTime)
        If Not Updated Then
            Updated = True
            PerformUpdate(ftime)
        End If
    End Sub
    Public MustOverride Sub PerformUpdate(ByVal ftime As UG.GameTime)
    Public MustOverride Function Relevant() As Boolean
    Public Function ComponentLists() As List(Of ComponentList)
        If ReferencedLists Is Nothing Then
            ReferencedLists = New List(Of ComponentList)
            Dim tp As Type = Me.GetType
            While tp.BaseType IsNot Nothing
                Dim lst As ComponentList = Parent.FindDynamicList(tp)
                If lst IsNot Nothing Then
                    ReferencedLists.Add(lst)
                End If
                tp = tp.BaseType
            End While
        End If
        Return ReferencedLists
    End Function
End Class