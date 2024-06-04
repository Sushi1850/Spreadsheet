// Kevin Lee
// CS 3500
// u1175570
//
// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, HashSet<String>> Dependee = new Dictionary<string, HashSet<String>>();
        private Dictionary<string, HashSet<String>> Dependent = new Dictionary<string, HashSet<String>>();
        private int size = 0;
        private int temp = 0;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get { if (Dependee.ContainsKey(s)) return Dependee[s].Count; else return 0; }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (Dependent.ContainsKey(s))
            {
                if (Dependent[s].Count > 0) return true;
            }
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (Dependee.ContainsKey(s))
            {
                if (Dependee[s].Count > 0) return true;
            }
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (Dependent.ContainsKey(s))
            {
                return Dependent[s];
            }
            return new HashSet<String>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (Dependee.ContainsKey(s))
            {
                return Dependee[s];
            }
            return new HashSet<String>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            // See if the key is already in the Dependent dicitionary, if dictionary contains the key it will add another element to List, else create a new key with its dependent
            if (!Dependent.ContainsKey(s))
            {
                SizeCounter(s, t);
                Dependent.Add(s, new HashSet<String>());
                Dependent[s].Add(t);
            }
            else if (!Dependent[s].Contains(t))
            {
                SizeCounter(s, t);
                Dependent[s].Add(t); 
            }

            // See if the key is already in the Dependee dicitionary, if dictionary contains the key it will add another element to List, else create a new key with its dependee
            if (!Dependee.ContainsKey(t))
            {
                SizeCounter(s, t);
                Dependee.Add(t, new HashSet<String>());
                Dependee[t].Add(s);
            }
            else if (!Dependee[t].Contains(s))
            {
                SizeCounter(s, t);
                Dependee[t].Add(s);
            }

        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (Dependent.ContainsKey(s))
            {
                if (Dependent[s].Contains(t))
                {
                    if (Dependee[t].Contains(s))
                    {
                        size--;
                        Dependent[s].Remove(t);
                        Dependee[t].Remove(s);
                    }
                }
            }
            if (Dependee.ContainsKey(s))
            {
                if (Dependee[s].Contains(t))
                {
                    if (Dependent[t].Contains(s))
                    {
                        size--;
                        Dependent[s].Remove(t);
                        Dependee[t].Remove(s);
                    }
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            HashSet<string> copy = new HashSet<string>(GetDependents(s));
            foreach (string r in copy)
            {
                RemoveDependency(s, r);
            }
            foreach (string t in newDependents)
            {
                AddDependency(s, t);
            }            
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            HashSet<string> copy = new HashSet<string>(GetDependees(s));
            foreach (string r in copy)
            {
                RemoveDependency(r, s);
            }
            foreach (string t in newDependees)
            {
                AddDependency(t, s);
            }
        }


        /// <summary>
        /// Add 1 to size when the ordered pair is new
        /// </summary>
        /// <param name="s">The key for dictionary</param>
        /// <param name="t">The value of the dictionary key</param>
        private void SizeCounter(String s, String t)
        {
            temp = 0;
            if (Dependent.ContainsKey(s))
            {
                if (Dependent[s].Contains(t)) temp = 1;
            }

            if (Dependee.ContainsKey(t))
            {
                if (Dependee[t].Contains(s)) temp = 1;
            }

            if (temp != 1) size++;     
        }

    }

}