/*
 * Kevin Lee
 * u1175570
 * CS 3500
 */
/*
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, cell> Cells = new Dictionary<string, cell>();
        private DependencyGraph dg = new DependencyGraph();
        private cell TempCell;

        public override bool Changed { get; protected set; }

        /// <summary>
        /// Creates an empty spreadsheet
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            Changed = false;
        }

        /// <summary>
        /// Creates an empty spreadsheet
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(s => isValid(s), s => normalize(s), version)
        {
            Changed = false;
        }

        public Spreadsheet(String FilePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(s => isValid(s), s => normalize(s), version)
        {
            Changed = false;
            String SavedVersion = GetSavedVersion(FilePath);
            if (SavedVersion != Version)
                throw new SpreadsheetReadWriteException("The saved version and the version sent in did not match");

            try
            {
                String CellName;

                // Read the files using the XmlReader
                using (XmlReader reader = XmlReader.Create(FilePath))
                {
                    while (reader.Read())
                    {

                        if (reader.Name == "cell")
                        {
                            while (reader.Read())
                            {
                                if (reader.Name == "name")
                                {
                                    reader.Read();
                                    CellName = reader.Value;

                                    while (reader.Read())
                                    {
                                        if (reader.Name == "contents")
                                        {
                                            reader.Read();
                                            SetContentsOfCell(CellName, reader.Value);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { throw new SpreadsheetReadWriteException("The file cannot be properly read, open, or close"); }

        }

        private class cell
        {
            public object contents { get; private set; }

            /// <summary>
            /// Creates a cell for Formula
            /// </summary>
            /// <param name="f">Stores the Formula into contents</param>
            public cell(Formula f)
            {
                contents = f;
            }
            /// <summary>
            /// Creates a cell for String
            /// </summary>
            /// <param name="s">Stores the String into contents</param>
            public cell(String s)
            {
                contents = s;
            }
            /// <summary>
            /// Creates a cell for Double
            /// </summary>
            /// <param name="c">Stores the Double into contents</param>
            public cell(Double d)
            {
                contents = d;
            }
        }

        public override object GetCellContents(string name)
        {
            NameValidator(name);
            IsValid(name);

            String NameNormalize = Normalize(name);

            if (Cells.ContainsKey(NameNormalize))
                return Cells[NameNormalize].contents;
            else
                return "";
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (string key in Cells.Keys)
                yield return key;
        }

        protected override IList<string> SetCellContents(string name, double number)
        {
            //Check to see if the Dictionary contains the variable in the keys
            if (!Cells.ContainsKey(name))
            {
                //If it does not contain then it creates a new key and add it with a cell to the dictionary
                cell StringCell = new cell(number);
                Cells.Add(name, StringCell);
            }
            // If it does exist then the content is replace with the new content
            else Cells[name] = new cell(number);

            //Replace the dependees with an empty Enumerable
            dg.ReplaceDependees(name, Enumerable.Empty<string>());
            GetCellValue(name);


            return GetCellsToRecalculate(name).ToList();
        }

        protected override IList<string> SetCellContents(string name, string text)
        {
            //Check to see if the Dictionary contains the variable in the keys
            if (!Cells.ContainsKey(name))
            {
                //If it does not contain then it creates a new cell and add it to the dictionary
                cell StringCell = new cell(text);
                Cells.Add(name, StringCell);
            }
            // If it does exist then it the content is replace
            else Cells[name] = new cell(text);

            if (text == "") Cells.Remove(name);

            //Replace the dependee with an empty list and 
            dg.ReplaceDependees(name, Enumerable.Empty<string>());


            return GetCellsToRecalculate(name).ToList();
        }

        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            dg.ReplaceDependees(name, formula.GetVariables());
            //Testing for Circular Dependency
            try
            {
                if (!Cells.ContainsKey(name)) //Creates a new key if it does not exist in the dictionary
                    TempCell = new cell("");
                else
                    TempCell = Cells[name]; //Store the previous cell into a tempCell, in case for a circular exception

                GetCellsToRecalculate(name);
            }
            catch
            {
                // Replace the old cell back
                Cells[name] = TempCell;
                dg.ReplaceDependees(name, dg.GetDependees(name));

                // If it doesn't have a cell then remove it from the dictionary
                if (Cells[name].contents == (Object)"")
                    Cells.Remove(name);

                throw new CircularException();
            }
            //Creates a new key if it does not exist in the dictionary
            if (!Cells.ContainsKey(name))
            {
                cell StringCell = new cell(formula);
                Cells.Add(name, StringCell);
                TempCell = new cell("");
            }
            else
            {
                //Store the previous cell into a tempCell, in case for a circular exception
                TempCell = Cells[name];
                Cells[name] = new cell(formula);
            }

            return GetCellsToRecalculate(name).ToList();
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            NameValidator(name);
            return dg.GetDependents(name);
        }

        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    string VersionName = "";
                    while (reader.IsStartElement())
                    {
                        if (reader.HasAttributes)
                        {
                            VersionName = reader.GetAttribute("version");
                            break;
                        }
                        else throw new SpreadsheetReadWriteException("File failed to open");
                    }
                    return VersionName;

                }
            }
            catch { throw new SpreadsheetReadWriteException("The file cannot be properly read, open, or close"); }
        }

        public override void Save(string filename)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";

                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version); // Creates the version attribute with the version as the string

                    //Stores all the Name and Content within the cell element which contain contents and name element
                    foreach (string key in Cells.Keys)
                    {
                        writer.WriteStartElement("cell");

                        writer.WriteElementString("name", key.ToString());


                        if (GetCellContents(key).GetType() == typeof(String))
                            writer.WriteElementString("contents", (String)Cells[key].contents);

                        else if (GetCellContents(key).GetType() == typeof(Double))
                            writer.WriteElementString("contents", Cells[key].contents.ToString()); //Gets the string representation of a double

                        else if (GetCellContents(key).GetType() == typeof(Formula))
                            writer.WriteElementString("contents", "=" + Cells[key].contents.ToString()); //Adds an eqaul sign to the front of the string

                        writer.WriteEndElement(); //Ends cell element
                    }

                    writer.WriteEndElement(); // Ends spreadsheet element
                    writer.WriteEndDocument(); // Ends the document

                }
                Changed = false;

            }//End of catch curly braces
            catch { throw new SpreadsheetReadWriteException("The file could not be open, close, or read properly"); }
        }

        public override object GetCellValue(string name)
        {
            NameValidator(name);
            IsValid(name);

            String NameNormalize = Normalize(name);
            double OutPutDouble;
            if (Cells.ContainsKey(NameNormalize))
            {
                //Checks for double
                if (Double.TryParse(GetCellContents(NameNormalize).ToString(), out OutPutDouble))
                {
                    return OutPutDouble;
                }
                //Checks for Formula
                else if (GetCellContents(NameNormalize).GetType() == typeof(Formula))
                {
                    double DoubleTemp;
                    object contentValue = ((Formula)GetCellContents(NameNormalize)).Evaluate(Lookup);
                    if (double.TryParse(contentValue.ToString(), out DoubleTemp))
                    {
                        return DoubleTemp;
                    }
                    return contentValue;
                }
                //If both check fails, return the string
                else return GetCellContents(NameNormalize).ToString();
            }
            else
                return "";
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content == null) throw new ArgumentNullException();
            NameValidator(name);
            if (!IsValid(name)) throw new InvalidNameException();

            Changed = true;
            string NameNormalized = Normalize(name);
            List<string> List = new List<string>();
            double value;

            // Check to see if it can parse into a double, if it can then call the SetCellContents with the double parameter
            if (Double.TryParse(content, out value))
                List = (List<string>)SetCellContents(NameNormalized, value);

            //Check if its a Formula by seeing if the first character is an eqaul sign
            else if (content.StartsWith("="))
            {
                Formula f;
                //Using a try catch to see if the content can parse into a formula without the eqaul sign
                try { f = new Formula(content.Substring(1), Normalize, IsValid); }
                catch { throw new SpreadsheetUtilities.FormulaFormatException("The string " + content + " cannot be converted into a formula"); }

                List = (List<string>)SetCellContents(NameNormalized, f);
            }
            //If it does not match any of the other two then SetCellContents with a string
            else List = (List<string>)SetCellContents(NameNormalized, content);


            return List;
        }

        /// <summary>
        /// Throw an InvalidNameException when name is either an invalid variable or null
        /// </summary>
        /// <param name="name">The string that is being validated</param>
        private void NameValidator(string name)
        {
            if (name == null) throw new InvalidNameException();
            if (!isVariable(name)) throw new InvalidNameException();
        }

        /// <summary>
        /// Check whether or not the string is a variable
        /// </summary>
        /// <param name="expression">The string that is being tested if its a varaible.</param>
        /// <returns></returns>
        private bool isVariable(string expression)
        {
            string VariablePattern = "^[a-zA-z_][a-zA-Z0-9_]*$";
            return Regex.IsMatch(expression, VariablePattern); ;
        }

        private double Lookup(string v)
        {
            double TempDouble;
            if (Double.TryParse(GetCellValue(v).ToString(), out TempDouble))
                return TempDouble;
            else throw new ArgumentException("The lookup method did not return a double");
        }
    }
}
*/



using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, cell> Cells = new Dictionary<string, cell>();
        private DependencyGraph dg = new DependencyGraph();
        private cell TempCell;

        public override bool Changed { get; protected set; }

        /// <summary>
        /// Creates an empty spreadsheet
        /// </summary>
        public Spreadsheet() : base(s => true, s=> s, "default")
        {
            Changed = false;
        }

        /// <summary>
        /// Creates an empty spreadsheet
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(s => isValid(s), s => normalize(s), version)
        {
            Changed = false;
        }

        public Spreadsheet(String FilePath,Func<string, bool> isValid, Func<string, string> normalize, string version) : base(s => isValid(s), s => normalize(s), version)
        {
            Changed = false;
            String SavedVersion = GetSavedVersion(FilePath);
            if (SavedVersion != Version)
                throw new SpreadsheetReadWriteException("The saved version and the version sent in did not match");

            try
            {
                String CellName;

                // Read the files using the XmlReader
                using (XmlReader reader = XmlReader.Create(FilePath))
                {
                    while (reader.Read())
                    {

                        if (reader.Name == "cell")
                        {
                            while (reader.Read())
                            {
                                if (reader.Name == "name")
                                {
                                    reader.Read();
                                    CellName = reader.Value;

                                    while (reader.Read())
                                    {
                                        if (reader.Name == "contents")
                                        {
                                            reader.Read();
                                            SetContentsOfCell(CellName, reader.Value);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { throw new SpreadsheetReadWriteException("The file cannot be properly read, open, or close"); }

        }

        private class cell 
        {
            public object contents { get; private set; }
            public object value { get; private set; }

            /// <summary>
            /// Creates a cell for Formula
            /// </summary>
            /// <param name="f">Stores the Formula into contents</param>
            public cell(Formula f, object t)
            {
                contents = f;
                value = t;
            }
            /// <summary>
            /// Creates a cell for String
            /// </summary>
            /// <param name="s">Stores the String into contents</param>
            public cell(String s)
            {
                contents = s;
                value = s;
            }
            /// <summary>
            /// Creates a cell for Double
            /// </summary>
            /// <param name="c">Stores the Double into contents</param>
            public cell(Double d)
            {
                contents = d;
                value = d;
            }
        }

        public override object GetCellContents(string name)
        {
            NameValidator(name);
            IsValid(name);

            String NameNormalize = Normalize(name);

            if (Cells.ContainsKey(NameNormalize))
                return Cells[NameNormalize].contents;
            else
                return "";
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (string key in Cells.Keys)
                yield return key;
        }

        protected override IList<string> SetCellContents(string name, double number)
        {
            //Check to see if the Dictionary contains the variable in the keys
            if (!Cells.ContainsKey(name))
            {
                //If it does not contain then it creates a new key and add it with a cell to the dictionary
                cell StringCell = new cell(number);
                Cells.Add(name, StringCell);
            }
            // If it does exist then the content is replace with the new content
            else Cells[name] = new cell(number);

            //Replace the dependees with an empty Enumerable
            //dg.ReplaceDependees(name, Enumerable.Empty<string>());

            //foreach (String a in GetCellsToRecalculate(name))
            //{
            //    Console.WriteLine(a);

            //}
            return GetCellsToRecalculate(name).ToList();
        }

        protected override IList<string> SetCellContents(string name, string text)
        {
            //Check to see if the Dictionary contains the variable in the keys
            if (!Cells.ContainsKey(name))
            {
                //If it does not contain then it creates a new cell and add it to the dictionary
                cell StringCell = new cell(text);
                Cells.Add(name, StringCell);
            }
            // If it does exist then it the content is replace
            else Cells[name] = new cell(text);

            if (text == "") Cells.Remove(name);

            //Replace the dependee with an empty list and 
            dg.ReplaceDependees(name, Enumerable.Empty<string>());


            return GetCellsToRecalculate(name).ToList();
        }

        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            //Creates a new key if it does not exist in the dictionary
            if (!Cells.ContainsKey(name))
            {
                cell StringCell = new cell(formula, formula.Evaluate(Lookup));
                Cells.Add(name, StringCell);
                TempCell = new cell("");
            }
            else
            {
                //Store the previous cell into a tempCell, in case for a circular exception
                TempCell = Cells[name];
                Cells[name] = new cell(formula, formula.Evaluate(Lookup));
            }

            IEnumerable<string> tempDependee = dg.GetDependees(name);
            dg.ReplaceDependees(name, formula.GetVariables());

            try { GetCellsToRecalculate(name); }
            catch
            {
                // Replace the old cell back
                Cells[name] = TempCell;
                dg.ReplaceDependees(name, tempDependee);

                // If it doesn't have a cell then remove it from the dictionary
                if (Cells[name].contents == (Object)"")
                    Cells.Remove(name);

                throw new CircularException();
            }

            return GetCellsToRecalculate(name).ToList();
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            NameValidator(name);
            return dg.GetDependents(name);
        }

        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    string VersionName = "";
                    while (reader.IsStartElement())
                    {
                        if (reader.HasAttributes)
                        {
                            VersionName = reader.GetAttribute("version");
                            break;
                        }
                        else throw new SpreadsheetReadWriteException("File failed to open");
                    }
                    return VersionName;

                }
            }
            catch { throw new SpreadsheetReadWriteException("The file cannot be properly read, open, or close"); }
        }

        public override void Save(string filename)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";

                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version); // Creates the version attribute with the version as the string

                    //Stores all the Name and Content within the cell element which contain contents and name element
                    foreach (string key in Cells.Keys)
                    {
                        writer.WriteStartElement("cell");

                        writer.WriteElementString("name", key.ToString());


                        if (GetCellContents(key).GetType() == typeof(String))
                            writer.WriteElementString("contents", (String)Cells[key].contents);

                        else if (GetCellContents(key).GetType() == typeof(Double))
                            writer.WriteElementString("contents", Cells[key].contents.ToString()); //Gets the string representation of a double

                        else if (GetCellContents(key).GetType() == typeof(Formula))
                            writer.WriteElementString("contents", "=" + Cells[key].contents.ToString()); //Adds an eqaul sign to the front of the string

                        writer.WriteEndElement(); //Ends cell element
                    }

                    writer.WriteEndElement(); // Ends spreadsheet element
                    writer.WriteEndDocument(); // Ends the document

                }
                Changed = false;

            }//End of catch curly braces
            catch { throw new SpreadsheetReadWriteException("The file could not be open, close, or read properly"); }
        }

        public override object GetCellValue(string name)
        {
            NameValidator(name);
            IsValid(name);
            String NameNormalize = Normalize(name);
            if (Cells.ContainsKey(NameNormalize))
            {
                if (Cells[NameNormalize].contents.GetType().Equals(typeof(Formula)))
                    return ((Formula)Cells[NameNormalize].contents).Evaluate(Lookup);

                return Cells[NameNormalize].value;
            }
            else
                return "";
            //double OutPutDouble;
            //if (Cells.ContainsKey(NameNormalize))
            //{
            //    //Checks for double
            //    if (Double.TryParse(GetCellContents(NameNormalize).ToString(), out OutPutDouble))
            //    {
            //        return OutPutDouble;
            //    }
            //    //Checks for Formula
            //    else if (GetCellContents(NameNormalize).GetType() == typeof(Formula))
            //    {
            //        double DoubleTemp;
            //        object contentValue = ((Formula)GetCellContents(NameNormalize)).Evaluate(Lookup);
            //        if (double.TryParse(contentValue.ToString(), out DoubleTemp))
            //        {
            //            return DoubleTemp;
            //        }
            //        return contentValue;
            //    }
            //    //If both check fails, return the string
            //    else return GetCellContents(NameNormalize).ToString();
            //}
            //else
            //    return "";
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content == null) throw new ArgumentNullException();
            NameValidator(name);
            if (!IsValid(name)) throw new InvalidNameException();

            Changed = true;
            string NameNormalized = Normalize(name);
            List<string> List = new List<string>();
            double value;

            // Check to see if it can parse into a double, if it can then call the SetCellContents with the double parameter
            if (Double.TryParse(content, out value))
                List = (List<string>)SetCellContents(NameNormalized, value);

            //Check if its a Formula by seeing if the first character is an eqaul sign
            else if (content.StartsWith("="))
            {
                Formula f;
                //Using a try catch to see if the content can parse into a formula without the eqaul sign
                try { f = new Formula(content.Substring(1), Normalize, IsValid); }
                catch { throw new SpreadsheetUtilities.FormulaFormatException("The string " + content + " cannot be converted into a formula"); }
                List = (List<string>)SetCellContents(NameNormalized, f);
            }
            //If it does not match any of the other two then SetCellContents with a string
            else List = (List<string>)SetCellContents(NameNormalized, content);

            if (!GetCellValue(name).GetType().Equals(typeof(FormulaError)))
            {
                foreach (string a in List)
                {
                    if (!a.Equals(NameNormalized))
                    {
                        if (!GetCellValue(a).Equals(typeof(FormulaError)))
                        {
                            if (GetCellContents(a).GetType().Equals(typeof(Formula)))
                                SetCellContents(a, (Formula)GetCellContents(a));
                            else if (GetCellContents(a).GetType().Equals(typeof(String)))
                                SetCellContents(a, (String)GetCellContents(a));
                            else if (GetCellContents(a).GetType().Equals(typeof(Double)))
                                SetCellContents(a, (double)GetCellContents(a));
                        }
                    }

                }
            }
            return List;
        }

/// <summary>
/// Throw an InvalidNameException when name is either an invalid variable or null
/// </summary>
/// <param name="name">The string that is being validated</param>
private void NameValidator(string name)
{
    if (name == null) throw new InvalidNameException();
    if (!isVariable(name)) throw new InvalidNameException();
}

/// <summary>
/// Check whether or not the string is a variable
/// </summary>
/// <param name="expression">The string that is being tested if its a varaible.</param>
/// <returns></returns>
private bool isVariable(string expression)
{
    string VariablePattern = "^[a-zA-z_][a-zA-Z0-9_]*$";
    return Regex.IsMatch(expression, VariablePattern); ;
}

private double Lookup(string v)
{
    double TempDouble;
    if (Double.TryParse(GetCellValue(v).ToString(), out TempDouble))
        return TempDouble;
    else throw new ArgumentException("The lookup method did not return a double");
}
    }
}


/*
* Kevin Lee
* u1175570
* CS 3500
*/
/*
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, cell> Cells = new Dictionary<string, cell>();
        private DependencyGraph dg = new DependencyGraph();
        private cell TempCell;

        public override bool Changed { get; protected set; }

        /// <summary>
        /// Creates an empty spreadsheet
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            Changed = false;
        }

        /// <summary>
        /// Creates an empty spreadsheet
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(s => isValid(s), s => normalize(s), version)
        {
            Changed = false;
        }

        public Spreadsheet(String FilePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(s => isValid(s), s => normalize(s), version)
        {
            Changed = false;
            String SavedVersion = GetSavedVersion(FilePath);
            if (SavedVersion != Version)
                throw new SpreadsheetReadWriteException("The saved version and the version sent in did not match");

            try
            {
                String CellName;

                // Read the files using the XmlReader
                using (XmlReader reader = XmlReader.Create(FilePath))
                {
                    while (reader.Read())
                    {

                        if (reader.Name == "cell")
                        {
                            while (reader.Read())
                            {
                                if (reader.Name == "name")
                                {
                                    reader.Read();
                                    CellName = reader.Value;

                                    while (reader.Read())
                                    {
                                        if (reader.Name == "contents")
                                        {
                                            reader.Read();
                                            Console.WriteLine("reached");
                                            SetContentsOfCell(CellName, reader.Value);
                                            Console.WriteLine("reach");

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { throw new SpreadsheetReadWriteException("The file cannot be properly read, open, or close"); }

        }

        private class cell
        {
            public object contents { get; private set; }

            /// <summary>
            /// Creates a cell for Formula
            /// </summary>
            /// <param name="f">Stores the Formula into contents</param>
            public cell(Formula f)
            {
                contents = f;
            }
            /// <summary>
            /// Creates a cell for String
            /// </summary>
            /// <param name="s">Stores the String into contents</param>
            public cell(String s)
            {
                contents = s;
            }
            /// <summary>
            /// Creates a cell for Double
            /// </summary>
            /// <param name="c">Stores the Double into contents</param>
            public cell(Double d)
            {
                contents = d;
            }
        }

        public override object GetCellContents(string name)
        {
            NameValidator(name);
            IsValid(name);

            String NameNormalize = Normalize(name);

            if (Cells.ContainsKey(NameNormalize))
                return Cells[NameNormalize].contents;
            else
                return "";
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (string key in Cells.Keys)
                yield return key;
        }

        protected override IList<string> SetCellContents(string name, double number)
        {
            //Check to see if the Dictionary contains the variable in the keys
            if (!Cells.ContainsKey(name))
            {
                //If it does not contain then it creates a new key and add it with a cell to the dictionary
                cell StringCell = new cell(number);
                Cells.Add(name, StringCell);
            }
            // If it does exist then the content is replace with the new content
            else Cells[name] = new cell(number);

            //Replace the dependees with an empty Enumerable
            dg.ReplaceDependees(name, Enumerable.Empty<string>());
            GetCellValue(name);


            return GetCellsToRecalculate(name).ToList();
        }

        protected override IList<string> SetCellContents(string name, string text)
        {
            //Check to see if the Dictionary contains the variable in the keys
            if (!Cells.ContainsKey(name))
            {
                //If it does not contain then it creates a new cell and add it to the dictionary
                cell StringCell = new cell(text);
                Cells.Add(name, StringCell);
            }
            // If it does exist then it the content is replace
            else Cells[name] = new cell(text);

            if (text == "") Cells.Remove(name);

            //Replace the dependee with an empty list and 
            dg.ReplaceDependees(name, Enumerable.Empty<string>());


            return GetCellsToRecalculate(name).ToList();
        }

        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            //Creates a new key if it does not exist in the dictionary
            if (!Cells.ContainsKey(name))
            {
                cell StringCell = new cell(formula);
                Cells.Add(name, StringCell);
                TempCell = new cell("");
            }
            else
            {
                //Store the previous cell into a tempCell, in case for a circular exception
                TempCell = Cells[name];
                Cells[name] = new cell(formula);
            }

            return GetCellsToRecalculate(name).ToList();
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            NameValidator(name);
            return dg.GetDependents(name);
        }

        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    string VersionName = "";
                    while (reader.IsStartElement())
                    {
                        if (reader.HasAttributes)
                        {
                            VersionName = reader.GetAttribute("version");
                            break;
                        }
                        else throw new SpreadsheetReadWriteException("File failed to open");
                    }
                    return VersionName;

                }
            }
            catch { throw new SpreadsheetReadWriteException("The file cannot be properly read, open, or close"); }
        }

        public override void Save(string filename)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";

                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version); // Creates the version attribute with the version as the string

                    //Stores all the Name and Content within the cell element which contain contents and name element
                    foreach (string key in Cells.Keys)
                    {
                        writer.WriteStartElement("cell");

                        writer.WriteElementString("name", key.ToString());


                        if (GetCellContents(key).GetType() == typeof(String))
                            writer.WriteElementString("contents", (String)Cells[key].contents);

                        else if (GetCellContents(key).GetType() == typeof(Double))
                            writer.WriteElementString("contents", Cells[key].contents.ToString()); //Gets the string representation of a double

                        else if (GetCellContents(key).GetType() == typeof(Formula))
                            writer.WriteElementString("contents", "=" + Cells[key].contents.ToString()); //Adds an eqaul sign to the front of the string

                        writer.WriteEndElement(); //Ends cell element
                    }

                    writer.WriteEndElement(); // Ends spreadsheet element
                    writer.WriteEndDocument(); // Ends the document

                }
                Changed = false;

            }//End of catch curly braces
            catch { throw new SpreadsheetReadWriteException("The file could not be open, close, or read properly"); }
        }

        public override object GetCellValue(string name)
        {
            NameValidator(name);
            IsValid(name);

            String NameNormalize = Normalize(name);
            double OutPutDouble;
            if (Cells.ContainsKey(NameNormalize))
            {
                //Checks for double
                if (Double.TryParse(GetCellContents(NameNormalize).ToString(), out OutPutDouble))
                {
                    return OutPutDouble;
                }
                //Checks for Formula
                else if (GetCellContents(NameNormalize).GetType() == typeof(Formula))
                {
                    double DoubleTemp;
                    object contentValue = ((Formula)GetCellContents(NameNormalize)).Evaluate(Lookup);
                    if (double.TryParse(contentValue.ToString(), out DoubleTemp))
                    {
                        return DoubleTemp;
                    }
                    return contentValue;
                }
                //If both check fails, return the string
                else return GetCellContents(NameNormalize).ToString();
            }
            else
                return "";
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content == null) throw new ArgumentNullException();
            NameValidator(name);
            if (!IsValid(name)) throw new InvalidNameException();

            Changed = true;
            string NameNormalized = Normalize(name);
            List<string> List = new List<string>();
            double value;

            // Check to see if it can parse into a double, if it can then call the SetCellContents with the double parameter
            if (Double.TryParse(content, out value))
                List = (List<string>)SetCellContents(NameNormalized, value);

            //Check if its a Formula by seeing if the first character is an eqaul sign
            else if (content.StartsWith("="))
            {
                Formula f;
                //Using a try catch to see if the content can parse into a formula without the eqaul sign
                try { f = new Formula(content.Substring(1), Normalize, IsValid); }
                catch { throw new SpreadsheetUtilities.FormulaFormatException("The string " + content + " cannot be converted into a formula"); }

                dg.ReplaceDependees(NameNormalized, f.GetVariables());
                //Testing for Circular Dependency
                try
                {
                    if (!Cells.ContainsKey(NameNormalized)) //Creates a new key if it does not exist in the dictionary
                        TempCell = new cell("");
                    else
                        TempCell = Cells[NameNormalized]; //Store the previous cell into a tempCell, in case for a circular exception

                    GetCellsToRecalculate(NameNormalized);
                }
                catch
                {
                    // Replace the old cell back
                    Cells[NameNormalized] = TempCell;
                    dg.ReplaceDependees(NameNormalized, dg.GetDependees(NameNormalized));

                    // If it doesn't have a cell then remove it from the dictionary
                    if (Cells[NameNormalized].contents == (Object)"")
                        Cells.Remove(NameNormalized);

                    throw new CircularException();
                }
                List = (List<string>)SetCellContents(NameNormalized, f);
            }
            //If it does not match any of the other two then SetCellContents with a string
            else List = (List<string>)SetCellContents(NameNormalized, content);


            return List;
        }

        /// <summary>
        /// Throw an InvalidNameException when name is either an invalid variable or null
        /// </summary>
        /// <param name="name">The string that is being validated</param>
        private void NameValidator(string name)
        {
            if (name == null) throw new InvalidNameException();
            if (!isVariable(name)) throw new InvalidNameException();
        }

        /// <summary>
        /// Check whether or not the string is a variable
        /// </summary>
        /// <param name="expression">The string that is being tested if its a varaible.</param>
        /// <returns></returns>
        private bool isVariable(string expression)
        {
            string VariablePattern = "^[a-zA-z_][a-zA-Z0-9_]*$";
            return Regex.IsMatch(expression, VariablePattern); ;
        }

        private double Lookup(string v)
        {
            double TempDouble;
            if (Double.TryParse(GetCellValue(v).ToString(), out TempDouble))
                return TempDouble;
            else throw new ArgumentException("The lookup method did not return a double");
        }
    }
}*/