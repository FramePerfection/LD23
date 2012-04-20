Imports System.Reflection
Public Class ComponentList
    Public TypeName As String
    Public Components As New List(Of GameComponent)
    Public Type As Type

    ''' <summary>
    ''' This function creates an object with the given Type out of a given Assembly.
    ''' </summary>
    ''' <param name="TargetGame">The Game the component is being attached to.</param>
    ''' <returns>Standard GameComponent. Can be used for... anything</returns>
    ''' <remarks></remarks>
    Public Function Create(ByVal TargetGame As Game) As GameComponent
        Dim T() As Type = Assembly.GetEntryAssembly.GetTypes
        Dim out As GameComponent = Nothing
        For i As Integer = 0 To T.Length - 1
            If TypeName = T(i).FullName Then
                If Not T(i).IsAbstract Then
                    Dim info() As ConstructorInfo = T(i).GetConstructors
                    Dim used As ConstructorInfo = Nothing
                    For j As Integer = 0 To info.Length - 1
                        If used Is Nothing OrElse info(j).GetParameters.Length < used.GetParameters.Length Then
                            used = info(j)
                        End If
                    Next
                    Dim params(used.GetParameters.Length - 1) As Object
                    For j As Integer = 0 To used.GetParameters.Length - 1
                        If used.GetParameters(j).ParameterType.FullName = GetType(Game).FullName Then
                            params(j) = TargetGame
                        End If
                    Next
                    out = used.Invoke(params)
                End If
            End If
        Next
        Return out
    End Function

    Public Shared Function Create(ByVal _TypeName As String) As ComponentList
        Dim out As New ComponentList()
        out.TypeName = _TypeName
        Dim source As Assembly = Assembly.GetEntryAssembly
        For Each t As Type In source.GetTypes()
            If out.TypeName = t.FullName Then
                out.Type = t
            End If
        Next
        Return out
    End Function
    Public Function IsInheritedFrom(ByVal Basetype As Type) As Boolean
        Dim tp As Type = Type
        While tp.BaseType IsNot Nothing
            tp = tp.BaseType
            If tp.FullName = Basetype.FullName Then
                Return True
            End If
        End While
        Return False
    End Function
    Public Shared Function CreateAllLists() As List(Of ComponentList)
        Return CreateAllLists(New List(Of Assembly))
    End Function
    Public Shared Function CreateAllLists(ByVal AdditionalAssemblies As List(Of Assembly)) As List(Of ComponentList)
        Dim out As New List(Of ComponentList)
        For Each Base As Assembly In AppDomain.CurrentDomain.GetAssemblies
            AdditionalAssemblies.Add(Base)
        Next
        Dim ComponentName As String = GetType(GameComponent).FullName
        For Each current As Assembly In AdditionalAssemblies
            For Each tp As Type In current.GetTypes
                If tp IsNot Nothing Then
                    Dim base As Type = tp.BaseType
                    While base IsNot Nothing
                        If base.FullName = ComponentName Then
                            out.Add(Create(tp.FullName))
                            Exit While
                        End If
                        base = base.BaseType
                    End While
                End If
            Next
        Next
        Return out
    End Function
End Class
