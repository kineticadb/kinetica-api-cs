using Avro;
using Avro.IO;
using Avro.Specific;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace kinetica
{
    /// <summary>
    /// KineticaData - class to help with Avro Encoding for Kinetica
    /// </summary>
    public class KineticaData : ISpecificRecord
    {
        private RecordSchema m_schema;
        private PropertyInfo[] m_properties;

        /// <summary>
        /// Avro Schema for this class
        /// </summary>
        public Schema Schema
        {
            get
            {
                return m_schema;
            }
        }

        /// <summary>
        /// Constructor from Kinetica Type
        /// </summary>
        /// <param name="type">Type received from Kinetica Server</param>
        public KineticaData(KineticaType type)
        {
            m_schema = Avro.Schema.Parse(type.getSchemaString()) as RecordSchema;
            m_properties = this.GetType().GetProperties();
        }

        /// <summary>
        /// Default constructor, with optional System.Type
        /// </summary>
        /// <param name="type">System.Type for this class to map</param>
        public KineticaData(System.Type type = null)
        {
            var t = type ?? this.GetType();
            m_schema = SchemaFromType(t, null);
            m_properties = this.GetType().GetProperties();
        }

        /// <summary>
        /// Retrieve a specific property from this object
        /// </summary>
        /// <param name="fieldPos">Index of property to retrieve</param>
        /// <returns>Value of the requested property</returns>
        public object Get(int fieldPos)
        {
            return m_properties[fieldPos].GetValue(this);
        }

        /// <summary>
        /// Write a specific property to this object
        /// </summary>
        /// <param name="fieldPos">Index of the property to write</param>
        /// <param name="fieldValue">New value to be written</param>
        public void Put(int fieldPos, object fieldValue)
        {
            m_properties[fieldPos].SetValue(this, fieldValue);
        }



        /// <summary>
        /// Create an Avro Schema from a System.Type and a KineticaType.
        /// </summary>
        /// <param name="t">System.Type to be processed.</param>
        /// <param name="ktype">KineticaType to be processed;</param>
        /// <returns></returns>
        public static RecordSchema SchemaFromType( System.Type t, KineticaType ktype = null )
        {
            string jsonType = AvroType( t, ktype );
            return Avro.Schema.Parse( jsonType ) as RecordSchema;
        }


        /// <summary>
        /// Create a JSON type-string from System.Type
        /// </summary>
        /// <param name="t">System.Type to be evaluated</param>
        /// <param name="ktype">A KineticaType object that describes the whole type
        /// to which <paramref name="t"/> belongs. </param>
        /// <returns>JSON-formatted String</returns>
        private static string AvroType( System.Type t, KineticaType ktype )
        {
            if ( t == null )
                throw new KineticaException( "Null type passed to AvroType()" );

            switch ( t.Name)
            {
                case "Boolean": return "\"boolean\"";
                case "Int32": return "\"int\"";
                case "Int64": return "\"long\"";
                case "Double": return "\"double\"";
                case "Single": return "\"float\"";
                case "Byte[]": return "\"bytes\"";
                case "String": return "\"string\"";
                case "String[]": return $"{{ \"type\":\"array\", \"items\":\"string\"}}";
                case "String[][]": return $"{{ \"type\":\"array\", \"items\":{{ \"type\":\"array\", \"items\":\"string\"}}}}";

                // For a nullable object, return the avro type of the underlying type (e.g. double)
                case "Nullable`1": return AvroType( Nullable.GetUnderlyingType( t ), ktype );

                case "List`1":
                case "IList`1":
                    if ( t.IsGenericType )
                    {
                        var genericParams = t.GenericTypeArguments;
                        if (1 == genericParams.Length)
                        {
                            return $"{{ \"type\":\"array\", \"items\":{AvroType( genericParams[0], ktype )}}}";
                        }
                    }
                    break;

                case "Dictionary`2":
                case "IDictionary`2":
                    if (t.IsGenericType)
                    {
                        var genericParams = t.GenericTypeArguments;
                        if (2 == genericParams.Length)
                        {
                            return $"{{ \"type\":\"map\", \"values\":{AvroType( genericParams[1], ktype )}}}";
                        }
                    }
                    break;

                // Ignore the "Schema" property inherited from KineticaData
                case "Schema": break;

                // Ignore the "RecordSchema" property inherited from KineticaRecord
                case "RecordSchema":
                    break;

                // If Type is an object, treat it as a sub-record in Avro
                default:
                    if (t.IsSubclassOf(typeof(Object)))
                    {
                        string fields = "";
                        // Create the avro string for each property of the class
                        foreach (var prop in t.GetProperties())
                        {
                            bool is_nullable = false;
                            var prop_type = prop.PropertyType;
                            if ( prop_type.IsGenericType && prop_type.GetGenericTypeDefinition() == typeof( Nullable<> ) )
                            {   // the property is nullable based on reflection
                                is_nullable = true;
                            }
                            else if ( (ktype != null) && ktype.getColumn( prop.Name ).isNullable() )
                            {  // the property is nullable based on information saved in the associated KineticaType
                                is_nullable = true;
                            }

                            // Get the avro type string for the property type
                            string avroType = AvroType( prop_type, ktype );
                            if ( !String.IsNullOrWhiteSpace( avroType ) )
                            {
                                if ( is_nullable )
                                {   // the field is nullable
                                    fields += $"{{\"name\":\"{prop.Name}\",\"type\":[{avroType},\"null\"]}},";
                                }
                                else
                                {   // it's a regular field
                                    fields += $"{{\"name\":\"{prop.Name}\",\"type\":{avroType}}},";
                                }
                            }
                        }

                        // Trim the trailing comma from the fields
                        char[] comma = { ',' };
                        fields = fields.TrimEnd( comma );

                        // Put together the avro fields with the name to create a record type
                        return $"{{\"type\":\"record\",\"name\":\"{t.Name}\",\"fields\":[{fields}]}}";
                    }
                    System.Diagnostics.Debug.WriteLine($"Unkonwn type: {t.Name}"); break;
            }

            return "";
        }  // end AvroType

        /* Code to copy current object into a new GenericRecord - Not currently used (or tested)
        public Avro.Generic.GenericRecord CopyTo()
        {
            Avro.Generic.GenericRecord record = new Avro.Generic.GenericRecord(m_schema);
            foreach (var prop in m_properties)
            {
                if (m_schema.Contains(prop.Name))
                {
                    record.Add(prop.Name, prop.GetValue(this));
                }
            }

            return record;
        }
        */
    }
}
