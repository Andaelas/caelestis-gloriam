using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Genealogy
{
    public class Family
    {
        public string Id { get; set; }
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
    }

    public class Event
    {
        public string Type { get; set; }
        public string Place { get; set; }
        public string Date { get; set; }
    }

    public class BirthEvent : Event
    {

    }
    
    public class DeathEvent : Event
    {
        
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

                //Put together our list of individuals and families
                List<Individual> individuals = new List<Individual>();
                List<Family> families = new List<Family>();

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

                        // Throw the Individual into the List
                        individuals.Add(indi);
                    }
                    else if (lines[0].Contains("FAM"))
                    {
                        //Create the new family and marriage
                        Family fam = new Family();
                        MarriageEvent mar = new MarriageEvent();

                        //grab Fam id from node early on to keep from doing it over and over
                        fam.Id = lines[0].Replace("@ FAM", "");

                        //Look for Marriage Event, deal with alternate events.
                        foreach (string line in lines)
                        {

                        }


                        // Look at each line of node
                        foreach (string line in lines)
                        {
                            /* So as an explanation, Gedcom uses Husband/Wife nomenclature. Gedcom explains that this is because the Marriage Event is
                            a record of a union that produced a child and is strictly a matter of bloodline. This will inevitably change when the LDS
                            changes their own family record keeping 
                            
                            My own software will allow for adoption events and be gender neutral regarding spouses (but will likely still only allow two
                            participants until marriage law changes) */

                            // If node is HUSB
                            if (line.Contains("1 HUSB "))
                            {
                                string indId = line.Replace("1 HUSB ", "").Replace("@", "").Trim();
                                Individual temp = (Individual)individuals.Where(indi => indi.Id.ToString().Equals(indId));
                                if (temp != null)
                                {
                                    fam.Husband = temp;
                                }
                            }
                            //If node for Wife
                            else if (line.Contains("1 WIFE "))
                            {
                                string indId = line.Replace("1 WIFE ", "").Replace("@", "").Trim();
                                Individual temp = (Individual)individuals.Where(indi => indi.Id.ToString().Equals(indId));
                                if (temp != null)
                                {
                                    fam.Wife = temp;
                                }
                            }
                            //if node for multi children
                            else if (line.Contains("1 CHIL "))
                            {
                                string indId = line.Replace("1 CHIL ", "").Replace("@", "").Trim();
                                Individual temp = (Individual)individuals.Where(indi => indi.Id.ToString().Equals(indId));
                                if (temp != null)
                                {
                                    fam.Children.Add(temp);
                                }
                            }
                        }
                    }   
                }
            }
            catch(Exception e)
            {
                throw new NotImplementedException(e.Message);
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
