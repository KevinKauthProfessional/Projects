namespace Assets.Scripts.EntLogic.SerializationObjects
{
    public static class StatementTypeEnum
    {
        public const byte Assignment = 0;
        public const byte Condition = 1;
        public const byte ConditionalLeftStatement = 2;
        public const byte LeftMethodCall = 3;
        public const byte LeftStatement = 4;
        public const byte LiteralValue = 5;
        public const byte ReadOnlyVariable = 6;
        public const byte ReadWriteVariable = 7;
        public const byte RightMethodCall = 8;
        public const byte RightStatement = 9;
        public const byte RightStatementOperation = 10;
        public const byte RootStatement = 11;
        public const byte MethodSignature = 12;
        public const int Count = 13;
    }
}
