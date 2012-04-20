Public MustInherit Class BaseModel(Of VertexType)
    Public VertexBuffer As VertexBuffer
    Public IndexBuffer As IndexBuffer
    Public Data() As VertexType = {}
    Public IndData() As Integer = {}
    Public EffectIndices() As Integer = {0}
    Public Effects As New List(Of Effect)
    Public LightingEffect As New List(Of Boolean)
    Public Transform As Matrix
    Private EffectNames As New List(Of String)
    Private Format As VertexFormats
    Private MaxVerts As Integer
    Private MaxInds As Integer

    Public Sub New(ByVal VertexFormats As VertexFormats, Optional ByVal MaxVertices As Integer = UShort.MaxValue, Optional ByVal MaxIndices As Integer = Integer.MaxValue)
        Format = VertexFormats
        MaxVerts = MaxVertices
        MaxInds = MaxIndices
        Create()
    End Sub

    Public Sub New()
    End Sub

    Protected Sub Create()
        AddEffect("Final PPL", True)
        Game.Device.RenderState.AntiAliasedLineEnable = True
        VertexBuffer = New VertexBuffer(GetType(VertexType), MaxVerts, Game.Device, Usage.None, Format, Pool.Managed)
        IndexBuffer = New IndexBuffer(GetType(Integer), MaxInds, Game.Device, Usage.None, Pool.Managed)
    End Sub

    Public Function CheckVertexClick() As Integer()
        Dim WVP As Matrix = Transform * Game.Device.Transform.View * Game.Device.Transform.Projection
        Dim out As New List(Of Integer)
        For i As Short = 0 To Data.Length - 1
            Dim v As Object = Data(i)
            Try
                Dim p As Vector3 = Vector3.TransformCoordinate(v.Position, WVP)
                If p.X ^ 2 + p.Y ^ 2 < 0.05 ^ 2 Then
                    out.Add(i)
                End If
            Catch ex As Exception
                MsgBox("No position data")
            End Try
        Next
        Return out.toarray
    End Function


    Public Sub AddEffect(ByVal EffectName As String, Optional ByVal IsLighting As Boolean = False)
        LightingEffect.Add(IsLighting)
        EffectNames.Add(EffectName)
        Effects.Add(Model.AllEffects.Find(EffectName))
    End Sub
    Public Sub ClearEffects()
        LightingEffect.Clear()
        EffectNames.Clear()
        Effects.Clear()
    End Sub

    Public Sub AddVertex(ByVal Input As VertexType)
        AddVertices(New VertexType() {Input})
    End Sub

    Public Sub AddVertices(ByVal Input() As VertexType)
        Dim preIndData As Integer = IndData.Length
        Array.Resize(Data, Data.Length + Input.Length)
        For i As Short = Data.Length - Input.Length To Data.Length - 1
            Data(i) = Input(i - preIndData)
        Next
    End Sub

    Public Sub AddIndex(ByVal Input As Integer)
        AddIndices(New Integer() {Input})
    End Sub

    Public Sub AddIndices(ByVal Input() As Integer)
        Dim preIndData As Integer = IndData.Length
        Array.Resize(IndData, IndData.Length + Input.Length)
        For i As Short = IndData.Length - Input.Length To IndData.Length - 1
            IndData(i) = Input(i - preIndData)
        Next
    End Sub

    Public Sub ApplyData()
        VertexBuffer.SetData(Data, 0, LockFlags.None)
        IndexBuffer.SetData(IndData, 0, LockFlags.None)
    End Sub

    Public Sub Save(ByVal FileName As String)
        Dim str As New IO.FileStream(FileName, IO.FileMode.Create)
        Dim wr As New IO.BinaryWriter(str)
        SaveToFile(wr)
        wr.Close()
        str.Close()
    End Sub

    Public Sub SaveToFile(ByVal wr As IO.BinaryWriter)
        wr.Write(Format)
        wr.Write(MaxVerts)
        wr.Write(MaxInds)

        wr.Write(Data.Length)
        For i As Integer = 0 To Data.Length - 1
            SaveVertex(Data(i), wr)
        Next

        wr.Write(IndData.Length)
        For i As Integer = 0 To IndData.Length - 1
            wr.Write(IndData(i))
        Next
    End Sub

    Public Sub ReadFromFile(ByVal rd As IO.BinaryReader)
        Format = rd.ReadInt32
        MaxVerts = rd.ReadInt32
        MaxInds = rd.ReadInt32
        Create()

        Array.Resize(Data, rd.ReadInt32)
        For i As Integer = 0 To Data.Length - 1
            ReadVertex(Data(i), rd)
        Next

        Array.Resize(IndData, rd.ReadInt32)
        For i As Integer = 0 To IndData.Length - 1
            IndData(i) = rd.ReadInt32
        Next
        ApplyData()
    End Sub

    Public MustOverride Sub SaveVertex(ByVal SourceData As VertexType, ByVal wr As IO.BinaryWriter)
    Public MustOverride Sub ReadVertex(ByRef Destination As VertexType, ByVal rd As IO.BinaryReader)

    Public Overridable Sub SetVertexPosition(ByRef Vertex As Microsoft.DirectX.Direct3D.CustomVertex.PositionNormalTextured, ByVal newPosition As Microsoft.DirectX.Vector3)
        Vertex.Position = Vector3.TransformCoordinate(newPosition, Matrix.Invert(Transform))
    End Sub

    Public Overridable Sub Draw()
        Game.Device.SetStreamSource(0, VertexBuffer, 0)
        Game.Device.Indices = IndexBuffer
        Game.Device.VertexFormat = Format
        For i As Short = 0 To EffectIndices.Length - 1
            Dim effect As Effect = Effects(EffectIndices(i))
            Try
                effect.SetValue(EffectHandle.FromString("World"), Transform)
                effect.SetValue(EffectHandle.FromString("View"), Game.Device.Transform.View)
                effect.SetValue(EffectHandle.FromString("Projection"), Game.Device.Transform.Projection)
                effect.SetValue(EffectHandle.FromString("CameraPosition"), New Vector4(Model.CameraPosition.X, Model.CameraPosition.Y, Model.CameraPosition.Z, 0))
            Catch ex As Exception
                MsgBox("aah" & EffectIndices(i) & vbCrLf & EffectNames(EffectIndices(i)))
            End Try

            If LightingEffect(EffectIndices(i)) Then
                LightSetting.LightingEffect = effect
                effect.Begin(FX.None)
                LightSetting.Current.Enable(0)
                effect.BeginPass(0)
                Game.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Data.Length, 0, IndData.Length / 3)
                effect.EndPass()
                Game.Device.RenderState.AlphaBlendOperation = BlendOperation.Add
                For LightIndex As Integer = 0 To LightSetting.Current.Lights.Count - 1
                    If LightSetting.Current.Lights(LightIndex).Enabled Then
                        LightSetting.Current.Enable(LightIndex)
                        effect.BeginPass(1)
                        Game.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Data.Length, 0, IndData.Length / 3)
                        effect.EndPass()
                    End If
                Next
                Game.Device.RenderState.AlphaBlendOperation = BlendOperation.Max
            Else
                Dim c As Integer = effect.Begin(FX.None)
                For j As Short = 0 To c - 1
                    effect.BeginPass(j)
                    Game.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Data.Length, 0, IndData.Length / 3)
                    effect.EndPass()
                Next
            End If
            effect.End()
        Next
    End Sub

    Public Sub DrawToShadowMap()
        Game.Device.SetStreamSource(0, VertexBuffer, 0)
        Game.Device.Indices = IndexBuffer
        Game.Device.VertexFormat = Format
        Game.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Data.Length, 0, IndData.Length / 3)
    End Sub
End Class

Public Class ColorMesh
    Inherits BaseModel(Of CustomVertex.PositionColored)
    Public Sub New(Optional ByVal MaxVerts As Integer = UShort.MaxValue, Optional ByVal MaxIndices As Integer = Integer.MaxValue)
        MyBase.New(CustomVertex.PositionColored.Format, MaxVerts, MaxIndices)
    End Sub

    Public Overrides Sub ReadVertex(ByRef Destination As Microsoft.DirectX.Direct3D.CustomVertex.PositionColored, ByVal rd As System.IO.BinaryReader)
        Destination = New CustomVertex.PositionColored(rd.ReadSingle, rd.ReadSingle, rd.ReadSingle, rd.ReadInt32)
    End Sub

    Public Overrides Sub SaveVertex(ByVal SourceData As Microsoft.DirectX.Direct3D.CustomVertex.PositionColored, ByVal wr As System.IO.BinaryWriter)
        wr.Write(SourceData.X) : wr.Write(SourceData.Y) : wr.Write(SourceData.Z) : wr.Write(SourceData.Color)
    End Sub
    Public Shared Function Load(ByVal FileName As String) As ColorMesh
        Dim str As New IO.FileStream(FileName, IO.FileMode.Open)
        Dim rd As New IO.BinaryReader(str)
        Dim out As New ColorMesh()
        out.ReadFromFile(rd)
        rd.Close()
        str.Close()
        Return out
    End Function
End Class

Public Class NormalTextureMesh
    Inherits BaseModel(Of CustomVertex.PositionNormalTextured)
    Public Texture As Texture

    Public Sub New(Optional ByVal MaxVerts As Integer = UShort.MaxValue, Optional ByVal MaxIndices As Integer = Integer.MaxValue)
        MyBase.New(CustomVertex.PositionNormalTextured.Format, MaxVerts, MaxIndices)
    End Sub

    Public Sub InvertNormals()
        Dim dat() As CustomVertex.PositionNormalTextured = VertexBuffer.Lock(0, LockFlags.None)
        For i As Short = 0 To dat.Length - 1
            dat(i).Nx *= -1
            dat(i).Ny *= -1
            dat(i).Nz *= -1
        Next
        Data = dat
        VertexBuffer.Unlock()
        VertexBuffer.SetData(dat, 0, LockFlags.None)
    End Sub

    Public Overrides Sub ReadVertex(ByRef Destination As Microsoft.DirectX.Direct3D.CustomVertex.PositionNormalTextured, ByVal rd As System.IO.BinaryReader)
        Destination = New CustomVertex.PositionNormalTextured(rd.ReadSingle, rd.ReadSingle, rd.ReadSingle, rd.ReadSingle, rd.ReadSingle, rd.ReadSingle, rd.ReadSingle, rd.ReadSingle)
    End Sub

    Public Overrides Sub SaveVertex(ByVal SourceData As Microsoft.DirectX.Direct3D.CustomVertex.PositionNormalTextured, ByVal wr As System.IO.BinaryWriter)
        wr.Write(SourceData.X) : wr.Write(SourceData.Y) : wr.Write(SourceData.Z)
        wr.Write(SourceData.Nx) : wr.Write(SourceData.Ny) : wr.Write(SourceData.Nz)
        wr.Write(SourceData.Tu) : wr.Write(SourceData.Tv)
    End Sub
    Public Shared Function Load(ByVal FileName As String) As NormalTextureMesh
        Dim str As New IO.FileStream(FileName, IO.FileMode.Open)
        Dim rd As New IO.BinaryReader(str)
        Dim out As New NormalTextureMesh()
        out.ReadFromFile(rd)
        rd.Close()
        str.Close()
        Return out
    End Function
    Public Overrides Sub Draw()
        For Each ef As Effect In Effects
            Try
                ef.SetValue(EffectHandle.FromString("Texture"), Texture)
            Catch ex As Exception
            End Try
        Next
        MyBase.Draw()
    End Sub
End Class

Public Class BillBoard

End Class
