using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Converters;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSharpClassViewer
{
    //string[]={"abstract", "as","base","bool"
    //"break"
    //"byte"
    //"case"
    //"catch"
    //"char"
    //"checked"
    //"class"
    //"const"
    //"continue"
    //"decimal"
    //"default"
    //"delegate"
    //"do"
    //"double"
    //"else"
    //"enum"
    //"event"
    //"explicit"
    //"extern"
    //"false"
    //"finally"
    //"fixed"
    //"float"
    //"for"
    //"foreach"
    //"goto"
    //"if"
    //"implicite
    //"in
    //"int
    //"interface
    //"internal
    //"is
    //"lock
    //"long
    //"namespace
    //"new
    //"null
    //"object
    //"operator
    //"out
    //"override
    //"params
    //"private
    //"protected
    //"public
    //"readonly
    //"ref
    //return
    //"sbyte
    //"sealed
    //"short
    //"sizeof
    //"stackalloc

    //"static
    //"string
    //"struct
    //"switch
    //"this
    //"throw
    //"true
    //"try
    //"typeof
    //"uint
    //"ulong
    //"unchecked
    //"unsafe
    //"ushort
    //"using
    //"virtual
    //"void
    //"volatile
    //"while
    //}
    public class CSharpFile
    {
        private string? fileContents;
        string current_namespace;
        CSharpClass currentClass = new("", "", "");
        List<CSharpClass> myClasses = [];
        public bool Load(string filename)
        {
            TextReader reader;
            fileContents = "";

            try
            {
                reader = new StreamReader(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            try
            {
                fileContents = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
        public void Parse()
        {
            string? line = "";
            LineStruct retour;

            using StringReader reader = new(fileContents);
            while ((line = reader.ReadLine()) != null)
            {

                line = Regex.Replace(line, @"\s+", " ");
                line = line.Trim();
                Debug.WriteLine(line);

                if (line.Length < 2)
                    continue;
                if (line.Substring(0, 2) == "//")
                    continue;
                if (line[0] == '[')
                    continue;
                if (line[0] == '#')
                    continue;

                retour = ParseLine(line, currentClass);
                if (retour.isNameSpace)
                {
                    current_namespace = retour.name;
                    continue;
                }
                if (retour.isClass)
                {
                    currentClass = new CSharpClass(current_namespace, retour.name, retour.access);
                    myClasses.Add(currentClass);
                    continue;
                }
                if (retour.isField)
                {
                    currentClass.fields.Add(new Field(retour.access, retour.type, retour.name));
                    continue;
                }
                if (retour.isProperty)
                {
                    currentClass.properties.Add(new Property(retour.access, retour.type, retour.name));
                    SkipBloc(line, reader);
                    continue;
                }
                if (retour.isConstructor)
                {
                    currentClass.constructor = new Constructor(retour.access, retour.name);
                    SkipBloc(line, reader);
                    continue;
                }
                if (retour.isMethod)
                {
                    currentClass.methods.Add(new Method(retour.access, retour.type, retour.name));
                    SkipBloc(line, reader);
                    continue;
                }
            }
        }
        void SkipBloc(string line, StringReader reader)
        {
            int countOpen = 0;
            do
            {
                if (line.Contains('{'))
                    countOpen += line.Count(f => f == '{');
                if (line.Contains('}'))
                {
                    countOpen -= line.Count(f => f == '}');
                    if (countOpen == 0) return;
                }
                line = reader.ReadLine();
                Debug.WriteLine(line);
            } while (line != null);
        }
        public struct LineStruct
        {
            public bool isNameSpace;
            public bool isStatic;
            public bool isProperty;
            public bool isConstructor;
            public bool isField;
            public bool isReadonly;
            public bool isClass;
            public bool isMethod;
            public string access;
            public string type;
            public string name;
        }
        bool SearchType(Collection<string> myCollection, out int index)
        {
            index = -1;
            for (int i = 0; i < myCollection.Count; i++)
                if (myCollection[i] == "int" || myCollection[i] == "long" || myCollection[i] == "float"
                    || myCollection[i] == "double" || myCollection[i] == "bool" || myCollection[i] == "char"
                    || myCollection[i] == "string" || myCollection[i] == "void")
                {
                    index = i;
                    return true;
                }
            return false;
        }
        bool SearchAccess(Collection<string> myCollection, out int index)
        {
            index = -1;
            for (int i = 0; i < myCollection.Count; i++)
                if (myCollection[i] == "public" || myCollection[i] == "private" || myCollection[i] == "protected" || myCollection[i] == "internal")
                {
                    index = i;
                    return true;
                }
            return false;
        }
        public LineStruct ParseLine(string s, CSharpClass csc)
        {
            LineStruct retour = new();
            retour.access = "private";
            int index;
            Collection<string> collection = new();

            string[] tokens = s.Split(' ');
            foreach (string s1 in tokens)
                collection.Add(s1);

            if (collection[0] == "using")
                return retour;

            if (collection.Contains("namespace"))
            {
                retour.isNameSpace = true;
                collection.Remove("namespace");
                return retour;
            }
            if (collection.Contains("static"))
            {
                retour.isStatic = true;
                collection.Remove("static");
            }
            if (collection.Contains("readonly"))
            {
                retour.isReadonly = true;
                collection.Remove("readonly");
            }
            if (SearchAccess(collection, out index))
            {
                retour.access = collection[index];
                collection.RemoveAt(index);
            }
            if (SearchType(collection, out index))
            {
                retour.type = collection[index];
                collection.RemoveAt(index);
            }
            if (collection.Contains("class"))
            {
                retour.isClass = true;
                collection.Remove("class");
                retour.name = collection[0];
                return retour;
            }
            // Constructeur ou Méthode
            if (collection[0].Contains('('))
            {
                string tmp = collection[0].Split('(')[0];
                if (tmp == csc.name)
                {
                    retour.isConstructor = true;
                    retour.name = tmp;
                    return retour;
                }
                else
                {
                    retour.isMethod = true;
                    retour.name = tmp;
                    return retour;
                }
            }
            // Field
            if (s.Contains(';'))
            {
                retour.isField = true;
                if (retour.type == null)
                {
                    retour.type = collection[0];
                    retour.name = collection[1].Split(';')[0];
                }
                else
                    retour.name = collection[0].Split(';')[0];
                return retour;
            }
            // Property
            retour.isProperty = true;
            if (retour.type == null)
            {
                retour.type = collection[0];
                retour.name = collection[1].Split(';')[0];
            }
            else
                retour.name = collection[0].Split(';')[0];
            return retour;
        }
        // TODO 
//        public bool IsDisposed { get { return isDisposed; } private set { isDisposed = value; } }
//        public event PropertyChangedEventHandler PropertyChanged;
//        Methode de même nom
    }
}
