using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlParsingProducerConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("** Processing Using Producer/Consumer: ");
            ProcessXml process = new ProcessXml("books.xml");
            Console.WriteLine("** Processing Using ParalleForEach: ");
            ProcessXmlMultithread process2 = new ProcessXmlMultithread("books.xml");
            Console.ReadLine();
        }
    }
}
