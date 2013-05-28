using System;

namespace dokimi.core
{
    public class Specifcations
    {
        static readonly Lazy<Specifcations> _instance = new Lazy<Specifcations>(()=> new Specifcations());
        public static Specifcations Catalog { get { return _instance.Value; } }

        private Specifcations()
        {
            
        }
    }
}