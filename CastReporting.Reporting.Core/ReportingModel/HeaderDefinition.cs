using System;
using System.Collections;
using System.Collections.Generic;

namespace CastReporting.Reporting.ReportingModel
{
    public class HeaderDefinition {
        public HeaderDefinition(params string[] headers) {
            Headers = new List<string>(headers);
        }

        protected List<string> Headers;

        public int IndexOf(string header) {
            return Headers.IndexOf(header);
        }

        public void Insert(int pos, string header) {
            if (ReadOnly)
                throw new InvalidOperationException("Cannot add a new header after data has been added");
            Headers.Insert(pos, header);
        }

        public void Append(string header) {
            if (ReadOnly)
                throw new InvalidOperationException("Cannot add a new header after data has been added");
            Headers.Add(header);
        }

        public void Append(string header, bool enabled) {
            if (enabled)
                Append(header);
        }

        public IEnumerable<string> Labels => Headers;

        public int Count => Headers.Count;

        private bool ReadOnly => _data != null;

        private DataDefinition _data;

        public IDataDefinition CreateDataRow()
        {
            return _data ?? (_data = new DataDefinition(this));
        }

        private class DataDefinition : IDataDefinition
        {
            public DataDefinition(HeaderDefinition headerDef) {
                _headerPos = new Dictionary<string, int>();
                for (int i = 0; i < headerDef.Headers.Count; i++) {
                    _headerPos.Add(headerDef.Headers[i], i);
                }
                _datas = new string[headerDef.Count];
            }

            private readonly Dictionary<string, int> _headerPos;
            private readonly string[] _datas;

            public void Set(int pos, string value) {
                if (0 <= pos && pos < _datas.Length)
                    _datas[pos] = value ?? string.Empty;
            }

            public void Set(string label, string value) {
                int pos;
                if (_headerPos.TryGetValue(label, out pos))
                    _datas[pos] = value ?? string.Empty;
            }

            public void Reset() {
                for (int i = 0; i < _datas.Length; i++)
                    _datas[i] = string.Empty;
            }

            public IEnumerator<string> GetEnumerator() {
                return Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return Values.GetEnumerator();
            }

            public IEnumerable<string> Values => _datas;
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
