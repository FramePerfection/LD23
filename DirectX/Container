Imports Microsoft.DirectX, Microsoft.DirectX.Direct3D
Public Class Container(Of T)
    Public Elements As New List(Of T)
    Public Names As New List(Of String)
    Public Overridable Sub Add(ByVal obj As T, ByVal Name As String)
        Elements.Add(obj)
        Names.Add(Name)
    End Sub
    Public Sub Remove(ByVal Name As String)
        For i As Integer = 0 To Elements.Count - 1
            If Names.Item(i) = Name Then
                Elements.RemoveAt(i)
            End If
        Next
    End Sub
    Public Function Find(ByVal Name As String) As T
        For i As Integer = 0 To Elements.Count - 1
            If Names.Item(i) = Name Then
                Return Elements.Item(i)
            End If
        Next
        Return Nothing
    End Function
End Class

Public Class TextureContainer
    Inherits Container(Of BaseTexture)
    Public Source As New List(Of String)
    Public Shadows Sub Add(ByVal SourceFile As String, ByVal Name As String)
        Dim outFile As BaseTexture = Nothing
        If Not SourceFile.EndsWith(".dds") Then
            outFile = TextureLoader.FromFile(Game.Device, SourceFile)
        Else
            Try
                outFile = TextureLoader.FromCubeFile(Game.Device, SourceFile)
            Catch ex As Exception
            End Try
            If outFile Is Nothing Then
                Try
                    outFile = TextureLoader.FromVolumeFile(Game.Device, SourceFile)
                Catch ex2 As Exception

                End Try
            End If
        End If
        MyBase.Add(outFile, Name)
        Source.Add(SourceFile)
    End Sub
    Public Shared Function FromDirectory(ByVal SourceDirectory As String) As TextureContainer
        Dim out As New TextureContainer()
        For Each Name As String In IO.Directory.GetFiles(SourceDirectory)
            If Name.EndsWith(".png") Or Name.EndsWith(".jpg") Or Name.EndsWith(".bmp") Or Name.EndsWith(".tga") Or Name.EndsWith(".dds") Then
                out.Add(Name, Name.Remove(0, (SourceDirectory).Length + 1).Split(".")(0))
            End If
        Next
        Return out
    End Function
    Public Sub Reload(ByVal Graphics As Device)
        For i As Integer = 0 To Source.Count - 1
            Elements(i) = TextureLoader.FromFile(Graphics, Source(i))
        Next
    End Sub
End Class

Public Class EffectContainer
    Inherits Container(Of Effect)
    Public Source As New List(Of String)
    Public Shared DefaultPool As New EffectPool
    Public Shadows Sub Add(ByVal SourceFile As String, ByVal Name As String)
        Try

            MyBase.Add(Effect.FromFile(Game.Device, SourceFile, Nothing, Nothing, ShaderFlags.None, DefaultPool), Name)
            Source.Add(SourceFile)
        Catch ex As Exception

        End Try
    End Sub
    Public Shared Function FromDirectory(ByVal SourceDirectory As String) As EffectContainer
        Dim out As New EffectContainer()
        For Each Name As String In IO.Directory.GetFiles(SourceDirectory)
            If Name.EndsWith(".fx") Then
                out.Add(Name, Name.Remove(0, (SourceDirectory).Length + 1).Split(".")(0))
            End If
        Next
        Return out
    End Function
    Public Sub Reload(ByVal Graphics As Device)
        For i As Integer = 0 To Source.Count - 1
            Elements(i) = Effect.FromFile(Game.Device, Source(i), Nothing, Nothing, ShaderFlags.None, DefaultPool)
        Next
    End Sub
End Class
