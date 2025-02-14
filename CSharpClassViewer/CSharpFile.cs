﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
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
    public class CSharpFile
    {
        public string fileContents="";
        public string? filename;
        string current_namespace="";
        CSharpClassOrStruct? current;
        public List<string> myNamespaces = [];
        public List<CSharpClassOrStruct> myClasses = [];
        public List<CSharpClassOrStruct> myStructs = [];
        public bool Load(string filename)
        {
            TextReader reader;
            fileContents = "";

            this.filename = filename;
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
                if (line.Contains("//"))
                    line = line[..line.IndexOf("//")];
                line = line.Trim();
                //Debug.WriteLine(line);

                if (line.Length < 2)
                    continue;
                if (line[0] == '[')
                {
                    SkipBloc(line, '[', ']', reader);
                    continue;
                }
                if (line[0] == '#')
                    continue;
                if (line.StartsWith("using"))
                    continue;

                retour = ParseLine(line, current);
                if (retour.isNameSpace)
                {
                    current_namespace = retour.name;
                    myNamespaces.Add(retour.name);
                    continue;
                }
                if (retour.isEnum)
                {
                    SkipBloc(line, '{', '}', reader);
                    continue;
                }
                if (retour.isClass)
                {
                    current = new CSharpClassOrStruct(retour.isStatic, MetaType.Class, current_namespace, retour.derivedFrom, retour.name, retour.access, retour.isPartial);
                    myClasses.Add(current);
                    continue;
                }
                if (retour.isStruct)
                {
                    current = new CSharpClassOrStruct(retour.isStatic, MetaType.Struct, current_namespace, retour.derivedFrom, retour.name, retour.access, retour.isPartial);
                    myStructs.Add(current);
                    continue;
                }
                if (retour.isField)
                {
                    current?.fields.Add(new Field(retour.isStatic, retour.access, retour.type, retour.name, retour.isConst));
                    continue;
                }
                if (retour.isProperty)
                {
                    current?.properties.Add(new Property(retour.isStatic, retour.access, retour.type, retour.name));
                    SkipBloc(line, '{', '}', reader);
                    continue;
                }
                if (retour.isConstructor)
                {
                    if (current != null)
                        current.constructor = new Constructor(retour.isStatic, retour.access, retour.name);
                    SkipBloc(line, '{', '}', reader);
                    continue;
                }
                if (retour.isMethod)
                {
                    if (current != null)
                    {
                        if (!MethodExists(retour.name))
                            current.methods.Add(new Method(retour.isStatic, retour.access, retour.type, retour.name));
                        else
                            current.methods.Add(new Method(retour.isStatic, retour.access, retour.type, retour.name));
                    }
                    SkipBloc(line, '{', '}', reader);
                    continue;
                }
            }
        }
        bool MethodExists(string name)
        {
            foreach (Method m in current.methods)
                if (m.name == name)
                    return true;
            return false;
        }

        /// <summary>
        /// Passe un bloc un ensemble de lignes entre begin et end
        /// </summary>
        /// <param name="line"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="reader"></param>
        static void SkipBloc(string line, char begin, char end, StringReader reader)
        {
            int countOpen = 0;
            do
            {
                if (line.Contains(begin))
                    countOpen += line.Count(f => f == begin);
                if (line.Contains(end))
                {
                    countOpen -= line.Count(f => f == end);
                    if (countOpen == 0) return;
                }
                line = reader.ReadLine();
                line = RemoveSubstringOrChar(line);
                //Debug.WriteLine(line);
            } while (line != null);
        }

        /// <summary>
        /// retire une sous-chaine ou un caractère d'une chaine
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static string RemoveSubstringOrChar(string s)
        {
            string result = "";
            bool ignore = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '"' || s[i] == '\'')
                    ignore = !ignore;
                if (!ignore & s[i] != '"' & s[i] != '\'')
                    result += s[i];
            }
            return result;
        }
        public struct LineStruct
        {
            public bool isNameSpace;
            public bool isStatic;
            public bool isProperty;
            public bool isConstructor;
            public bool isField;
            public bool isReadonly;
            public bool isEvent;
            public bool isClass;
            public bool isStruct;
            public bool isMethod;
            public bool isPartial;
            public bool isConst;
            public bool isEnum;
            public string access;
            public string type;
            public string name;
            public string derivedFrom;
        }

        static bool SearchType(Collection<string> myCollection, out int index)
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

        static bool SearchAccess(Collection<string> myCollection, out int index)
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
        public static LineStruct ParseLine(string s, CSharpClassOrStruct? csc)
        {
            LineStruct retour = new();
            retour.access = "private";
            Collection<string> collection = [];

            string[] tokens = s.Split(' ');
            foreach (string s1 in tokens)
                collection.Add(s1);

            if (collection[0] == "using")
                return retour;

            if (collection.Contains("namespace"))
            {
                retour.isNameSpace = true;
                retour.name = collection[1];
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
            if (collection.Contains("partial"))
            {
                retour.isPartial = true;
                collection.Remove("partial");
            }
            if (collection.Contains("const"))
            {
                retour.isConst = true;
                collection.Remove("const");
            }
            if (collection.Contains("event"))
            {
                retour.isEvent = true;
                retour.type = collection[0];
                retour.name = collection[1];
                return retour;
            }
            if (SearchAccess(collection, out int index))
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
                retour.name = collection[1];
                if (collection.Contains(":"))
                    retour.derivedFrom = collection[collection.Count-1];
                return retour;
            }
            if (collection.Contains("enum"))
            {
                retour.isEnum = true;
                retour.name = collection[1];
                return retour;
            }
            if (collection.Contains("struct"))
            {
                retour.isStruct = true;
                retour.name = collection[1];
                return retour;
            }
            // Constructeur ou Méthode de type standard
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
            // Méthode de type non standard
            if (collection.Count > 1)
                if (collection[1].Contains('('))
                {
                    string tmp = collection[1].Split('(')[0];
                    retour.isMethod = true;
                    retour.type = collection[0];
                    retour.name = tmp;
                    return retour;
                }
            // Field
            if (s.EndsWith(';'))
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
        public override string ToString()
        {
            return filename + "(" + myStructs.Count + " structs;  " + myClasses.Count + " class)";
        }
        // TODO 
        //        public bool IsDisposed { get { return isDisposed; } private set { isDisposed = value; } }
        //        public event PropertyChangedEventHandler PropertyChanged;
        //        Methode de même nom
    }
    //class A
    //{
    //    public string name;
    //    protected string type;
    //    int u;
    //    public int V { get { return u; } }
    //    void dd ()
    //    {
    //        type = "kk";
    //    }
    //}
    //class B: A {
    //    public string name1;
    //    public string type1;
    //    int u1;
    //    void eee ()
    //    {
    //        A a = new();
    //        a.
    //        type = "ff";
    //        name = "";
    //        u = 0;
    //        int p = V;
    //    }
    //}
}
