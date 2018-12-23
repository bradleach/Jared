Jared
=====

Jared is .NET Standard 2.0 library for building and operating integration apps.

It provides some basic opionated infrastructure like logging (via Serilog) and dependency injection (via Autofac).

Concepts
========

Job
---

A job marshalls various workers to achieve an outcome.

Worker
------

A worker is responsible for completing a discrete part of work. For example, say you need to populate a table "Orders" in a database from the results of a web service call. You may create an "OrderWorker". Likewise, let's assume you also need to update a product catalogue table from the same or a different web service. You could create a "ProductWorker" to achieve this. These workers may be marshalled by a job named "eCommerceSyncJob".

How to Use
==========

1) Create a new project that is compliant with .NET Standard 2.0 (e.g. .NET Core 2.2 or .NET Framework 4.7.2)
2) Add a refernce to the Jared nuget package.
3) Replace your Program.cs class with the following:

```
class Program : JaredApp
{
    static async Task<int> Main(string[] args)
    {
        var program = new Program();
        return await program.Execute(args);
    }
}
```

4) Create a job class and inherit from `ManualSortJob`.
5) Create a worker class and implement the `IWorker` interface.
6) Modify your job class and override the `Workers` property

```public override IEnumerable<Type> Workers => new Type[] { typeof(YourWorker) };```

7) Run your app.

Why is it called Jared?
=======================

Naming Stuff is Hard™. Jared was built to solve a problem I was having with operating integration apps. Jared is the name of a character on the TV show "Silicon Valley". Jared's role on that show was an Operations Manager. I know, the link is tenuous at best, but as I said, naming stuff is hard. Additionally, the name "Jared" was available on nuget.