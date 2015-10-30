using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Geneology
{
    public class Family
    {
        public MarriageEvent MarriageEvent { get; set; }
        public Individual Husband { get; set; }
        public Individual Wife { get; set; }
        public List<Individual> Children { get; set; }
    }
    
    public class MarriageEvent
    {
        public string Place { get; set; }
        public string Date { get; set; }
    }
    
    public class Individual
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public BirthEvent BirthEvent { get; set; }
        public DeathEvent DeathEvent { get; set; }

        List<Family> SpouseFamilies { get; set; }
    }
    
    public class BirthEvent
    {
        public string Place { get; set; }
        public string Date { get; set; }
    }
    
    public class DeathEvent
    {
        public string Place { get; set; }
        public string Date { get; set; }
    }

    public class CustomEvent
    {
        public string Type { get; set; }
        public string Date { get; set; }
    }


    class GEDCOM
    {
        private const string VERSION = "5.5.1";
        private const string CORP = "Cory Brown";

        public GEDCOM(string filePath)
        {
            try
            {
                StreamReader sr = new StreamReader(filePath);
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    //create individual blocks
                    string[] zeroLines = sr.ReadToEnd().Replace("0 @", "\u0646").Split('\u0646');
                    foreach(string block in zeroLines)
                    {
                        //cut these pieces into lines for parsing
                        string[] lines = block.Replace("\r\n", "\r").Split('\r');
                        
                        if(lines[0].Contains("INDI"))
                        {
                            //create a new individual
                            Individual indi = new Individual();
                            //Find, replace junk characters, and save data.
                            indi.Id = lines[0].Replace("@", "").Replace(" INDI", "").Trim();
                            indi.Name = lines[FindIndexinArray(lines, "NAME")].Replace("1 NAME", "").Replace("/", "").Trim();
                            indi.Sex = lines[FindIndexinArray(lines, "SEX")].Replace("1 SEX ", "").Trim();

                            //special conditional data

                            #region Birth Event
                            if (FindIndexinArray(lines, "1 BIRT ") != -1)
                            {
                                BirthEvent be = new BirthEvent();

                                // add birthday event
                                int i = 1;
                                while (lines[FindIndexinArray(lines, "1 BIRT ") + i].StartsWith("2"))
                                {
                                    switch (lines[FindIndexinArray(lines, "1 BIRT ") + i].Substring(0,5))
                                    {
                                        case "2 Date":
                                            be.Date = lines[FindIndexinArray(lines, "1 BIRT ") + 1].Replace("2 DATE ", "").Trim();
                                            break;
                                        case "2 Plac":
                                            be.Place = lines[FindIndexinArray(lines, "1 BIRT ") + 2].Replace("2 PLAC ", "").Trim();
                                            break;
                                    }   
                                }
                            }
                            #endregion

                            #region Death Event
                            if (FindIndexinArray(lines, "1 DEAT ") != -1)
                            {
                                BirthEvent be = new BirthEvent();

                                // add birthday event
                                int i = 1;
                                while (lines[FindIndexinArray(lines, "1 DEAT ") + i].StartsWith("2"))
                                {
                                    switch (lines[FindIndexinArray(lines, "1 DEAT ") + i].Substring(0, 5))
                                    {
                                        case "2 Date":
                                            be.Date = lines[FindIndexinArray(lines, "1 DEAT ") + 1].Replace("2 DATE ", "").Trim();
                                            break;
                                        case "2 Plac":
                                            be.Place = lines[FindIndexinArray(lines, "1 DEAT ") + 2].Replace("2 PLAC ", "").Trim();
                                            break;
                                    }
                                }
                            }
                            #endregion


                        }




                    }
                }
            }
            catch
            {

            }
        }

        private int FindIndexinArray(string[] Arr, string search)
        {
            int Val = -1;
            for (int i = 0; i < Arr.Length; i++)
            {
                if (Arr[i].Contains(search))
                {
                    Val = i;
                }
            }
            return Val;
        }
    }
}
