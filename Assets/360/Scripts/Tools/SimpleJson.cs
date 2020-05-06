/* * * * *
 * A simple JSON Parser / builder
 * ------------------------------
 * 
 * It mainly has been written as a simple JSON parser. It can build a JSON string
 * from the node-tree, or generate a node tree from any valid JSON string.
 * 
 * If you want to use compression when saving to file / stream / B64 you have to include
 * SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ ) in your project and
 * define "USE_SharpZipLib" at the top of the file
 * 
 * Written by Bunny83 
 * 2012-06-09
 * 
 * [2012-06-09 First Version]
 * - provides strongly typed node classes and lists / dictionaries
 * - provides easy access to class members / array items / data values
 * - the parser now properly identifies types. So generating JSON with this framework should work.
 * - only double quotes (") are used for quoting strings.
 * - provides "casting" properties to easily convert to / from those types:
 *   int / float / double / bool
 * - provides a common interface for each node so no explicit casting is required.
 * - the parser tries to avoid errors, but if malformed JSON is parsed the result is more or less undefined
 * - It can serialize/deserialize a node tree into/from an experimental compact binary format. It might
 *   be handy if you want to store things in a file and don't want it to be easily modifiable
 * 
 * 
 * [2012-12-17 Update]
 * - Added internal JSONLazyCreator class which simplifies the construction of a JSON tree
 *   Now you can simple reference any item that doesn't exist yet and it will return a JSONLazyCreator
 *   The class determines the required type by it's further use, creates the type and removes itself.
 * - Added binary serialization / deserialization.
 * - Added support for BZip2 zipped binary format. Requires the SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ )
 *   The usage of the SharpZipLib library can be disabled by removing or commenting out the USE_SharpZipLib define at the top
 * - The serializer uses different types when it comes to store the values. Since my data values
 *   are all of type string, the serializer will "try" which format fits best. The order is: int, float, double, bool, string.
 *   It's not the most efficient way but for a moderate amount of data it should work on all platforms.
 * 
 * [2017-03-08 Update]
 * - Optimised parsing by using a StringBuilder for token. This prevents performance issues when large
 *   string data fields are contained in the json data.
 * - Finally refactored the badly named JSONClass into JSONObject.
 * - Replaced the old JSONData class by distict typed classes ( JSONString, JSONNumber, JSONBool, JSONNull ) this
 *   allows to propertly convert the node tree back to json without type information loss. The actual value
 *   parsing now happens at parsing time and not when you actually access one of the casting properties.
 * 
 * [2017-04-11 Update]
 * - Fixed parsing bug where empty string values have been ignored.
 * - Optimised "ToString" by using a StringBuilder internally. This should heavily improve performance for large files
 * - Changed the overload of "ToString(string aIndent)" to "ToString(int aIndent)"
 * 
 * [2017-11-29 Update]
 * - Removed the IEnumerator implementations on JSONArray & JSONObject and replaced it with a common
 *   struct Enumerator in JSONNode that should avoid garbage generation. The enumerator always works
 *   on KeyValuePair<string, JSONNode>, even for JSONArray.
 * - Added two wrapper Enumerators that allows for easy key or value enumeration. A JSONNode now has
 *   a "Keys" and a "Values" enumerable property. Those are also struct enumerators / enumerables
 * - A KeyValuePair<string, JSONNode> can now be implicitly converted into a JSONNode. This allows
 *   a foreach loop over a JSONNode to directly access the values only. Since KeyValuePair as well as
 *   all the Enumerators are structs, no garbage is allocated.
 * - To add Linq support another "LinqEnumerator" is available through the "Linq" property. This
 *   enumerator does implement the generic IEnumerable interface so most Linq extensions can be used
 *   on this enumerable object. This one does allocate memory as it's a wrapper class.
 * - The Escape method now escapes all control characters (# < 32) in strings as uncode characters
 *   (\uXXXX) and if the static bool JSONNode.forceASCII is set to true it will also escape all
 *   characters # > 127. This might be useful if you require an ASCII output. Though keep in mind
 *   when your strings contain many non-ascii characters the strings become much longer (x6) and are
 *   no longer human readable.
 * - The node types JSONObject and JSONArray now have an "Inline" boolean switch which will default to
 *   false. It can be used to serialize this element inline even you serialize with an indented format
 *   This is useful for arrays containing numbers so it doesn't place every number on a new line
 * - Extracted the binary serialization code into a seperate extension file. All classes are now declared
 *   as "partial" so an extension file can even add a new virtual or abstract method / interface to
 *   JSONNode and override it in the concrete type classes. It's of course a hacky approach which is
 *   generally not recommended, but i wanted to keep everything tightly packed.
 * - Added a static CreateOrGet method to the JSONNull class. Since this class is immutable it could
 *   be reused without major problems. If you have a lot null fields in your data it will help reduce
 *   the memory / garbage overhead. I also added a static setting (reuseSameInstance) to JSONNull
 *   (default is true) which will change the behaviour of "CreateOrGet". If you set this to false
 *   CreateOrGet will not reuse the cached instance but instead create a new JSONNull instance each time.
 *   I made the JSONNull constructor private so if you need to create an instance manually use
 *   JSONNull.CreateOrGet()
 * 
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2012-2017 Markus Göbel (Bunny83)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * * * * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleJSON
{
    /// <summary>
    /// Json节点类型
    /// </summary>
    public enum JSONNodeType
    {
        /// <summary>
        /// 数组
        /// </summary>
        Array = 1,
        /// <summary>
        /// Json对象
        /// </summary>
        Object = 2,
        /// <summary>
        /// 字符串
        /// </summary>
        String = 3,
        /// <summary>
        /// 数字
        /// </summary>
        Number = 4,
        /// <summary>
        /// 空值
        /// </summary>
        NullValue = 5,
        /// <summary>
        /// 布尔类型
        /// </summary>
        Boolean = 6,
        /// <summary>
        /// None
        /// </summary>
        None = 7,
        /// <summary>
        /// Custom
        /// </summary>
        Custom = 0xFF,
    }
    /// <summary>
    /// Json文本模式
    /// </summary>
    public enum JSONTextMode
    {
        /// <summary>
        /// 紧凑
        /// </summary>
        Compact,
        /// <summary>
        /// 缩进
        /// </summary>
        Indent
    }

    /// <summary>
    /// Json节点
    /// </summary>
    public abstract partial class JSONNode
    {
        #region Enumerators
        /// <summary>
        /// 结构体
        /// </summary>
        public struct Enumerator
        {
            private enum Type { None, Array, Object }
            private Type type;
            private Dictionary<string, JSONNode>.Enumerator m_Object;
            private List<JSONNode>.Enumerator m_Array;
            /// <summary>
            /// 是否有效
            /// </summary>
            public bool IsValid { get { return type != Type.None; } }

            /// <summary>
            /// 构造函数 
            /// </summary>
            /// <param name="aArrayEnum"></param>
            public Enumerator(List<JSONNode>.Enumerator aArrayEnum)
            {
                type = Type.Array;
                m_Object = default(Dictionary<string, JSONNode>.Enumerator);
                m_Array = aArrayEnum;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="aDictEnum"></param>
            public Enumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
            {
                type = Type.Object;
                m_Object = aDictEnum;
                m_Array = default(List<JSONNode>.Enumerator);
            }

            /// <summary>
            /// 当前键值对
            /// </summary>
            public KeyValuePair<string, JSONNode> Current
            {
                get
                {
                    if (type == Type.Array)
                        return new KeyValuePair<string, JSONNode>(string.Empty, m_Array.Current);
                    else if (type == Type.Object)
                        return m_Object.Current;
                    return new KeyValuePair<string, JSONNode>(string.Empty, null);
                }
            }

            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (type == Type.Array)
                    return m_Array.MoveNext();
                else if (type == Type.Object)
                    return m_Object.MoveNext();
                return false;
            }
        }

        /// <summary>
        /// 结构体
        /// </summary>
        public struct ValueEnumerator
        {
            private Enumerator m_Enumerator;
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="aArrayEnum"></param>
            public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="aDictEnum"></param>
            public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="aEnumerator"></param>
            public ValueEnumerator(Enumerator aEnumerator) { m_Enumerator = aEnumerator; }
            /// <summary>
            /// 当前节点
            /// </summary>
            public JSONNode Current { get { return m_Enumerator.Current.Value; } }
            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns></returns>
            public bool MoveNext() { return m_Enumerator.MoveNext(); }

            /// <summary>
            /// 获取当前迭代器
            /// </summary>
            /// <returns></returns>
            public ValueEnumerator GetEnumerator() { return this; }
        }

        /// <summary>
        /// 结构体
        /// </summary>
        public struct KeyEnumerator
        {
            private Enumerator m_Enumerator;
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="aArrayEnum"></param>
            public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="aDictEnum"></param>
            public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="aEnumerator"></param>
            public KeyEnumerator(Enumerator aEnumerator) { m_Enumerator = aEnumerator; }
            /// <summary>
            /// 当前节点
            /// </summary>
            public JSONNode Current { get { return m_Enumerator.Current.Key; } }
            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns></returns>
            public bool MoveNext() { return m_Enumerator.MoveNext(); }
            /// <summary>
            /// 获取迭代器
            /// </summary>
            /// <returns></returns>
            public KeyEnumerator GetEnumerator() { return this; }
        }

        /// <summary>
        /// LinqEnumerator迭代器
        /// </summary>
        public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IEnumerable<KeyValuePair<string, JSONNode>>
        {
            private JSONNode m_Node;
            private Enumerator m_Enumerator;
            internal LinqEnumerator(JSONNode aNode)
            {
                m_Node = aNode;
                if (m_Node != null)
                    m_Enumerator = m_Node.GetEnumerator();
            }

            /// <summary>
            /// 当前键值对
            /// </summary>
            public KeyValuePair<string, JSONNode> Current { get { return m_Enumerator.Current; } }
            object IEnumerator.Current { get { return m_Enumerator.Current; } }

            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns></returns>
            public bool MoveNext() { return m_Enumerator.MoveNext(); }

            /// <summary>
            /// 释放
            /// </summary>
            public void Dispose()
            {
                m_Node = null;
                m_Enumerator = new Enumerator();
            }

            /// <summary>
            /// 获取迭代器
            /// </summary>
            /// <returns></returns>
            public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator()
            {
                return new LinqEnumerator(m_Node);
            }

            /// <summary>
            /// 重置
            /// </summary>
            public void Reset()
            {
                if (m_Node != null)
                    m_Enumerator = m_Node.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new LinqEnumerator(m_Node);
            }
        }

        #endregion Enumerators

        #region common interface

        /// <summary>
        /// 强制ASCII编码
        /// </summary>
        public static bool forceASCII = false; // Use Unicode by default

        /// <summary>
        /// tag
        /// </summary>
        public abstract JSONNodeType Tag { get; }

        /// <summary>
        /// 获取Json节点
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns></returns>
        public virtual JSONNode this[int aIndex] { get { return null; } set { } }

        /// <summary>
        /// 获取Json节点
        /// </summary>
        /// <param name="aKey"></param>
        /// <returns></returns>
        public virtual JSONNode this[string aKey] { get { return null; } set { } }

        /// <summary>
        /// Value
        /// </summary>
        public virtual string Value { get { return ""; } set { } }

        /// <summary>
        /// 数量
        /// </summary>
        public virtual int Count { get { return 0; } }

        /// <summary>
        /// 是否是数字
        /// </summary>
        public virtual bool IsNumber { get { return false; } }
        /// <summary>
        /// 是否是字符串
        /// </summary>
        public virtual bool IsString { get { return false; } }
        /// <summary>
        /// 是否是布尔类型
        /// </summary>
        public virtual bool IsBoolean { get { return false; } }
        /// <summary>
        /// 是否是空值
        /// </summary>
        public virtual bool IsNull { get { return false; } }
        /// <summary>
        /// 是否是集合
        /// </summary>
        public virtual bool IsArray { get { return false; } }
        /// <summary>
        /// 是否是对象
        /// </summary>
        public virtual bool IsObject { get { return false; } }

        /// <summary>
        /// 是否内联
        /// </summary>
        public virtual bool Inline { get { return false; } set { } }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="aKey"></param>
        /// <param name="aItem"></param>
        public virtual void Add(string aKey, JSONNode aItem)
        {
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="aItem"></param>
        public virtual void Add(JSONNode aItem)
        {
            Add("", aItem);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="aKey"></param>
        /// <returns></returns>
        public virtual JSONNode Remove(string aKey)
        {
            return null;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns></returns>
        public virtual JSONNode Remove(int aIndex)
        {
            return null;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="aNode"></param>
        /// <returns></returns>
        public virtual JSONNode Remove(JSONNode aNode)
        {
            return aNode;
        }

        /// <summary>
        /// 子节点
        /// </summary>
        public virtual IEnumerable<JSONNode> Children
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        public IEnumerable<JSONNode> DeepChildren
        {
            get
            {
                foreach (var C in Children)
                    foreach (var D in C.DeepChildren)
                        yield return D;
            }
        }

        /// <summary>
        /// 重写ToString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            WriteToStringBuilder(sb, 0, 0, JSONTextMode.Compact);
            return sb.ToString();
        }

        /// <summary>
        /// ToString方法扩展
        /// </summary>
        /// <param name="aIndent"></param>
        /// <returns></returns>
        public virtual string ToString(int aIndent)
        {
            StringBuilder sb = new StringBuilder();
            WriteToStringBuilder(sb, 0, aIndent, JSONTextMode.Indent);
            return sb.ToString();
        }
        internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public abstract Enumerator GetEnumerator();
        /// <summary>
        /// 返回Linq迭代器
        /// </summary>
        public IEnumerable<KeyValuePair<string, JSONNode>> Linq { get { return new LinqEnumerator(this); } }
        /// <summary>
        /// 获取Key迭代器
        /// </summary>
        public KeyEnumerator Keys { get { return new KeyEnumerator(GetEnumerator()); } }
        /// <summary>
        /// 获取Value迭代器
        /// </summary>
        public ValueEnumerator Values { get { return new ValueEnumerator(GetEnumerator()); } }

        #endregion common interface

        #region typecasting properties

        /// <summary>
        /// 转成Double类型
        /// </summary>
        public virtual double AsDouble
        {
            get
            {
                double v = 0.0;
                if (double.TryParse(Value, out v))
                    return v;
                return 0.0;
            }
            set
            {
                Value = value.ToString();
            }
        }

        /// <summary>
        /// 转成Int类型
        /// </summary>
        public virtual int AsInt
        {
            get { return (int)AsDouble; }
            set { AsDouble = value; }
        }

        /// <summary>
        /// 转成Float类型
        /// </summary>
        public virtual float AsFloat
        {
            get { return (float)AsDouble; }
            set { AsDouble = value; }
        }

        /// <summary>
        /// 转成Bool类型
        /// </summary>
        public virtual bool AsBool
        {
            get
            {
                bool v = false;
                if (bool.TryParse(Value, out v))
                    return v;
                return !string.IsNullOrEmpty(Value);
            }
            set
            {
                Value = (value) ? "true" : "false";
            }
        }

        /// <summary>
        /// 转成集合类型
        /// </summary>
        public virtual JSONArray AsArray
        {
            get
            {
                return this as JSONArray;
            }
        }

        /// <summary>
        /// 转成对象类型
        /// </summary>
        public virtual JSONObject AsObject
        {
            get
            {
                return this as JSONObject;
            }
        }


        #endregion typecasting properties

        #region operators
        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator JSONNode(string s)
        {
            return new JSONString(s);
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator string(JSONNode d)
        {
            return (d == null) ? null : d.Value;
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator JSONNode(double n)
        {
            return new JSONNumber(n);
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator double(JSONNode d)
        {
            return (d == null) ? 0 : d.AsDouble;
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator JSONNode(float n)
        {
            return new JSONNumber(n);
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator float(JSONNode d)
        {
            return (d == null) ? 0 : d.AsFloat;
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator JSONNode(int n)
        {
            return new JSONNumber(n);
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator int(JSONNode d)
        {
            return (d == null) ? 0 : d.AsInt;
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="b"></param>
        public static implicit operator JSONNode(bool b)
        {
            return new JSONBool(b);
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator bool(JSONNode d)
        {
            return (d == null) ? false : d.AsBool;
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="aKeyValue"></param>
        public static implicit operator JSONNode(KeyValuePair<string, JSONNode> aKeyValue)
        {
            return aKeyValue.Value;
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(JSONNode a, object b)
        {
            if (ReferenceEquals(a, b))
                return true;
            bool aIsNull = a is JSONNull || ReferenceEquals(a, null) || a is JSONLazyCreator;
            bool bIsNull = b is JSONNull || ReferenceEquals(b, null) || b is JSONLazyCreator;
            if (aIsNull && bIsNull)
                return true;
            return !aIsNull && a.Equals(b);
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(JSONNode a, object b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion operators

        [ThreadStatic]
        private static StringBuilder m_EscapeBuilder;
        internal static StringBuilder EscapeBuilder
        {
            get
            {
                if (m_EscapeBuilder == null)
                    m_EscapeBuilder = new StringBuilder();
                return m_EscapeBuilder;
            }
        }
        internal static string Escape(string aText)
        {
            var sb = EscapeBuilder;
            sb.Length = 0;
            if (sb.Capacity < aText.Length + aText.Length / 10)
                sb.Capacity = aText.Length + aText.Length / 10;
            foreach (char c in aText)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    default:
                        if (c < ' ' || (forceASCII && c > 127))
                        {
                            ushort val = c;
                            sb.Append("\\u").Append(val.ToString("X4"));
                        }
                        else
                            sb.Append(c);
                        break;
                }
            }
            string result = sb.ToString();
            sb.Length = 0;
            return result;
        }

        static void ParseElement(JSONNode ctx, string token, string tokenName, bool quoted)
        {
            if (quoted)
            {
                ctx.Add(tokenName, token);
                return;
            }
            string tmp = token.ToLower();
            if (tmp == "false" || tmp == "true")
                ctx.Add(tokenName, tmp == "true");
            else if (tmp == "null")
                ctx.Add(tokenName, null);
            else
            {
                double val;
                if (double.TryParse(token, out val))
                    ctx.Add(tokenName, val);
                else
                    ctx.Add(tokenName, token);
            }
        }

        /// <summary>
        /// 转成Json节点
        /// </summary>
        /// <param name="aJSON"></param>
        /// <returns></returns>
        public static JSONNode Parse(string aJSON)
        {
            Stack<JSONNode> stack = new Stack<JSONNode>();
            JSONNode ctx = null;
            int i = 0;
            StringBuilder Token = new StringBuilder();
            string TokenName = "";
            bool QuoteMode = false;
            bool TokenIsQuoted = false;
            while (i < aJSON.Length)
            {
                switch (aJSON[i])
                {
                    case '{':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }
                        stack.Push(new JSONObject());
                        if (ctx != null)
                        {
                            ctx.Add(TokenName, stack.Peek());
                        }
                        TokenName = "";
                        Token.Length = 0;
                        ctx = stack.Peek();
                        break;

                    case '[':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }

                        stack.Push(new JSONArray());
                        if (ctx != null)
                        {
                            ctx.Add(TokenName, stack.Peek());
                        }
                        TokenName = "";
                        Token.Length = 0;
                        ctx = stack.Peek();
                        break;

                    case '}':
                    case ']':
                        if (QuoteMode)
                        {

                            Token.Append(aJSON[i]);
                            break;
                        }
                        if (stack.Count == 0)
                            throw new Exception("JSON Parse: Too many closing brackets");

                        stack.Pop();
                        if (Token.Length > 0 || TokenIsQuoted)
                        {
                            ParseElement(ctx, Token.ToString(), TokenName, TokenIsQuoted);
                            TokenIsQuoted = false;
                        }
                        TokenName = "";
                        Token.Length = 0;
                        if (stack.Count > 0)
                            ctx = stack.Peek();
                        break;

                    case ':':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }
                        TokenName = Token.ToString();
                        Token.Length = 0;
                        TokenIsQuoted = false;
                        break;

                    case '"':
                        QuoteMode ^= true;
                        TokenIsQuoted |= QuoteMode;
                        break;

                    case ',':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }
                        if (Token.Length > 0 || TokenIsQuoted)
                        {
                            ParseElement(ctx, Token.ToString(), TokenName, TokenIsQuoted);
                            TokenIsQuoted = false;
                        }
                        TokenName = "";
                        Token.Length = 0;
                        TokenIsQuoted = false;
                        break;

                    case '\r':
                    case '\n':
                        break;

                    case ' ':
                    case '\t':
                        if (QuoteMode)
                            Token.Append(aJSON[i]);
                        break;

                    case '\\':
                        ++i;
                        if (QuoteMode)
                        {
                            char C = aJSON[i];
                            switch (C)
                            {
                                case 't':
                                    Token.Append('\t');
                                    break;
                                case 'r':
                                    Token.Append('\r');
                                    break;
                                case 'n':
                                    Token.Append('\n');
                                    break;
                                case 'b':
                                    Token.Append('\b');
                                    break;
                                case 'f':
                                    Token.Append('\f');
                                    break;
                                case 'u':
                                    {
                                        string s = aJSON.Substring(i + 1, 4);
                                        Token.Append((char)int.Parse(
                                            s,
                                            System.Globalization.NumberStyles.AllowHexSpecifier));
                                        i += 4;
                                        break;
                                    }
                                default:
                                    Token.Append(C);
                                    break;
                            }
                        }
                        break;

                    default:
                        Token.Append(aJSON[i]);
                        break;
                }
                ++i;
            }
            if (QuoteMode)
            {
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }
            return ctx;
        }

    }
    // End of JSONNode

    /// <summary>
    /// Json集合
    /// </summary>
    public partial class JSONArray : JSONNode
    {
        private List<JSONNode> m_List = new List<JSONNode>();
        private bool inline = false;

        /// <summary>
        /// 是否内联
        /// </summary>
        public override bool Inline
        {
            get { return inline; }
            set { inline = value; }
        }

        /// <summary>
        /// Tag
        /// </summary>
        public override JSONNodeType Tag { get { return JSONNodeType.Array; } }
        /// <summary>
        /// 是否是集合
        /// </summary>
        public override bool IsArray { get { return true; } }
        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public override Enumerator GetEnumerator() { return new Enumerator(m_List.GetEnumerator()); }

        /// <summary>
        /// 获取Json节点
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns></returns>
        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    return new JSONLazyCreator(this);
                return m_List[aIndex];
            }
            set
            {
                if (value == null)
                    value = JSONNull.CreateOrGet();
                if (aIndex < 0 || aIndex >= m_List.Count)
                    m_List.Add(value);
                else
                    m_List[aIndex] = value;
            }
        }

        /// <summary>
        /// 获取Json节点
        /// </summary>
        /// <param name="aKey"></param>
        /// <returns></returns>
        public override JSONNode this[string aKey]
        {
            get { return new JSONLazyCreator(this); }
            set
            {
                if (value == null)
                    value = JSONNull.CreateOrGet();
                m_List.Add(value);
            }
        }

        /// <summary>
        /// 数量
        /// </summary>
        public override int Count
        {
            get { return m_List.Count; }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="aKey"></param>
        /// <param name="aItem"></param>
        public override void Add(string aKey, JSONNode aItem)
        {
            if (aItem == null)
                aItem = JSONNull.CreateOrGet();
            m_List.Add(aItem);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns></returns>
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_List.Count)
                return null;
            JSONNode tmp = m_List[aIndex];
            m_List.RemoveAt(aIndex);
            return tmp;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="aNode"></param>
        /// <returns></returns>
        public override JSONNode Remove(JSONNode aNode)
        {
            m_List.Remove(aNode);
            return aNode;
        }

        /// <summary>
        /// 子节点
        /// </summary>
        public override IEnumerable<JSONNode> Children
        {
            get
            {
                foreach (JSONNode N in m_List)
                    yield return N;
            }
        }


        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('[');
            int count = m_List.Count;
            if (inline)
                aMode = JSONTextMode.Compact;
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    aSB.Append(',');
                if (aMode == JSONTextMode.Indent)
                    aSB.AppendLine();

                if (aMode == JSONTextMode.Indent)
                    aSB.Append(' ', aIndent + aIndentInc);
                m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JSONTextMode.Indent)
                aSB.AppendLine().Append(' ', aIndent);
            aSB.Append(']');
        }
    }
    // End of JSONArray

    /// <summary>
    /// Json对象
    /// </summary>
    public partial class JSONObject : JSONNode
    {
        private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

        private bool inline = false;

        /// <summary>
        /// 是否内联
        /// </summary>
        public override bool Inline
        {
            get { return inline; }
            set { inline = value; }
        }

        /// <summary>
        /// Tag
        /// </summary>
        public override JSONNodeType Tag { get { return JSONNodeType.Object; } }

        /// <summary>
        /// 是否是对象
        /// </summary>
        public override bool IsObject { get { return true; } }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public override Enumerator GetEnumerator() { return new Enumerator(m_Dict.GetEnumerator()); }


        /// <summary>
        /// 获取Json节点
        /// </summary>
        /// <param name="aKey"></param>
        /// <returns></returns>
        public override JSONNode this[string aKey]
        {
            get
            {
                if (m_Dict.ContainsKey(aKey))
                    return m_Dict[aKey];
                else
                    return new JSONLazyCreator(this, aKey);
            }
            set
            {
                if (value == null)
                    value = JSONNull.CreateOrGet();
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = value;
                else
                    m_Dict.Add(aKey, value);
            }
        }

        /// <summary>
        /// 获取Json节点
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns></returns>
        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return null;
                return m_Dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (value == null)
                    value = JSONNull.CreateOrGet();
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return;
                string key = m_Dict.ElementAt(aIndex).Key;
                m_Dict[key] = value;
            }
        }

        /// <summary>
        /// 数量
        /// </summary>
        public override int Count
        {
            get { return m_Dict.Count; }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="aKey"></param>
        /// <param name="aItem"></param>
        public override void Add(string aKey, JSONNode aItem)
        {
            if (aItem == null)
                aItem = JSONNull.CreateOrGet();

            if (!string.IsNullOrEmpty(aKey))
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = aItem;
                else
                    m_Dict.Add(aKey, aItem);
            }
            else
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="aKey"></param>
        /// <returns></returns>
        public override JSONNode Remove(string aKey)
        {
            if (!m_Dict.ContainsKey(aKey))
                return null;
            JSONNode tmp = m_Dict[aKey];
            m_Dict.Remove(aKey);
            return tmp;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns></returns>
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
                return null;
            var item = m_Dict.ElementAt(aIndex);
            m_Dict.Remove(item.Key);
            return item.Value;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="aNode"></param>
        /// <returns></returns>
        public override JSONNode Remove(JSONNode aNode)
        {
            try
            {
                var item = m_Dict.Where(k => k.Value == aNode).First();
                m_Dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 子节点
        /// </summary>
        public override IEnumerable<JSONNode> Children
        {
            get
            {
                foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                    yield return N.Value;
            }
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('{');
            bool first = true;
            if (inline)
                aMode = JSONTextMode.Compact;
            foreach (var k in m_Dict)
            {
                if (!first)
                    aSB.Append(',');
                first = false;
                if (aMode == JSONTextMode.Indent)
                    aSB.AppendLine();
                if (aMode == JSONTextMode.Indent)
                    aSB.Append(' ', aIndent + aIndentInc);
                aSB.Append('\"').Append(Escape(k.Key)).Append('\"');
                if (aMode == JSONTextMode.Compact)
                    aSB.Append(':');
                else
                    aSB.Append(" : ");
                k.Value.WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JSONTextMode.Indent)
                aSB.AppendLine().Append(' ', aIndent);
            aSB.Append('}');
        }

    }
    // End of JSONObject

    /// <summary>
    /// Json字符串
    /// </summary>
    public partial class JSONString : JSONNode
    {
        private string m_Data;

        /// <summary>
        /// Tag
        /// </summary>
        public override JSONNodeType Tag { get { return JSONNodeType.String; } }
        /// <summary>
        /// 是否是字符串
        /// </summary>
        public override bool IsString { get { return true; } }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public override Enumerator GetEnumerator() { return new Enumerator(); }


        /// <summary>
        /// 值
        /// </summary>
        public override string Value
        {
            get { return m_Data; }
            set
            {
                m_Data = value;
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="aData"></param>
        public JSONString(string aData)
        {
            m_Data = aData;
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('\"').Append(Escape(m_Data)).Append('\"');
        }

        /// <summary>
        /// 重写Equals方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;
            string s = obj as string;
            if (s != null)
                return m_Data == s;
            JSONString s2 = obj as JSONString;
            if (s2 != null)
                return m_Data == s2.m_Data;
            return false;
        }

        /// <summary>
        /// 重写GetHashCode方法
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return m_Data.GetHashCode();
        }
    }
    // End of JSONString

    /// <summary>
    /// Json数字
    /// </summary>
    public partial class JSONNumber : JSONNode
    {
        private double m_Data;

        /// <summary>
        /// Tag
        /// </summary>
        public override JSONNodeType Tag { get { return JSONNodeType.Number; } }
        /// <summary>
        /// 是否是数字
        /// </summary>
        public override bool IsNumber { get { return true; } }
        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        /// <summary>
        /// 获取值
        /// </summary>
        public override string Value
        {
            get { return m_Data.ToString(); }
            set
            {
                double v;
                if (double.TryParse(value, out v))
                    m_Data = v;
            }
        }

        /// <summary>
        /// 转成Double类型
        /// </summary>
        public override double AsDouble
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="aData"></param>
        public JSONNumber(double aData)
        {
            m_Data = aData;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="aData"></param>
        public JSONNumber(string aData)
        {
            Value = aData;
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append(m_Data);
        }
        private static bool IsNumeric(object value)
        {
            return value is int || value is uint
                || value is float || value is double
                || value is decimal
                || value is long || value is ulong
                || value is short || value is ushort
                || value is sbyte || value is byte;
        }

        /// <summary>
        /// 重写Equals方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (base.Equals(obj))
                return true;
            JSONNumber s2 = obj as JSONNumber;
            if (s2 != null)
                return m_Data == s2.m_Data;
            if (IsNumeric(obj))
                return Convert.ToDouble(obj) == m_Data;
            return false;
        }

        /// <summary>
        /// 重写GetHashCode方法
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return m_Data.GetHashCode();
        }
    }
    // End of JSONNumber
    
    /// <summary>
    /// JsonBool类型
    /// </summary>
    public partial class JSONBool : JSONNode
    {
        private bool m_Data;

        /// <summary>
        /// Tag
        /// </summary>
        public override JSONNodeType Tag { get { return JSONNodeType.Boolean; } }
        /// <summary>
        /// 是否是布尔类型
        /// </summary>
        public override bool IsBoolean { get { return true; } }
        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        /// <summary>
        /// 获取值
        /// </summary>
        public override string Value
        {
            get { return m_Data.ToString(); }
            set
            {
                bool v;
                if (bool.TryParse(value, out v))
                    m_Data = v;
            }
        }

        /// <summary>
        /// 转成布尔类型
        /// </summary>
        public override bool AsBool
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="aData"></param>
        public JSONBool(bool aData)
        {
            m_Data = aData;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="aData"></param>
        public JSONBool(string aData)
        {
            Value = aData;
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append((m_Data) ? "true" : "false");
        }

        /// <summary>
        /// 重写Equals方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is bool)
                return m_Data == (bool)obj;
            return false;
        }
        /// <summary>
        /// 重写GetHashCode方法
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return m_Data.GetHashCode();
        }
    }
    // End of JSONBool
    /// <summary>
    /// JsonNull类型
    /// </summary>
    public partial class JSONNull : JSONNode
    {
        static JSONNull m_StaticInstance = new JSONNull();
        /// <summary>
        /// 是否重复使用相同实例
        /// </summary>
        public static bool reuseSameInstance = true;

        /// <summary>
        /// 创建或者获取实例
        /// </summary>
        /// <returns></returns>
        public static JSONNull CreateOrGet()
        {
            if (reuseSameInstance)
                return m_StaticInstance;
            return new JSONNull();
        }
        private JSONNull() { }

        /// <summary>
        /// Tag
        /// </summary>
        public override JSONNodeType Tag { get { return JSONNodeType.NullValue; } }
        /// <summary>
        /// 是否是Null类型
        /// </summary>
        public override bool IsNull { get { return true; } }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        /// <summary>
        /// 获取值
        /// </summary>
        public override string Value
        {
            get { return "null"; }
            set { }
        }

        /// <summary>
        /// 是布尔类型
        /// </summary>
        public override bool AsBool
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// 重写Equals方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;
            return (obj is JSONNull);
        }

        /// <summary>
        /// 重写GetHashCode方法
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 0;
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append("null");
        }
    }
    // End of JSONNull

    internal partial class JSONLazyCreator : JSONNode
    {
        private JSONNode m_Node = null;
        private string m_Key = null;
        public override JSONNodeType Tag { get { return JSONNodeType.None; } }
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public JSONLazyCreator(JSONNode aNode)
        {
            m_Node = aNode;
            m_Key = null;
        }

        public JSONLazyCreator(JSONNode aNode, string aKey)
        {
            m_Node = aNode;
            m_Key = aKey;
        }

        private void Set(JSONNode aVal)
        {
            if (m_Key == null)
            {
                m_Node.Add(aVal);
            }
            else
            {
                m_Node.Add(m_Key, aVal);
            }
            m_Node = null; // Be GC friendly.
        }

        public override JSONNode this[int aIndex]
        {
            get
            {
                return new JSONLazyCreator(this);
            }
            set
            {
                var tmp = new JSONArray();
                tmp.Add(value);
                Set(tmp);
            }
        }

        public override JSONNode this[string aKey]
        {
            get
            {
                return new JSONLazyCreator(this, aKey);
            }
            set
            {
                var tmp = new JSONObject();
                tmp.Add(aKey, value);
                Set(tmp);
            }
        }

        public override void Add(JSONNode aItem)
        {
            var tmp = new JSONArray();
            tmp.Add(aItem);
            Set(tmp);
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            var tmp = new JSONObject();
            tmp.Add(aKey, aItem);
            Set(tmp);
        }

        public static bool operator ==(JSONLazyCreator a, object b)
        {
            if (b == null)
                return true;
            return System.Object.ReferenceEquals(a, b);
        }

        public static bool operator !=(JSONLazyCreator a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return true;
            return System.Object.ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override int AsInt
        {
            get
            {
                JSONNumber tmp = new JSONNumber(0);
                Set(tmp);
                return 0;
            }
            set
            {
                JSONNumber tmp = new JSONNumber(value);
                Set(tmp);
            }
        }

        public override float AsFloat
        {
            get
            {
                JSONNumber tmp = new JSONNumber(0.0f);
                Set(tmp);
                return 0.0f;
            }
            set
            {
                JSONNumber tmp = new JSONNumber(value);
                Set(tmp);
            }
        }

        public override double AsDouble
        {
            get
            {
                JSONNumber tmp = new JSONNumber(0.0);
                Set(tmp);
                return 0.0;
            }
            set
            {
                JSONNumber tmp = new JSONNumber(value);
                Set(tmp);
            }
        }

        public override bool AsBool
        {
            get
            {
                JSONBool tmp = new JSONBool(false);
                Set(tmp);
                return false;
            }
            set
            {
                JSONBool tmp = new JSONBool(value);
                Set(tmp);
            }
        }

        public override JSONArray AsArray
        {
            get
            {
                JSONArray tmp = new JSONArray();
                Set(tmp);
                return tmp;
            }
        }

        public override JSONObject AsObject
        {
            get
            {
                JSONObject tmp = new JSONObject();
                Set(tmp);
                return tmp;
            }
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append("null");
        }
    }
    // End of JSONLazyCreator
    
    /// <summary>
    /// Json静态类
    /// </summary>
    public static class JSON
    {
        /// <summary>
        /// 解析字符串成Json节点
        /// </summary>
        /// <param name="aJSON"></param>
        /// <returns></returns>
        public static JSONNode Parse(string aJSON)
        {
            return JSONNode.Parse(aJSON);
        }
    }
}