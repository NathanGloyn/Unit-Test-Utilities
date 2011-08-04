﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Collections.Specialized;

namespace UnitTest.Database
{
    /// <summary>
    /// Class is a stub data reader so that it can be used for testing
    /// purposes where a data reader is required to test but we
    /// do not want to have to actually query the database
    /// </summary>
    public class StubDbDataReader : DbDataReader
    {

        private StubDataReader _stubReader;
        private List<bool> _hasRows = new List<bool>();
        private DataTable _table = null;
        private int _currentDataTable = 0;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StubDataReader"/> class. 
        /// Each DataTable is a ResultSet
        /// </summary>
        /// <param name="tables">The tables to use as Results</param>
        public StubDbDataReader(params DataTable[] tables)
        {
            StubResultSetCollection col = new StubResultSetCollection();
            foreach (DataTable item in tables)
            {
                _hasRows.Add((item.Rows.Count > 0));
                col.Add(new StubResultSet(item));
            }

            _stubReader = new StubDataReader(col);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubDataReader"/> class. 
        /// Each row in the arraylist is a result set.
        /// </summary>
        /// <param name="resultSets">The result sets to add.</param>
        public StubDbDataReader(params StubResultSet[] resultSets)
        {
            _stubReader = new StubDataReader(resultSets);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubDataReader"/> class. 
        /// Each row in the arraylist is a result set.
        /// </summary>
        /// <param name="stubResultSets">The result sets.</param>
        public StubDbDataReader(StubResultSetCollection stubResultSets)
        {
            _stubReader = new StubDataReader(stubResultSets);
        }

        #endregion

        public override void Close()
        {
            _stubReader.Close();
        }

        public override int Depth
        {
            get { return _stubReader.Depth; }
        }

        public override int FieldCount
        {
            get { return _stubReader.FieldCount; }
        }

        public override bool GetBoolean(int ordinal)
        {
            return _stubReader.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return _stubReader.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return _stubReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return _stubReader.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return _stubReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _stubReader.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return _stubReader.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return _stubReader.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return _stubReader.GetDouble(ordinal);
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            return _stubReader.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            return _stubReader.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return _stubReader.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return _stubReader.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return _stubReader.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return _stubReader.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return _stubReader.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return _stubReader.GetOrdinal(name);
        }

        public override DataTable GetSchemaTable()
        {
            return _table;
        }

        public override string GetString(int ordinal)
        {
            return _stubReader.GetString(ordinal); ;
        }

        public override object GetValue(int ordinal)
        {
            return _stubReader.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return _stubReader.GetValues(values);
        }

        public override bool HasRows
        {
            get
            {
                return (_currentDataTable == -1 ? false : _hasRows[_currentDataTable]);
            }
        }

        public override bool IsClosed
        {
            get { return _stubReader.IsClosed; }
        }

        public override bool IsDBNull(int ordinal)
        {
            return _stubReader.IsDBNull(ordinal);
        }

        public override bool NextResult()
        {
            bool result = _stubReader.NextResult();

            if (result)
                _currentDataTable++;
            else
                _currentDataTable = -1;

            return result;
        }

        public override bool Read()
        {
            return _stubReader.Read();
        }

        public override int RecordsAffected
        {
            get { return _stubReader.RecordsAffected; }
        }

        public override object this[string name]
        {
            get { return _stubReader[name]; }
        }

        public override object this[int ordinal]
        {
            get { return _stubReader[ordinal]; }
        }
    }

    /// <summary>
    /// This class fakes up a data reader.
    /// </summary>
    public class StubDataReader : IDataReader
    {
        StubResultSetCollection stubResultSets;
        private int currentResultsetIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="StubDataReader"/> class. 
        /// Each row in the arraylist is a result set.
        /// </summary>
        /// <param name="stubResultSets">The result sets.</param>
        public StubDataReader(StubResultSetCollection stubResultSets)
        {
            this.stubResultSets = stubResultSets;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubDataReader"/> class. 
        /// Each row in the arraylist is a result set.
        /// </summary>
        /// <param name="resultSets">The result sets to add.</param>
        public StubDataReader(params StubResultSet[] resultSets)
        {
            this.stubResultSets = new StubResultSetCollection();
            foreach (StubResultSet resultSet in resultSets)
            {
                this.stubResultSets.Add(resultSet);
            }
        }

        public void Close()
        {
        }

        public bool NextResult()
        {
            if (currentResultsetIndex >= this.stubResultSets.Count)
                return false;

            return (++currentResultsetIndex < this.stubResultSets.Count);
        }

        public bool Read()
        {
            return CurrentResultSet.Read();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <value></value>
        public int Depth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsClosed
        {
            get { return false; }
        }

        public int RecordsAffected
        {
            get { return 1; }
        }

        public void Dispose()
        {
        }

        public string GetName(int i)
        {
            return CurrentResultSet.GetFieldNames()[i];
        }

        public string GetDataTypeName(int i)
        {
            return this.CurrentResultSet.GetFieldNames()[i];
        }

        public Type GetFieldType(int i)
        {
            //KLUDGE: Since we're dynamically creating this, I'll have to 
            //		  look at the actual data to determine this.
            //		  We'll loook at the first row since it's the most likely 
            //			to have data.
            return this.stubResultSets[0][i].GetType();
        }

        public object GetValue(int i)
        {
            return CurrentResultSet[i];
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            return this.CurrentResultSet.GetIndexFromFieldName(name);
        }

        public bool GetBoolean(int i)
        {
            return (bool)CurrentResultSet[i];
        }

        public byte GetByte(int i)
        {
            return (byte)CurrentResultSet[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            //TODO: Need to test this method.

            byte[] totalBytes = (byte[])CurrentResultSet[i];

            int bytesRead = 0;
            for (int j = 0; j < length; j++)
            {
                long readIndex = fieldOffset + j;
                long writeIndex = bufferoffset + j;
                if (totalBytes.Length <= readIndex)
                    throw new ArgumentOutOfRangeException("fieldOffset", string.Format("Trying to read index '{0}' is out of range. (fieldOffset '{1}' + current position '{2}'", readIndex, fieldOffset, j));

                if (buffer.Length <= writeIndex)
                    throw new ArgumentOutOfRangeException("bufferoffset", string.Format("Trying to write to buffer index '{0}' is out of range. (bufferoffset '{1}' + current position '{2}'", readIndex, bufferoffset, j));

                buffer[writeIndex] = totalBytes[readIndex];
                bytesRead++;
            }
            return bytesRead;
        }

        public char GetChar(int i)
        {
            return (char)CurrentResultSet[i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            return (Guid)CurrentResultSet[i];
        }

        public short GetInt16(int i)
        {
            return (short)CurrentResultSet[i];
        }

        public int GetInt32(int i)
        {
            return (int)CurrentResultSet[i];
        }

        public long GetInt64(int i)
        {
            return (long)CurrentResultSet[i];
        }

        public float GetFloat(int i)
        {
            return (float)CurrentResultSet[i];
        }

        public double GetDouble(int i)
        {
            return (double)CurrentResultSet[i];
        }

        public string GetString(int i)
        {
            return (string)CurrentResultSet[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)CurrentResultSet[i];
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)CurrentResultSet[i];
        }

        public IDataReader GetData(int i)
        {
            StubDataReader reader = new StubDataReader(this.stubResultSets);
            reader.currentResultsetIndex = i;
            return reader;
        }

        public bool IsDBNull(int i)
        {
            //TODO: Deal with value types.
            return (
                        CurrentResultSet[i] == null
                        ||
                        CurrentResultSet[i] == DBNull.Value
                    );

        }

        public int FieldCount
        {
            get
            {
                return CurrentResultSet.GetFieldNames().Length;
            }
        }

        public object this[int i]
        {
            get
            {
                return CurrentResultSet[i];
            }
        }

        public object this[string name]
        {
            get
            {
                return CurrentResultSet[name];
            }
        }

        private StubResultSet CurrentResultSet
        {
            get
            {
                return this.stubResultSets[currentResultsetIndex];
            }
        }
    }

    /// <summary>
    /// Represents a result set for the StubDataReader.
    /// </summary>
    public class StubResultSet
    {
        int currentRowIndex = -1;
        StubRowCollection rows = new StubRowCollection();
        NameIndexCollection fieldNames = new NameIndexCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="StubResultSet"/> class with the column names.
        /// </summary>
        /// <param name="fieldNames">The column names.</param>
        public StubResultSet(params string[] fieldNames)
        {
            for (int i = 0; i < fieldNames.Length; i++)
            {
                this.fieldNames.Add(fieldNames[i], i);
            }
        }

        public StubResultSet(DataTable data)
        {
            // Add fields
            for (int i = 0; i < data.Columns.Count; i++)
            {
                this.fieldNames.Add(data.Columns[i].ColumnName, i);
            }

            foreach (DataRow row in data.Rows)
            {
                this.AddRow(row.ItemArray);
            }
        }

        public string[] GetFieldNames()
        {
            return fieldNames.GetAllKeys();
        }

        public string GetFieldName(int i)
        {
            return GetFieldNames()[i];
        }

        /// <summary>
        /// Adds the row.
        /// </summary>
        /// <param name="values">The values.</param>
        public void AddRow(params object[] values)
        {
            if (values.Length != fieldNames.Count)
            {
                throw new ArgumentOutOfRangeException("values", string.Format("The Row must contain '{0}' items", fieldNames.Count));
            }
            rows.Add(new StubRow(values));
        }

        public int GetIndexFromFieldName(string name)
        {
            return this.fieldNames[name];
        }

        public bool Read()
        {
            return ++this.currentRowIndex < this.rows.Count;
        }

        public StubRow CurrentRow
        {
            get
            {
                return this.rows[this.currentRowIndex];
            }
        }

        public object this[string key]
        {
            get
            {
                return CurrentRow[GetIndexFromFieldName(key)];
            }
        }

        public object this[int i]
        {
            get
            {
                return CurrentRow[i];
            }
        }
    }

    //We can always replace the internal implementation with something better later.
    public class NameIndexCollection : NameObjectCollectionBase
    {
        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="key">The name.</param>
        /// <param name="index">The index.</param>
        public void Add(string key, int index)
        {
            this.BaseAdd(key, index);
        }

        /// <summary>
        /// Gets the <see cref="Int32"/> with the specified key.
        /// </summary>
        /// <value></value>
        public int this[string key]
        {
            get
            {
                return (int)this.BaseGet(key);
            }
        }

        public string[] GetAllKeys()
        {
            return this.BaseGetAllKeys();
        }
    }

    public class StubRow
    {
        object[] rowValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="StubRow"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public StubRow(params object[] values)
        {
            this.rowValues = values;
        }

        /// <summary>
        /// Gets the <see cref="Object"/> with the specified i.
        /// </summary>
        /// <value></value>
        public object this[int i]
        {
            get
            {
                return rowValues[i];
            }
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="StubRow">StubRow</see>.
    /// </summary>
    [Serializable]
    public class StubRowCollection : CollectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StubRowCollection">StubRowCollection</see> class.
        /// </summary>
        public StubRowCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubRowCollection">StubRowCollection</see> class containing the elements of the specified source collection.
        /// </summary>
        /// <param name="value">A <see cref="StubRowCollection">StubRowCollection</see> with which to initialize the collection.</param>
        public StubRowCollection(StubRowCollection value)
        {
            if (value != null)
            {
                this.AddRange(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubRowCollection">StubRowCollection</see> class containing the specified array of <see cref="StubRow">StubRow</see> Components.
        /// </summary>
        /// <param name="value">An array of <see cref="StubRow">StubRow</see> Components with which to initialize the collection. </param>
        public StubRowCollection(StubRow[] value)
        {
            if (value != null)
            {
                this.AddRange(value);
            }
        }

        /// <summary>
        /// Gets the <see cref="StubRowCollection">StubRowCollection</see> at the specified index in the collection.
        /// <para>
        /// In C#, this property is the indexer for the <see cref="StubRowCollection">StubRowCollection</see> class.
        /// </para>
        /// </summary>
        public StubRow this[int index]
        {
            get { return ((StubRow)(this.List[index])); }
        }

        public int Add(StubRow value)
        {
            if (value != null)
            {
                return this.List.Add(value);
            }
            return -1;
        }

        /// <summary>
        /// Copies the elements of the specified <see cref="StubRow">StubRow</see> array to the end of the collection.
        /// </summary>
        /// <param name="value">An array of type <see cref="StubRow">StubRow</see> containing the Components to add to the collection.</param>
        public void AddRange(StubRow[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot add a range from null.");

            if (value != null)
            {
                for (int i = 0; (i < value.Length); i = (i + 1))
                {
                    this.Add(value[i]);
                }
            }
        }

        /// <summary>
        /// Adds the contents of another <see cref="StubRowCollection">StubRowCollection</see> to the end of the collection.
        /// </summary>
        /// <param name="value">A <see cref="StubRowCollection">StubRowCollection</see> containing the Components to add to the collection. </param>
        public void AddRange(StubRowCollection value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot add a range from null.");

            if (value != null)
            {
                for (int i = 0; (i < value.Count); i = (i + 1))
                {
                    this.Add((StubRow)value.List[i]);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the specified <see cref="StubRowCollection">StubRowCollection</see>.
        /// </summary>
        /// <param name="value">The <see cref="StubRowCollection">StubRowCollection</see> to search for in the collection.</param>
        /// <returns><b>true</b> if the collection contains the specified object; otherwise, <b>false</b>.</returns>
        public bool Contains(StubRow value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Copies the collection Components to a one-dimensional <see cref="T:System.Array">Array</see> instance beginning at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array">Array</see> that is the destination of the values copied from the collection.</param>
        /// <param name="index">The index of the array at which to begin inserting.</param>
        public void CopyTo(StubRow[] array, int index)
        {
            this.List.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the index in the collection of the specified <see cref="StubRowCollection">StubRowCollection</see>, if it exists in the collection.
        /// </summary>
        /// <param name="value">The <see cref="StubRowCollection">StubRowCollection</see> to locate in the collection.</param>
        /// <returns>The index in the collection of the specified object, if found; otherwise, -1.</returns>
        public int IndexOf(StubRow value)
        {
            return this.List.IndexOf(value);
        }

        public void Insert(int index, StubRow value)
        {
            List.Insert(index, value);
        }

        public void Remove(StubRow value)
        {
            List.Remove(value);
        }

        public void Sort()
        {
            this.InnerList.Sort();
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the <see cref="StubRowCollection">StubRowCollection</see> instance.
        /// </summary>
        /// <returns>An <see cref="StubRowCollectionEnumerator">StubRowCollectionEnumerator</see> for the <see cref="StubRowCollection">StubRowCollection</see> instance.</returns>
        public new StubRowCollectionEnumerator GetEnumerator()
        {
            return new StubRowCollectionEnumerator(this);
        }

        /// <summary>
        /// Supports a simple iteration over a <see cref="StubRowCollection">StubRowCollection</see>.
        /// </summary>
        public class StubRowCollectionEnumerator : IEnumerator
        {
            private IEnumerator _enumerator;
            private IEnumerable _temp;

            /// <summary>
            /// Initializes a new instance of the <see cref="StubRowCollectionEnumerator">StubRowCollectionEnumerator</see> class referencing the specified <see cref="StubRowCollection">StubRowCollection</see> object.
            /// </summary>
            /// <param name="mappings">The <see cref="StubRowCollection">StubRowCollection</see> to enumerate.</param>
            public StubRowCollectionEnumerator(StubRowCollection mappings)
            {
                _temp = mappings;
                _enumerator = _temp.GetEnumerator();
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public StubRow Current
            {
                get { return ((StubRow)(_enumerator.Current)); }
            }

            object IEnumerator.Current
            {
                get { return _enumerator.Current; }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns><b>true</b> if the enumerator was successfully advanced to the next element; <b>false</b> if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            bool IEnumerator.MoveNext()
            {
                return _enumerator.MoveNext();
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                _enumerator.Reset();
            }

            void IEnumerator.Reset()
            {
                _enumerator.Reset();
            }
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="StubResultSet">ResultSet</see>.
    /// </summary>
    [Serializable]
    public class StubResultSetCollection : CollectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StubResultSetCollection">ResultSetCollection</see> class.
        /// </summary>
        public StubResultSetCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubResultSetCollection">ResultSetCollection</see> class containing the elements of the specified source collection.
        /// </summary>
        /// <param name="value">A <see cref="StubResultSetCollection">ResultSetCollection</see> with which to initialize the collection.</param>
        public StubResultSetCollection(StubResultSetCollection value)
        {
            if (value != null)
            {
                this.AddRange(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubResultSetCollection">ResultSetCollection</see> class containing the specified array of <see cref="StubResultSet">ResultSet</see> Components.
        /// </summary>
        /// <param name="value">An array of <see cref="StubResultSet">ResultSet</see> Components with which to initialize the collection. </param>
        public StubResultSetCollection(StubResultSet[] value)
        {
            if (value != null)
            {
                this.AddRange(value);
            }
        }

        /// <summary>
        /// Gets the <see cref="StubResultSetCollection">ResultSetCollection</see> at the specified index in the collection.
        /// <para>
        /// In C#, this property is the indexer for the <see cref="StubResultSetCollection">ResultSetCollection</see> class.
        /// </para>
        /// </summary>
        public StubResultSet this[int index]
        {
            get { return ((StubResultSet)(this.List[index])); }
        }

        public int Add(StubResultSet value)
        {
            if (value != null)
            {
                return this.List.Add(value);
            }
            return -1;
        }

        /// <summary>
        /// Copies the elements of the specified <see cref="StubResultSet">ResultSet</see> array to the end of the collection.
        /// </summary>
        /// <param name="value">An array of type <see cref="StubResultSet">ResultSet</see> containing the Components to add to the collection.</param>
        public void AddRange(StubResultSet[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot add a range from null.");

            if (value != null)
            {
                for (int i = 0; (i < value.Length); i = (i + 1))
                {
                    this.Add(value[i]);
                }
            }
        }

        /// <summary>
        /// Adds the contents of another <see cref="StubResultSetCollection">ResultSetCollection</see> to the end of the collection.
        /// </summary>
        /// <param name="value">A <see cref="StubResultSetCollection">ResultSetCollection</see> containing the Components to add to the collection. </param>
        public void AddRange(StubResultSetCollection value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot add a range from null.");

            if (value != null)
            {
                for (int i = 0; (i < value.Count); i = (i + 1))
                {
                    this.Add((StubResultSet)value.List[i]);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the specified <see cref="StubResultSetCollection">ResultSetCollection</see>.
        /// </summary>
        /// <param name="value">The <see cref="StubResultSetCollection">ResultSetCollection</see> to search for in the collection.</param>
        /// <returns><b>true</b> if the collection contains the specified object; otherwise, <b>false</b>.</returns>
        public bool Contains(StubResultSet value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Copies the collection Components to a one-dimensional <see cref="T:System.Array">Array</see> instance beginning at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array">Array</see> that is the destination of the values copied from the collection.</param>
        /// <param name="index">The index of the array at which to begin inserting.</param>
        public void CopyTo(StubResultSet[] array, int index)
        {
            this.List.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the index in the collection of the specified <see cref="StubResultSetCollection">ResultSetCollection</see>, if it exists in the collection.
        /// </summary>
        /// <param name="value">The <see cref="StubResultSetCollection">ResultSetCollection</see> to locate in the collection.</param>
        /// <returns>The index in the collection of the specified object, if found; otherwise, -1.</returns>
        public int IndexOf(StubResultSet value)
        {
            return this.List.IndexOf(value);
        }

        public void Insert(int index, StubResultSet value)
        {
            List.Insert(index, value);
        }

        public void Remove(StubResultSet value)
        {
            List.Remove(value);
        }

        public void Sort()
        {
            this.InnerList.Sort(new SortListCategoryComparer());
        }

        private sealed class SortListCategoryComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                StubResultSet first = (StubResultSet)x;
                StubResultSet second = (StubResultSet)y;
                return first.ToString().CompareTo(second.ToString());
            }
        }


        /// <summary>
        /// Returns an enumerator that can iterate through the <see cref="StubResultSetCollection">ResultSetCollection</see> instance.
        /// </summary>
        /// <returns>An <see cref="ResultSetCollectionEnumerator">ResultSetCollectionEnumerator</see> for the <see cref="StubResultSetCollection">ResultSetCollection</see> instance.</returns>
        public new ResultSetCollectionEnumerator GetEnumerator()
        {
            return new ResultSetCollectionEnumerator(this);
        }

        /// <summary>
        /// Supports a simple iteration over a <see cref="StubResultSetCollection">ResultSetCollection</see>.
        /// </summary>
        public class ResultSetCollectionEnumerator : IEnumerator
        {
            private IEnumerator _enumerator;
            private IEnumerable _temp;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResultSetCollectionEnumerator">ResultSetCollectionEnumerator</see> class referencing the specified <see cref="StubResultSetCollection">ResultSetCollection</see> object.
            /// </summary>
            /// <param name="mappings">The <see cref="StubResultSetCollection">ResultSetCollection</see> to enumerate.</param>
            public ResultSetCollectionEnumerator(StubResultSetCollection mappings)
            {
                _temp = mappings;
                _enumerator = _temp.GetEnumerator();
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public StubResultSet Current
            {
                get { return ((StubResultSet)(_enumerator.Current)); }
            }

            object IEnumerator.Current
            {
                get { return _enumerator.Current; }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns><b>true</b> if the enumerator was successfully advanced to the next element; <b>false</b> if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            bool IEnumerator.MoveNext()
            {
                return _enumerator.MoveNext();
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                _enumerator.Reset();
            }

            void IEnumerator.Reset()
            {
                _enumerator.Reset();
            }
        }
    }
}
