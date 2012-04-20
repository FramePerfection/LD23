Public Class LightSetting
    Inherits GameComponent
    Public Sub New(ByVal g As Game)
        MyBase.New(g, Integer.MinValue)
    End Sub
    Public Shared Current As LightSetting
    Public Lights As New List(Of Light)
    Private AmbientColor As Vector4
    Public ShadowEnable As Boolean = False
    Public Shared LightingEffect As Effect
    Public Sub ApplyChanges()
        For Each l As Light In Lights
            If l.Enabled Then
                AmbientColor += New Vector4(l.Ambient.R, l.Ambient.G, l.Ambient.B, l.Ambient.A) * l.Multiply
            End If
        Next
        AmbientColor *= (1 / 255)
        Current = Me
    End Sub
    Public Sub Enable(ByVal LightIndex As Integer)
        If Game.Valid Then
            'MsgBox(Model.AllEffects.Find("Advanced PPL") Is Nothing)
            LightingEffect.SetValue(EffectHandle.FromString("LightType"), Lights(LightIndex).Type)
            LightingEffect.SetValue(EffectHandle.FromString("TotalLightAmbient"), AmbientColor)
            LightingEffect.SetValue(EffectHandle.FromString("LightDiffuse"), ToV4(Lights(LightIndex).Diffuse))
            LightingEffect.SetValue(EffectHandle.FromString("LightSpecular"), ToV4(Lights(LightIndex).Specular))
            LightingEffect.SetValue(EffectHandle.FromString("LightDirection"), ToV4(Lights(LightIndex).Direction))
            LightingEffect.SetValue(EffectHandle.FromString("LightPosition"), ToV4(Lights(LightIndex).Position))
            LightingEffect.SetValue(EffectHandle.FromString("LightRange"), Lights(LightIndex).Range)
            LightingEffect.SetValue(EffectHandle.FromString("multiply"), Lights(LightIndex).Multiply)
            If ShadowEnable Then
                Try
                    For i As Short = 0 To 5
                        LightingEffect.SetValue(EffectHandle.FromString("ShadowMap"), Lights(LightIndex).ShadowMap)
                    Next
                Catch ex As Exception

                End Try
            End If
        End If
    End Sub
    Public Sub Enable(ByVal LightIndex As Integer, ByVal TargetEffect As Effect)
        TargetEffect.SetValue(EffectHandle.FromString("LightType"), Lights(LightIndex).Type)
        TargetEffect.SetValue(EffectHandle.FromString("TotalLightAmbient"), AmbientColor)
        TargetEffect.SetValue(EffectHandle.FromString("LightDiffuse"), ToV4(Lights(LightIndex).Diffuse))
        TargetEffect.SetValue(EffectHandle.FromString("LightSpecular"), ToV4(Lights(LightIndex).Specular))
        TargetEffect.SetValue(EffectHandle.FromString("LightDirection"), ToV4(Lights(LightIndex).Direction))
        TargetEffect.SetValue(EffectHandle.FromString("LightPosition"), ToV4(Lights(LightIndex).Position))
        TargetEffect.SetValue(EffectHandle.FromString("LightRange"), Lights(LightIndex).Range)
        TargetEffect.SetValue(EffectHandle.FromString("multiply"), Lights(LightIndex).Multiply)
    End Sub
    Public Shared Function ToV4(ByVal Input As Color) As Vector4
        Return New Vector4(Input.R, Input.G, Input.B, Input.A) * (1 / 255)
    End Function
    Public Shared Function ToV4(ByVal Input As Vector3) As Vector4
        Return New Vector4(Input.X, Input.Y, Input.Z, 0)
    End Function
    Friend Shared AllShadowMaps As New List(Of CubeTexture)
    Friend Shared ShadowMapUse As New List(Of Boolean)
    Public Class Light
        Public Ambient As Color
        Public Diffuse As Color
        Public Specular As Color
        Public Direction As Vector3
        Public Position As Vector3
        Public Range As Single
        Public Type As LightType
        Public Enabled As Boolean
        Public Multiply As Single = 1
        Public ShadowMap As CubeTexture
        Public Shared ShadowMapping As Effect
        Private ShadowMapDir As New List(Of Vector3)
        Private ShadowMapUp As New List(Of Vector3)
        Private numShadowMaps As Integer = 0
        Private Shared BaseMaps(5) As Texture
        Private Shared BlurVerts() As CustomVertex.PositionOnly = New CustomVertex.PositionOnly() {New CustomVertex.PositionOnly(-1, -1, 1), _
                                                                                                     New CustomVertex.PositionOnly(1, -1, 1), _
                                                                                                     New CustomVertex.PositionOnly(1, 1, 1), _
                                                                                                     New CustomVertex.PositionOnly(-1, 1, 1)}
        Private Shared ShadowRes As Integer = 1024
        Public Sub New(ByVal dir As Vector3, ByVal Amb As Color, ByVal Dif As Color, ByVal Spec As Color)
            Type = LightType.Directional
            Direction = dir
            Ambient = Amb : Diffuse = Dif : Specular = Spec
            Enabled = True
            If ShadowMapping Is Nothing Then
                ShadowMapping = Model.AllEffects.Find("CreateShadowMap")
            End If
            If AllShadowMaps.Count > 7 Then
                ShadowMap = ReUseShadowMap()
            Else
                ShadowMap = New CubeTexture(Game.Device, ShadowRes, 1, Usage.RenderTarget, Format.A16B16G16R16, Pool.Default)
                AllShadowMaps.Add(ShadowMap)
                ShadowMapUse.Add(True)
            End If
            CreateDirections()
        End Sub
        Public Sub New(ByVal pos As Vector3, ByVal pow As Single, ByVal Amb As Color, ByVal Dif As Color, ByVal Spec As Color)
            Type = LightType.Point
            Position = pos
            Range = pow
            Ambient = Amb : Diffuse = Dif : Specular = Spec
            Enabled = True
            If ShadowMapping Is Nothing Then
                ShadowMapping = Model.AllEffects.Find("CreateShadowMap")
            End If
            If AllShadowMaps.Count > 7 Then
                ShadowMap = ReUseShadowMap()
            Else
                ShadowMap = New CubeTexture(Game.Device, ShadowRes, 1, Usage.RenderTarget, Format.A16B16G16R16, Pool.Default)
                AllShadowMaps.Add(ShadowMap)
                ShadowMapUse.Add(True)
            End If
            CreateDirections()
        End Sub
        Private Sub CreateDirections()
            ShadowMapDir.Add(New Vector3(1, 0, 0)) : ShadowMapUp.Add(New Vector3(0, 1, 0))
            ShadowMapDir.Add(New Vector3(-1, 0, 0)) : ShadowMapUp.Add(New Vector3(0, 1, 0))
            ShadowMapDir.Add(New Vector3(0, 1, 0)) : ShadowMapUp.Add(New Vector3(0, 0, -1))
            ShadowMapDir.Add(New Vector3(0, -1, 0)) : ShadowMapUp.Add(New Vector3(0, 0, 1))
            ShadowMapDir.Add(New Vector3(0, 0, 1)) : ShadowMapUp.Add(New Vector3(0, 1, 0))
            ShadowMapDir.Add(New Vector3(0, 0, -1)) : ShadowMapUp.Add(New Vector3(0, 1, 0))
        End Sub
        Public Sub DrawShadowMaps(Optional ByVal Enable As Boolean = True)
            If ShadowMap IsNot Nothing Then
                If Enable Then
                    ShadowMapping.Begin(FX.None)
                End If
                Dim b As Surface = Game.Device.GetRenderTarget(0)
                For i As Short = 0 To 5
                    Game.Device.Clear(ClearFlags.Target Or ClearFlags.ZBuffer Or ClearFlags.Stencil, Color.White.ToArgb, 1, 0)
                    If Enable Then
                        ShadowMapping.BeginPass(0)
                        Dim View As Matrix = Matrix.LookAtLH(Position, Position + ShadowMapDir(i), ShadowMapUp(i))
                        Dim Proj As Matrix = Matrix.PerspectiveFovLH(Math.PI / 2, 1, 0.005, 1000)
                        Game.Device.SetRenderTarget(0, ShadowMap.GetCubeMapSurface(i, 0))
                        'ShadowMapping.SetValue(EffectHandle.FromString("xLightProjection"), Matrix.PerspectiveFovLH(Math.PI / 4, 1, 0.05, 45))
                        For Each mdl As Model In Model.AllModels
                            'MsgBox(VP.ToString & mdl.Transform.ToString)
                            ShadowMapping.SetValue(EffectHandle.FromString("xLightWorldView"), mdl.Transform * View)
                            ShadowMapping.SetValue(EffectHandle.FromString("xLightWorldViewProjection"), mdl.Transform * View * Proj)
                            ShadowMapping.CommitChanges()
                            mdl.DrawToShadowMap()
                        Next
                        ShadowMapping.EndPass()


                        'ShadowMapping.BeginPass(1)
                        'ShadowMapping.SetValue(EffectHandle.FromString("MapToBlur"), BaseMaps(i))
                        'ShadowMapping.CommitChanges()
                        'Game.Device.DrawUserPrimitives(PrimitiveType.TriangleFan, 2, BlurVerts)
                        'ShadowMapping.EndPass()

                        'Game.Device.SetRenderTarget(0, ShadowMap.GetCubeMapSurface(i, 0))
                        'Game.Device.Clear(ClearFlags.Target Or ClearFlags.ZBuffer Or ClearFlags.Stencil, Color.Blue.ToArgb, 1, 0)

                        'ShadowMapping.BeginPass(2)
                        'ShadowMapping.SetValue(EffectHandle.FromString("MapToBlur"), BaseMaps(i))
                        'ShadowMapping.CommitChanges()
                        'Game.Device.DrawUserPrimitives(PrimitiveType.TriangleFan, 2, BlurVerts)
                        'ShadowMapping.EndPass()
                    End If
                Next
                If Enable Then
                    ShadowMapping.End()
                End If
            End If
        End Sub

        Public Sub ReleaseData()
            For i As Short = 0 To AllShadowMaps.Count - 1
                If AllShadowMaps(i) Is ShadowMap Then
                    ShadowMapUse(i) = False
                End If
            Next
        End Sub
        Private Function ReUseShadowMap() As CubeTexture
            For i As Short = 0 To AllShadowMaps.Count - 1
                If Not ShadowMapUse(i) Then
                    ShadowMapUse(i) = True
                    Return AllShadowMaps(i)
                End If
            Next
            Return Nothing
        End Function
    End Class

    Public Overrides Sub Draw(ByVal ftime As Ultima_GameEngine.GameTime)
        If ShadowEnable Then
            Dim b As Surface = Game.Device.GetRenderTarget(0)
            Game.Device.SetSamplerState(1, SamplerStageStates.DMapOffset, 1)
            Game.Device.SetSamplerState(1, SamplerStageStates.MagFilter, Filter.Box)
            For Each l As Light In Lights
                l.DrawShadowMaps(ShadowEnable)
            Next
            Game.Device.SetRenderTarget(0, b)
        End If
    End Sub

    Public Overrides Sub PerformUpdate(ByVal ftime As Ultima_GameEngine.GameTime)

    End Sub

    Public Overrides Function Relevant() As Boolean
        Return Current Is Me
    End Function
End Class
