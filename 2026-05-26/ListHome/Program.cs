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
                    Title = "Living Room", 
                    Items = new List<string> 
                    { 
                        "Coat Rack", "Doormat", "Couch", "Cushions", "Chairs", 
                        "Coffee Table", "TV", "Book Case", "Floor Lamp", "Speakers", 
                        "Power Sockets", "Curtains", "Wall Art", "Ceiling Lights"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Dining Area", 
                    Items = new List<string> 
                    { 
                        "Dining Table", "Chairs", "Placemats", "Tablecloth", "Pot Holders"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Appliances", 
                    Items = new List<string> 
                    { 
                        "Refrigerator", "Microwave", "Oven", "Cooker", "Kettle", 
                        "Toaster", "Coffee Machine"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Glassware", 
                    Items = new List<string> 
                    { 
                        "Wine Glasses", "Water Glasses", "Pitcher"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Cookware", 
                    Items = new List<string> 
                    { 
                        "Frying Pan", "Wok", "Pots", "Saucepans"
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
                    Title = "Office", 
                    Items = new List<string> 
                    { 
                        "Desk", "Desk Light", "Book Shelf", "Desk Chair", "Keyboard / Mouse", 
                        "Mouse Mat", "Speakers", "Printer", "Computer", "Laptop Stand", 
                        "Pen", "Pencil", "Markers", "Paper", "Post-it Notes", 
                        "Envelopes", "Stamps", "Paperclips", "Tape", "Organizers"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Cutlery", 
                    Items = new List<string> 
                    { 
                        "Knives", "Forks", "Spoons", "Teaspoons"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Utensils", 
                    Items = new List<string> 
                    { 
                        "Serving Spoons", "Ladle", "Spatula", "Whisk", "Tongs", 
                        "Pizza Cutter", "Peeler", "Scissors", "Bottle Opener", "Corkscrew", 
                        "Colander", "Measuring Cup"
                    } 
                }
            } 
        }; 

        var col3 = new DataColumn 
        { 
            Sections = new List<DataSection> 
            { 
                new DataSection 
                { 
                    Title = "Cleaning", 
                    Items = new List<string> 
                    { 
                        "Vacuum Cleaner", "Broom", "Dust Pan", "Bucket", "Mop", 
                        "Cleaning Cloths", "Sponges", "Cleaning Gloves", "All-Purpose Cleaner", "Bleach", 
                        "Toilet Cleaner", "Window Cleaner", "Glass Wipes", "Step Ladder"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Knives", 
                    Items = new List<string> 
                    { 
                        "Chef's Knife", "Pairing Knife", "Slicing Knife", "Bread Knife", "Utility Knife", 
                        "Cutting Board"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Dinnerware", 
                    Items = new List<string> 
                    { 
                        "Plates", "Side Plates", "Bowls", "Deep Plates", "Serving Bowls", 
                        "Coffee Mugs", "Tea Cups"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Kitchen Misc.", 
                    Items = new List<string> 
                    { 
                        "Food Containers", "Jars", "Spice Rack", "Bin", "Oven Mitts", 
                        "Cutlery Tray", "Lighter"
                    } 
                }
            } 
        }; 

        var col4 = new DataColumn 
        { 
            Sections = new List<DataSection> 
            { 
                new DataSection 
                { 
                    Title = "Bedroom", 
                    Items = new List<string> 
                    { 
                        "Mattress", "Duvet", "Duvet Covers", "Bed Sheets", "Pillows", 
                        "Pillow Cases", "Closet", "Chest Of Drawers", "Night Table", "Night Light", 
                        "Chair", "Under-Bed Storage Boxes", "Curtains", "Curtain Rails", "Alarm Clock", 
                        "Clothing Hangers", "Ceiling Lights"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Bathroom", 
                    Items = new List<string> 
                    { 
                        "Bath Mat", "Shower Curtain", "Mirror", "Medicine Cabinet", "Towel Holder", 
                        "Towel Rail", "Bath Towels", "Hand Towels", "Shower Caddy", "Scale", 
                        "Hair Dryer", "Waste Bin", "Shelves", "Soap Dispenser", "Soap", 
                        "Shampoo", "Shower Gel", "Toothpaste", "Toothbrush"
                    } 
                }
            } 
        }; 

        var col5 = new DataColumn 
        { 
            Sections = new List<DataSection> 
            { 
                new DataSection 
                { 
                    Title = "Laundry", 
                    Items = new List<string> 
                    { 
                        "Washing Machine", "Laundry Basket", "Drying Rack", "Detergent", "Iron", 
                        "Ironing Board"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Toilet", 
                    Items = new List<string> 
                    { 
                        "Toilet Brush", "Toilet Paper", "Toilet Paper Holder", "Toilet Plunger", "Waste Bin", 
                        "Air Freshener", "Hand Soap", "Hand Towel"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Tools", 
                    Items = new List<string> 
                    { 
                        "Hammer", "Nails", "Screwdriver Set", "Screws", "Saw", 
                        "Wrench", "Pliers", "Scissors", "Tape Measure"
                    } 
                }, 
                new DataSection 
                { 
                    Title = "Other", 
                    Items = new List<string> 
                    { 
                        "Smoke Detector", "Batteries", "Power Outlet", "Power Extension Cords", "Storage Boxes", 
                        "Candles", "Light Bulbs", "Flash Light", "Memo Board", "Magnets", 
                        "Vase"
                    } 
                }
            } 
        };

        IPdfGeneratorStrategy strategy = new LayoutStrategy();
        var theme = new ReportTheme();

        string fileHeader = "First Apartment Checklist";
        string fileName = "home_checklist";

        var columns = new List<DataColumn> { col1, col2, col3, col4, col5};

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
