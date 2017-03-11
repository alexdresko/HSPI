Module Classes

    ' ==========================================================================
    ' ==========================================================================
    ' ==========================================================================
    '       These class objects are used to hold plug-in specific information 
    '   about its various triggers and actions.  If there is no information 
    '   needed other than the Trigger/Action number and/or the SubTrigger
    '   /SubAction number, then these are not needed as they are intended to 
    '   store additional information beyond those selection values.  The UID
    '   (Unique Trigger ID or Unique Action ID) can be used as the key to the
    '   storage of these class objects when the plug-in is running.  When the 
    '   plug-in is not running, the serialized copy of these classes is stored
    '   and restored by HomeSeer.
    ' ==========================================================================
    ' ==========================================================================
    ' ==========================================================================


    <Serializable()> _
    Friend Class MyAction1EvenTon

        Private mvarUID As Integer
        Public Property ActionUID As Integer
            Get
                Return mvarUID
            End Get
            Set(value As Integer)
                mvarUID = value
            End Set
        End Property

        Private mvarConfigured As Boolean
        Public ReadOnly Property Configured As Boolean
            Get
                Return mvarConfigured
            End Get
        End Property

        <Serializable()> _
        Friend Enum eSetTo
            Not_Set = 0
            Rounded = 1
            Unrounded = 2
        End Enum

        Private mvarSet As eSetTo
        Public Property SetTo As eSetTo
            Set(value As eSetTo)
                mvarConfigured = True
                mvarSet = value
            End Set
            Get
                If Not mvarConfigured Then Return eSetTo.Not_Set
                Return mvarSet
            End Get
        End Property
    End Class

    <Serializable()> _
    Friend Class MyAction2Euro

        Private mvarUID As Integer
        Public Property ActionUID As Integer
            Get
                Return mvarUID
            End Get
            Set(value As Integer)
                mvarUID = value
            End Set
        End Property

        Private mvarConfigured As Boolean
        Public ReadOnly Property Configured As Boolean
            Get
                Return mvarConfigured
            End Get
        End Property

        <Serializable()> _
        Friend Enum eVAction
            Not_Set = 0
            SetEuro = 1
            SetNorthAmerica = 2
            ResetAverage = 3
        End Enum

        Private mvarSet As eVAction
        Public Property ThisAction As eVAction
            Set(value As eVAction)
                mvarConfigured = True
                mvarSet = value
            End Set
            Get
                If Not mvarConfigured Then Return eVAction.Not_Set
                Return mvarSet
            End Get
        End Property
    End Class

    <Serializable()> _
    Friend Class MyTrigger1Ton

        Private mvarUID As Integer
        Public Property TriggerUID As Integer
            Get
                Return mvarUID
            End Get
            Set(value As Integer)
                mvarUID = value
            End Set
        End Property

        Private mvarTriggerWeight As Double
        Public Property TriggerWeight As Double
            Get
                Return mvarTriggerWeight
            End Get
            Set(value As Double)
                mvarTriggerWeight = value
            End Set
        End Property

        Private mvarCondition As Boolean
        Public Property Condition As Boolean
            Get
                Return mvarCondition
            End Get
            Set(value As Boolean)
                mvarCondition = value
            End Set
        End Property

        'Private mvarWeight As Double
        'Public Property Weight As Double
        '    Get
        '        Return mvarWeight
        '    End Get
        '    Set(value As Double)
        '        mvarWeight = value
        '    End Set
        'End Property

        Private mvarEvenTon As Boolean
        Public Property EvenTon As Boolean
            Get
                Return mvarEvenTon
            End Get
            Set(value As Boolean)
                mvarEvenTon = value
            End Set
        End Property

      
    End Class

    <Serializable()> _
    Friend Class MyTrigger2Shoe

        Private mvarSubTrig2 As Boolean = False

        Private mvarUID As Integer
        Public Property TriggerUID As Integer
            Get
                Return mvarUID
            End Get
            Set(value As Integer)
                mvarUID = value
            End Set
        End Property

        Public Property SubTrigger2 As Boolean
            Get
                Return mvarSubTrig2
            End Get
            Set(value As Boolean)
                mvarSubTrig2 = value
            End Set
        End Property

        Private mvarCondition As Boolean
        Public Property Condition As Boolean
            Get
                Return mvarCondition
            End Get
            Set(value As Boolean)
                mvarCondition = value
            End Set
        End Property
        Private mvarTriggerValue As Double
        Public Property TriggerValue As Double
            Get
                Return mvarTriggerValue
            End Get
            Set(value As Double)
                mvarTriggerValue = value
            End Set
        End Property

        Private mvarVoltTypeEuro As Boolean
        Public Property EuroVoltage As Boolean
            Get
                Return mvarVoltTypeEuro
            End Get
            Set(value As Boolean)
                mvarVoltTypeEuro = value
            End Set
        End Property
        Public Property NorthAMVoltage As Boolean
            Get
                Return Not mvarVoltTypeEuro
            End Get
            Set(value As Boolean)
                mvarVoltTypeEuro = Not value
            End Set
        End Property


       


    End Class


End Module
