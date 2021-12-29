using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsFormats;

namespace DarkSoulsRipper.Ripper
{
    class SoulsDataTable
    {
        private String name;
        private List<Column> columns;
        private List<Dictionary<String, object>> rows;

        public SoulsDataTable(String name, List<Column> columns) {
            this.name = name;
            this.columns = columns;
        }

        public String GetName() {
            return name;
        }

        public List<Column> GetColumns() {
            return columns;
        }

        public List<Dictionary<String,object>> GetRows() {
            return rows;
        }

        public void AddRow(Dictionary<String, object> row) {
            rows.Add(row);
        }

        /// <summary>
        /// Column definition for SoulsDataTable
        /// </summary>
        public class Column
        {
            public enum ColumnType
            {
                TEXT, INTEGER, DECIMAL
            };
            public static ColumnType GetColumnTypeFromDefType(PARAMDEF.DefType def) {
                switch (def) {
                    case PARAMDEF.DefType.s8:
                    case PARAMDEF.DefType.u8:
                    case PARAMDEF.DefType.s16:
                    case PARAMDEF.DefType.u16:
                    case PARAMDEF.DefType.s32:
                    case PARAMDEF.DefType.u32:
                        return ColumnType.INTEGER;

                    case PARAMDEF.DefType.f32:
                        return ColumnType.DECIMAL;

                    case PARAMDEF.DefType.dummy8:
                    case PARAMDEF.DefType.fixstr:
                    case PARAMDEF.DefType.fixstrW:
                    default:
                        return ColumnType.TEXT;
                }
            }

            private String name;
            private ColumnType columnType;
            /*private PARAMDEF.DefType defType;*/
            private bool isPrimaryKey;
            private bool hasDefaultValue;
            private object defaultValue;

            public String GetName() {
                return name;
            }

            public ColumnType GetColumnType() {
                return columnType;
            }

            public Column(String name, ColumnType columnType) {
                this.name = name;
                isPrimaryKey = false;
                hasDefaultValue = false;
                this.columnType = columnType;
            }
            public Column(String name, PARAMDEF.DefType defType) : this(name, Column.GetColumnTypeFromDefType(defType)) {
                // extends main constructor
            }

            public void PrimaryKey() {
                isPrimaryKey = true;
            }

            public void DefaultValue(object defaultValue) {
                hasDefaultValue = true;
                this.defaultValue = defaultValue;
            }
            public String SqliteDefinitionString() {
                StringBuilder definitionBuilder = new StringBuilder();
                definitionBuilder.Append(name);
                String sqliteTypeString = "";
                switch (columnType) {
                    case ColumnType.TEXT:
                        sqliteTypeString = "TEXT";
                        break;
                    case ColumnType.INTEGER:
                        sqliteTypeString = "INTEGER";
                        break;
                    case ColumnType.DECIMAL:
                        sqliteTypeString = "REAL";
                        break;
                }
                definitionBuilder.Append(" ");
                definitionBuilder.Append(sqliteTypeString);
                if (isPrimaryKey) {
                    definitionBuilder.Append(" ");
                    definitionBuilder.Append("PRIMARY KEY");
                } else if (hasDefaultValue) {
                    definitionBuilder.Append(" ");
                    definitionBuilder.Append("DEFAULT");
                    if (columnType == ColumnType.TEXT)
                        definitionBuilder.Append("'");
                    definitionBuilder.Append(defaultValue.ToString());
                    if (columnType == ColumnType.TEXT)
                        definitionBuilder.Append("'");
                }
                return definitionBuilder.ToString();
            }
        }
    }
}
