using NuDoq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace DocumentationLocalizer
{
    public class Localizer
    {
        public int LocalizeXMLFile(string filename)
        {
            XmlDocument doc = new XmlDocument();
            var langList = new List<string>();

            doc.Load(filename);

            XmlNodeList elemList = doc.GetElementsByTagName("localize");
            for (int i = 0; i < elemList.Count; i++)
            {
                var element = elemList[i];
                if (element != null)
                {
                    if (element.Attributes != null && element.Attributes["lang"] != null)
                    {
                        var attrValue = element.Attributes["lang"]?.Value;
                        if (!langList.Contains(attrValue))
                            langList.Add(attrValue);
                    }
                }
            }
            Console.WriteLine($@"Found Languages to create documentation for: {string.Join(" ,", langList)}");
            var docs = DocReader.Read(filename);
            var name = Path.GetFileNameWithoutExtension(filename);

            var filesCreated = this.ProcessLanguages(docs, langList, name);
            return filesCreated;
        }


        private int ProcessLanguages(DocumentMembers documentMembers, List<string> languages, string name)
        {
            
            var visitors = new List<LocalizationVisitor>();

            foreach (var lang in languages)
            {
                Console.WriteLine($@"Processing {lang} version of documentation");
                var visitor = new LocalizationVisitor(lang);
                visitors.Add(visitor);
                documentMembers.Accept(visitor);
                Console.WriteLine($@"Processing {lang} version of documentation complete");
            }

            foreach (var v in visitors)
            {
                var fileName = $@"{name}-{v.Local}.xml";
                Console.WriteLine($@"Saving {v.Local} version of documentation to {fileName}");
                v.Xml.Save(fileName);
            }

            return visitors.Count();
        }
    }
}