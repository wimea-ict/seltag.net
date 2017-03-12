using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SelTag.NET
{
    //special DataTable 
    class SDataTable : System.Data.DataTable
    {
        public SDataTable(List<string> tagNames) //extract Tags and make columns
        {
            foreach (string tagname in tagNames)
            {
                this.Columns.Add(tagname);
            }
        }
        public void AppendRow(Row row)
        {
            //get Tag Values.
            if (!row.ContainsErrors)
            {
                //match values
                DataRow newRow = this.NewRow();
                foreach (Tag tag in row.Tags)
                {
                    newRow[tag.Name] = tag.Value;
                }
                this.Rows.Add(newRow);
            }
        }
    }
}
