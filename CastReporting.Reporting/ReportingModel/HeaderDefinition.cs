using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CastReporting.Reporting.ReportingModel
{
    public class HeaderDefinition {
        public HeaderDefinition(params string[] headers) {
            _headers = new List<string>(headers);
        }

        private List<string> _headers;

        public int IndexOf(string header) {
            return _headers.IndexOf(header);
        }

        public void Insert(int pos, string header) {
            if (ReadOnly)
                throw new InvalidOperationException("Cannot add a new header after data has been added");
            _headers.Insert(pos, header);
        }

        public void Append(string header) {
            if (ReadOnly)
                throw new InvalidOperationException("Cannot add a new header after data has been added");
            _headers.Add(header);
        }

        public void Append(string header, bool enabled) {
            if (enabled)
                Append(header);
        }

        public void Remove(string header) {
            if (ReadOnly)
                throw new InvalidOperationException("Cannot remove a header after data has been added");
            _headers.Remove(header);
        }

        public void InsertBefore(string header, string newHeader) {
            var pos = IndexOf(header);
            if (pos < 0) {
                Insert(0, newHeader);
            } else {
                Insert(pos, newHeader);
            }
        }

        public void InsertAfter(string header, string newHeader) {
            var pos = IndexOf(header);
            if (pos < 0) {
                Append(newHeader);
            } else {
                Insert(pos, newHeader);
            }
        }

        public IEnumerable<string> Labels { get { return _headers; } }

        public int Count { get { return _headers.Count; } }

        private bool ReadOnly { get { return data != null; } }

        private DataDefinition data;

        public IDataDefinition CreateDataRow() { 
            if (data == null)
                data = new DataDefinition(this);
            return data;
        }

        private class DataDefinition : IDataDefinition
        {
            public DataDefinition(HeaderDefinition headerDef) {
                headers = headerDef;
                headerPos = new Dictionary<string, int>();
                for (int i = 0; i < headerDef._headers.Count; i++) {
                    headerPos.Add(headerDef._headers[i], i);
                }
                data = new string[headerDef.Count];
            }

            private readonly HeaderDefinition headers;
            private readonly Dictionary<string, int> headerPos;
            private readonly string[] data;

            public void Set(int pos, string value) {
                if (0 <= pos && pos < data.Length)
                    data[pos] = (value == null) ? string.Empty : value;
            }

            public void Set(string label, string value) {
                int pos;
                if (headerPos.TryGetValue(label, out pos))
                    data[pos] = (value == null) ? string.Empty : value;
            }

            public void Reset() {
                for (int i = 0; i < data.Length; i++)
                    data[i] = string.Empty;
            }

            public IEnumerator<string> GetEnumerator() {
                return Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return Values.GetEnumerator();
            }

            public IEnumerable<string> Values { get { return data; } }
        }

        public interface IDataDefinition : IEnumerable<string>
        {
            void Set(int pos, string value);
            void Set(string label, string value);
            void Reset();
            IEnumerable<string> Values { get; }
        }
    }
}
