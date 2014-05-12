using System;

namespace dokimi.core
{
    public class Specifications
    {
        static readonly Lazy<Specifications> _instance = new Lazy<Specifications>(()=> new Specifications());
        public static Specifications Catalog { get { return _instance.Value; } }

        private Specifications()
        {
            
        }
    }
}