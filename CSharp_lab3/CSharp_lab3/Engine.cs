using System.Xml.Serialization;

namespace PT_lab9
{
    public class Engine
    {
        public Engine(){}
        public Engine(double displacement, double horsePower, string model)
        {
            this.displacement = displacement;
            this.horsePower = horsePower;
            this.model = model;
        }

        [XmlElement("displacement")]
        public double displacement { get; set; }

        [XmlElement("horsePower")]
        public double horsePower { get; set; }

        [XmlAttribute("model")]
        public string model { get; set; }
    }
}
