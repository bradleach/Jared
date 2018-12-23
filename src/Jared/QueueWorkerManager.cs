using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jared
{
    public static class QueueWorkerManager
    {
        public static Queue<Type> QueueWorkers(IEnumerable<Type> typesToLoad, bool ignoreDependencies = false)
        {
            var queue = new Queue<Type>();

            typesToLoad = SortWorkers(typesToLoad, ignoreDependencies);

            foreach (var type in typesToLoad)
            {
                queue.Enqueue(type);
            }

            return queue;
        }

        public static Queue<Type> QueueWorkers(bool ignoreDependencies = false)
        {
            var typesToLoad = from assemblies in AppDomain.CurrentDomain.GetAssemblies()
                              from type in assemblies.GetTypes()
                              where typeof(IWorker).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass
                              select type;

            return QueueWorkers(typesToLoad, ignoreDependencies);
        }

        public static Queue<Type> QueueWorkers(string[] typesToResolve, bool ignoreDependencies = false)
        {
            var list = typesToResolve.ToList();

            var typesToLoad = from assemblies in AppDomain.CurrentDomain.GetAssemblies()
                              from type in assemblies.GetTypes()
                              where typeof(IWorker).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass && list.Contains(type.Name)
                              select type;

            return QueueWorkers(typesToLoad, ignoreDependencies);
        }

        private static IList<Type> SortWorkers(IEnumerable<Type> typesToLoad, bool ignoreMissingDependencies)
        {
            var hashSet = new HashSet<Type>(typesToLoad);
            var sortedList = new List<Type>();

            // Now we get into the interesting part - sorting the list by dependencies
            foreach (var type in hashSet)
            {
                ProcessAttributeDependencies(type, hashSet, sortedList, ignoreMissingDependencies);

                // If it doesn't already exist in the list, add it
                if (!sortedList.Contains(type))
                    sortedList.Add(type);
            }

            return sortedList;
        }

        private static void ProcessAttributeDependencies(Type type, HashSet<Type> hashSet, IList<Type> sortedList, bool ignoreMissingDependencies)
        {
            var attributes = type.GetCustomAttributes(typeof(DependencyAttribute), true);
            if (attributes.Count() > 0)
            {
                // Let's loop through each of the types registered in the DependencyAttribute
                foreach (Type dependentType in ((DependencyAttribute)attributes[0]).Types)
                {
                    // Ensure the dependency is in the dictionary
                    if (hashSet.Contains(dependentType))
                    {
                        // If we already contain the type in our final list, move to the next type
                        if (sortedList.Contains(dependentType))
                            continue;

                        // Process any dependencies of the dependentType by recursion
                        ProcessAttributeDependencies(dependentType, hashSet, sortedList, ignoreMissingDependencies);

                        // Add these entry's (and their dependencies to the list)
                        if (!sortedList.Contains(dependentType))
                            sortedList.Add(dependentType);
                    }
                    else if (ignoreMissingDependencies)
                    {
                        continue;
                    }
                    else
                    {
                        Log.Error("Dependency was not found in the loaded assembly");
                        throw new InvalidOperationException();
                    }
                }
            }
        }
    }
}
