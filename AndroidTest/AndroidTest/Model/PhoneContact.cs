
using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidTest.Model
{
    public class PhoneContact
    {
        private string firstName = null;
        private string lastName = null;
        private string fullName = null;
        private string phoneNumber1 = null;
        private string phoneNumber2 = null;
        private string emailAddress1 = null;
        private string emailAddress2 = null;
        private string address = null;

        /// <summary>
        /// Use this ID for lookups
        /// </summary>
        public string ContactId { get; set; }

        public string RawContactId { get; set; }

        public MessageThread MessageThread { get; set; }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(fullName) && (!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName)))
                {
                    return (FirstName + " " + LastName).Trim();
                }

                return fullName;
            }

            set
            {
                fullName = value;
                
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                if (value != firstName)
                {
                    HasChanged = true;
                    NameHasChanged = true;
                }

                firstName = value;
                FullName = firstName + " " + LastName;

            
            }
        }
        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                if (value != lastName)
                {
                    HasChanged = true;
                    NameHasChanged = true;
                }

                lastName = value;
                FullName = firstName + " " + LastName;
            }
        }

        public string NickName { get; set; }


        public string PhoneNumber1
        {
            get
            {
                return phoneNumber1;
            }

            set
            {
                if (value != phoneNumber1)
                {
                    HasChanged = true;
                    Phone1HasChanged = true;
                }

                phoneNumber1 = value;
            }
        }
        public int PhoneNumber1Type { get; set; }
        
        public string PhoneNumber2
        {
            get
            {
                return phoneNumber2;
            }

            set
            {
                if (value != phoneNumber2)
                {
                    HasChanged = true;
                    Phone2HasChanged = true;
                }

                phoneNumber2 = value;
            }
        }
        public int PhoneNumber2Type { get; set; }

        public string EmailAddress1
        {
            get
            {
                return emailAddress1;
            }

            set
            {
                if (value != emailAddress1)
                {
                    HasChanged = true;
                    Email1HasChanged = true;
                }

                emailAddress1 = value;
            }
        }

        public int EmailAddress1Type { get; set; }

        public string EmailAddress2
        {
            get
            {
                return emailAddress2;
            }

            set
            {
                if (value != emailAddress2)
                {
                    HasChanged = true;
                    Email2HasChanged = true;
                }

                emailAddress2 = value;
            }
        }

        public int EmailAddress2Type { get; set; }

        public string Address
        {
            get
            {
                return address;
            }

            set
            {
                if (value != address)
                {
                    HasChanged = true;
                    AddressHasChanged = true;
                }

                address = value;
            }
        }

        public int AddressType { get; set; }

        public string DisplayValue1
        {
            get
            {
                string val = null;

                if (!string.IsNullOrEmpty(FullName))
                {
                    val = FullName;
                 
                }
                else
                {
                    val = Name;
                    
                }

                
                return val;
            }
        }

        public string DisplayValue2
        {
            get
            {
                if (!string.IsNullOrEmpty(PhoneNumber1))
                {
                    return "[" + PhoneNumber1 + "]";
                }

                return string.Empty;
            }
        }

        public string Name { get => $"{FirstName} {LastName}"; }

        public int ListPosition { get; set; }
        public float SearchRsltSimilarity { get; set; }

        public bool HasChanged { get; set; }
        public bool NameHasChanged { get; set; }
        public bool Phone1HasChanged { get; set; }
        public bool Phone2HasChanged { get; set; }
        public bool Email1HasChanged { get; set; }
        public bool Email2HasChanged { get; set; }
        
        public bool AddressHasChanged { get; set; }

        public PhoneContact()
        {
            PhoneNumber1Type = 2;
            PhoneNumber2Type = 1;
            EmailAddress1Type = 1;
            EmailAddress2Type = 2;
            AddressType = 1;
        }

        public void ResetChangeFlags()
        {
            NameHasChanged = false;
            Phone1HasChanged = false;
            Phone2HasChanged = false;
            Email1HasChanged = false;
            Email2HasChanged = false;
            AddressHasChanged = false;
        }
    }
}
