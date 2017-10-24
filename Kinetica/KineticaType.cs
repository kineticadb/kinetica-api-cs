using System;
using System.Collections.Generic;
using System.Linq;
using Avro;
using Newtonsoft.Json.Linq;

namespace kinetica
{
    public class KineticaType
    {
        public class Column
        {
            public enum ColumnType
            {
                BYTES = Avro.EnumSchema.Type.Bytes,
                DOUBLE = Avro.EnumSchema.Type.Double,
                FLOAT = Avro.EnumSchema.Type.Float,
                INT = Avro.EnumSchema.Type.Int,
                LONG = Avro.EnumSchema.Type.Long,
                STRING = Avro.EnumSchema.Type.String,
                DEFAULT = Avro.EnumSchema.Type.Error
            };

            private string m_name;
            private ColumnType m_type;
            private bool m_isNullable;
            private IList<string> m_properties;

            /// <summary>
            /// Creates a Column object from the given name, type, and properties.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="type"></param>
            /// <param name="properties"></param>
            public Column(string name, ColumnType type, IList<string> properties = null)
            {
                m_name = name;
                m_type = type;
                m_isNullable = false;
                m_properties = properties ?? new List<string>();

                initialize();
            }

            /// <summary>
            /// Returns the name of the column.
            /// </summary>
            /// <returns></returns>
            public string getName() { return m_name; }

            /// <summary>
            /// Returns the enumeration for the column type.
            /// </summary>
            /// <returns></returns>
            public ColumnType getType() { return m_type; }

            /// <summary>
            /// Returns if the column is nullable.
            /// </summary>
            /// <returns></returns>
            public bool isNullable() { return m_isNullable; }

            /// <summary>
            /// Returns the properties for the column.
            /// </summary>
            /// <returns></returns>
            public IList<string> getProperties() { return m_properties; }

            internal void setIsNullable( bool val ) { m_isNullable = val;  }

            /// <summary>
            /// Returns the string format of the data type.
            /// </summary>
            /// <returns></returns>
            public string getTypeString()
            {
                switch (m_type)
                {
                    case ColumnType.BYTES:
                        return "bytes";
                    case ColumnType.DOUBLE:
                        return "double";
                    case ColumnType.FLOAT:
                        return "float";
                    case ColumnType.INT:
                        return "int";
                    case ColumnType.LONG:
                        return "long";
                    case ColumnType.STRING:
                        return "string";
                    default:
                        throw new KineticaException( "Unsupported column type: " + m_type );
                }
            }  // end getTypeString()

            private void initialize()
            {
                if (string.IsNullOrEmpty(m_name))
                {
                    throw new ArgumentException("Name must not be empty.");
                }

                switch (m_type)
                {
                    case ColumnType.BYTES:
                    case ColumnType.DOUBLE:
                    case ColumnType.FLOAT:
                    case ColumnType.INT:
                    case ColumnType.LONG:
                    case ColumnType.STRING:
                        break;

                    default:
                        throw new ArgumentException("Column " + m_name + " must be of type BYTES, DOUBLE, FLOAT, INT, LONG or STRING.");
                }

                foreach (var it in m_properties)
                {
                    if (string.IsNullOrEmpty(it))
                    {
                        throw new ArgumentException("Properties must not be empty.");
                    }

                    if (!m_isNullable && (it == ColumnProperty.NULLABLE))
                    {
                        m_isNullable = true;
                    }
                }
            }

            public override string ToString()
            {
                return $"{m_name} ({m_type.ToString()})";
            }
        }  // end class Column

        private class TypeData
        {
            public string label;
            public IList<Column> columns = new List<Column>();
            public Dictionary<string, int> columnMap = new Dictionary<string, int>();
            public string schemaString = null;
            public Schema schema = null;
            public Type sourceType = null;
        }

        private TypeData m_data = new TypeData();
        private IDictionary<string, IList<string>> m_properties = new Dictionary<string, IList<string>>();
        private string m_type_id = null;

        /// <summary>
        /// Create a KineticaType object based on an existing table in the database.
        /// </summary>
        /// <param name="kinetica"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static KineticaType fromTable(Kinetica kinetica, string tableName)
        {
            var response = kinetica.showTable(tableName);
            var typeIdCount = response.type_ids.Count;

            if (typeIdCount == 0)
            {
                throw new KineticaException("Table " + tableName + " does not exist.");
            }

            string typeID = response.type_ids[0];
            if (typeIdCount > 1)
            {
                for (int i = 1; i < typeIdCount; ++i)
                {
                    if (response.type_ids[i] != typeID)
                    {
                        throw new KineticaException("Table " + tableName + " is not homogeneous.");
                    }
                }
            }

            return new KineticaType(response.type_labels[0], response.type_schemas[0], response.properties[0], typeID );
        }

        /// <summary>
        /// Create a KineticaType object based on an existing type in the database.
        /// </summary>
        /// <param name="kinetica"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static KineticaType fromTypeID(Kinetica kinetica, string typeId)
        {
            var response = kinetica.showTypes(typeId, "");

            if (response.type_ids.Count < 1)
            {
                throw new KineticaException("Type " + typeId + " does not exist.");
            }

            return new KineticaType(response.labels[0], response.type_schemas[0], response.properties[0]);
        }


        /// <summary>
        /// Create a KineticaType object based on information provided in a dynamic schema.
        /// </summary>
        /// <param name="dynamic_table_schema_string">The dynamic schema string.</param>
        /// <param name="column_headers">List of column names.</param>
        /// <param name="column_types">List of column types.</param>
        /// <returns></returns>
        public static KineticaType fromDynamicSchema( string dynamic_table_schema_string,
                                                      Object[] column_headers, Object[] column_types )
        {
            // Make sure that the lists of column names and types are of the same length
            if ( column_headers.Length != column_types.Length )
                throw new KineticaException( "List of column names and types are not of the same length." );

            // Parse the schema string so that we can later check if a given column is nullable
            JObject dynamic_schema_json;
            try
            {
                dynamic_schema_json = JObject.Parse( dynamic_table_schema_string );
            }
            catch ( Exception ex )
            {
                throw new KineticaException( ex.ToString() );
            }

            // Create a delegate for checking if a field/column is nullable
            // ------------------------------------------------------------
            // The first parameter is the column name, the second is the JSON object
            var is_column_nulllable = new Func<string, JObject, bool>( (column_name, schema_json) => {
                // Find the appropriate field
                bool found_field = false;
                foreach ( var field in schema_json["fields"] )
                {
                    if ( (string)field[ "name" ] == column_name )
                    {
                        found_field = true; // found it!
                        // Get the type and see if it's a nullable type
                        // (each field is an array of the column type; so need
                        // to extract the type element first)
                        var type_element = field["type"]["items"];
                        if ( type_element is JValue ) // not an array, so can't be nullable
                            return false;
                        // If the type is an array and the second value is 'null', then it's a nullable
                        if ( (type_element is JArray) && ((string)((JArray)type_element)[ 1 ] == "null") )
                            return true;
                        return false;
                    } // end if
                }  // end foreach
                if ( !found_field )
                    throw new KineticaException( $"Could not find the field named '{column_name}'" );
                return false; // shouldn't ever get here
            } );

            // Create appropriate columns and column properties
            // ------------------------------------------------
            IList<Column> columns = new List<Column>();
            IDictionary<string, IList<string>> column_properties = new Dictionary<string, IList<string>>();
            for ( int i = 0; i < column_headers.Length; ++i )
            {
                // Get the column's name
                string column_name = column_headers[ i ].ToString();

                // Get the column's type in string format, which might be a property and not a primitive type
                // (if so, then we'll have to infer the primitive type and save the property)
                string column_type_string = column_types[ i ].ToString();

                // Need to create a list for the properties of the column (we'll
                // extract at most one from the column type)
                IList<string> column_property = new List<string>();

                // We need to also infer the primitive type for this column
                Column.ColumnType column_type = Column.ColumnType.DEFAULT;

                // Infer the type and property from the given 'type'
                switch ( column_type_string )
                {
                    // Primitive type string (and all properties based on it)
                    case "string":
                        column_type = Column.ColumnType.STRING;
                        break;

                    // All properties allowed for the primitive string type
                    case ColumnProperty.CHAR1:
                    case ColumnProperty.CHAR2:
                    case ColumnProperty.CHAR4:
                    case ColumnProperty.CHAR8:
                    case ColumnProperty.CHAR16:
                    case ColumnProperty.CHAR32:
                    case ColumnProperty.CHAR64:
                    case ColumnProperty.CHAR128:
                    case ColumnProperty.CHAR256:
                    case ColumnProperty.DATE:
                    case ColumnProperty.DATETIME:
                    case ColumnProperty.DECIMAL: 
                    case ColumnProperty.IPV4:
                    case ColumnProperty.TIME:
                        column_type = Column.ColumnType.STRING;
                        column_property.Add( column_type_string );
                        break;

                    // Primitive type integer
                    case "int":
                        column_type = Column.ColumnType.INT;
                        break;

                    // Properties allowed for the primitive integer type
                    case ColumnProperty.INT8:
                    case ColumnProperty.INT16:
                        column_type = Column.ColumnType.INT;
                        column_property.Add( column_type_string );
                        break;

                    // Primitive type long
                    case "long":
                        column_type = Column.ColumnType.LONG;
                        break;

                    // Properties allowed for the long type
                    case ColumnProperty.TIMESTAMP:
                        column_type = Column.ColumnType.LONG;
                        column_property.Add( column_type_string );
                        break;

                    // Primitive type float
                    case "float":
                        column_type = Column.ColumnType.FLOAT;
                        break;

                    // Primitive type double
                    case "double":
                        column_type = Column.ColumnType.DOUBLE;
                        break;

                    // Primitive type bytes
                    case "bytes":
                        column_type = Column.ColumnType.BYTES;
                        break;

                    default:
                        throw new KineticaException( "Unknown data type/property: " + column_type_string );
                }  // end switch

                // Check if the column is nullable (where the column name is "column_#" as returned by Kinetica)
                if ( is_column_nulllable( $"column_{i + 1}", dynamic_schema_json ) )
                    column_property.Add( ColumnProperty.NULLABLE );

                // Now that we have the name, the type and potentially a property for the column,
                // create a Column type and add it to the list
                Column column = new Column( column_name, column_type, column_property );
                columns.Add( column );

                // Also, save the column property in the column name->property map
                column_properties.Add( column_name, column_property );
            }  // end looping over column headers and types


            // Create and return the KineticaType object based on the columns and properties
            return new KineticaType( "", columns, column_properties );
        }  // end fromDynamicSchema()



        /// <summary>
        /// Create a KineticaType object from properties of a record class and Kinetica column properties.
        /// It ignores any properties inherited from base classes, and also ignores any member fields of
        /// the class.
        /// 
        /// For integer, long, float, and double column types, the user can use the nullable type (e.g. int?)
        /// to declare the column to be nullable.  The <paramref name="properties"/> does not need to contain
        /// the <see cref="ColumnProperty.NULLABLE"/> property.  However, for string type columns, instead of
        /// using nullable type, use the regular string type; additionally, add the
        /// <see cref="ColumnProperty.NULLABLE"/> in <paramref name="properties"/>.
        /// 
        /// </summary>
        /// <param name="recordClass">A class type.</param>
        /// <param name="properties">Properties for the columns.</param>
        /// <returns></returns>
        public static KineticaType fromClass( Type recordClass, IDictionary<string, IList<string>> properties = null )
        {
            return fromClass( recordClass, "", properties );
        }   // end fromClass()


        /// <summary>
        /// Create a KineticaType object from properties of a record class and Kinetica column properties.
        /// It ignores any properties inherited from base classes, and also ignores any member fields of
        /// the class.
        /// </summary>
        /// <param name="recordClass">A class type.</param>
        /// <param name="label">Any label for the type.</param>
        /// <param name="properties">Properties for the columns.</param>
        /// <returns></returns>
        public static KineticaType fromClass( Type recordClass, string label, IDictionary<string, IList<string>> properties = null )
        {
            // Get the fields in order (******skipping properties inherited from base classes******)
            // (fields only from this type, i.e. do not include any inherited fields), and public types only
            System.Reflection.PropertyInfo[] type_properties = recordClass.GetProperties( System.Reflection.BindingFlags.DeclaredOnly |
                                                                                          System.Reflection.BindingFlags.Instance |
                                                                                          System.Reflection.BindingFlags.Public );
            Array.Sort( type_properties, delegate ( System.Reflection.PropertyInfo p1, System.Reflection.PropertyInfo p2 )
                                         { return p1.MetadataToken.CompareTo( p2.MetadataToken ); } );

            // Need to have a list of columns
            List<Column> columns = new List<Column>();
            List<string> column_names = new List<string>();

            // Per property, check that it is one of: int, long, float, double, string, bytes
            foreach ( var property in type_properties )
            {
                string            column_name = "";
                Column.ColumnType column_type = Column.ColumnType.DEFAULT;
                IList<string>     column_properties = null;
                bool              is_column_nullable = false;

                // Get the column name
                column_name = property.Name;

                Type prop_type = property.PropertyType;

                // Check if the field is nullable (declared as T? or Nullable<T>)
                if ( property.PropertyType.IsGenericType &&
                          ( property.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) )
                {   // the field is a nullable field
                    is_column_nullable = true;
                    // Change the property type to be the underlying type
                    prop_type = Nullable.GetUnderlyingType( prop_type );
                }

                // Check the column data type (must be one of int, long, float, double, string, and bytes)
                if ( prop_type == typeof( System.String ) )
                {
                    column_type = Column.ColumnType.STRING;
                }
                else if ( prop_type == typeof( System.Int32 ) )
                {
                    column_type = Column.ColumnType.INT;
                }
                else if ( prop_type == typeof( System.Int64 ) )
                {
                    column_type = Column.ColumnType.LONG;
                }
                else if ( prop_type == typeof( float ) )
                {
                    column_type = Column.ColumnType.FLOAT;
                }
                else if ( prop_type == typeof( double ) )
                {
                    column_type = Column.ColumnType.DOUBLE;
                }
                else if ( prop_type == typeof( byte ) )
                {
                    column_type = Column.ColumnType.BYTES;
                }
                else
                    throw new KineticaException( "Unsupported data type for " + prop_type.Name +
                                              ": " + prop_type +
                                              " (must be one of int, long, float, double, string, and byte)" );

                // Extract the given column's properties, if any
                if ( properties != null )
                {
                    // This column has properties given
                    properties.TryGetValue( column_name, out column_properties );
                }

                // Keep a list of the column names for checking the properties
                column_names.Add( column_name );

                // Create the column
                Column column = new Column( column_name, column_type, column_properties );
                if ( is_column_nullable ) // Set the appropriate nullable flag for the column
                    column.setIsNullable( true );

                // Save the column
                columns.Add( column );
            }  // end looping over all members of the class type

            // Check for extraneous properties
            if ( properties != null )
            {
                IEnumerable<string> property_keys = properties.Keys;
                var unknown_columns = property_keys.Where( e => !column_names.Contains( e ) );
                // Check if any property is provided for wrong/non-existing columns
                if ( unknown_columns.Any() )
                    throw new KineticaException( "Properties specified for unknown columns." );
            }

            // Create the kinetica type
            KineticaType kType = new KineticaType( label, columns, properties );

            // Save the class information in the type
            kType.saveSourceType( recordClass );

            return kType;
        } // end fromClass()


        /// <summary>
        /// Create a KineticaType object from properties of a record object and Kinetica column properties.
        /// It ignores any properties inherited from base classes, and also ignores any member fields of
        /// the class.
        /// </summary>
        /// <param name="recordObj">A record object.</param>
        /// <param name="properties">Properties for the columns.</param>
        /// <returns></returns>
        public static KineticaType fromObject( Object recordObj, IDictionary<string, IList<string>> properties = null )
        {
            return fromObject( recordObj, "", properties );
        }  // end fromObject()


        /// <summary>
        /// Create a KineticaType object from properties of a record object and Kinetica column properties.
        /// It ignores any properties inherited from base classes, and also ignores any member fields of
        /// the class.
        /// </summary>
        /// <param name="recordObj">A record object.</param>
        /// <param name="label">Any label for the type.</param>
        /// <param name="properties">Properties for the columns.</param>
        /// <returns></returns>
        public static KineticaType fromObject(Object recordObj, string label = "", IDictionary<string, IList<string>> properties = null)
        {
            // Create the type schema from the object
            // --------------------------------------
            // Get the class type
            Type object_type = recordObj.GetType();

            return fromClass( object_type, label, properties );
        }  // end fromObject()

        /// <summary>
        /// Create a KineticaType object with the given column information.
        /// </summary>
        /// <param name="columns">A list of Columns with information on all the columns for the type.</param>
        public KineticaType(IList<Column> columns)
        {
            m_data.columns = columns;
            initialize();
            createSchema();  // create the schema from columns
        }

        /// <summary>
        /// Create a KineticaType object with the given column and label information.
        /// </summary>
        /// <param name="label">The label for the type.</param>
        /// <param name="columns">A list of Columns with information on all the columns for the type.</param>
        public KineticaType(string label, IList<Column> columns) : this(columns)
        {
            m_data.label = label;
        }

        /// <summary>
        /// Create a KineticaType object with the given column, label, and property information.
        /// </summary>
        /// <param name="label">The label for the type.</param>
        /// <param name="columns">A list of Columns with information on all the columns for the type.</param>
        /// <param name="properties">A per-column property information</param>
        public KineticaType( string label, IList<Column> columns, IDictionary<string, IList<string>> properties ) : this( label, columns )
        {
            m_properties = properties ?? new Dictionary<string, IList<string>>();
        }

        /// <summary>
        /// Create a KineticaType object using the string-formatted schema for the type.
        /// </summary>
        /// <param name="typeSchema"></param>
        public KineticaType(string typeSchema)
        {
            m_data.schemaString = typeSchema;
            createSchemaFromString( typeSchema );
            createSchema();
        }

        /// <summary>
        /// Create a KineticaType object using the string-formatted schema and properties for its columns.
        /// </summary>
        /// <param name="label">The label for the type.</param>
        /// <param name="typeSchema">The string-formatted schema for the type.</param>
        /// <param name="properties">A per-column based set of properties.</param>
        /// <param name="typeID">An optional ID for this type with which to identify it in the database.</param>
        public KineticaType(string label, string typeSchema, IDictionary<string, IList<string>> properties, string typeID = null )
        {
            m_properties = properties;
            m_type_id = typeID;
            m_data.label = label;
            m_data.schemaString = typeSchema;
            createSchemaFromString(typeSchema, properties);
            createSchema();
        }

        public string getLabel() { return m_data.label; }
        public IList<Column> getColumns() { return m_data.columns; }
        public Column getColumn(int index) { return m_data.columns[index]; }
        public Column getColumn(string name) { return m_data.columns[getColumnIndex(name)]; }
        public int getColumnCount() { return m_data.columns.Count; }
        public int getColumnIndex(string name) { return m_data.columnMap[name]; }
        public bool hasColumn(string name) { return m_data.columnMap.ContainsKey(name); }
        public Schema getSchema() { return m_data.schema; }
        public string getSchemaString() { return m_data.schemaString; }
        public string getTypeID() { return m_type_id;  }

        /// <summary>
        /// Saves the given type as this KineticaType's source type.
        /// </summary>
        /// <param name="sourceType">The type that works as the source. </param>
        public void saveSourceType( Type sourceType )
        {
            this.m_data.sourceType = sourceType;
        }  // end saveSourceType


        /// <summary>
        /// Given a handle to the server, creates a type in the database based
        /// on this data type.
        /// </summary>
        /// <param name="kinetica">The handle to the database server.</param>
        /// <returns>The ID with which the database identifies this type.</returns>
        public string create(Kinetica kinetica)
        {
            // Save the association between this KineticaType's source and itself in the Kinetica object
            // for future reference (it helps with encoding and decoding records)
            if ( this.m_data.sourceType != null )
                kinetica.SetKineticaSourceClassToTypeMapping( this.m_data.sourceType, this );

            // Register the type with Kinetica
            CreateTypeResponse response = kinetica.createType( m_data.schemaString, m_data.label, m_properties);
            return response.type_id;
        }  // end create()

        private KineticaType() { }

        /// <summary>
        /// Initializes the type based on the columns.  Verifies that the columns
        /// are valid.
        /// </summary>
        private void initialize()
        {
            int columnCount = m_data.columns.Count;

            if (columnCount == 0)
            {
                throw new ArgumentException("At least one column must be specified.");
            }

            for (int i = 0; i < columnCount; ++i)
            {
                string columnName = m_data.columns[i].getName();

                if (m_data.columnMap.ContainsKey(columnName))
                {
                    throw new ArgumentException("Duplicate column name " + columnName + " specified.");
                }

                m_data.columnMap[columnName] = i;
            }
        }  // end initialize()


        /// <summary>
        /// Creates a schema object from a string.
        /// </summary>
        /// <param name="typeSchema">The schema in a string format.</param>
        /// <param name="properties">Properties for the columns.</param>
        private void createSchemaFromString( string typeSchema,
                                             IDictionary<string, IList<string>> properties = null)
        {
            // Create the avro schema from the string and save it
            try
            {
                m_data.schema = RecordSchema.Parse(typeSchema);
            }
            catch (Exception ex)
            {
                throw new KineticaException(ex.ToString());
            }

            var root = JObject.Parse(typeSchema);

            var rootType = root["type"];
            if ((null == rootType) || !rootType.ToString().Contains("record"))
            {
                throw new ArgumentException("Schema must be of type record.");
            }

            var fields = root["fields"];
            if ((null == fields) || !fields.HasValues)
            {
                throw new ArgumentException("Schema has no fields.");
            }

            foreach (var field in fields)
            {
                //if (!field->first.empty() || field->second.empty())
                //{
                //    throw std::invalid_argument("Schema has invalid field.");
                //}

                // Do NOT use ToString 'cause it includes the double quotes (turns it into a JSON representation)
                var fieldName = (string) field["name"];
                if (string.IsNullOrEmpty(fieldName))
                {
                    throw new ArgumentException("Schema has unnamed field.");
                }

                if (m_data.columnMap.ContainsKey(fieldName))
                {
                    throw new ArgumentException($"Duplicate field name {fieldName}.");
                }

                var fieldType = field["type"];
                if (null == fieldType)
                {
                    throw new ArgumentException($"Field {fieldName} has no type.");
                }

                // Flag for nullability
                bool is_column_nullable = false;

                if (fieldType.HasValues) // If it has children
                {
                    var fieldTypeArray = fieldType;

                    foreach (var fieldTypeElement in fieldTypeArray.Children())
                    {
                        bool valid = false;
                        //if (fieldTypeElement->first.empty())
                        {
                            var fieldTypeElementString = fieldTypeElement.ToString();

                            if (!string.IsNullOrEmpty(fieldTypeElementString))
                            {
                                if (fieldTypeElementString == "null" || fieldTypeElementString == "\"null\"")
                                {
                                    is_column_nullable = true;
                                    valid = true;
                                }
                                else //if (fieldType->empty())
                                {
                                    fieldType = fieldTypeElement; // fieldTypeElementString;
                                    valid = true;
                                }
                            }
                        }

                        if (!valid)
                        {
                            throw new ArgumentException("Field {fieldName} has invalid type.");
                        }
                    }

                    //if (fieldType->empty())
                    //{
                    //    throw new ArgumentException("Field " + *fieldName + " has invalid type.");
                    //}
                }

                Column.ColumnType columnType;

                if (fieldType.ToString().Equals("bytes") || fieldType.ToString().Equals("\"bytes\""))
                {
                    columnType = Column.ColumnType.BYTES;
                }
                else if (fieldType.ToString().Equals("double") || fieldType.ToString().Equals("\"double\""))
                {
                    columnType = Column.ColumnType.DOUBLE;
                }
                else if (fieldType.ToString().Equals("float") || fieldType.ToString().Equals("\"float\""))
                {
                    columnType = Column.ColumnType.FLOAT;
                }
                else if (fieldType.ToString().Equals("int") || fieldType.ToString().Equals("\"int\""))
                {
                    columnType = Column.ColumnType.INT;
                }
                else if (fieldType.ToString().Equals("long") || fieldType.ToString().Equals("\"long\""))
                {
                    columnType = Column.ColumnType.LONG;
                }
                else if (fieldType.ToString().Equals("string") || fieldType.ToString().Equals("\"string\""))
                {
                    columnType = Column.ColumnType.STRING;
                }
                else
                {
                    throw new ArgumentException("Field {fieldName} must be of type bytes, double, float, int, long or string.");
                }

                IList<string> columnProperties = null;
                if (null != properties) properties.TryGetValue(fieldName, out columnProperties);
                // Check the column properties for nullability
                if ( ( null != columnProperties ) &&
                     ( columnProperties.Contains( ColumnProperty.NULLABLE ) ) )
                    is_column_nullable = true;

                // Create the column to be added
                Column column = new Column( fieldName, columnType, columnProperties );

                // Set the "nullability"
                column.setIsNullable( is_column_nullable );

                m_data.columns.Add( column );

                m_data.columnMap[fieldName] = m_data.columns.Count - 1;
            }
        }  // end createSchemaFromString()

        /// <summary>
        /// Create an avro schema from either the columns or the schema string.
        /// </summary>
        private void createSchema()
        {
            // First, check if the schema has already been created
            if (m_data.schema != null)
            {
                // nothing to do
                return;
            }

            // Check if the schema string exists, if so, create the schema from that
            if (m_data.schemaString != null)
            {
                try
                {
                    m_data.schema = RecordSchema.Parse(m_data.schemaString);
                    return;
                }
                catch (Exception ex)
                {
                    throw new KineticaException(ex.ToString());
                }
            }  // done creating the schema from the schema string

            // Since the shortcuts didn't apply, create a JSON object from the columns
            // and then create the schema and the schema string off it
            // --------------------------------------------------------------------------
            // Create the json string for the type
            string schema_string = "";
            // Create the json string opening with empty fields (with a generic 'type_name' (because the
            // server always replaces the name with this string anyway) )
            string schema_opening = "{'type':'record','name':'type_name','fields':[";
            // Create the json string closing
            string schema_closing = "]}";

            schema_string += schema_opening;

            // Create the json substrings for the columns
            foreach (var column in m_data.columns)
            {
                // Add the name
                string field_name = ("'name':'" + column.getName() + "'");

                // Add the type
                string field_type = "";
                if (column.isNullable())
                {  // the column is nullable, so we need a union
                    field_type = ("['" + column.getTypeString() + "','null']");
                }
                else  // regular type, no union needed
                {
                    field_type = ( "'" + column.getTypeString() + "'" );
                }
                field_type = ("'type':" + field_type);

                // Put the field together
                string field = ("{" + field_name + "," + field_type + "},");
                schema_string += field;
            }  // end looping over the fields

            // Trim the trailing comma from the fields
            char[] comma = { ',' };
            schema_string = schema_string.TrimEnd(comma);
            // Add the ending of the json string
            schema_string += schema_closing;

            // Create the RecordSchema from the JSON string
            try
            {
                m_data.schema = RecordSchema.Parse(schema_string);
            }
            catch (Exception ex)
            {
                throw new KineticaException(ex.ToString());
            }

            // Save the schema string
            m_data.schemaString = m_data.schema.ToString();
            return;
        }  // end createSchema()
    }  // end class KineticaType
}  // end namespace kinetica
