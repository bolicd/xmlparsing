using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XmlParsingProducerConsumer
{
    /// <summary>
    /// Process XML file using Producer Consumer Patter
    /// File is read using one producer and parsed using multiple consumers
    /// This allows for fine grained control when it comes to load balancing
    /// </summary>
    class ProcessXml
    {

        // Using blocking collection for Queue which is a part of TPL 4.5
        BlockingCollection<String> Queue = new BlockingCollection<string>(5);
        int Parsed = 0;

        public ProcessXml(String FileToProcess)
        {
            //Producer is run in separate thread and reads chunks of XML file storing them in Queue
            var producer = Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        using (XmlReader reader = XmlReader.Create(FileToProcess))
                        {
                            while (!reader.EOF)
                            {
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "book")
                                {
                                    // ReadOuterXml will read XML node as string
                                    // and advance pointer within reader
                                    Queue.Add(reader.ReadOuterXml());
                                }
                                else
                                {
                                    reader.Read();
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception " + ex.Message);
                    }
                });

            // Multiple consumers will use extracted XML, process it and do something with it
            // Probably insert in database
            var consumers = Enumerable.Range(1, 3).Select(
               _ => Task.Factory.StartNew(
                   () =>
                   {
                       foreach (var item in Queue.GetConsumingEnumerable())
                       {
                           //Extract ID of the book
                           XElement CurrentNode = XElement.Parse(item);

                           Console.WriteLine("Book Id: " + CurrentNode.Attribute("id").Value);
                           Interlocked.Increment(ref Parsed);
                           // Simulate big wait like xml parsing and sending to database
                           Thread.Sleep(1000);
                       }
                   }
                   )).ToArray();

            Task.WaitAll(producer);
            Queue.CompleteAdding();
            Task.WaitAll(consumers);
            Console.WriteLine("Parsed XML items: " + Parsed);
        }

    }
}
