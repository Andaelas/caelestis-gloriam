using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy
{
    public class Person
    {
        #region Attributes
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private char _sex;
        private DateTime _dateOfBirth;
        private string _birthCertText;
        private DateTime _dateOfDeath;
        private string _deathCertText;
        private string _gedcomCode;

        #endregion
        

        //Properties
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                _firstName = value;
            }
        }
        public string MiddleName
        {
            get
            {
                return _middleName;
            }

            set
            {
                _middleName = value;
            }
        }
        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                _lastName = value;
            }
        }

        public char Sex
        {
            get
            {
                return _sex;
            }

            set
            {
                _sex = value;
            }
        }

        public DateTime DateOfBirth
        {
            get
            {
                return _dateOfBirth;
            }

            set
            {
                _dateOfBirth = value;
            }
        }
        public string BirthCertText
        {
            get
            {
                return _birthCertText;
            }

            set
            {
                _birthCertText = value;
            }
        }

        public DateTime DateOfDeath
        {
            get
            {
                return _dateOfDeath;
            }

            set
            {
                _dateOfDeath = value;
            }
        }
        public string DeathCertText
        {
            get
            {
                return _deathCertText;
            }

            set
            {
                _deathCertText = value;
            }
        }

        public string GedcomCode
        {
            get
            {
                return _gedcomCode;
            }

            set
            {
                _gedcomCode = value;
            }
        }

        
    }
}
