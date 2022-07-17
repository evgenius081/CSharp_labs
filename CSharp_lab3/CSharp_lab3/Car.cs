using System.Xml.Serialization;

namespace PT_lab9
{
    [XmlType("car")]
    public class Car
    {
        public Car(){}
        public Car(string model, Engine engine,int year)
        {
            this.model = model;
            this.year = year;
            this.motor = engine;
        }

        [XmlElement("model")]
        public string model { get; set; }
        
        [XmlElement("engine")]
        public Engine motor { get; set; }
        
        [XmlElement("year")]
        public int year { get; set; }
    }
}
