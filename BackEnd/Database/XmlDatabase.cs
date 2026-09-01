using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MauiApp1.BackEnd.Database
{
    public static class XmlDatabase
    {
        static readonly object _sync = new object();

        // Default locations (adjust at runtime as needed)
        public static string DefaultXmlPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "Resources", "MediaDatabase.xml");
        public static string DefaultXsdPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "Resources", "DatabaseSchema.xsd");

        // Read whole table into typed objects
        public static List<T> ReadTable<T>(string tableName, string xmlPath = null, string xsdPath = null)
        {
            xmlPath = xmlPath ?? DefaultXmlPath;
            xsdPath = xsdPath ?? DefaultXsdPath;

            lock (_sync)
            {
                EnsureXmlExists(xmlPath);
                var doc = XDocument.Load(xmlPath);

                var container = doc.Root?.Element(tableName);
                if (container == null) return new List<T>();

                string elementName = GetElementNameForType(typeof(T), tableName);

                var elements = container.Elements(elementName);
                var result = new List<T>();
                foreach (var el in elements)
                {
                    var obj = DeserializeFromXElement<T>(el);
                    result.Add(obj);
                }
                return result;
            }
        }

        // Add object to specified table. Will set a unique integer id if Id/SeriesId is present and <= 0.
        public static bool AddToTable<T>(string tableName, T item, string xmlPath = null)
        {
            xmlPath = xmlPath ?? DefaultXmlPath;
            try
            {
                lock (_sync)
                {
                    EnsureXmlExists(xmlPath);
                    var doc = XDocument.Load(xmlPath);

                    var root = doc.Root ?? throw new InvalidOperationException("XML root missing");
                    var container = root.Element(tableName);
                    if (container == null)
                    {
                        container = new XElement(tableName);
                        root.Add(container);
                    }

                    string elementName = GetElementNameForType(typeof(T), tableName);
                    // ensure unique id
                    SetUniqueIdForNewItem(container, elementName, item);

                    var el = SerializeToXElement(item, elementName);
                    container.Add(el);

                    doc.Save(xmlPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g. log them)
                throw new Exception("Error adding item to XML database", ex);
            }
        }

        // Update first item that matches predicate; returns true if updated
        public static bool UpdateInTable<T>(string tableName, Func<T, bool> predicate, T newItem, string xmlPath = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            xmlPath = xmlPath ?? DefaultXmlPath;

            lock (_sync)
            {
                EnsureXmlExists(xmlPath);
                var doc = XDocument.Load(xmlPath);
                var container = doc.Root?.Element(tableName);
                if (container == null) return false;

                string elementName = GetElementNameForType(typeof(T), tableName);

                foreach (var el in container.Elements(elementName).ToList())
                {
                    var obj = DeserializeFromXElement<T>(el);
                    if (predicate(obj))
                    {
                        var newEl = SerializeToXElement(newItem, elementName);
                        el.ReplaceWith(newEl);
                        doc.Save(xmlPath);
                        return true;
                    }
                }
                return false;
            }
        }

        // Delete first item that matches predicate; returns true if deleted
        public static bool DeleteFromTable<T>(string tableName, Func<T, bool> predicate, string xmlPath = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            xmlPath = xmlPath ?? DefaultXmlPath;

            lock (_sync)
            {
                EnsureXmlExists(xmlPath);
                var doc = XDocument.Load(xmlPath);
                var container = doc.Root?.Element(tableName);
                if (container == null) return false;

                string elementName = GetElementNameForType(typeof(T), tableName);

                foreach (var el in container.Elements(elementName).ToList())
                {
                    var obj = DeserializeFromXElement<T>(el);
                    if (predicate(obj))
                    {
                        el.Remove();
                        doc.Save(xmlPath);
                        return true;
                    }
                }
                return false;
            }
        }

        // Helper: ensure xml file and root/schema element exist
        static void EnsureXmlExists(string xmlPath)
        {
            var dir = Path.GetDirectoryName(xmlPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (!File.Exists(xmlPath))
            {
                // Create minimal document matching DatabaseSchema.xsd root
                var root = new XElement("Library",
                    new XElement("Books"),
                    new XElement("Music"),
                    new XElement("Videos"),
                    new XElement("Others"),
                    new XElement("Collections")
                );
                var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
                doc.Save(xmlPath);
            }
        }

        // Map types -> element names. Cd must map to "music"
        static string GetElementNameForType(Type t, string tableName)
        {
            if (t.Name.Equals("Cd", StringComparison.OrdinalIgnoreCase) ||
                t.FullName?.EndsWith(".Cd") == true)
                return "music";

            // For collections, we expect 'Collection' elements inside 'Collections'
            if (t.Name.Equals("Collection", StringComparison.OrdinalIgnoreCase))
                return "Collection";

            // Default: use type name (Book, Video, Other)
            return t.Name;
        }

        // Serialize object to XElement with given element name
        static XElement SerializeToXElement<T>(T item, string elementName)
        {
            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(elementName));
            using var ms = new MemoryStream();
            using var writer = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = new System.Text.UTF8Encoding(false), OmitXmlDeclaration = true });
            serializer.Serialize(writer, item);
            ms.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(ms);
            var s = sr.ReadToEnd();
            return XElement.Parse(s);
        }

        // Deserialize XElement to T
        static T DeserializeFromXElement<T>(XElement el)
        {
            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(el.Name.LocalName));
            using var sr = new StringReader(el.ToString());
            return (T)serializer.Deserialize(sr)!;
        }

        // Determine id property name ("SeriesId" for Collections, otherwise "Id")
        static string GetIdPropertyName(Type t)
        {
            if (t.Name.Equals("Collection", StringComparison.OrdinalIgnoreCase))
                return "SeriesId";

            return "Id";
        }
        
        // Ensure new item gets a unique positive int id; supports Id or SeriesId properties (int or int?)
        static void SetUniqueIdForNewItem<T>(XElement container, string elementName, T item)
        {
            var t = typeof(T);
            var idPropName = GetIdPropertyName(t);
            var idProp = t.GetProperty(idPropName, BindingFlags.Public | BindingFlags.Instance);
            if (idProp == null) return;

            // examine existing ids in container
            var existingIds = container.Elements(elementName)
                .Select(x => x.Element(idPropName))
                .Where(x => x != null)
                .Select(x =>
                {
                    if (int.TryParse(x.Value, out var v)) return v;
                    return (int?)null;
                })
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

            int nextId = 1;
            if (existingIds.Any())
            {
                nextId = existingIds.Max() + 1;
            }

            // read current value
            var currentVal = idProp.GetValue(item);
            int currInt = 0;
            if (currentVal is int i) currInt = i;

            if (currInt <= 0 || existingIds.Contains(currInt))
            {
                // set new id
                if (idProp.PropertyType == typeof(int) || idProp.PropertyType == typeof(int?))
                {
                    idProp.SetValue(item, nextId);
                }
            }
        }
        

        // Optional: validate the XML against the XSD. Throws XmlSchemaValidationException on error.
        public static void ValidateXml(string xmlPath = null, string xsdPath = null)
        {
            xmlPath = xmlPath ?? DefaultXmlPath;
            xsdPath = xsdPath ?? DefaultXsdPath;
            if (!File.Exists(xmlPath)) throw new FileNotFoundException("XML file not found", xmlPath);
            if (!File.Exists(xsdPath)) throw new FileNotFoundException("XSD file not found", xsdPath);

            var schemas = new XmlSchemaSet();
            schemas.Add("", xsdPath);

            var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema, Schemas = schemas };
            settings.ValidationEventHandler += (s, e) =>
            {
                if (e.Severity == XmlSeverityType.Error)
                    throw new XmlSchemaValidationException(e.Message, e.Exception);
            };

            using var reader = XmlReader.Create(xmlPath, settings);
            while (reader.Read()) { /* validation happens while reading */ }
        }
    }
}