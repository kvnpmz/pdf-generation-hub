return {
    id = "power_outage_foodlist",
    template = "power_outage_foodlist",
    output_name = "power_outage_foodlist",
    completed_date = "2026-11-12",

    header_line1 = "Power Outage",
    header_line2 = "Food List",
    subtitle = "What to eat first, what needs no heat or water, and three days of no-cook meals",
    plan_card_title = "Outage details",
    plan_card_fields = { "People", "Power out time", "Date" },

    eat_first_title = "Eat this first",
    eat_first = {
        {
            label = "Fridge",
            window = "4 hours",
            note = "Keep the door shut",
            items = { "Milk & cream", "Eggs", "Leftovers", "Soft cheese", "Cut fruit", "Deli meat" }
        },
        {
            label = "Freezer",
            window = "24–48 hours",
            note = "A full freezer holds cold longer",
            items = { "Frozen meat", "Frozen produce", "Ice cream", "Bread", "Frozen meals", "Ice packs" }
        },
        {
            label = "Pantry",
            window = "Day 2–3",
            note = "No heat, no water needed",
            items = { "Canned fish", "Crackers", "Nut butter", "Shelf milk", "Fruit cups", "Granola bars" }
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
            items = { "Crackers", "Tortillas", "Bread, use first", "Cold cereal", "Rice cakes", "Granola bars", "Canned pasta", "Potato chips" }
        },
        {
            title = "Fruit & veg",
            items = { "Canned peaches", "Canned pears", "Canned pineapple", "Applesauce cups", "Fruit cups in juice", "Dried raisins", "Dried apricots", "Green beans" }
        },
        {
            title = "Snacks",
            items = { "Trail mix", "Nuts & seeds", "Popcorn", "Cookies", "Chocolate bars", "Hard candy", "Pudding cups", "Fruit snacks" }
        },
        {
            title = "Drinks & gear",
            items = { "Bottled water", "Electrolyte mix", "Shelf milk", "Juice boxes", "Bottled iced tea", "Can opener", "Paper plates", "Wet wipes" }
        }
    },

    menu_title = "Three days, no heat",
    menu_aside = "Eat the fridge and freezer down first",
    menu_days = { "Day 1", "Day 2", "Day 3" },
    menu_rows = {
        {
            meal = "Breakfast",
            cells = { "Cold cereal, milk, berries", "Granola bar, applesauce, juice", "Cereal, fruit cup, milk" }
        },
        {
            meal = "Lunch",
            cells = { "Deli sandwich, apple, chips", "Tuna, crackers, pickles", "Bean salad, crackers, olives" }
        },
        {
            meal = "Dinner",
            cells = { "Chicken, bread, salad kit", "Chilli, rice cakes, peaches", "Stew, bread, green beans" }
        },
        {
            meal = "Snacks",
            cells = { "Ice cream, cookies, fruit", "Trail mix, pudding, candy", "Nuts, dried fruit, popcorn" }
        }
    },

    stock_title = "Stock up before the storm",
    stock_aside = "2 people · 3 days · scale to your table",
    targets_title = "Targets",
    targets = {
        { label = "Calories", value = "2,000 cal/day" },
        { label = "Water", value = "1 gal/day" },
        { label = "Ice", value = "2 bags + cooler" },
        { label = "3 days", value = "6,000 cal" },
        { label = "Can opener", value = "Manual" }
    },
    shopping = {
        {
            title = "Protein",
            items = { "Tuna x6", "Chicken x2", "Sardines x2", "Peanut butter", "Beans x3", "Jerky" }
        },
        {
            title = "Carbs",
            items = { "Crackers x2", "Tortillas", "Bread", "Cereal", "Rice cakes", "Granola x12" }
        },
        {
            title = "Fruit/drinks",
            items = { "Canned fruit x6", "Applesauce x6", "Juice boxes x6", "Shelf milk x4", "Water 1 gal/day", "Electrolyte mix" }
        },
        {
            title = "Extras",
            items = { "Can opener", "Cooler & ice", "Paper plates", "Wet wipes", "Flashlight", "Batteries" }
        }
    },

}
