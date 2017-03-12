using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelTag.NET
{
    class Row
    {
        public List <Tag> Tags
        {
            get;
            set;
        }
        public bool ContainsErrors
        {
            get;
            set;
        }
        public Row(string rowLine)
        {
                Tags = this.GetTags(rowLine);
        }

        private List<Tag> GetTags(string rowLine)
        {
            List<Tag> tags = new List<Tag>();
            //attempt extract and remove date
            {
                //get index of TZ tag
                int tzIndex = rowLine.IndexOf("TZ");  
                string date = rowLine.Substring(0, tzIndex-1); //parse to datetime
                tags.Add(new Tag("DATETIME", date));
            }


            if(rowLine.Contains('[')) //remove ADDR RSSI information
            {
               // int index = rowLine.IndexOf('[');
                rowLine = rowLine.Replace("[", "");
            }
            if (rowLine.Contains(']')) //remove ADDR RSSI information
            {
                // int index = rowLine.IndexOf('[');
                rowLine = rowLine.Replace("]", "");
            }

            string [] rawTags = rowLine.Split(' ');  //remove date and split using spaces

            foreach (string item in rawTags)
            {
                if (item != "&:") //neglect separator
                {
                    if (item.Contains('='))           //semi-valid tag
                    {
                        string[] keyval = item.Split('=');
                        if (keyval[0].ToString() != "" & keyval[1].ToString() != "")//neglect value-less tags    or tag-less values
                        {
                            if (keyval[0].Contains("TXT")) { keyval[0] = "TXT"; } //stray xters in TXT

                            tags.Add(new Tag(keyval[0], keyval[1]));
                        }
                    }
                }
            }
            return tags;
        }
    }
}
