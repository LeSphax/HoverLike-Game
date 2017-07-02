using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace Utilities
{
    [Serializable]
    public class Map<T1, T2>
    {
        private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public int Count
        {
            get
            {
                Assert.AreEqual(_forward.Count, _reverse.Count);
                return _forward.Count;
            }
        }

        public Map()
        {
            Init();
        }

        private void Init()
        {
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        [Serializable]
        public class Indexer<T3, T4>
        {
            private Dictionary<T3, T4> _dictionary;

            public Dictionary<T3, T4>.KeyCollection Keys
            {
                get
                {
                    return _dictionary.Keys;
                }
            }

            public Indexer()
            {

            }

            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }
            public bool TryGetValue(T3 index, out T4 value)
            {
                return _dictionary.TryGetValue(index, out value);
            }

            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public void Remove(T1 t1, T2 t2)
        {
            _forward.Remove(t1);
            _reverse.Remove(t2);
        }

        public bool ContainsKey(T1 key)
        {
            return _forward.ContainsKey(key);
        }

        public bool ContainsKey(T2 key)
        {
            return _reverse.ContainsKey(key);
        }

        public XmlSchema GetSchema()
        {
            return ((IXmlSerializable)_forward).GetSchema();
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer forwardSerializer = new XmlSerializer(typeof(Dictionary<T1, T2>));
            XmlSerializer reverseSerializer = new XmlSerializer(typeof(Dictionary<T2, T1>));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            reader.ReadStartElement("forward");
            _forward = (Dictionary<T1, T2>)forwardSerializer.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("reverse");
            _reverse = (Dictionary<T2,T1>)reverseSerializer.Deserialize(reader);
            reader.ReadEndElement();

            Init();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer forwardSerializer = new XmlSerializer(typeof(Dictionary<T1, T2>));
            XmlSerializer reverseSerializer = new XmlSerializer(typeof(Dictionary<T2, T1>));

            writer.WriteStartElement("forward");
            forwardSerializer.Serialize(writer, _forward);
            writer.WriteEndElement();

            writer.WriteStartElement("reverse");
            reverseSerializer.Serialize(writer, _reverse);
            writer.WriteEndElement();
        }

        public Indexer<T1, T2> Forward { get; set; }
        public Indexer<T2, T1> Reverse { get; set; }
    }
}
