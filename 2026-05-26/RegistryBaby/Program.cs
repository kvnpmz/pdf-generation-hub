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
                    Title = "Nursery & Decor",
                    Items = new List<string>
                    {
                        "Crib", "Crib Mattress", "Waterproof Mattress Pad", "Fitted Crib Sheets",
                        "Changing Table/Dresser", "Changing Pad", "Bassinet", "Glider/Ottoman",
                        "Storage Baskets/Bins", "Hamper", "White Noise Machine", "Blackout Curtains",
                        "Baby Sized Hangers"
                    }
                },
                new DataSection
                {
                    Title = "Bath & Potty",
                    Items = new List<string>
                    {
                        "Baby Bathtub", "Bath Support", "Baby Shampoo/Lotion", "Diapers & Wipes",
                        "Diaper Pail/Refills", "Bath Toys", "Bathtub Rinse Cup", "Hooded Towels",
                        "Baby Washcloths"
                    }
                },
                new DataSection
                {
                    Title = "Clothing",
                    Items = new List<string>
                    {
                        "Onesies", "Footie Pajamas", "Swaddles", "Sleep Sacks", "Mittens & Hats", "Socks"
                    }
                },
            }
        };

        var col2 = new DataColumn
        {
            Sections = new List<DataSection>
            {
                new DataSection
                {
                    Title = "Baby Gear & Travel",
                    Items = new List<string>
                    {
                        "Stroller/Travel System", "Infant/Convertible Car Seat", "Car Mirror",
                        "Car Seat Cover", "Baby Carrier", "Swing", "Jumper", "Playard", "Play Gym"
                    }
                },
                new DataSection
                {
                    Title = "Nursing & Feeding",
                    Items = new List<string>
                    {
                        "Breast Pump", "Breast Milk Storage Containers", "Nursing Bras",
                        "Nursing Pads", "Nipple Cream", "Nursing Pillow", "Formula",
                        "Bottles & Nipples", "Bottle Brush", "Drying Rack",
                        "Dishwasher Basket", "Highchair", "Burp Cloths",
                        "Bibs", "Silicone Bibs", "Pacifiers/Pacifier Clips",
                        "Teethers", "Portable Breast Pump", "Sippy Cups",
                        "Utensils, Plates, & Bowls", "Placemats"
                    }
                },
            }
        };

        var col3 = new DataColumn
        {
            Sections = new List<DataSection>
            {
                new DataSection
                {
                    Title = "Health & Safety",
                    Items = new List<string>
                    {
                        "Baby Monitor", "Baby Proofing Locks", "Safety Gates", "First Aid Kit",
                        "Humidifier", "Thermometer", "Baby Toothbrush", "Teething Gel", "Shopping Cart Cover"
                    }
                },
                new DataSection
                {
                    Title = "Toys & Learning",
                    Items = new List<string>
                    {
                        "Books", "Stuffed Animals", "Developmental Toys", "Rattle Toys"
                    }
                },
                new DataSection
                {
                    Title = "For Mom",
                    Items = new List<string>
                    {
                        "Postpartum Care Package", "Diaper Bag", "Hospital Bag", "Memory Book", "Photo Album"
                    }
                },
                new DataSection
                {
                    Title = "Other",
                    Items = new List<string>
                    {
                        "___", 
                        "___", 
                        "___", 
                        "___", 
                        "___", 
                        "___", 
                        "___", 
                        "___", 
                        "___", 
                        "___"
                    }
                }
            }
        };

        IPdfGeneratorStrategy strategy = new LayoutStrategy();
        var theme = new ReportTheme();

        string fileHeader = "Baby Registry Checklist";
        string fileName = "registry_checklist";

        var columns = new List<DataColumn> { col1, col2, col3};

        var pageConfigurations = new[]
        {
            new { Suffix = "letter", Size = PageSize.Letter },
            new { Suffix = "a4", Size = PageSize.A4 }
        };

        foreach (var config in pageConfigurations)
        {
            strategy.Generate($"{fileName}_{config.Suffix}", fileHeader, config.Size, columns, theme);
        }

        sw.Stop();
        Console.WriteLine("--- Generation Complete ---");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_letter.pdf")}");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_a4.pdf")}");
        Console.WriteLine($"Total time: {sw.Elapsed.TotalSeconds:F3}s");
    }
}
