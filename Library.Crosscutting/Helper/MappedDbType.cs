using System;
using System.Collections.Generic;
using System.Data;

namespace Library.Crosscutting.Helper
{
    public static class MappedDbType
    {
        private static Dictionary<Type, DbType> typeMap;

        private static void InitializeDbType()
        {
            Dictionary<Type, DbType> dictionary = new Dictionary<Type, DbType>();
            Type index1 = typeof(DBNull);
            dictionary[index1] = DbType.AnsiString;
            Type index2 = typeof(byte);
            dictionary[index2] = DbType.Byte;
            Type index3 = typeof(sbyte);
            dictionary[index3] = DbType.SByte;
            Type index4 = typeof(short);
            dictionary[index4] = DbType.Int16;
            Type index5 = typeof(ushort);
            dictionary[index5] = DbType.UInt16;
            Type index6 = typeof(int);
            dictionary[index6] = DbType.Int32;
            Type index7 = typeof(uint);
            dictionary[index7] = DbType.UInt32;
            Type index8 = typeof(long);
            dictionary[index8] = DbType.Int64;
            Type index9 = typeof(ulong);
            dictionary[index9] = DbType.UInt64;
            Type index10 = typeof(float);
            dictionary[index10] = DbType.Single;
            Type index11 = typeof(double);
            dictionary[index11] = DbType.Double;
            Type index12 = typeof(decimal);
            dictionary[index12] = DbType.Decimal;
            Type index13 = typeof(bool);
            dictionary[index13] = DbType.Boolean;
            Type index14 = typeof(string);
            dictionary[index14] = DbType.AnsiString;
            Type index15 = typeof(char);
            dictionary[index15] = DbType.StringFixedLength;
            Type index16 = typeof(Guid);
            dictionary[index16] = DbType.Guid;
            Type index17 = typeof(DateTime);
            dictionary[index17] = DbType.DateTime;
            Type index18 = typeof(DateTimeOffset);
            dictionary[index18] = DbType.DateTimeOffset;
            Type index19 = typeof(byte[]);
            dictionary[index19] = DbType.Binary;
            Type index20 = typeof(byte?);
            dictionary[index20] = DbType.Byte;
            Type index21 = typeof(sbyte?);
            dictionary[index21] = DbType.SByte;
            Type index22 = typeof(short?);
            dictionary[index22] = DbType.Int16;
            Type index23 = typeof(ushort?);
            dictionary[index23] = DbType.UInt16;
            Type index24 = typeof(int?);
            dictionary[index24] = DbType.Int32;
            Type index25 = typeof(uint?);
            dictionary[index25] = DbType.UInt32;
            Type index26 = typeof(long?);
            dictionary[index26] = DbType.Int64;
            Type index27 = typeof(ulong?);
            dictionary[index27] = DbType.UInt64;
            Type index28 = typeof(float?);
            dictionary[index28] = DbType.Single;
            Type index29 = typeof(double?);
            dictionary[index29] = DbType.Double;
            Type index30 = typeof(decimal?);
            dictionary[index30] = DbType.Decimal;
            Type index31 = typeof(bool?);
            dictionary[index31] = DbType.Boolean;
            Type index32 = typeof(char?);
            dictionary[index32] = DbType.StringFixedLength;
            Type index33 = typeof(Guid?);
            dictionary[index33] = DbType.Guid;
            Type index34 = typeof(DateTime?);
            dictionary[index34] = DbType.DateTime;
            Type index35 = typeof(DateTimeOffset?);
            dictionary[index35] = DbType.DateTimeOffset;
            typeMap = dictionary;
        }

        public static DbType GetDbType(Type appType)
        {
            if (typeMap.IsNull())
                InitializeDbType();
            DbType type = typeMap[appType];
            return type.IsNull() ? DbType.AnsiString : type;
        }
    }
}