using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuestPDF.Infrastructure;

class Program {
    static void Main() {
        QuestPDF.Settings.License = LicenseType.Community;
        Stopwatch sw = Stopwatch.StartNew();

        var col1 = new DataColumn
        {
            Sections = new List<DataSection>
            {
                new DataSection
                {
                    Title = "BEDROOM",
                    Items = new List<string> 
                    {
                        "mattress/bed frame",
                        "mattress pad",
                        "comforter/duvet cover",
                        "sheet sets (2)",
                        "bed pillows (2)",
                        "blanket/throw",
                        "decorative pillows (optional)",
                        "dresser/nightstand",
                        "full-length mirror",
                        "hangers/closet organization",
                        "window curtains/blinds",
                        "rug (optional)"
                    }
                },
                new DataSection
                {
                    Title = "LIVING ROOM",
                    Items = new List<string> 
                    {
                        "couch",
                        "side table (1-2)",
                        "coffee table",
                        "lamps (1-2)",
                        "tv/stand",
                        "wall shelves/shelving unit",
                        "comfy throw blanket",
                        "decorative pillows (optional)",
                        "area rug (optional)",
                        "wall decor (optional)",
                        "candles/plants/pictures (optional)"
                    }
                },
                new DataSection
                {
                    Title = "BATHROOM",
                    Items = new List<string> 
                    {
                        "bath towels (2-4)",
                        "hand towels (2-4)",
                        "washcloths (2-4)",
                        "shower curtain/liner/hooks",
                        "bath mat/rug",
                        "soap/soap dish",
                        "trash can/bags",
                        "toilet brush/plunger",
                        "cleaning supplies",
                        "decorative accessories (optional)"
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
                    Title = "KITCHEN",
                    Items = new List<string> 
                    {
                        "table/chairs",
                        "microwave",
                        "blender/mixer",
                        "toaster",
                        "dish drying rack",
                        "aluminum foil",
                        "storage bags",
                        "spice rack",
                        "coffee maker/tea kettle",
                        "set of 4 plates/cups/bowls",
                        "set of 4 coffee mugs",
                        "1 large fry pan",
                        "1 small/med fry pan",
                        "1 large saucepan",
                        "1 small/med sauce pan",
                        "1 stock pot (optional)",
                        "silverware",
                        "knives",
                        "spatulas (2)",
                        "tongs",
                        "wooden spoons (2)",
                        "cutting board",
                        "can/bottle opener",
                        "colander",
                        "vegetable peeler",
                        "tupperware",
                        "casserole dish (1-2)",
                        "dishcloths/sponges (2-4)",
                        "dish towels (2-4)",
                        "oven mitts (2)",
                        "utensil tray/holder",
                        "cookie sheet",
                        "measuring cups & spoons",
                        "mixing bowls",
                        "paper towel holder",
                        "water filter pitcher",
                        "trash can/bags",
                        "broom/dust pan",
                        "mop/bucket",
                        "dish soap/hand soap",
                        "kitchen rug (optional)"
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
                    Title = "LAUNDRY",
                    Items = new List<string> 
                    {
                        "hamper/laundry basket",
                        "iron/clothes steamer",
                        "clothes detergent",
                        "fabric softener",
                        "dryer sheets",
                        "stain remover"
                    }
                },
                new DataSection
                {
                    Title = "EXTRA SUPPLIES",
                    Items = new List<string> 
                    {
                        "vacuum/mini vacuum",
                        "windex/glass cleaner",
                        "all-purpose cleaner",
                        "paper towels/toilet paper",
                        "garbage bags",
                        "extension cords",
                        "surge protectors",
                        "tv cable",
                        "batteries",
                        "bins/storage organizers",
                        "fan",
                        "basic toolkit",
                        "duct tape",
                        "nails/screws",
                        "hammer",
                        "Command hooks",
                        "flashlight",
                        "first aid kit/medicine",
                        "light bulbs",
                        "scissors"
                    }
                },
                new DataSection
                {
                    Title = "OTHER OPTIONALS",
                    Items = new List<string> 
                    {
                        "step stool",
                        "bath scale",
                        "shower caddy",
                        "pictures/frames",
                        "foyer/entrance rug",
                        "desk/chair",
                        "alarm clock",
                        "drink coasters",
                        "clothes drying rack"
                    }
                }
            }
        };

        IPdfGeneratorStrategy strategy = new LayoutStrategy();
        var theme = new ReportTheme();

	string fileName = "apartment_list";

	var columns = new List<DataColumn> { col1, col2, col3 };
	
    strategy.Generate($"{fileName}_letter", PageSize.Letter, columns, theme);
    strategy.Generate($"{fileName}_a4", PageSize.A4, columns, theme, new HashSet<int> { 1 });

    sw.Stop();
	Console.WriteLine("--- Generation Complete ---");
	Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_letter.pdf")}");
	Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_a4.pdf")}");
	Console.WriteLine($"Total time: {sw.Elapsed.TotalSeconds:F3}s");
    }
}
