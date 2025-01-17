using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpClassViewer
{
    public struct Event
    {
        public string name;
        public string access;
        public string type;
        public Event(string access, string type, string name)
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
        public bool isConst;
        public Field(string access, string type, string name, bool isConst)
        {
            this.name = name;
            this.access = access;
            this.type = type;
            this.isConst = isConst;
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
    public enum MetaType { Class = 0, Struct }
    public class CSharpClassOrStruct
    {
        public MetaType metaType;
        public string myNamespace;
        public string name;
        public string access;
        public bool isPartial = false;
        public List<Event> events = [];
        public List<Field> fields = [];
        public List<Property> properties = [];
        public Constructor constructor;
        public List<Method> methods = [];
        public CSharpClassOrStruct(MetaType mt, string myNamespace, string name, string access, bool partial)
        {
            this.metaType = mt;
            this.myNamespace = myNamespace;
            this.name = name;
            this.access = access;
            this.isPartial = partial;
        }
        public override string ToString()
        {
            return access + " " + name;
        }
    }
}
