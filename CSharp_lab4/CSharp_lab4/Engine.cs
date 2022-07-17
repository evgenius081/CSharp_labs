using System;
using System.Xml.Serialization;

namespace PT_lab10._2
{
    public class Engine : IComparable
    {
        public Engine(){}
        public Engine(double displacement, double horsePower, string model)
        {
            this.displacement = displacement;
            this.horsePower = horsePower;
            this.model = model;
        }
        
        public double displacement { get; set; }
        
        public double horsePower { get; set; }
        
        public string model { get; set; }


        public int CompareTo(object obj)
        {
            Engine eng = obj as Engine;
            return this.horsePower.CompareTo(eng.horsePower);
        }

        public override string ToString()
        {
            return $"{model} {displacement.ToString()} ({horsePower.ToString()} hp)";
        }
    }
}
