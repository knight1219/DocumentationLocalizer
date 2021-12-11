using NuDoq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DocumentationLocalizer
{
    public class LocalizationVisitor : Visitor
    {
        XElement? currentElement;
        public string Local;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlVisitor"/> class.
        /// </summary>
        public LocalizationVisitor(string local)
        {
            this.Xml = new XDocument
            {
                Declaration = null
            };
            this.Local = local;
        }

        /// <summary>
        /// Visits the entire set of members read by the <see cref="DocReader.Read(Assembly)" />.
        /// </summary>
        public override void VisitAssembly(AssemblyMembers assembly)
        {
            currentElement = new XElement("members");
            Xml.Add(new XElement("doc",
                new XElement("assembly",
                    new XElement("name", assembly.Assembly.GetName().Name)),
                    currentElement));

            base.VisitAssembly(assembly);
        }

        /// <summary>
        /// Visits the entire set of members read by the <see cref="DocReader.Read(string)" />.
        /// </summary>
        public override void VisitDocument(DocumentMembers document)
        {
            if (currentElement == null)
            {
                currentElement = new XElement("members");
                Xml.Add(new XElement("doc", currentElement));
            }

            base.VisitDocument(document);
        }

        /// <summary>
        /// Visit the generic base class <see cref="Member" />.
        /// </summary>
        /// <param name="member"></param>
        /// <remarks>
        /// This method is called for all <see cref="Member" />-derived
        /// types.
        /// </remarks>
        public override void VisitMember(Member member) => AddXml("member", member, base.VisitMember);

        /// <summary>
        /// Visits the <c>c</c> documentation element.
        /// </summary>
        public override void VisitC(C code) => AddXml(new XElement("c", new XText(code.Content)), code, base.VisitC);

        /// <summary>
        /// Visits the <c>code</c> documentation element.
        /// </summary>
        public override void VisitCode(Code code) => AddXml(new XElement("code", new XText(code.Content)), code, base.VisitCode);

        /// <summary>
        /// Visits the <c>description</c> documentation element.
        /// </summary>
        public override void VisitDescription(Description description) => AddXml("description", description, base.VisitDescription);

        /// <summary>
        /// Visits the <c>example</c> documentation element.
        /// </summary>
        public override void VisitExample(Example example) => AddXml("example", example, base.VisitExample);

        /// <summary>
        /// Visits the <c>exception</c> documentation element.
        /// </summary>
        public override void VisitException(NuDoq.Exception exception) => AddXml("exception", exception, base.VisitException);

        /// <summary>
        /// Visits the <c>item</c> documentation element.
        /// </summary>
        public override void VisitItem(Item item) => AddXml("item", item, base.VisitItem);

        /// <summary>
        /// Visits the <c>list</c> documentation element.
        /// </summary>
        public override void VisitList(List list) => AddXml("list", list, base.VisitList);

        /// <summary>
        /// Visits the <c>listheader</c> documentation element.
        /// </summary>
        public override void VisitListHeader(ListHeader header) => AddXml("listheader", header, base.VisitListHeader);

        /// <summary>
        /// Visits the <c>para</c> documentation element.
        /// </summary>
        public override void VisitPara(Para para) => AddXml("para", para, base.VisitPara);

        /// <summary>
        /// Visits the <c>param</c> documentation element.
        /// </summary>
        public override void VisitParam(Param param) => AddXml("param", param, base.VisitParam);

        /// <summary>
        /// Visits the <c>paramref</c> documentation elemnet.
        /// </summary>
        public override void VisitParamRef(ParamRef paramRef) => AddXml("paramref", paramRef, base.VisitParamRef);

        /// <summary>
        /// Visits the <c>remarks</c> documentation element.
        /// </summary>
        public override void VisitRemarks(Remarks remarks) => AddXml("remarks", remarks, base.VisitRemarks);

        /// <summary>
        /// Visits the <c>see</c> documentation element.
        /// </summary>
        public override void VisitSee(See see) => AddXml("see", see, base.VisitSee);

        /// <summary>
        /// Visits the <c>seealso</c> documentation element.
        /// </summary>
        public override void VisitSeeAlso(SeeAlso seeAlso) => AddXml("seealso", seeAlso, base.VisitSeeAlso);

        /// <summary>
        /// Visits the <c>summary</c> documentation element.
        /// </summary>
        public override void VisitSummary(Summary summary) => AddXml("summary", summary, base.VisitSummary);

        /// <summary>
        /// Visits the <c>term</c> documentation element.
        /// </summary>
        public override void VisitTerm(Term term) => AddXml("term", term, base.VisitTerm);

        /// <summary>
        /// Visits the literal text inside other documentation elements.
        /// </summary>
        public override void VisitText(Text text)
        {
            currentElement?.Add(new XText(text.Content));
            base.VisitText(text);
        }

        /// <summary>
        /// Visits the <c>typeparam</c> documentation element.
        /// </summary>
        public override void VisitTypeParam(TypeParam typeParam) => AddXml("typeparam", typeParam, base.VisitTypeParam);

        /// <summary>
        /// Visits the <c>typeparamref</c> documentation element.
        /// </summary>
        public override void VisitTypeParamRef(TypeParamRef typeParamRef) => AddXml("typeparamref", typeParamRef, base.VisitTypeParamRef);

        /// <summary>
        /// Visits an unknown documentation element.
        /// </summary>
        public override void VisitUnknownElement(UnknownElement element)
        {
            if (element.Xml.Name != "localize")
            {
                // Just add the unknown element and continue, nothing to see here
                AddXml(new XElement(element.Xml.Name, element.Xml.Attributes()), element, base.VisitUnknownElement);
            }
        }

        /// <summary>
        /// Visits the <c>value</c> documentation element.
        /// </summary>
        public override void VisitValue(Value value) => AddXml("value", value, base.VisitValue);

        /// <summary>
        /// Gets the XML.
        /// </summary>
        public XDocument Xml { get; }

        void AddXml<TVisitable>(string elementName, TVisitable element, Action<TVisitable> visit)
            => AddXml(new XElement(elementName), element, visit);

        void AddXml<TVisitable>(XElement xml, TVisitable visitable, Action<TVisitable> visit)
        {
            currentElement?.Add(xml);
            currentElement = xml;

            // UnknownElement already populates its own attributes from the original XElement
            if (visitable is Element element && visitable is not UnknownElement)
                xml.Add(element.Attributes.Select(x => new XAttribute(x.Key, x.Value)));

            if(visitable is Container container)
            {
                // Detect Locales, process, and call it a day
                var localizeElements = container.Elements.Where(e => e.GetType() == typeof(UnknownElement)).Cast<UnknownElement>().ToList().Where(l => l.Xml.Name == "localize");
                var defaultText = "";
                if (localizeElements.Any())
                {
                    // See if there is english set, if so make default.
                    if (localizeElements.Any(l => l.Xml.Attribute("lang")?.Value == "en-US"))
                    {
                        var elementText = localizeElements.FirstOrDefault(l => l.Xml.Attribute("lang")?.Value == "en-US")?.Xml.Attribute("text")?.Value;

                        if (!string.IsNullOrEmpty(elementText))
                        {
                            defaultText = elementText;
                        }
                    }
                    else
                    {
                        // Ok, no english so we set the first in the list so we have some text just in case
                        var elementText = localizeElements.FirstOrDefault()?.Xml.Attribute("text")?.Value;

                        if (!string.IsNullOrEmpty(elementText))
                        {
                            defaultText = elementText;
                        }
                    }


                    // So now that we have our default prepped, now all we need to do is see if there is a locale set for the locale we are running against
                    var currentLocaleElement = localizeElements.FirstOrDefault(x => x.Xml.Attribute("lang")?.Value == this.Local);
                    var newText = "";
                    if(currentLocaleElement != null)
                    {
                        // There is, cool. Use it
                        newText = currentLocaleElement.Xml.Attribute("text")?.Value;

                    }
                    else
                    {
                        // There isnt. So no translation. Use the default
                        newText = defaultText;
                    }

                    // Now, add it and continue
                    currentElement?.Add(new XText(newText));
                }
            }

            visit(visitable);

            currentElement = currentElement.Parent;
        }
    }
}
