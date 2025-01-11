using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpClassViewer
{
    public struct Property
    {
        public string name;
        public string access;
        public string type;
        public Property(string access, string type, string name)
        {
            this.name = name;
            this.access = access;
            this.type = type;
        }
        public override string ToString()
        {
            return access + " " + type + " " + name;
        }
    }
    public struct Field
    {
        public string name;
        public string access;
        public string type;
        public Field(string access, string type, string name)
        {
            this.name = name;
            this.access = access;
            this.type = type;
        }
        public override string ToString()
        {
            return access + " " + type + " " + name;
        }
    }
    public struct Method
    {
        public string name;
        public string access;
        public string type;
        public Method(string access, string type, string name)
        {
            this.name = name;
            this.access = access;
            this.type = type;
        }
        public override string ToString()
        {
            return access + " " + type + " " + name;
        }
    }
    public struct Constructor
    {
        public string name;
        public string access;
        public Constructor(string access, string name)
        {
            this.name = name;
            this.access = access;
        }
        public override string ToString()
        {
            return access + " " + name;
        }
    }
    public class CSharpClass
    {
        public string myNamespace;
        public string name;
        public string access;
        public List<Field> fields = [];
        public List<Property> properties = [];
        public Constructor constructor;
        public List<Method> methods = [];
        public CSharpClass(string myNamespace, string name, string access)
        {
            this.myNamespace = myNamespace;
            this.name = name;
            this.access = access;
        }
        public override string ToString()
        {
            return access + " " + name;
        }
    }
}
