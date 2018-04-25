using LiveSplit.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace LiveSplit.Web
{
    public static class JSON
    {
        public static dynamic FromResponse(WebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                return FromStream(stream);
            }
        }

        public static dynamic FromStream(Stream stream)
        {
            var reader = new StreamReader(stream);
            var json = "";
            try
            {
                json = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return FromString(json);
        }

        public static dynamic FromString(string value)
        {
            var serializer = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            };
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            return serializer.Deserialize<object>(value);
        }

        public static dynamic FromUri(Uri uri)
        {
            var request = WebRequest.Create(uri);

            using (var response = request.GetResponse())
            {
                return FromResponse(response);
            }
        }

        public static string Escape(string value)
        {
            return HttpUtility.JavaScriptStringEncode(value);
        }

        public static dynamic FromUriPost(Uri uri, params string[] postValues)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/json";

            var parameters = new StringBuilder();

            parameters.Append("{");

            for (var i = 0; i < postValues.Length; i += 2)
            {
                parameters.AppendFormat("\"{0}\": \"{1}\", ",
                    Escape(postValues[i]),
                    Escape(postValues[i + 1]));
            }

            parameters.Length -= 2;

            parameters.Append("}");

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(parameters.ToString());
            }

            using (var response = request.GetResponse())
            {
                return FromResponse(response);
            }
        }
    }

    public sealed class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
        }
    }

    public sealed class DynamicJsonObject : DynamicObject
    {
        private readonly IDictionary<string, object> _dictionary;

        //public IDictionary<string, object> Properties { get { return _dictionary; } }

        public DynamicJsonObject()
            : this(new Dictionary<string, object>()) 
        { }

        public DynamicJsonObject(IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            _dictionary = dictionary;
        }

        public override string ToString()
        {
            var sb = new StringBuilder("{\r\n");
            ToString(sb);
            return sb.ToString();
        }

        private void ToString(StringBuilder sb, int depth = 1)
        {
            var firstInDictionary = true;
            foreach (var pair in _dictionary)
            {
                if (!firstInDictionary)
                    sb.Append(",\r\n");
                sb.Append('\t', depth);
                firstInDictionary = false;
                var value = pair.Value;
                var name = pair.Key;
                if (value == null)
                {
                    sb.AppendFormat("\"{0}\": {1}", HttpUtility.JavaScriptStringEncode(name), "null");
                }
                else if (value is string)
                {
                    sb.AppendFormat("\"{0}\": \"{1}\"", HttpUtility.JavaScriptStringEncode(name), HttpUtility.JavaScriptStringEncode((string)value));
                }
                else if (value is DynamicJsonObject)
                {
                    sb.Append("\"" + HttpUtility.JavaScriptStringEncode(name) + "\": {\r\n");
                    ((DynamicJsonObject)value).ToString(sb, depth + 1);
                }
                else if (value is IDictionary<string, object>)
                {
                    sb.Append("\"" + HttpUtility.JavaScriptStringEncode(name) + "\": {\r\n");
                    new DynamicJsonObject((IDictionary<string, object>)value).ToString(sb, depth + 1);
                }
                else if (value is IEnumerable<object>)
                {
                    sb.Append("\"" + HttpUtility.JavaScriptStringEncode(name) + "\": [\r\n");
                    var firstInArray = true;
                    foreach (var arrayValue in (IEnumerable<object>)value)
                    {
                        if (!firstInArray)
                            sb.Append(",\r\n");
                        sb.Append('\t', depth+1);
                        firstInArray = false;
                        if (arrayValue is IDictionary<string, object>)
                            new DynamicJsonObject((IDictionary<string, object>)arrayValue).ToString(sb, depth + 2);
                        else if (arrayValue is DynamicJsonObject)
                        {
                            sb.Append("{\r\n");
                            ((DynamicJsonObject)arrayValue).ToString(sb, depth + 2);
                        }
                        else if (arrayValue is string)
                            sb.AppendFormat("\"{0}\"", HttpUtility.JavaScriptStringEncode((string)arrayValue));
                        else if (arrayValue is decimal)
                            sb.AppendFormat("{0}", HttpUtility.JavaScriptStringEncode(((decimal)arrayValue).ToString(CultureInfo.InvariantCulture)));
                        else
                            sb.AppendFormat("\"{0}\"", HttpUtility.JavaScriptStringEncode((arrayValue ?? "").ToString()));

                    }
                    sb.Append("\r\n");
                    sb.Append('\t', depth);
                    sb.Append("]");
                }
                else if (value is bool)
                {
                    sb.AppendFormat("\"{0}\": {1}", HttpUtility.JavaScriptStringEncode(name), (bool)value ? "true" : "false");
                }
                else if (IsLongType(value))
                {
                    sb.AppendFormat("\"{0}\": {1}", HttpUtility.JavaScriptStringEncode(name), HttpUtility.JavaScriptStringEncode(Convert.ToInt64(value).ToString(CultureInfo.InvariantCulture)));
                }
                else if (IsULongType(value))
                {
                    sb.AppendFormat("\"{0}\": {1}", HttpUtility.JavaScriptStringEncode(name), HttpUtility.JavaScriptStringEncode(Convert.ToUInt64(value).ToString(CultureInfo.InvariantCulture)));
                }
                else if (IsDoubleType(value))
                {
                    sb.AppendFormat("\"{0}\": {1}", HttpUtility.JavaScriptStringEncode(name), HttpUtility.JavaScriptStringEncode(Convert.ToDouble(value).ToString(CultureInfo.InvariantCulture)));
                }
                else if (value is decimal)
                {
                    sb.AppendFormat("\"{0}\": {1}", HttpUtility.JavaScriptStringEncode(name), HttpUtility.JavaScriptStringEncode(((decimal)value).ToString(CultureInfo.InvariantCulture)));
                }
                else
                {
                    sb.AppendFormat("\"{0}\": \"{1}\"", HttpUtility.JavaScriptStringEncode(name), HttpUtility.JavaScriptStringEncode((value ?? "").ToString()));
                }
            }
            sb.Append("\r\n");
            sb.Append('\t', depth - 1);
            sb.Append("}");
        }

        private static bool IsLongType(object value)
        {
            return value is sbyte || value is short || value is int || value is long;
        }

        private static bool IsULongType(object value)
        {
            return value is byte || value is ushort || value is uint || value is ulong;
        }

        private static bool IsDoubleType(object value)
        {
            return value is float || value is double;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_dictionary.ContainsKey(binder.Name))
            {
                _dictionary[binder.Name] = value;
                return true;
            }
            else
            {
                _dictionary.Add(binder.Name, value);
                return true;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name == "Properties")
            {
                result = _dictionary
                    .Select(x => new KeyValuePair<string, dynamic>(x.Key, WrapResultObject(x.Value)))
                    .ToDictionary(x => x.Key, x => x.Value);
                return true;
            }

            if (!_dictionary.TryGetValue(binder.Name, out result))
            {
                // return null to avoid exception.  caller can check for null this way...
                result = null;
                return true;
            }

            result = WrapResultObject(result);

            if (result is string)
                result = JavaScriptStringDecode(result as string);

            return true;
        }

        public static string JavaScriptStringDecode(string source)
        {
            // Replace some chars.
            var decoded = source.Replace(@"\'", "'")
                        .Replace(@"\""", @"""")
                        .Replace(@"\/", "/")
                        .Replace(@"\t", "\t")
                        .Replace(@"\n", "\n");

            // Replace unicode escaped text.
            var rx = new Regex(@"\\[uU]([0-9A-F]{4})");

            decoded = rx.Replace(decoded, match => ((char)int.Parse(match.Value.Substring(2), NumberStyles.HexNumber))
                                                    .ToString(CultureInfo.InvariantCulture));

            return decoded;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length == 1 && indexes[0] != null)
            {
                if (!_dictionary.TryGetValue(indexes[0].ToString(), out result))
                {
                    // return null to avoid exception.  caller can check for null this way...
                    result = null;
                    return true;
                }

                result = WrapResultObject(result);
                return true;
            }

            return base.TryGetIndex(binder, indexes, out result);
        }

        private static object WrapResultObject(object result)
        {
            var dictionary = result as IDictionary<string, object>;
            if (dictionary != null)
                return new DynamicJsonObject(dictionary);

            var arrayList = result as ArrayList;
            if (arrayList != null && arrayList.Count > 0)
            {
                return arrayList[0] is IDictionary<string, object>
                    ? new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)))
                    : new List<object>(arrayList.Cast<object>());
            }

            return result;
        }
    }
}
