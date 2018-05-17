using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechJobsConsole
{
    class JobData
    {
        static ImmutableList<Dictionary<string, string>> AllJobs;
        static bool IsDataLoaded = false;

        public static ImmutableList<Dictionary<string, string>> FindAll()
        {
            LoadData();
            return AllJobs;
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. 
         */
        public static List<string> FindAll(string column)
        {
            LoadData();

            HashSet<string> searchValues = new HashSet<string>();

            foreach (Dictionary<string, string> job in AllJobs)
            {
                searchValues.Add(job[column]);
            }
            return searchValues.OrderBy(q => q).ToList();
            }

        public static List<Dictionary<string, string>> FindByValue(string searchInput)
        {
            LoadData();
            HashSet<Dictionary<string, string>> listJobs = new HashSet<Dictionary<string, string>>();
            List<Dictionary<string, string>> findJobs = new List<Dictionary<string, string>>();
            
            if (listJobs.Count < 1)
            {
                Console.WriteLine("No Results Found");
            }
            foreach (Dictionary<string, string> job in AllJobs)
            {
                foreach (string key in job.Keys)
                {

                    if (job[key].IndexOf(searchInput, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        listJobs.Add(job);
                    }
                }
            }
            if (findJobs.Count < 1)
            {
                Console.WriteLine("No Results Found");
            }
            foreach (Dictionary<string, string> job in listJobs)
            {
                findJobs.Add(job);
            }
            return sortJobs(findJobs, "employer");
        }

        public static List<Dictionary<string, string>> FindByColumnAndValue(string column, string value)
        {
            // load data, if not already loaded
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();
            
            if (jobs.Count < 1)
            {
                Console.WriteLine("No Results Found");
            }
            foreach (Dictionary<string, string> row in AllJobs)
            {
                string field = row[column];

                if (field.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    jobs.Add(row);
                }
                
            }

            return sortJobs(jobs, column);
        }

        /*
         * Load and parse data from job_data.csv
         */
        private static void LoadData()
        {

            if (IsDataLoaded)
            {
                return;
            }

            List<string[]> rows = new List<string[]>();

            using (StreamReader reader = File.OpenText("job_data.csv"))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    string[] rowArrray = CSVRowToStringArray(line);
                    if (rowArrray.Length > 0)
                    {
                        rows.Add(rowArrray);
                    }
                }
            }

            string[] headers = rows[0];
            rows.Remove(headers);
            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();
            // Parse each row array into a more friendly Dictionary
            foreach (string[] row in rows)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length; i++)
                {
                    rowDict.Add(headers[i], row[i]);
                }
                jobs.Add(rowDict);
            }
            AllJobs = jobs.ToImmutableList();
            IsDataLoaded = true;
        }

        /*
         * Parse a single line of a CSV file into a string array
         */
        private static string[] CSVRowToStringArray(string row, char fieldSeparator = ',', char stringSeparator = '\"')
        {
            bool isBetweenQuotes = false;
            StringBuilder valueBuilder = new StringBuilder();
            List<string> rowValues = new List<string>();

            // Loop through the row string one char at a time
            foreach (char c in row.ToCharArray())
            {
                if ((c == fieldSeparator && !isBetweenQuotes))
                {
                    rowValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                }
                else
                {
                    if (c == stringSeparator)
                    {
                        isBetweenQuotes = !isBetweenQuotes;
                    }
                    else
                    {
                        valueBuilder.Append(c);
                    }
                }
            }

            // Add the final value
            rowValues.Add(valueBuilder.ToString());
            valueBuilder.Clear();

            return rowValues.ToArray();
        }
        private static List<Dictionary<string, string>> sortJobs(List<Dictionary<string, string>> jobs, string column)
        {
            var orderedListJobs = jobs.OrderBy(x => x[column]);
            return orderedListJobs.ToList();
        }
    }
}
