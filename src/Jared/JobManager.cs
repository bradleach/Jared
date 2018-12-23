using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jared
{
    public class JobManager
    {
        private IEnumerable<Type> jobTypes;
        private Type defaultJob = null;
        private Dictionary<char, Type> shortNameTable = new Dictionary<char, Type>();
        private Dictionary<string, Type> longNameTable = new Dictionary<string, Type>();

        public Type FindJobTypeFromArguments(string[] args)
        {
            LoadJobTypes();

            if (!ValidateJobs())
                return null;

            if (args != null && args.Length == 0)
            {
                if (defaultJob == null)
                {
                    Log.Fatal("No Default Job has been established. Add one via the [Option(Default = true)] attribute");
                    return null;
                }
                else
                {
                    return defaultJob;
                }
            }

            // Really simple (and probably broken) command line parser
            if (args.Length > 0)
            {
                string fullArgument = args[0];

                if (fullArgument.StartsWith("--"))
                {
                    string targetJob = fullArgument.Substring(2);
                    if (longNameTable.ContainsKey(targetJob))
                    {
                        return longNameTable[targetJob];
                    }
                }
                else if (fullArgument.StartsWith("-"))
                {
                    char shortName = fullArgument.ToCharArray(1, fullArgument.Length - 1).FirstOrDefault();

                    if (shortNameTable.ContainsKey(shortName))
                    {
                        return shortNameTable[shortName];
                    }
                }
            }

            return null;
        }

        private void LoadJobTypes()
        {
            // Grab all the IJobs from the passed in assembly
            jobTypes = from assemblies in AppDomain.CurrentDomain.GetAssemblies()
                       from type in assemblies.GetTypes()
                       where typeof(IJob).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass
                       select type;
        }

        private bool ValidateJobs()
        {
            defaultJob = null;
            shortNameTable = new Dictionary<char, Type>();
            longNameTable = new Dictionary<string, Type>();

            foreach (var jobType in jobTypes)
            {
                OptionAttribute attribute = Attribute.GetCustomAttribute(jobType, typeof(OptionAttribute)) as OptionAttribute;

                if (attribute == null)
                {
                    if (defaultJob == null)
                    {
                        defaultJob = jobType;
                    }
                    else
                    {
                        Log.Fatal("More than one default job type exists in the assembly");
                        return false;
                    }
                }
                else
                {
                    if (shortNameTable.ContainsKey(attribute.ShortName))
                    {
                        Log.Fatal("Job Short Name is duplicated by the jobs {0} and {1}", shortNameTable[attribute.ShortName].Name, jobType.Name);
                        return false;
                    }
                    else
                    {
                        if (attribute.ShortName != '\0')
                            shortNameTable.Add(attribute.ShortName, jobType);
                    }

                    if (!string.IsNullOrWhiteSpace(attribute.LongName))
                    {
                        if (longNameTable.ContainsKey(attribute.LongName))
                        {
                            Log.Fatal("Job Long Name is duplicated by the jobs {0} and {1}", longNameTable[attribute.LongName].Name, jobType.Name);
                            return false;
                        }
                        else
                        {
                            longNameTable.Add(attribute.LongName, jobType);
                        }
                    }

                    if (attribute.IsDefaultJob)
                    {
                        if (defaultJob == null)
                        {
                            defaultJob = jobType;
                        }
                        else
                        {
                            Log.Fatal("More than one default job type exists in the assembly");
                            return false;
                        }
                    }
                }
            }

            // If we get to here, there are no detected errors with the job types
            return true;
        }
    }
}
