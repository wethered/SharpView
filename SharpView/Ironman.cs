using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SharpView
{
    class Ironman
    {
        /* Update these with the project details */

        public static int killDate = 20210119; //INT: Do not operate on this day onwards

        /*
         * ALL VALUES NEED TO BE SHA256 and lowercase
         * From command line: 
         *              echo -n {string} | tr '[:upper:]' '[:lower:]' | sha256sum
         */


        public static string[] wlDomains = { }; //ARRAY (SHA256): AD domains that will execute code - To allow execution on any domain leave blank
        public static string[] blDomains = { }; //ARRAY (SHA256): AD domains that will be ignored - to allow execution on any domain leave blank
        public static string[] wlHosts = { }; //ARRAY (SHA256): hosts that will execute code - To allow execution on any host leave blank
        public static string[] blHosts = { }; //ARRAY (SHA256): hosts that will be ignored - To allow execution on any host leave blank
        public static string[] wlUsers = { }; //ARRAY (SHA256): Users that will execute code - To allow execution for any user leave blank
        public static string[] blUsers = { "04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb" }; //ARRAY (SHA256): Users that will be ignored - To allow execution for any user leave blank

        public static string projectCODENAME = "ODAIBA_DANGO"; //STRING: What's the project's codename?


        static void Main(string[] args)
        {
            DateTime date = DateTime.Now;
            int shortDate = int.Parse(date.ToString("yyyyMMdd"));

            //Check Domains
            bool whiteListedDomains;
            bool blackListedDomains;
            string userDOMAIN = ComputeSha256Hash(System.Environment.GetEnvironmentVariable("USERDOMAIN").ToLower());
            if (wlDomains.Length != 0)
            {
                wlDomains = Array.ConvertAll(wlDomains, x => x.ToLower());
                whiteListedDomains = wlDomains.Contains(userDOMAIN);
            }
            else
            {
                whiteListedDomains = true;
            }


            if (blDomains.Length != 0)
            {
                blDomains = Array.ConvertAll(blDomains, x => x.ToLower());
                blackListedDomains = blDomains.Contains(userDOMAIN);
            }
            else
            {
                blackListedDomains = false;
            }


            //Check hostnames (i.e. computer names)
            bool whiteListedHosts;
            bool blackListedHosts;
            string computerNAME = ComputeSha256Hash(System.Environment.GetEnvironmentVariable("COMPUTERNAME").ToLower());
            if (wlHosts.Length != 0)
            {
                wlHosts = Array.ConvertAll(wlHosts, x => x.ToLower());
                whiteListedHosts = wlHosts.Contains(computerNAME);

            }
            else
            {
                whiteListedHosts = true;
            }


            if (blHosts.Length != 0)
            {
                blHosts = Array.ConvertAll(blHosts, x => x.ToLower());
                blackListedHosts = blHosts.Contains(computerNAME);
            }
            else
            {
                blackListedHosts = false;
            }

            //Check current users
            bool whiteListedUsers;
            bool blackListedUsers;
            string computerUSER = ComputeSha256Hash(System.Environment.GetEnvironmentVariable("USERNAME").ToLower());
            if (wlUsers.Length != 0)
            {
                wlUsers = Array.ConvertAll(wlUsers, x => x.ToLower());
                whiteListedUsers = wlUsers.Contains(computerUSER);
            }
            else
            {
                whiteListedUsers = true;
            }


            if (blUsers.Length != 0)
            {
                blUsers = Array.ConvertAll(blUsers, x => x.ToLower());
                blackListedUsers = blUsers.Contains(computerUSER);
            }
            else
            {
                blackListedUsers = false;
            }



            if ((shortDate >= killDate) || (!whiteListedDomains) || (blackListedDomains) || (!whiteListedHosts) || (blackListedHosts) || (!whiteListedUsers) || (blackListedUsers))

            {
                Decoy();
                return;
            }
            try
            {
                SharpView.Program.Main(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        static void Decoy()
        {
            Console.WriteLine("Application: {0}", System.AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("Framework Version: v4.0.{0}", killDate);
            Console.WriteLine("Description: The application requested process termination through System.Environment.FailFast(string message).");
            Console.WriteLine("Message: Out of Memory: Insufficient memory to continue the execution of the program.");
            Console.WriteLine("Stack:");
            Console.WriteLine("at System.Environment.FailFast(System.String)");
            Console.WriteLine("at {0}.OutOfMemoryException.Program.ThrowExample()", projectCODENAME);
            Console.WriteLine("at {0}.OutOfMemoryException.Program.Main(System.String[])", projectCODENAME);

        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }

}

