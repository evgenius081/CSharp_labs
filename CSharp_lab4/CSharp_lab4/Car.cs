using System.ComponentModel;
using System.Xml.Serialization;

namespace PT_lab10._2
{
    public class Car
    {
        public Car()
        {
            motor = new Engine();
        }
        public Car(string model, Engine engine,int year)
        {
            this.model = model;
            this.year = year;
            this.motor = engine;
        }
        
        public string model { get; set; }
        
        public Engine motor { get; set; }
        
        public int year { get; set; }

        public override string ToString()
        {
            return $"{model} {motor} {year}";
        }
    }
}
