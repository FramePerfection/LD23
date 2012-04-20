''' <summary>
''' This is a base class for GameComponents
''' </summary>
''' <remarks>This class is abstract</remarks>
Public MustInherit Class GameComponent
    Public Events As New List(Of ComponentManager.ObjEvent)
    Public EventNames As New List(Of String)
    Public Sub ExecuteEvent(ByVal EventName As String, ByVal senders() As GameComponent)
        For i As Integer = 0 To EventNames.Count - 1
            If EventNames(i) = EventName Then
                ExecuteEvent(i, senders)
                Exit Sub
            End If
        Next
    End Sub
    Public Sub ExecuteEvent(ByVal index As Integer, ByVal senders() As GameComponent)
        Events(index).Invoke(senders)
    End Sub
    Public Sub AddEvent(ByVal Name As String)
        If Not EventNames.Contains(Name) Then
            EventNames.Add(Name)
            Events.Add(New ComponentManager.ObjEvent)
        End If
    End Sub
    Public Function FindEvent(ByVal EventName As String) As ComponentManager.ObjEvent
        For i As Integer = 0 To Events.Count - 1
            If EventNames(i) = EventName Then
                Return Events(i)
            End If
        Next
        Return Nothing
    End Function
    Public MustOverride Sub Update(ByVal ftime As GameTime)
    Public MustOverride Sub Draw(ByVal ftime As GameTime)
End Class