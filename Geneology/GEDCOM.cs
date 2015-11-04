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
	public class GedcomCollection
	{
		// accessible lists
		public List<Individual> individuals = new List<Individual>();
		public List<Family> families = new List<Family>();
	}

	public class Individual
	{
		private string _firstName;
		private string _middleName;
		private string _surName;

		public string Id { get; set; }
		public string Name { get; set; }
		public string FirstName {
			get
			{
				//The first name will be the first part of the string before any the '/' or a ' '.
				if (Name.Contains('/'))
				{
					string cutname = Name.Remove(Name.IndexOf('/'), Name.IndexOf('/') - Name.LastIndexOf('/'));
					if (cutname.Contains(' '))
					{
						return cutname.Split(' ')[0];
					}
					else
					{
						return null;
					}
				}
				else if(Name.Contains(' '))
				{
					return Name.Split(' ')[0];
				}
				else
				{
					return null;
				}
			}
			set;
		}
		public string MiddleName {
			get
			{
				//The first name will be the first part of the string before any the '/' or a ' '.
				if (Name.Contains('/'))
				{
					string cutname = Name.Remove(Name.IndexOf('/'), Name.IndexOf('/') - Name.LastIndexOf('/'));
					if (cutname.Contains(' '))
					{
						return cutname.Split(' ')[(Name.Length-1)];
					}
					else
					{
						return null;
					}
				}
				else if(Name.Contains(' '))
				{
					return Name.Split(' ')[(Name.Length-1)];
				}
				else
				{
					return null;
				}
			}
			set;
		}
		public string SurName {
			get
			{
				if (_surName != null)
				{
					return _surName;
				}
				else
				{
					if (Name.Contains('/'))
					{
						string[] nameSplit = Name.Split('/');
						_surName = nameSplit[1];
						return _surName;
					}
					else
					{
						return null;
					}
				}
			}
			set;
		}
		public string Sex { get; set; }
		public BirthEvent BirthEvent { get; set; }
		public DeathEvent DeathEvent { get; set; }
		public bool IsDead 
		{ 
			get {
				if (DeathEvent != null)
				{
					return true;
				}
				else
				{
					return false;
				}
			} 
		}
		public List<Event> LifeEvents { get; set; }
	}

    public class Family
    {
        public string Id { get; set; }
        public MarriageEvent MarriageEvent { get; set; }
        public Individual Husband { get; set; }
        public Individual Wife { get; set; }
        public List<Individual> Children { get; set; }
    }
    
    private interface Event
    {
        public string Type { get; set; }
        public string Place { get; set; }
        public string Date { get; set; }
    }

    public class BirthEvent : Event
    {
		public string Type = "BIRT";
    }
    
    public class DeathEvent : Event
    {
		public string Type = "DEAT";
    }

	public class MarriageEvent : Event
	{
		public string Type = "MARR";
	}
	
	//Custom Errors
	
	//Individuals must be included in the same file where a family record is located.
	private class IndividualNotFound : Exception 
	{ 
		public IndividualNotFound()
		{
		}

		public IndividualNotFound(string message)
			: base(message)
		{
		}

		public IndividualNotFound(string message, Exception inner)
			: base(message, inner)
		{
		}		
	}
	
    class GEDCOM
    {
        private const string VERSION = "5.5.1";
        private const string CORP = "Cory Brown";
		
		//Constructors
		public GEDCOM()
		{
			//Empty Default
		}
		/// <summary>
		/// Imports a GEDCOM file from a filepath
		/// </summary>
		/// <param name="filePath">Path to a .gedcom file</param>
        public GedcomCollection GEDCOM(string filePath)
        {
            return ReadFromFile(filePath);
        }

		/// <summary>
		/// Reads from file and attempts to fill Individuals
		/// </summary>
		/// <returns>
		/// A Collection of 
		/// </returns>
		public GedcomCollection ReadFromFile(string filePath)
		{
			GedcomCollection gcc = new GedcomCollection();
			string[] zeroLines;
			if (!filePath.EndsWith(".ged"))
			{
				throw new Exception("File is not a GedCom File");
			}
			using (StreamReader sr = new StreamReader(filePath))
			{
				if (sr != null)
				{
					//create individual blocks
					zeroLines = sr.ReadToEnd().Replace("0 @", "\u0646").Split('\u0646');
				}
				else
				{
					throw new NullReferenceException("StreamReader failed to create a stream from path: "+filePath);
				}
			}
			foreach (string block in zeroLines)
			{
				//cut these pieces into lines for parsing
				string[] lines = block.Replace("\r\n", "\r").Split('\r');

				if (lines[0].Contains("INDI"))
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

						be = GetEventData<BirthEvent>(lines, be.Type);

						indi.BirthEvent = be;
					}
					#endregion

					#region Death Event
					if (FindIndexinArray(lines, "1 DEAT ") != -1)
					{
						DeathEvent de = new DeathEvent();

						de = GetEventData<DeathEvent>(lines, de.Type);

						indi.DeathEvent = de;
					}
					#endregion

					// Throw the Individual into the List
					gcc.individuals.Add(indi);
				}
				else if (lines[0].Contains("FAM"))
				{
					//Create the new family and marriage
					Family fam = new Family();


					//grab Fam id from node early on to keep from doing it over and over
					fam.Id = lines[0].Replace("@ FAM", "");

					// Look at each line of node
					foreach (string line in lines)
					{
						//Look for a Marriage Event
						if (line.Contains("1 MARR "))
						{
							MarriageEvent mar = new MarriageEvent();

							mar = GetEventData<MarriageEvent>(lines, mar.Type);

							fam.MarriageEvent = mar;
						}


						/* So as an explanation, Gedcom uses Husband/Wife nomenclature. Gedcom explains that this is because the Marriage Event is
						a record of a union that produced a child and is strictly a matter of bloodline. This will inevitably change when the LDS
						changes their own family record keeping 
                            
						My own software will allow for adoption events and be gender neutral regarding spouses (but will likely still only allow two
						participants until marriage law changes) */

						// If node is HUSB
						if (line.Contains("1 HUSB "))
						{
							string indId = line.Replace("1 HUSB ", "").Replace("@", "").Trim();
							Individual temp = (Individual)gcc.individuals.Where(indi => indi.Id.ToString().Equals(indId));
							if (temp != null)
							{
								fam.Husband = temp;
							}
							else
							{
								throw new IndividualNotFound("Husband: " + indId + " Not found in record for " + fam.Id);
							}
						}
						//If node for Wife
						else if (line.Contains("1 WIFE "))
						{
							string indId = line.Replace("1 WIFE ", "").Replace("@", "").Trim();
							Individual temp = (Individual)gcc.individuals.Where(indi => indi.Id.ToString().Equals(indId));
							if (temp != null)
							{
								fam.Wife = temp;
							}
							else
							{
								throw new IndividualNotFound("Wife: " + indId + " Not found in record for " + fam.Id);
							}
						}
						//if node for multi children
						else if (line.Contains("1 CHIL "))
						{
							string indId = line.Replace("1 CHIL ", "").Replace("@", "").Trim();
							Individual temp = (Individual)gcc.individuals.Where(indi => indi.Id.ToString().Equals(indId));
							if (temp != null)
							{
								fam.Children.Add(temp);
							}
							else
							{
								throw new IndividualNotFound("Child: " + indId + " Not found in record for " + fam.Id);
							}
						}
					}
				}
			}
			return gcc;
		}

		public bool CreateNewFile(string filePath)
		{
			throw new NotImplementedException("Creating a File is not yet implemented");
		}
		/// <summary>
		/// Generic Event reader. Will not pull any event unique data
		/// </summary>
		/// <typeparam name="T">Event Class Type</typeparam>
		/// <param name="lines">Lines of Code where event is located</param>
		/// <param name="eventCode">Event Code, ie. MARR, BIRT, DEAT</param>
		/// <returns>Event cast as requested type</returns>
		private T GetEventData<T>(string[] lines, string eventCode) where T : Event
		{
			Event newEvent = default(T);
			foreach (string line in lines)
			{
				if (line.Contains("1 "+ eventCode +" "))
				{
					
					int i = 1;
					while (lines[FindIndexinArray(lines, "1 " + eventCode + " ") + i].StartsWith("2"))
					{
						switch (lines[FindIndexinArray(lines, "1 " + eventCode + " ") + i].Substring(0, 5))
						{
							case "2 DATE":
								newEvent.Date = lines[FindIndexinArray(lines, "1 " + eventCode + " ") + 1].Replace("2 DATE ", "").Trim();
								break;
							case "2 PLAC":
								newEvent.Place = lines[FindIndexinArray(lines, "1 " + eventCode + " ") + 2].Replace("2 PLAC ", "").Trim();
								break;
						}
					}
				}
			}
			return (T)Convert.ChangeType(newEvent, typeof(T));
		}
		/// <summary>
		/// Finds the last index in an array that contains the search string
		/// </summary>
		/// <param name="Arr">Haystack - Searched Array</param>
		/// <param name="search">Needle - search string</param>
		/// <returns>Zero-based array index, or -1 for error</returns>
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
