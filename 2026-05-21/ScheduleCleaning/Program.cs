using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuestPDF.Infrastructure;

class Program
{
    static void Main()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        Stopwatch sw = Stopwatch.StartNew();

        var col1 = new DataColumn
        {
            Sections = new List<DataSection>
            {
                new DataSection
                {
                    Title = "mon KITCHEN",
                    Items = new List<string>
                    {
                        "Wipe countertops and cabinet doors",
                        "Clean microwave, oven, and stovetop",
                        "Empty fridge of old food",
                        "Take out trash and clean bin",
                        "Mop kitchen floor"
                    }
                },
                new DataSection
                {
                    Title = "tues BATHROOM",
                    Items = new List<string>
                    {
                        "Scrub toilet, sink, and bathtub/shower",
                        "Clean mirrors and wipe counters",
                        "Replace towels",
                        "Empty bathroom trash",
                        "Mop the floor"
                    }
                },
                new DataSection
                {
                    Title = "wed LIVING ROOM",
                    Items = new List<string>
                    {
                        "Dust furniture and shelves",
                        "Vacuum sofas and carpets",
                        "Wipe TV and remote controls",
                        "Organize clutter",
                        "Sweep/mop floors"
                    }
                },
                new DataSection
                {
                    Title = "thurs BEDROOMS",
                    Items = new List<string>
                    {
                        "Change bed linens",
                        "Dust furniture",
                        "Vacuum or sweep floors",
                        "Tidy up closets and drawers",
                        "Organize nightstands"
                    }
                },
                new DataSection
                {
                    Title = "fri FLOORS & TRASH",
                    Items = new List<string>
                    {
                        "Vacuum or sweep all rooms",
                        "Mop floors (main areas)",
                        "Take out all garbage",
                        "Clean entryway",
                        "Shake out rugs"
                    }
                },
                new DataSection
                {
                    Title = "sat\nsun LAUNDRY & GROCERIES",
                    Items = new List<string>
                    {
                        "Wash, dry, fold, and put away laundry",
                        "Wipe laundry area surfaces",
                        "Clean out expired pantry items",
                        "Grocery shopping",
                        "Restock pantry"
                    }
                }
            }
        };

        var col2 = new DataColumn
        {
            Sections = new List<DataSection>
            {
                new DataSection
                {
                    Title = "",
                    Items = new List<string>
                    {
                    }
                },
            }
        };

        IPdfGeneratorStrategy strategy = new LayoutStrategy();
        var theme = new ReportTheme();

        string fileName = "apartment_list";

        var columns = new List<DataColumn> { col1, col2 };

        strategy.Generate($"{fileName}_letter", PageSize.Letter, columns, theme);
        strategy.Generate($"{fileName}_a4", PageSize.A4, columns, theme);

        sw.Stop();
        Console.WriteLine("--- Generation Complete ---");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_letter.pdf")}");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_a4.pdf")}");
        Console.WriteLine($"Total time: {sw.Elapsed.TotalSeconds:F3}s");
    }
}
