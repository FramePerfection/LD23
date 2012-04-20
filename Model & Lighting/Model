Public Class Model
    Public Shared CameraPosition As Vector3
    Public Meshes As New List(Of Object)
    Public Transforms As New List(Of Matrix)
    Public Shared AllModels As New List(Of Model)
    Public Shared AllMeshes As New ObjectContainer
    Public Shared AllEffects As EffectContainer
    Public Shared AllTextures As TextureContainer
    Public Transform As Matrix

    Public Shared Sub Init()
        If Model.AllEffects Is Nothing Then
            Model.AllEffects = EffectContainer.FromDirectory(Application.StartupPath & "\Shaders")
            Model.AllTextures = TextureContainer.FromDirectory(Application.StartupPath & "\Textures")
            LightSetting.LightingEffect = Model.AllEffects.Find("Final PPL")
        End If
    End Sub

    Public Sub New()
        AllModels.Add(Me)
    End Sub
    Public Sub AddMesh(ByVal Mesh As Object)
        AddMesh(Mesh, Matrix.Identity)
    End Sub
    Public Sub AddMesh(ByVal Mesh As Object, ByVal SubsetTransform As Matrix)
        If Mesh.GetType.BaseType.FullName.Contains("BaseModel") Then
            If Not AllMeshes.Elements.Contains(Mesh) Then
                AllMeshes.Add(Mesh, "Mesh " & AllMeshes.Elements.Count)
            End If
            Meshes.Add(Mesh)
            Transforms.Add(SubsetTransform)
        Else
            MsgBox("Invalid Model Data")
        End If
    End Sub
    Public Sub DrawToShadowMap()
        For Each m As Object In Meshes
            m.Transform = Transform
            m.DrawToShadowMap()
        Next
    End Sub
    Public Sub Draw()
        While Transforms.Count < Meshes.Count
            Transforms.Add(Matrix.Identity)
        End While
        For i As Integer = 0 To Meshes.Count - 1
            Try
                Meshes(i).Transform = Transforms(i) * Transform
                Meshes(i).Draw()
            Catch ex As Exception
            End Try
        Next
    End Sub
    Public Class ObjectContainer
        Inherits Container(Of Object)
    End Class
    Public Shared Function GetDotXModelData(ByVal SourceFileName As String, Optional ByVal Correction As Integer = 0) As NormalTextureMesh
        Dim m As Mesh = Mesh.FromFile(SourceFileName, MeshFlags.SystemMemory, Game.Device)
        Dim out As New NormalTextureMesh(m.NumberVertices, m.NumberFaces * 40)
        Dim dat() As CustomVertex.PositionNormalTextured = m.LockVertexBuffer(GetType(CustomVertex.PositionNormalTextured), LockFlags.None, New Integer() {m.NumberVertices})
        For Each vert As CustomVertex.PositionNormalTextured In dat

            out.AddVertex(New CustomVertex.PositionNormalTextured(vert.Position, -vert.Normal, vert.Tu, vert.Tv))
        Next
        m.UnlockVertexBuffer()
        Dim inds() As Integer = {}
        Dim Basedata() As Short = m.LockIndexBuffer(GetType(Short), LockFlags.ReadOnly, New Integer() {100000})
        Array.Resize(inds, Basedata.Length)
        Dim size As Integer = 0
        For i As Integer = 0 To Basedata.Length - 1
            If i > 1000 And Basedata(i) = 0 Then
                Array.Resize(inds, i)
                Exit For
            End If
            inds(i) = Basedata(i)
        Next
        Array.Resize(inds, inds.Length - Correction * 3)
        out.AddIndices(inds)
        m.UnlockIndexBuffer()
        out.ApplyData()
        Return out
    End Function
End Class
