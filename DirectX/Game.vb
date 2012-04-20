Imports UG = Ultima_GameEngine
Public MustInherit Class Game
    Inherits Ultima_GameEngine.Game

    Public Shared Frm As Form
    Public Components As New List(Of GameComponent)
    Public RelevantComponents As New List(Of GameComponent)
    Public PermanentComponents As New List(Of GameComponent)
    Public ComponentLists As List(Of ComponentList) = ComponentList.CreateAllLists()
    Public RelevantLists As List(Of ComponentList) = ComponentList.CreateAllLists()
    Public CreateReducedLists As Boolean = True
    Public UseReducedLists As Boolean = True
    Public Shared Device As Device
    Private ToRemove As New List(Of GameComponent)
    Private ToAdd As New List(Of GameComponent)

    Public Overrides Sub Load()
        Frm.Size = New Size(800, 600)
        Device = CreateDevice(DefaultParams())
    End Sub

    Public Overrides Sub Update(ByVal ftime As UG.GameTime)
        For Each obj As GameComponent In ToAdd
            Insert(obj)
        Next
        For Each obj As GameComponent In ToRemove
            Kill(obj)
        Next
        ToAdd.Clear()
        ToRemove.Clear()
        RelevantComponents.Clear()
        For Each lst As ComponentList In RelevantLists
            lst.Components.Clear()
        Next
        If CreateReducedLists Then
            For Each obj As GameComponent In Components
                CheckObjRelevant(obj)
            Next
        End If
        For Each obj As GameComponent In PermanentComponents
            CheckObjRelevant(obj)
        Next
        If UseReducedLists Then
            Dim i As Integer = 0
            While i < RelevantComponents.Count
                RelevantComponents(i).PerformUpdate(ftime)
                i += 1
            End While
            i = 0
            While i < RelevantComponents.Count
                RelevantComponents(i).Updated = False
                i += 1
            End While
        Else
            For Each obj As GameComponent In Components
                obj.PerformUpdate(ftime)
            Next
            For Each cp As GameComponent In Components
                cp.Updated = False
            Next
        End If
        Valid = Frm.Created
    End Sub

    Public Sub CheckObjRelevant(ByVal obj As GameComponent)
        If obj.Relevant() Then
            For i As Integer = 0 To RelevantComponents.Count - 1
                If RelevantComponents(i).Layer > obj.Layer Then
                    RelevantComponents.Insert(i, obj)
                    GoTo Inserted
                End If
            Next
            RelevantComponents.Add(obj)
Inserted:
            For Each lst As ComponentList In obj.ComponentLists
                lst.Components.Add(obj)
            Next
        End If
        obj.Updated = False
    End Sub

    Public Sub AddPermanent(ByVal obj As GameComponent)
        For i As Integer = 0 To PermanentComponents.Count - 1
            If PermanentComponents(i).Layer > obj.Layer Then
                PermanentComponents.Insert(i, obj)
                Exit Sub
            End If
        Next
        PermanentComponents.Add(obj)
    End Sub

    Public Overrides Sub Draw(ByVal ftime As UG.GameTime)
        If Valid Then
            Try

                Device.EndScene()
                Device.Present()
            Catch ex As Exception

            End Try
            Device.BeginScene()
            PrepareDraw(ftime)
            If UseReducedLists Then
                For Each cp As GameComponent In RelevantComponents
                    cp.Draw(ftime)
                Next
            Else
                For Each cp As GameComponent In Components
                    cp.Draw(ftime)
                Next
            End If
            EndDraw(ftime)
        End If
    End Sub
    Public MustOverride Sub PrepareDraw(ByVal ftime As GameTime)
    Public MustOverride Sub EndDraw(ByVal ftime As GameTime)

    Public Sub UpdateLists()
        For Each obj As GameComponent In ToAdd
            Insert(obj)
        Next
        For Each obj As GameComponent In ToRemove
            Kill(obj)
        Next
        ToAdd.Clear()
        ToRemove.Clear()
        RelevantComponents.Clear()
        For Each lst As ComponentList In RelevantLists
            lst.Components.Clear()
        Next
        For Each obj As GameComponent In Components
            If obj.Relevant() Then
                RelevantComponents.Add(obj)
                For Each lst As ComponentList In obj.ComponentLists
                    lst.Components.Add(obj)
                Next
            End If
            obj.Updated = False
        Next
    End Sub

    Public Sub Add(ByVal Component As GameComponent, ByVal Layer As Integer)
        Component.Layer = Layer
        If GameComponent.CreateNew = GameComponent.ComponentCreateHandle.InstantAdd Then
            Insert(Component)
        Else
            ToAdd.Add(Component)
        End If
    End Sub

    Public Sub Add(ByVal Components As List(Of GameComponent))
        For Each Component As GameComponent In Components
            If GameComponent.CreateNew = GameComponent.ComponentCreateHandle.InstantAdd Then
                Insert(Component)
            Else
                ToAdd.Add(Component)
            End If
        Next
    End Sub

    Private Sub Insert(ByVal Component As GameComponent)
        For i As Integer = 0 To Components.Count - 1
            If Components(i).Layer > Component.Layer Then
                Components.Insert(i, Component)
                GoTo Inserted
            End If
        Next
        Components.Add(Component)
Inserted:

        If FindList(Component.GetType) Is Nothing Then
            ComponentLists.Add(ComponentList.Create(Component.GetType.FullName))
            RelevantLists.Add(ComponentList.Create(Component.GetType.FullName))
        End If

        For Each lst As ComponentList In ComponentLists
            Dim tpName As Type = Component.GetType
            While tpName.BaseType IsNot Nothing
                If lst.TypeName = tpName.FullName Then
                    If Not lst.Components.Contains(Component) Then
                        lst.Components.Add(Component)
                    End If
                End If
                tpName = tpName.BaseType
            End While
        Next
    End Sub

    Public Sub RemoveAll()
        For Each c As GameComponent In Components
            Remove(c)
        Next
    End Sub

    Public Sub Remove(ByVal comp As GameComponent)
        ToRemove.Add(comp)
    End Sub

    Private Sub Kill(ByVal comp As GameComponent)
        Components.Remove(comp)
        PermanentComponents.Remove(comp)
        For Each lst As ComponentList In ComponentLists
            lst.Components.Remove(comp)
        Next
    End Sub

    Public Function FindList(Of T As GameComponent)(Optional ByVal Relevants As Boolean = True) As ComponentList
        If Relevants Then
            Dim nm As String = GetType(T).FullName
            For Each lst As ComponentList In RelevantLists
                If lst.TypeName = nm Then
                    Return lst
                End If
            Next
        Else
            Dim nm As String = GetType(T).FullName
            For Each lst As ComponentList In ComponentLists
                If lst.TypeName = nm Then
                    Return lst
                End If
            Next
        End If
        Return Nothing
    End Function

    Public Function FindList(ByVal Type As Type) As ComponentList
        Dim nm As String = Type.FullName
        For Each lst As ComponentList In ComponentLists
            If lst.TypeName = nm Then
                Return lst
            End If
        Next
        Return Nothing
    End Function

    Public Function FindDynamicList(ByVal tp As Type) As ComponentList
        Dim nm As String = tp.FullName
        For Each lst As ComponentList In RelevantLists
            If lst.TypeName = nm Then
                Return lst
            End If
        Next
        Return Nothing
    End Function

    Public Shared Function DefaultParams(Optional ByVal Fullscreen As Boolean = True) As PresentParameters
        Dim params As New PresentParameters()
        params.BackBufferCount = 1
        params.SwapEffect = SwapEffect.Discard
        params.Windowed = Not Fullscreen
        params.DeviceWindow = Frm
        If Fullscreen Then
            params.BackBufferWidth = Frm.Width
            params.BackBufferHeight = Frm.Height
            params.BackBufferFormat = Format.X8R8G8B8
        End If
        params.EnableAutoDepthStencil = True
        params.AutoDepthStencilFormat = DepthFormat.D24S8
        Return params
    End Function

    Public Shared Function CreateDevice(ByVal params As PresentParameters) As Device
        Dim out As Device = Nothing
        Try
            out = New Device(0, DeviceType.Hardware, params.DeviceWindow, CreateFlags.HardwareVertexProcessing Or CreateFlags.MultiThreaded, params)
        Catch ex As Exception
            Try
                out = New Device(0, DeviceType.Hardware, params.DeviceWindow, CreateFlags.SoftwareVertexProcessing, params)
            Catch ex2 As Exception
                Try
                    out = New Device(0, DeviceType.Reference, params.DeviceWindow, CreateFlags.SoftwareVertexProcessing, params)
                Catch ex3 As Exception
                    MsgBox("Failed to create DirectX Device")
                End Try
            End Try
        End Try
        If out IsNot Nothing Then
            out.beginScene()
        End If
        Return out
    End Function

End Class
