using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Library.Crosscutting.Helper
{
    public static class ExtensionMethods
    {
        public const string UserCookieName = "LogUserInfo";

        public static string GetRandomNumber(int count)
        {
            if (count < 4)
                count = 4;
            string element = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat<string>(element, count).Select<string, char>(s => s[random.Next(s.Length)]).ToArray<char>());
        }

        public static void Copy(this object dst, object src)
        {
            foreach (PropertyInfo property1 in src.GetType().GetProperties())
            {
                if (property1.CanRead && (uint)property1.GetIndexParameters().Length <= 0U)
                {
                    PropertyInfo property2 = dst.GetType().GetProperty(property1.Name);
                    if (property2 != null && property2.CanWrite)
                        property2.SetValue(dst, property1.GetValue(src, null), null);
                }
            }
        }

        public static string FromDictionaryToJson(this Dictionary<string, string> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select<KeyValuePair<string, string>, string>(kvp => string.Format("\"{0}\":\"{1}\"", kvp.Key, "," + kvp.Value))) + "}";
        }

        public static Dictionary<string, string> FromJsonToDictionary(this string json)
        {
            return ((IEnumerable<string>)json.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Split(',')).ToDictionary<string, string, string>(item => item.Split(':')[0], item => item.Split(':')[1]);
        }

        public static List<KeyValuePair<int, string>> GetAllMonths()
        {
            List<KeyValuePair<int, string>> keyValuePairList = new List<KeyValuePair<int, string>>();
            for (int index = 1; index <= 12; ++index)
            {
                if (DateTimeFormatInfo.CurrentInfo != null)
                    keyValuePairList.Add(new KeyValuePair<int, string>(index, DateTimeFormatInfo.CurrentInfo.GetMonthName(index)));
            }
            return keyValuePairList;
        }

        public static string BuildXmlString(string xmlRootName, IEnumerable<string> values)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("<{0}>", xmlRootName);
            foreach (string str in values)
                stringBuilder.AppendFormat("<value>{0}</value>", str);
            stringBuilder.AppendFormat("</{0}>", xmlRootName);
            return stringBuilder.ToString();
        }

        public static IList<object> ListFrom<T>()
        {
            List<object> objectList = new List<object>();
            Type enumType = typeof(T);
            foreach (object obj in Enum.GetValues(enumType))
                objectList.Add(new
                {
                    Name = Enum.GetName(enumType, obj),
                    Value = obj
                });
            return objectList;
        }

        public static IEnumerable<BaseModel> GetChanges(
          this IEnumerable<BaseModel> list)
        {
            return list.Where<BaseModel>(item =>
            {
                if (!item.IsAdded && !item.IsModified)
                    return item.IsDeleted;
                return true;
            });
        }

        public static IEnumerable<BaseModel> GetChanges(
          this IEnumerable<BaseModel> list,
          ModelState itemState)
        {
            return list.Where<BaseModel>(item => item.ModelState.Equals(itemState));
        }

        public static IEnumerable<BaseModel>[] Reverse(
          this IEnumerable<BaseModel>[] list)
        {
            IEnumerable<BaseModel>[] baseModelsArray = new IEnumerable<BaseModel>[list.Length];
            int index = 0;
            for (int length = list.Length; length >= 1; --length)
            {
                baseModelsArray[index] = list[length - 1];
                ++index;
            }
            return baseModelsArray;
        }

        public static bool IsBaseModel(this Type type)
        {
            if (type.BaseType == null)
                return false;
            if (type.BaseType == typeof(BaseModel))
                return true;
            return type.BaseType.IsBaseModel();
        }

        public static void AcceptChanges<T>(this List<T> list) where T : class
        {
            if (!typeof(T).IsBaseModel())
                return;
            foreach (BaseModel baseModel in list.Cast<BaseModel>().ToArray<BaseModel>())
            {
                switch (baseModel.ModelState)
                {
                    case ModelState.Added:
                    case ModelState.Modified:
                        baseModel.SetUnchanged();
                        break;
                    case ModelState.Deleted:
                    case ModelState.Detached:
                        list.Remove((T)Convert.ChangeType(baseModel, typeof(T)));
                        break;
                }
            }
        }

        public static List<T> Copy<T>(this List<T> list)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, list);
            memoryStream.Position = 0L;
            return (List<T>)binaryFormatter.Deserialize(memoryStream);
        }

        public static bool Compare<T>(T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }

        public static bool NotEquals<T>(this T val, T compVal)
        {
            return !Compare(val, compVal);
        }

        public static bool IsZero(this long val)
        {
            return val.Equals(0L);
        }

        public static bool IsNotZero(this long val)
        {
            return !val.Equals(0L);
        }

        public static bool IsZero(this decimal val)
        {
            return val.Equals(decimal.Zero);
        }

        public static bool IsNotZero(this decimal val)
        {
            return !val.Equals(decimal.Zero);
        }

        public static bool IsZero(this double val)
        {
            return val.Equals(0.0);
        }

        public static bool IsNotZero(this double val)
        {
            return !val.Equals(0.0);
        }

        public static bool IsZero(this int val)
        {
            return val.Equals(0);
        }

        public static bool IsNotZero(this int val)
        {
            return !val.Equals(0);
        }

        public static bool IsTrue(this bool val)
        {
            return val;
        }

        public static bool IsFalse(this bool val)
        {
            return !val;
        }

        public static bool IsNull(this BaseModel obj)
        {
            return obj == null;
        }

        public static bool IsNotNull(this BaseModel obj)
        {
            return obj != null;
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        public static bool IsNullOrDbNull(this object obj)
        {
            return obj == null || obj == DBNull.Value;
        }

        public static bool IsMinValue(this DateTime obj)
        {
            return obj == DateTime.MinValue;
        }

        public static bool IsNotMinValue(this DateTime obj)
        {
            return obj != DateTime.MinValue;
        }

        public static bool IsNullOrMinValue(this DateTime? obj)
        {
            int num;
            if (obj.HasValue)
            {
                DateTime? nullable = obj;
                DateTime minValue = DateTime.MinValue;
                num = nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == minValue ? 1 : 0) : 1) : 0;
            }
            else
                num = 1;
            return num != 0;
        }

        public static bool GetBoolean(this IDataRecord reader, string columnName)
        {
            return !reader[columnName].Equals(DBNull.Value) && (bool)reader[columnName];
        }

        public static byte[] GetBytes(this DbDataRecord reader, string columnName)
        {
            return reader[columnName].Equals(DBNull.Value) ? null : (byte[])reader[columnName];
        }

        public static DateTime GetDateTime(this IDataRecord reader, string columnName)
        {
            return reader[columnName].Equals(DBNull.Value) ? DateTime.MinValue : DateTime.Parse(reader[columnName].ToString());
        }

        public static decimal GetDecimal(this IDataRecord reader, string columnName)
        {
            return reader[columnName].Equals(DBNull.Value) ? new decimal(0, 0, 0, false, 1) : (decimal)reader[columnName];
        }

        public static double GetDouble(this IDataRecord reader, string columnName)
        {
            return reader[columnName].Equals(DBNull.Value) ? 0.0 : (double)reader[columnName];
        }

        public static short GetInt16(this IDataRecord reader, string columnName)
        {
            return reader[columnName].Equals(DBNull.Value) ? (short)0 : (short)reader[columnName];
        }

        public static int GetInt32(this IDataRecord reader, string columnName)
        {
            return reader[columnName].Equals(DBNull.Value) ? 0 : (int)reader[columnName];
        }

        public static long GetInt64(this IDataRecord reader, string columnName)
        {
            return reader[columnName].Equals(DBNull.Value) ? 0L : (long)reader[columnName];
        }

        public static string GetString(this IDataRecord reader, string columnName)
        {
            return reader[columnName].ToString();
        }

        public static void CloseReader(this IDataReader reader)
        {
            if (!reader.IsNotNull() || reader.IsClosed)
                return;
            reader.Close();
        }

        public static bool IsNullable<T>(this T obj)
        {
            if (((object)obj).IsNull())
                return true;
            Type nullableType = typeof(T);
            if (!nullableType.IsValueType)
                return true;
            return Nullable.GetUnderlyingType(nullableType).IsNotNull();
        }

        public static bool IsNullable(this Type type)
        {
            if (type.IsNull() || !type.IsValueType)
                return true;
            return Nullable.GetUnderlyingType(type).IsNotNull();
        }

        public static T MapField<T>(this object value)
        {
            if (value.GetType() == typeof(T))
                return (T)value;
            if (value == DBNull.Value)
                return default(T);
            Type conversionType = Nullable.GetUnderlyingType(typeof(T));
            if (conversionType.IsNull())
                conversionType = typeof(T);
            return (T)Convert.ChangeType(value, conversionType);
        }

        public static object MapField(this object value, Type type)
        {
            if (value.GetType() == type)
                return value;
            if (value == DBNull.Value)
                return null;
            Type conversionType = Nullable.GetUnderlyingType(type);
            if (conversionType.IsNull())
                conversionType = type;
            return Convert.ChangeType(value, conversionType);
        }

        public static DateTime MapTimeField(this object value)
        {
            if (value != DBNull.Value)
                return DateTime.MinValue.Add((TimeSpan)value);
            if (value.IsNullable<object>())
                return new DateTime();
            throw new NullReferenceException();
        }

        public static DateTime? MapTimeNullableField(this object value)
        {
            if (value != DBNull.Value)
                return new DateTime?(DateTime.MinValue.Add((TimeSpan)value));
            if (value.IsNullable<object>())
                return new DateTime?();
            throw new NullReferenceException();
        }

        public static object Value(this object value)
        {
            return value ?? DBNull.Value;
        }

        public static object Value(this DateTime dateTime, string format = "dd-MMM-yyyy")
        {
            if (dateTime.Equals(DateTime.MinValue))
                return DBNull.Value;
            return dateTime.ToString(format);
        }

        public static object Value(this DateTime? dateTime, string format = "dd-MMM-yyyy")
        {
            if (!dateTime.HasValue || dateTime.Equals(DateTime.MinValue))
                return DBNull.Value;
            return Convert.ToDateTime(dateTime).ToString(format);
        }

        public static string StringValue(this DateTime dateTime)
        {
            return dateTime.Equals(DateTime.MinValue) ? string.Empty : dateTime.ToShortDateString();
        }

        public static string StringValue(this DateTime? dateTime)
        {
            return !dateTime.HasValue ? string.Empty : Convert.ToDateTime(dateTime).ToShortDateString();
        }

        public static int GetAge(this DateTime dateOfBirth)
        {
            DateTime today = DateTime.Today;
            return ((today.Year * 100 + today.Month) * 100 + today.Day - ((dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day)) / 10000;
        }

        public static void Sort<T, TU>(
          this List<T> list,
          Func<T, TU> expression,
          IComparer<TU> comparer)
          where TU : IComparable<TU>
        {
            list.Sort((x, y) => comparer.Compare(expression(x), expression(y)));
        }

        public static Type GetTypeFromName(this string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));
            bool flag1 = false;
            bool flag2 = false;
            if (typeName.IndexOf("[]", StringComparison.Ordinal) != -1)
            {
                flag1 = true;
                typeName = typeName.Remove(typeName.IndexOf("[]", StringComparison.Ordinal), 2);
            }
            if (typeName.IndexOf("?", StringComparison.Ordinal) != -1)
            {
                flag2 = true;
                typeName = typeName.Remove(typeName.IndexOf("?", StringComparison.Ordinal), 1);
            }
            typeName = typeName.ToLower();
            string typeName1 = null;
            switch (typeName)
            {
                case "bool":
                case "boolean":
                    typeName1 = "System.bool";
                    break;
                case "byte":
                    typeName1 = "System.Byte";
                    break;
                case "char":
                    typeName1 = "System.Char";
                    break;
                case "datetime":
                    typeName1 = "System.DateTime";
                    break;
                case "datetimeoffset":
                    typeName1 = "System.DateTimeOffset";
                    break;
                case "decimal":
                    typeName1 = "System.Decimal";
                    break;
                case "double":
                    typeName1 = "System.Double";
                    break;
                case "float":
                    typeName1 = "System.Single";
                    break;
                case "int":
                case "int32":
                    typeName1 = "System.int";
                    break;
                case "int16":
                case "short":
                    typeName1 = "System.Int16";
                    break;
                case "int64":
                case "long":
                    typeName1 = "System.Int64";
                    break;
                case "object":
                    typeName1 = "System.object";
                    break;
                case "sbyte":
                    typeName1 = "System.SByte";
                    break;
                case "string":
                    typeName1 = "System.string";
                    break;
                case "timespan":
                    typeName1 = "System.TimeSpan";
                    break;
                case "uint":
                case "uint32":
                    typeName1 = "System.UInt32";
                    break;
                case "uint16":
                case "ushort":
                    typeName1 = "System.UInt16";
                    break;
                case "uint64":
                case "ulong":
                    typeName1 = "System.UInt64";
                    break;
            }
            if (typeName1 != null)
            {
                if (flag1)
                    typeName1 += "[]";
                if (flag2)
                    typeName1 = "System.Nullable`1[" + typeName1 + "]";
            }
            else
                typeName1 = typeName;
            return Type.GetType(typeName1);
        }
    }
}