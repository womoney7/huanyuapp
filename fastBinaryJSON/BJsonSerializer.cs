﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Dynamic;
using System.Data;

namespace fastBinaryJSON
{
    internal class BJSONSerializer
    {
        private readonly MemoryStream _output = new MemoryStream();
        readonly bool useMinimalDataSetSchema;
        readonly int _MAX_DEPTH = 10;
        bool _useGlobalTypes = false;
        int _current_depth = 0;
        bool _useunicode = true;
        private Dictionary<string, int> _globalTypes = new Dictionary<string, int>();

        private bool useExtension = true;
        private bool serializeNulls = true;

        internal BJSONSerializer(bool UseMinimalDataSetSchema, bool useUnicodeStrings)
        {
            this.useMinimalDataSetSchema = UseMinimalDataSetSchema;
            this._useunicode = useUnicodeStrings;
        }

        internal byte[] ConvertToBJSON(object obj)
        {
            WriteValue(obj);
            // FIX : add $types
            return _output.ToArray();

            //string str = "";
            //if (_useGlobalTypes)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append("{\"$types\":{");
            //    bool pendingSeparator = false;
            //    foreach (var kv in _globalTypes)
            //    {
            //        if (pendingSeparator) sb.Append(',');
            //        pendingSeparator = true;
            //        sb.Append("\"");
            //        sb.Append(kv.Key);
            //        sb.Append("\":\"");
            //        sb.Append(kv.Value);
            //        sb.Append("\"");
            //    }
            //    sb.Append("},");
            //    str = sb.ToString() + _output.ToString();
            //}
            //else
            //    str = _output.ToString();

            //return str;
        }

        private void WriteValue(object obj)
        {
            if (obj == null || obj is DBNull)
                WriteNull();

            else if (obj is string)
                WriteString((string)obj);

            else if (obj is char)
                WriteChar((char)obj);

            else if (obj is Guid)
                WriteGuid((Guid)obj);

            else if (obj is bool)
                WriteBool((bool)obj);

            else if (obj is int)
                WriteInt((int)obj);

            else if (obj is uint)
                WriteUInt((uint)obj);

            else if (obj is long)
                WriteLong((long)obj);

            else if (obj is ulong)
                WriteULong((ulong)obj);

            else if (obj is decimal)
                WriteDecimal((decimal)obj);

            else if (obj is byte)
                WriteByte((byte)obj);

            else if (obj is double)
                WriteDouble((double)obj);

            else if (obj is float)
                WriteFloat((float)obj);

            else if (obj is short)
                WriteShort((short)obj);

            else if (obj is ushort)
                WriteUShort((ushort)obj);

            else if (obj is DateTime)
                WriteDateTime((DateTime)obj);

            else if (obj is IDictionary && obj.GetType().IsGenericType && obj.GetType().GetGenericArguments()[0] == typeof(string))
                WriteStringDictionary((IDictionary)obj);

            else if (obj is IDictionary)
                WriteDictionary((IDictionary)obj);
#if !SILVERLIGHT
            else if (obj is DataSet)
                WriteDataset((DataSet)obj);

            else if (obj is DataTable)
                this.WriteDataTable((DataTable)obj);
#endif
            else if (obj is byte[])
                WriteBytes((byte[])obj);

            else if ((obj is Array || obj is IList || obj is ICollection || obj is IEnumerable))
                WriteArray((IEnumerable)obj);

            else if (obj is Enum)
                WriteEnum((Enum)obj);

#if CUSTOMTYPE
            else if (JSON.Instance.IsTypeRegistered(obj.GetType()))
                WriteCustom(obj);
#endif
            else
                WriteObject(obj);
        }

        private void WriteUShort(ushort p)
        {
            _output.WriteByte(TOKENS.USHORT);
            _output.Write(Helper.GetBytes(p, false), 0, 2);
        }

        private void WriteShort(short p)
        {
            _output.WriteByte(TOKENS.SHORT);
            _output.Write(Helper.GetBytes(p, false), 0, 2);
        }

        private void WriteFloat(float p)
        {
            _output.WriteByte(TOKENS.FLOAT);
            byte[] b = BitConverter.GetBytes(p);
            _output.Write(b, 0, b.Length);
        }

        private void WriteDouble(double p)
        {
            _output.WriteByte(TOKENS.DOUBLE);
            var b = BitConverter.GetBytes(p);
            _output.Write(b, 0, b.Length);
        }

        private void WriteByte(byte p)
        {
            _output.WriteByte(TOKENS.BYTE);
            _output.WriteByte(p);
        }

        private void WriteDecimal(decimal p)
        {
            _output.WriteByte(TOKENS.DECIMAL);
            var b = decimal.GetBits(p);
            foreach (var c in b)
                _output.Write(Helper.GetBytes(c, false), 0, 4);
        }

        private void WriteULong(ulong p)
        {
            _output.WriteByte(TOKENS.ULONG);
            _output.Write(Helper.GetBytes((long)p, false), 0, 8);
        }

        private void WriteUInt(uint p)
        {
            _output.WriteByte(TOKENS.UINT);
            _output.Write(Helper.GetBytes(p, false), 0, 4);
        }

        private void WriteLong(long p)
        {
            _output.WriteByte(TOKENS.LONG);
            _output.Write(Helper.GetBytes(p, false), 0, 8);
        }

        private void WriteChar(char p)
        {
            // FIX : 
            //_output.WriteByte(TOKENS.CHAR);
            //_output.Write(Helper.GetBytes(
            throw new Exception("char not implemented yet");
        }

        private void WriteBytes(byte[] p)
        {
            _output.WriteByte(TOKENS.BYTEARRAY);
            _output.Write(Helper.GetBytes(p.Length, false), 0, 4);
            _output.Write(p, 0, p.Length);
        }

        private void WriteBool(bool p)
        {
            if (p)
                _output.WriteByte(TOKENS.TRUE);
            else
                _output.WriteByte(TOKENS.FALSE);
        }

        private void WriteNull()
        {
            _output.WriteByte(TOKENS.NULL);
        }

#if CUSTOMTYPE
        private void WriteCustom(object obj)
        {
            Serialize s;
            JSON.Instance._customSerializer.TryGetValue(obj.GetType(), out s);
            WriteStringFast(s(obj));
        }
#endif
        private void WriteColon()
        {
            _output.WriteByte(TOKENS.COLON);
        }

        private void WriteComma()
        {
            _output.WriteByte(TOKENS.COMMA);
        }

        private void WriteEnum(Enum e)
        {
            WriteString(e.ToString());
        }

        private void WriteInt(int i)
        {
            _output.WriteByte(TOKENS.INT);
            _output.Write(Helper.GetBytes(i, false), 0, 4);
        }

        private void WriteGuid(Guid g)
        {
            _output.WriteByte(TOKENS.GUID);
            _output.Write(g.ToByteArray(), 0, 16);
        }

        private void WriteDateTime(DateTime dateTime)
        {
            DateTime dt = dateTime;
            //dt = dateTime.ToUniversalTime();

            _output.WriteByte(TOKENS.DATETIME);
            byte[] b = Helper.GetBytes(dt.Ticks, false);
            _output.Write(b, 0, b.Length);
        }

#if !SILVERLIGHT
        private DatasetSchema GetSchema(DataTable ds)
        {
            if (ds == null) return null;

            DatasetSchema m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.TableName;

            foreach (DataColumn c in ds.Columns)
            {
                m.Info.Add(ds.TableName);
                m.Info.Add(c.ColumnName);
                m.Info.Add(c.DataType.ToString());
            }
            // TODO : serialize relations and constraints here

            return m;
        }

        private DatasetSchema GetSchema(DataSet ds)
        {
            if (ds == null) return null;

            DatasetSchema m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.DataSetName;

            foreach (DataTable t in ds.Tables)
            {
                foreach (DataColumn c in t.Columns)
                {
                    m.Info.Add(t.TableName);
                    m.Info.Add(c.ColumnName);
                    m.Info.Add(c.DataType.ToString());
                }
            }
            // TODO : serialize relations and constraints here

            return m;
        }

        private string GetXmlSchema(DataTable dt)
        {
            using (var writer = new StringWriter())
            {
                dt.WriteXmlSchema(writer);
                return dt.ToString();
            }
        }

        private void WriteDataset(DataSet ds)
        {
            _output.WriteByte(TOKENS.DOC_START);
            {
                WritePair("$schema", useMinimalDataSetSchema ? (object)GetSchema(ds) : ds.GetXmlSchema());
                WriteComma();
            }
            bool tablesep = false;
            foreach (DataTable table in ds.Tables)
            {
                if (tablesep) WriteComma();
                tablesep = true;
                WriteDataTableData(table);
            }
            // end dataset
            _output.WriteByte(TOKENS.DOC_END);
        }

        private void WriteDataTableData(DataTable table)
        {
            WriteName(table.TableName);
            WriteColon();
            _output.WriteByte(TOKENS.ARRAY_START);
            DataColumnCollection cols = table.Columns;
            bool rowseparator = false;
            foreach (DataRow row in table.Rows)
            {
                if (rowseparator) WriteComma();
                rowseparator = true;
                _output.WriteByte(TOKENS.ARRAY_START);

                bool pendingSeperator = false;
                foreach (DataColumn column in cols)
                {
                    if (pendingSeperator) WriteComma();
                    WriteValue(row[column]);
                    pendingSeperator = true;
                }
                _output.WriteByte(TOKENS.ARRAY_END);
            }

            _output.WriteByte(TOKENS.ARRAY_END);
        }

        void WriteDataTable(DataTable dt)
        {
            _output.WriteByte(TOKENS.DOC_START);
            //if (this.useExtension)
            {
                this.WritePair("$schema", this.useMinimalDataSetSchema ? (object)this.GetSchema(dt) : this.GetXmlSchema(dt));
                WriteComma();
            }

            WriteDataTableData(dt);

            // end datatable
            _output.WriteByte(TOKENS.DOC_END);
        }
#endif
        bool _firstWritten = false;

        private void WriteObject(object obj)
        {
            if (_useGlobalTypes == false)
                _output.WriteByte(TOKENS.DOC_START);
            else
            {
                if (_firstWritten)
                    _output.WriteByte(TOKENS.DOC_START);
            }
            _firstWritten = true;
            _current_depth++;
            if (_current_depth > _MAX_DEPTH)
                throw new Exception("Serializer encountered maximum depth of " + _MAX_DEPTH);
            Dictionary<string, string> map = new Dictionary<string, string>();
            Type t = obj.GetType();
            bool append = false;
            //if (useExtension)
            //{
            //    if (_useGlobalTypes == false)
            //        WritePairFast("$type", BJSON.Instance.GetTypeAssemblyName(t));
            //    else
            //    {
            //        int dt = 0;
            //        string ct = BJSON.Instance.GetTypeAssemblyName(t);
            //        if (_globalTypes.TryGetValue(ct, out dt) == false)
            //        {
            //            dt = _globalTypes.Count + 1;
            //            _globalTypes.Add(ct, dt);
            //        }
            //        WritePairFast("$type", dt.ToString());
            //    }
            //    append = true;
            //}

            List<Getters> g = BJSON.Instance.GetGetters(t);
            foreach (var p in g)
            {
                if (append)
                    WriteComma();
                var o = p.Getter(obj);
                if ((o == null || o is DBNull) && serializeNulls == false)
                    append = false;
                else
                {
                    WritePair(p.Name, o);
                    if (o != null && useExtension)
                    {
                        Type tt = o.GetType();
                        if (tt == typeof(System.Object))
                            map.Add(p.Name, tt.ToString());
                    }
                    append = true;
                }
            }

            _current_depth--;
            _output.WriteByte(TOKENS.DOC_END);
            _current_depth--;

        }

        //private void WriteObject(object obj)
        //{
        //    if (_useGlobalTypes == false)
        //        _output.WriteByte(TOKENS.DOC_START);
        //    else
        //    {
        //        if (_firstWritten)
        //            _output.WriteByte(TOKENS.DOC_START);
        //    }
        //    _firstWritten = true;
        //    _current_depth++;
        //    if (_current_depth > _MAX_DEPTH)
        //        throw new Exception("Serializer encountered maximum depth of " + _MAX_DEPTH);

        //    Dictionary<string, string> map = new Dictionary<string, string>();
        //    Type t = obj.GetType();
        //    bool append = false;
        //    //if (useExtension)
        //    //{
        //    //    if (_useGlobalTypes == false)
        //    //        WritePairFast("$type", BJSON.Instance.GetTypeAssemblyName(t));
        //    //    else
        //    //    {
        //    //        int dt = 0;
        //    //        string ct = BJSON.Instance.GetTypeAssemblyName(t);
        //    //        if (_globalTypes.TryGetValue(ct, out dt) == false)
        //    //        {
        //    //            dt = _globalTypes.Count + 1;
        //    //            _globalTypes.Add(ct, dt);
        //    //        }
        //    //        WritePairFast("$type", dt.ToString());
        //    //    }
        //    //    append = true;
        //    //}

        //    //2012-5-18 重要修改，因原代码不能处理匿名对象的操作
        //    //List<Getters> g = BJSON.Instance.GetGetters(t);
        //    var g = obj.ToDictionary();
        //    foreach (var p in g.Keys)
        //    {
        //        if (append)
        //            WriteComma();

        //        //var o = p.Getter(obj);
        //        var o = g[p];

        //        if ((o == null || o is DBNull) && serializeNulls == false)
        //            append = false;
        //        else
        //        {
        //            WritePair(p, o);
        //            if (o != null && useExtension)
        //            {
        //                Type tt = o.GetType();
        //                if (tt == typeof(System.Object))
        //                    map.Add(p, tt.ToString());
        //            }
        //            append = true;
        //        }
        //    }
        //    _current_depth--;
        //    _output.WriteByte(TOKENS.DOC_END);
        //    _current_depth--;
        //}


        private void WritePairFast(string name, string value)
        {
            if ((value == null) && serializeNulls == false)
                return;
            WriteName(name);

            WriteColon();

            WriteString(value);
        }

        private void WritePair(string name, object value)
        {
            if ((value == null || value is DBNull) && serializeNulls == false)
                return;
            WriteName(name);

            WriteColon();

            WriteValue(value);
        }

        private void WriteArray(IEnumerable array)
        {
            _output.WriteByte(TOKENS.ARRAY_START);

            bool pendingSeperator = false;

            foreach (object obj in array)
            {
                if (pendingSeperator) WriteComma();

                WriteValue(obj);

                pendingSeperator = true;
            }
            _output.WriteByte(TOKENS.ARRAY_END);
        }

        private void WriteStringDictionary(IDictionary dic)
        {
            _output.WriteByte(TOKENS.DOC_START);

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) WriteComma();

                WritePair((string)entry.Key, entry.Value);

                pendingSeparator = true;
            }
            _output.WriteByte(TOKENS.DOC_END);
        }

        private void WriteDictionary(IDictionary dic)
        {
            _output.WriteByte(TOKENS.ARRAY_START);

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) WriteComma();
                _output.WriteByte(TOKENS.DOC_START);
                WritePair("k", entry.Key);
                WriteComma();
                WritePair("v", entry.Value);
                _output.WriteByte(TOKENS.DOC_END);

                pendingSeparator = true;
            }
            _output.WriteByte(TOKENS.ARRAY_END);
        }

        private void WriteName(string s)
        {
            _output.WriteByte(TOKENS.NAME);
            byte[] b = BJSON.Instance.utf8.GetBytes(s);
            _output.WriteByte((byte)b.Length);
            _output.Write(b, 0, b.Length % 256);
        }

        private void WriteString(string s)
        {
            byte[] b = null;
            if (_useunicode)
            {
                _output.WriteByte(TOKENS.UNICODE_STRING);
                b = BJSON.Instance.unicode.GetBytes(s);
            }
            else
            {
                _output.WriteByte(TOKENS.STRING);
                b = BJSON.Instance.utf8.GetBytes(s);
            }
            _output.Write(Helper.GetBytes(b.Length, false), 0, 4);
            _output.Write(b, 0, b.Length);
        }
    }
}
