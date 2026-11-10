return {
    id = "movein_budget",
    template = "movein_budget",
    output_name = "movein_budget",
    completed_date = "2026-11-10",

    header = "First Apartment Move-In Budget",
    subtitle = "Room-by-room setup costs, buy-now priorities, and the cash you need before the keys",
    header_fields = { "Move-in date", "Total budget", "Cash on hand" },

    startup_title = "Cash needed before move-in",
    startup_note = "one-time costs",
    startup_fields = {
        "Security deposit",
        "First month's rent",
        "Utility deposits",
        "Internet setup",
        "Moving truck or help",
        "Renters insurance"
    },
    startup_total = "Total cash needed",

    table_title = "Room-by-room setup",
    table_note = "check what you already own",
    columns = { "Item", "Priority", "Est.", "Actual", "Bought", "Notes" },
    rooms = {
        {
            name = "Entry and hallway",
            items = {
                { name = "Doormat", priority = "now", est = "$20" },
                { name = "Shoe rack", priority = "wait", est = "$30" },
                { name = "Coat hooks", priority = "now", est = "$15" },
                { name = "Hall mirror", priority = "wait", est = "$35" }
            }
        },
        {
            name = "Living room",
            items = {
                { name = "Sofa", priority = "now", est = "$350" },
                { name = "Coffee table", priority = "wait", est = "$60" },
                { name = "Floor lamp", priority = "now", est = "$35" },
                { name = "TV stand", priority = "wait", est = "$80" },
                { name = "Area rug", priority = "wait", est = "$70" }
            }
        },
        {
            name = "Bedroom",
            items = {
                { name = "Mattress and frame", priority = "now", est = "$400" },
                { name = "Sheet set", priority = "now", est = "$40" },
                { name = "Comforter", priority = "now", est = "$50" },
                { name = "Pillows", priority = "now", est = "$30" },
                { name = "Dresser", priority = "wait", est = "$120" }
            }
        },
        {
            name = "Kitchen",
            items = {
                { name = "Cookware set", priority = "now", est = "$100" },
                { name = "Dinnerware set", priority = "now", est = "$45" },
                { name = "Utensil set", priority = "now", est = "$20" },
                { name = "Knife set", priority = "now", est = "$40" },
                { name = "Cutting board", priority = "now", est = "$15" },
                { name = "Dish towels", priority = "now", est = "$12" }
            }
        },
        {
            name = "Bathroom",
            items = {
                { name = "Shower curtain and liner", priority = "now", est = "$20" },
                { name = "Bath mat", priority = "now", est = "$15" },
                { name = "Towel set", priority = "now", est = "$35" },
                { name = "Toilet brush", priority = "now", est = "$8" },
                { name = "Small trash can", priority = "wait", est = "$12" }
            }
        },
        {
            name = "Cleaning and tools",
            items = {
                { name = "Vacuum", priority = "now", est = "$120" },
                { name = "Broom and dustpan", priority = "now", est = "$20" },
                { name = "Mop and bucket", priority = "wait", est = "$25" },
                { name = "All-purpose cleaner", priority = "now", est = "$10" },
                { name = "Tool kit", priority = "wait", est = "$35" }
            }
        }
    },
    total_label = "Setup total",

    footer_title = "First 7 days",
    footer_items = {
        "Toilet paper",
        "Paper towels",
        "Trash bags",
        "Hand soap",
        "Dish soap",
        "Bottled water"
    },
    footer_note = "Estimates are typical starter prices — adjust for your area and check secondhand first."
}
