''' <summary>
''' Provides a base class for Games.
''' </summary>
''' <remarks>This class is abstract</remarks>
Public MustInherit Class Game
    ''' <summary>
    ''' Defines wether the Game is valid. As soon as this Variable is false, the Game will end.
    ''' </summary>
    Public Shared Valid As Boolean = True

    Public GameTime As GameTime

    ''' <summary>
    ''' Runs the Game.
    ''' </summary>
    Public Shared GameSpeed As Single = 1
    Public Shared FPS As Single = 60
    Public Shared VIS As Single = 60
    Public Sub Run()
        Load()
        GameTime = GameTime.Default
        Dim TimeToProcess As Single
        Dim TimeToDraw As Single
        Dim tm As New Stopwatch
        While Valid
            tm.Reset()
            tm.Start()
            Application.DoEvents()
            If valid Then
                If TimeToDraw <= 0 Then
                    TimeToDraw += 1 / VIS
                    Draw(GameTime)
                End If
            End If
            If TimeToProcess > 0.5 Then
                TimeToProcess = 0
            End If
            While TimeToProcess > GameTime.ElapsedRealTime
                TimeToProcess -= GameTime.ElapsedRealTime
                Update(GameTime)
                GameTime.ElapsedRealTime = 1 / FPS
                GameTime.ElapsedTime = GameSpeed * GameTime.ElapsedRealTime
                GameTime.TotalElapsedTime += GameTime.ElapsedTime
                GameTime.TotalElapsedRealTime += GameTime.ElapsedRealTime
            End While
            While tm.ElapsedTicks < 3
                Application.DoEvents()
            End While
            tm.Stop()
            TimeToProcess += tm.ElapsedTicks / Stopwatch.Frequency
            TimeToDraw -= tm.ElapsedTicks / Stopwatch.Frequency
        End While
        Unload()
    End Sub
    ''' <summary>
    ''' Loads the Game
    ''' </summary>
    ''' <remarks>This is a TODO</remarks>
    Public MustOverride Sub Load()
    ''' <summary>
    ''' Unloads the Game. Destroy all Objects here.
    ''' </summary>
    ''' <remarks>This is a TODO</remarks>
    Public MustOverride Sub Unload()
    ''' <summary>
    ''' Updates the Game. Insert Update-Logic here.
    ''' </summary>
    ''' <remarks>This is a TODO</remarks>
    Public MustOverride Sub Update(ByVal ftime As GameTime)
    ''' <summary>
    ''' Renders the Game. Insert Drawing-Logic here.
    ''' </summary>
    ''' <remarks>This is a TODO</remarks>
    Public MustOverride Sub Draw(ByVal ftime As GameTime)
End Class

''' <summary>
''' This structure is used to handle time-problems that occur with simple Timer-Updates
''' </summary>
''' <remarks></remarks>
Public Structure GameTime
    Public ElapsedTime As Single
    Public TotalElapsedTime As Single
    Public ElapsedRealTime As Single
    Public TotalElapsedRealTime As Single
    Public Shared ReadOnly Property [Default] As GameTime
        Get
            Return New GameTime
        End Get
    End Property
    Public Sub Reset()
        TotalElapsedTime = 0
        TotalElapsedRealTime = 0
    End Sub
End Structure
