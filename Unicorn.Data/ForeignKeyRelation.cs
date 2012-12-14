using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Unicorn.Data
{
    [Serializable]
    public class ForeignKeyRelation
    {
        public string FKField { get; set; }
        public string PKTable { get; set; }
        public string PKField { get; set; }
        public string Name { get; set; }
        public static bool operator ==(ForeignKeyRelation c1, ForeignKeyRelation c2)
        {
            if (null == (object)c1 )
                if (null ==(object)c2 )
                    return true;
                else
                    return false;
            else if (null == (object)c2 )
                return false;
            return c1.FKField == c2.FKField && c1.PKField == c2.PKField && c1.PKTable == c2.PKTable;

        }
        public static bool operator !=(ForeignKeyRelation c1, ForeignKeyRelation c2)
        {
            return !(c1 == c2);
        }

    }
    [Serializable]
    public class ForeignKeyRelationCollection : List<ForeignKeyRelation>
    {
        [NonSerialized]
        private TableInfo table;
        public ForeignKeyRelationCollection(TableInfo table)
        {
            this.table = table;
        }
        public void Load()
        {
            Clear();
            foreach (var fk in SchemaUtility.GetForeignKeyRelations(table.TableName))
            {
                Add(fk);
            }
        }
        public ForeignKeyRelation this[ColumnInfo c]
        {
            get
            {
                foreach (var fk in this)
                {
                    if (fk.FKField == c.ColumnName)
                        return fk;
                }
                return null;
            }
        }
    }
}
