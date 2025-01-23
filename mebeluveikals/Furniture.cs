using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mebeluveikals
{
    public class Furniture
    {
        public string Name;
        public string Description;
        public double Price;
        public int Height;
        public int Width;
        public int Length;

        public Furniture(string name, string description, double price,
            int height, int width, int length)
        {
            Name = name;
            Description = description;
            Price = price;
            Height = height;
            Width = width;
            Length = length;
        }

        public override string ToString()
        {
            return Name.ToString() + "," + Description.ToString() + "," + Price.ToString() + "," + Height.ToString() + "," + Width.ToString() + "," + Length.ToString();
        }
    }
}
