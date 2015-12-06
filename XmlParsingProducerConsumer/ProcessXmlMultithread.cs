using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XmlParsingProducerConsumer
{
    class ProcessXmlMultithread
    {
        int Parsed = 0;

        public ProcessXmlMultithread(String XmlFile)
        {

            using (XmlReader reader = XmlReader.Create(XmlFile))
            {
                ParallelOptions Options = new ParallelOptions();
                Options.MaxDegreeOfParallelism = 5; // specify number of threads here
                Parallel.ForEach(ExtractXmlNode("book", reader),
                    Options,
                    (node) =>
                 {
                     // Processing goes here
                     //Extract ID of the book
                     XElement CurrentNode = XElement.Parse(node);
                     Console.WriteLine("PBook Id: " + CurrentNode.Attribute("id").Value);
                     Interlocked.Increment(ref Parsed);
                     // Simulate big wait like xml parsing and sending to database
                     Thread.Sleep(1000);
                 });
            }
            Console.WriteLine("Parsed XML items: " + Parsed);

        }

        public IEnumerable<String> ExtractXmlNode(String node,XmlReader reader)
        {
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "book")
                {
                    // ReadOuterXml will read XML node as string
                    // and advance pointer within reader
                   yield return reader.ReadOuterXml();
                }
                else
                {
                    reader.Read();
                }
            }
        }
    }
}
