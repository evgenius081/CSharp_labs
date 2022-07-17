using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.EntityFrameworkCore;

namespace PT_lab9
{
    public class MainModel
    {
        [XmlArray("cars")]
        public List<Car> Cars { get; set; }
    }

    public class PT_lab9
    {
        [XmlArray("cars")]
        static public List<Car> myCars = new List<Car>(){
            new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
            new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
            new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
            new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
            new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
            new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
            new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
        };
        
        public static void createXmlFromLinq(List<Car> myCars)
        {
            IEnumerable<XElement> nodes = from i in myCars
                select new XElement("car", 
                    new XElement("model", i.model),
                    new XElement("engine",
                        new XAttribute("model", i.motor.model),
                        new XElement("horsePower", i.motor.horsePower),
                        new XElement("displacement", i.motor.displacement)),
                    new XElement("year", i.year));
            XElement rootNode = new XElement("cars", nodes); //create a root node to contain the query results
            rootNode.Save("CarsFromLinq.xml");
        }
        
        public static void createTableFromLinq(List<Car> myCars)
        {
            IEnumerable<XElement> nodes = from i in myCars
                select new XElement("tr", 
                    new XElement("td", i.model),
                    new XElement("td", i.motor.model),
                    new XElement("td", i.motor.horsePower),
                    new XElement("td", i.motor.displacement),
                    new XElement("td", i.year));
            XElement rootNode = new XElement("table", nodes); //create a root node to contain the query results
            rootNode.Save("CarsFromLinq.html");
        }
        
        
        static void Main(string[] args)
        {

            // zad1 a
            var list = from car in myCars
                       where car.model == "A6"
                       select new { engineType = (car.motor.model == "TDI" ? "diesel" : "petrol"), hppl = car.motor.horsePower / car.motor.displacement };
            foreach (var item in list) Console.WriteLine(item);
            
            // zad1 b
            var list1 = from item in list
                group item by item.engineType;
            
            foreach (var item in list1)
            {
                Console.WriteLine("{0}: {1}", item.Key, item.Average(item => item.hppl));
            }
            
            // zad2 serialization
            XmlRootAttribute root = new XmlRootAttribute("cars");  
            XmlSerializer serializer = new XmlSerializer(typeof(List<Car>), root);
            TextWriter writer = new StreamWriter("CarsCollection.xml");
            serializer.Serialize(writer, myCars);
            writer.Close();
            
            // zad2 deserialization
            FileStream fileStream = new FileStream("CarsCollection.xml", FileMode.Open);
            List<Car> result = (List<Car>) serializer.Deserialize(fileStream);
            foreach (var item in result)
            {
                Console.WriteLine("({0}, ({1}, {2}, {3}), {4})", item.model, item.motor.displacement, item.motor.model, 
                    item.motor.horsePower, item.year);
            }
            fileStream.Close();
            
            // zad3 a
            XElement rootNode = XElement.Load("CarsCollection.xml");
            double avgHP = (double) rootNode.XPathEvaluate(
                "sum(//engine[@model!='TDI']/horsePower) div count(//engine[@model!='TDI']/horsePower)");
            Console.WriteLine(avgHP);
            
            // zad3 b
            IEnumerable<XElement> models = rootNode.XPathSelectElements("//car/model[not(.=preceding::car/model)]");
            foreach (var model in models)
            {
                Console.WriteLine(model);
            }
            
            //zad 4
            createXmlFromLinq(myCars);
            
            //zad5
            createTableFromLinq(myCars);
            
            // zad6
            var doc = XDocument.Load("CarsCollection.xml");
            foreach (XElement element in doc.Descendants("horsePower"))
            {
                element.Name = "hp";
            }
            foreach (var element in doc.Descendants("year"))
            {
                foreach (var item in element.Parent.Descendants("model"))
                {
                    item.SetAttributeValue("year", element.Value);
                }
            }
            doc.Descendants("year").Remove();
            doc.Save("Cars.xml");
        }
    }

}