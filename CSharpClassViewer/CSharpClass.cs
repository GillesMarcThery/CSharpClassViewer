using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpClassViewer
{
    public struct Event(string access, string type, string name)
    {
        public string name = name;
        public string access = access;
        public string type = type;

        public override readonly string ToString()
        {
            return access + " " + type + " " + name;
        }
    }
    public struct Property(bool isStatic, string access, string type, string name)
    {
        public string name = name;
        public string access = access;
        public string type = type;
        public bool isStatic = isStatic;
        public override readonly string ToString()
        {
            string retour = access + " ";

            if (isStatic)
                retour += "static ";
            retour += type + " ";
            retour += name;
            return retour;
        }
    }
    public struct Field(bool isStatic, string access, string type, string name, bool isConst)
    {
        public string name = name;
        public string access = access;
        public string type = type;
        public bool isConst = isConst;
        public bool isStatic = isStatic;
        public override readonly string ToString()
        {
            string retour = access + " ";

            if (isStatic)
                retour += "static ";
            if (isConst)
                retour += "const ";
            retour += type + " ";
            retour += name;
            return retour;
        }
    }
    public struct Method(bool isStatic, string access, string type, string name)
    {
        public string name = name;
        public string access = access;
        public string type = type;
        public bool isStatic = isStatic;
        public override readonly string ToString()
        {
            string retour = access + " ";

            if (isStatic)
                retour += "static ";
            retour += type + " ";
            retour += name;
            return retour;
        }
    }
    public struct Constructor(bool isStatic, string access, string name)
    {
        public string name = name;
        public string access = access;
        public bool isStatic = isStatic;
        public override readonly string ToString()
        {
            string retour = access + " ";

            if (isStatic)
                retour += "static ";
            retour += name;
            return retour;
        }
    }
    public enum MetaType { Class = 0, Struct }
    public class CSharpClassOrStruct(bool isStatic, MetaType mt, string myNamespace, string derivedFrom, string name, string access, bool partial)
    {
        public MetaType metaType = mt;
        public string myNamespace = myNamespace;
        public string derivedFrom = derivedFrom;
        public string name = name;
        public string access = access;
        public bool isPartial = partial;
        public bool isStatic = isStatic;
        public List<Event> events = [];
        public List<Field> fields = [];
        public List<Property> properties = [];
        public Constructor constructor;
        public List<Method> methods = [];

        public override string ToString()
        {
            string retour = access + " ";

            if (isStatic)
                retour += "static ";
            if (isPartial)
                retour += "partial ";
            switch (metaType)
            {
                case MetaType.Class:
                    retour += "class ";
                    break;
                case MetaType.Struct:
                    retour += "struct ";
                    break;
            }
            retour += name;
            if (derivedFrom != "")
                retour += " : " + derivedFrom;
            return retour;
        }
    }
}
