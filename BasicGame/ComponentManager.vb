Imports System.Reflection
Public Class ComponentManager
    Public Shared Function IsInheritedFrom(ByVal TestObject As Object, ByVal BaseClass As Type) As Boolean
        Dim tp As Type = TestObject.GetType.BaseType
        Return IsInheritedFrom(tp, BaseClass)
    End Function

    Public Shared Function IsInheritedFrom(ByVal TestType As Type, ByVal BaseClass As Type) As Boolean
        While TestType IsNot Nothing
            If BaseClass.FullName = TestType.FullName Then
                Return True
            End If
            TestType = TestType.BaseType
        End While
        Return False
    End Function

    ''' <summary>
    ''' This Class provides information about Events an Object can call to influence others.
    ''' </summary>
    ''' 
    Public Class ObjEvent
        ''' <summary>
        ''' This is the information about the Methods the Objects shall execute when this Event is called.
        ''' </summary>
        Public TargetMethods As New List(Of MethodInfo)
        ''' <summary>
        ''' This is the information about the Objects that shall react to this Event.
        ''' </summary>
        Public TargetObjects As New List(Of GameComponent)
        ''' <summary>
        ''' This is the information about the Parameters the Objects' Methods shall be executed with.
        ''' </summary>
        Public TargetParams As New List(Of Object())
        ''' <summary>
        ''' This is the information about the Parameters the Objects' Methods shall be executed with.
        ''' </summary>
        Public Senders As New List(Of GameComponent())

        ''' <summary>
        ''' Adds an Object to the List of Objects reacting to this Event.
        ''' </summary>
        ''' <param name="TargetObject">The Object to be added</param>
        ''' <param name="TargetMethod">The Method this Object shall perform</param>
        ''' <param name="params">The parameters for this Method</param>
        Public Sub AddEvent(ByVal TargetObject As GameComponent, ByVal TargetMethod As String, ByVal params() As Object, ByVal _senders() As GameComponent)
            Dim tp As Type = TargetObject.GetType()

            Dim Methods() As MethodInfo = tp.GetMethods()
            Dim Used As MethodInfo = Nothing

            For Each Method As MethodInfo In Methods
                If Method.Name = TargetMethod Then
                    Dim args() As ParameterInfo = Method.GetParameters()
                    If args.Length < params.Length Then
                        GoTo NextObject
                    End If
                    For i As Integer = 0 To args.Length - 1
                        If i >= params.Length Then
                            If Not args(i).IsOptional Then
                                GoTo NextObject
                            End If
                        Else
                            If params(i) IsNot Nothing AndAlso (params(i).GetType.FullName <> args(i).ParameterType.FullName AndAlso Not IsInheritedFrom(params(i), args(i).ParameterType)) Then
                                'MsgBox(params(i).GetType.Name & vbCrLf & args(i).GetType.Name)
                                'MsgBox(IsInheritedFrom(params(i), args(i).GetType))
                                GoTo NextObject
                            End If
                        End If
                    Next
                Else
                    GoTo NextObject
                End If
                Used = Method
                Exit For
NextObject:
            Next

            If Used IsNot Nothing Then
                TargetObjects.Add(TargetObject)
                TargetMethods.Add(Used)
                TargetParams.Add(params)
                Senders.Add(_senders)
            Else
                Dim errArgs As String = "("
                For i As Integer = 0 To params.Length - 1
                    errArgs &= params(i).GetType.Name
                    If i < params.Length - 1 Then
                        errArgs &= ", "
                    End If
                Next
                MsgBox("No suitable Method found: " & TargetObject.GetType.Name & "." & TargetMethod & errArgs & ")")
            End If
        End Sub

        ''' <summary>
        ''' Removes the Event given by its Targetobject and the Method that shall be removed.
        ''' If no Method is given, all Events reffered to this Object will be removed
        ''' </summary>
        ''' <param name="TargetObject">The Object from which to remove the event</param>
        ''' <param name="TargetMethod">The Method to remove</param>
        ''' <remarks></remarks>
        Public Sub RemoveEvent(ByVal TargetObject As GameComponent, ByVal TargetMethod As String)
            Dim i As Integer = 0
            While i < TargetObjects.Count
                If TargetObject Is TargetObjects(i) And (TargetMethod = TargetMethods(i).Name Or TargetMethod = "") Then
                    TargetObjects.Remove(TargetObjects(i))
                    TargetMethods.Remove(TargetMethods(i))
                    TargetParams.Remove(TargetParams(i))
                    i -= 1
                End If
                i += 1
            End While
        End Sub

        ''' <summary>
        ''' Handles this Event. This causes all Objects in this Event to execute their Method(s).
        ''' </summary>
        Public Sub Invoke(ByVal _Senders() As GameComponent)
            For i As Integer = 0 To TargetMethods.Count - 1
                Dim Valid As Boolean = True
                If _Senders.Length <> Senders(i).Length Then
                    Valid = False
                Else
                    For j As Integer = 0 To Senders(i).Length - 1
                        If Senders(i)(j) IsNot _Senders(j) Then
                            Valid = False
                        End If
                    Next
                End If
                If Valid Then
                    TargetMethods(i).Invoke(TargetObjects(i), TargetParams(i))
                End If
            Next
        End Sub

        ''' <summary>
        ''' Invokes a single Object found in this Event with different parameters.
        ''' </summary>
        ''' <param name="Obj">The Object to handle</param>
        ''' <param name="newParams">The new parameters</param>
        Public Sub InvokeSingleObject(ByVal Obj As GameComponent, ByVal newParams() As Object)
            Try
                For i As Integer = 0 To TargetObjects.Count - 1
                    If TargetObjects(i) Is Obj Then
                        TargetMethods(i).Invoke(TargetObjects(i), newParams)
                        Exit Sub
                    End If
                Next
            Catch ex As Exception
                MsgBox("An error occured while handling Event with different parameters")
            End Try
        End Sub
    End Class
End Class
