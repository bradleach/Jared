using System;

namespace Jared
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class DependencyAttribute : Attribute
    {
        private readonly Type[] types;

        public DependencyAttribute(params Type[] types)
        {
            this.types = types;
        }

        public Type[] Types
        {
            get { return types; }
        }
    }
}
