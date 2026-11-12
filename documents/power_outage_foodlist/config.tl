return {
    id = "power_outage_foodlist",
    template = "power_outage_foodlist",
    output_name = "power_outage_foodlist",
    completed_date = "2026-11-12",

    kicker = "Power outage · no-cook meals",
    header_line1 = "Power Outage",
    header_line2 = "Food List",
    subtitle = "What to eat first, what needs no heat or water, and three days of no-cook meals",
    plan_card_title = "My outage",
    plan_card_fields = { "People", "Power out at", "Date" },

    eat_first_title = "Eat this first",
    eat_first_aside = "40°F · when in doubt, throw it out",
    eat_first = {
        {
            label = "Fridge",
            window = "4 hours",
            note = "Keep the door shut",
            items = { "Milk & cream", "Eggs", "Cooked leftovers", "Soft cheese", "Cut fruit", "Deli meat" }
        },
        {
            label = "Freezer",
            window = "24–48 hours",
            note = "A full freezer holds cold longer",
            items = { "Frozen meat & fish", "Frozen fruit & veg", "Ice cream", "Bread", "Frozen meals", "Ice packs" }
        },
        {
            label = "Pantry",
            window = "Day 2–3",
            note = "No heat, no water needed",
            items = { "Canned fish", "Crackers", "Nut butter", "Shelf-stable milk", "Fruit cups", "Granola bars" }
        }
    },

    list_title = "No-cook food list",
    list_aside = "Ready to eat · no heat · no water",
    food_groups = {
        {
            title = "Protein",
            items = { "Canned tuna", "Canned salmon", "Canned chicken", "Sardines", "Canned beans", "Peanut butter", "Almond butter", "Beef jerky" }
        },
        {
            title = "Carbs",
            items = { "Crackers", "Tortillas", "Bread (use first)", "Ready-to-eat cereal", "Rice cakes", "Granola bars", "Canned pasta", "Potato chips" }
        },
        {
            title = "Fruit & veg",
            items = { "Canned peaches", "Canned pears", "Canned pineapple", "Applesauce cups", "Fruit cups in juice", "Dried raisins", "Dried apricots", "Canned green beans" }
        },
        {
            title = "Snacks",
            items = { "Trail mix", "Nuts & seeds", "Pre-popped popcorn", "Cookies", "Chocolate bars", "Hard candy", "Pudding cups", "Fruit snacks" }
        },
        {
            title = "Drinks & gear",
            items = { "Bottled water", "Electrolyte powder", "Shelf-stable milk", "Juice boxes", "Bottled iced tea", "Manual can opener", "Paper plates", "Wet wipes" }
        }
    },

    menu_title = "Three days, no heat",
    menu_aside = "Eat the fridge and freezer down first",
    menu_days = { "Day 1", "Day 2", "Day 3" },
    menu_rows = {
        {
            meal = "Breakfast",
            cells = { "Cold cereal, milk, berries", "Granola bars, applesauce, juice box", "Cereal, fruit cup, shelf-stable milk" }
        },
        {
            meal = "Lunch",
            cells = { "Deli sandwich, apple, chips", "Tuna, crackers, pickles", "Bean salad, crackers, olives" }
        },
        {
            meal = "Dinner",
            cells = { "Rotisserie chicken, bread, salad kit", "Canned chilli, rice cakes, peaches", "Canned stew, bread, green beans" }
        },
        {
            meal = "Snacks",
            cells = { "Ice cream, cookies, fruit", "Trail mix, pudding cup, hard candy", "Nuts, dried fruit, popcorn" }
        }
    },

    stock_title = "Stock up before the storm",
    stock_aside = "2 people · 3 days · scale to your table",
    targets_title = "Targets",
    targets = {
        { label = "Calories", value = "2,000 / person / day" },
        { label = "Water", value = "1 gal / person / day" },
        { label = "Ice", value = "2 bags + cooler" },
        { label = "3-day total", value = "6,000 cal / person" }
    },
    shopping = {
        {
            title = "Protein",
            items = { "Canned tuna (6)", "Canned chicken (2)", "Sardines (2)", "Peanut butter (1 jar)", "Canned beans (3)", "Beef jerky (1 bag)" }
        },
        {
            title = "Carbs",
            items = { "Crackers (2 boxes)", "Tortillas (1 pack)", "Bread (1 loaf)", "Cereal (1 box)", "Rice cakes (1 pack)", "Granola bars (12)" }
        },
        {
            title = "Fruit & drinks",
            items = { "Canned fruit (6)", "Applesauce (6)", "Juice boxes (6)", "Shelf milk (4)", "Water (1 gal/person/day)", "Electrolyte powder" }
        },
        {
            title = "Extras",
            items = { "Manual can opener", "Cooler & ice", "Paper plates", "Wet wipes", "Trash bags", "Flashlight & batteries" }
        }
    },

    footer_note = "Eat fridge food within 4 hours · Freezer food within 24–48 hours · When in doubt, throw it out · Rotate stock every 6 months"
}
