// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Reflection;

// using mshtml;

namespace O2.External.IE.WebObjects
{
    public class O2Link
    {
        /*private readonly GeckoElement geckoElement;

        public O2Link(GeckoElement _geckoElement)
        {
            geckoElement = _geckoElement;
            DI.log.debug(geckoElement.ToString());
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                object propertyValue = DI.reflection.getProperty(property.Name, this);
                DI.log.debug("    {0} = {1}", property.Name, propertyValue.ToString());
            }
        }


        public String Id
        {
            get { return geckoElement.GetAttribute("id"); }
            set { geckoElement.SetAttribute("id", value); }
        }

        public String Href
        {
            get { return geckoElement.GetAttribute("href"); }
            set { geckoElement.SetAttribute("href", value); }
        }

        public String Text
        {
            get { return geckoElement.InnerHtml; }
            set { geckoElement.InnerHtml = value; }
        }

        public String Target
        {
            get { return geckoElement.GetAttribute("target"); }
            set { geckoElement.SetAttribute("target", value); }
        }

        //public String OuterHtml
        public String TextContent
        {
            get { return geckoElement.TextContent; }
            set { geckoElement.TextContent = value; }
        }

        public override string ToString()
        {
            return Text ?? "";
        }*/
    }
}