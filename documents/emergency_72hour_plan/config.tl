return {
    id = "emergency_72hour_plan",
    output_name = "emergency_72hour_plan",
    template = "emergency_plan",
    header_line1 = "72-Hour Emergency",
    header_line2 = "Food & Water Plan",
    kicker = "Disaster preparedness",
    subtitle = "How much to store, what to eat for three days, and what to buy",
    plan_card_title = "My plan",
    plan_card_fields = { "People", "Pets", "Date started" },
    completed_date = "2026-11-06",
    household = {
        { people = "1", water = "3 gal", water7 = "7 gal", meals = "9 meals", meals7 = "21 meals" },
        { people = "2", water = "6 gal", water7 = "14 gal", meals = "18 meals", meals7 = "42 meals" },
        { people = "3", water = "9 gal", water7 = "21 gal", meals = "27 meals", meals7 = "63 meals" },
        { people = "4", water = "12 gal", water7 = "28 gal", meals = "36 meals", meals7 = "84 meals" },
        { people = "5", water = "15 gal", water7 = "35 gal", meals = "45 meals", meals7 = "105 meals" },
        { people = "6", water = "18 gal", water7 = "42 gal", meals = "54 meals", meals7 = "126 meals" }
    },
    quantity_notes = {
        "1 gallon of water per person per day — drinking, cooking and basic hygiene.",
        "Add 1 quart per day for each pet, and extra for hot climates or manual labour.",
        "Aim for 2,000 calories per person per day. No fridge, no stove, no running water."
    },
    menu_days = { "Day 1", "Day 2", "Day 3" },
    menu_rows = {
        {
            meal = "Breakfast",
            cells = {
                "Instant oatmeal, raisins, instant coffee",
                "Shelf-stable milk, cereal, banana chips",
                "Granola bars, applesauce, tea"
            }
        },
        {
            meal = "Lunch",
            cells = {
                "Tuna, crackers, apple, granola bar",
                "Chicken pouch, tortillas, salsa packet",
                "Sardines, pita crackers, olives"
            }
        },
        {
            meal = "Dinner",
            cells = {
                "Canned chilli, rice cakes, peach cup",
                "Lentil soup, crackers, fruit cup",
                "Canned pasta, green beans, pudding cup"
            }
        },
        {
            meal = "Snacks",
            cells = {
                "Peanut butter, trail mix, hard candy",
                "Nuts, beef jerky, hot chocolate",
                "Popcorn, dried mango, boiled sweets"
            }
        }
    },
    shopping = {
        {
            title = "Water & drinks",
            items = {
                "6 gallons drinking water",
                "Water purification tablets",
                "Electrolyte powder sachets",
                "Instant coffee & tea bags",
                "Juice boxes (6)"
            }
        },
        {
            title = "Protein",
            items = {
                "Canned tuna (6)",
                "Chicken pouches (3)",
                "Sardines (2 tins)",
                "Peanut butter (1 jar)",
                "Beef jerky (2 packs)",
                "Canned beans (3)",
                "Lentil soup (2)"
            }
        },
        {
            title = "Carbs & fruit",
            items = {
                "Crackers (2 boxes)",
                "Tortillas or pita (2 packs)",
                "Instant oatmeal (6 sachets)",
                "Granola bars (12)",
                "Rice cakes (1 pack)",
                "Canned peaches & pears (4)",
                "Applesauce cups (6)",
                "Dried fruit & raisins"
            }
        },
        {
            title = "Tools & extras",
            items = {
                "Manual can opener",
                "Flashlight & spare batteries",
                "Battery or hand-crank radio",
                "First aid kit",
                "Wet wipes & hand sanitiser",
                "Heavy-duty trash bags",
                "Waterproof matches",
                "Cash in small notes",
                "Copies of ID & prescriptions"
            }
        }
    },
    prep_title = "Before the power goes out",
    prep_steps = {
        { label = "Water", text = "Fill the tub and every bottle" },
        { label = "Ice", text = "Freeze bottles for fridge ice" },
        { label = "Power", text = "Charge phones and power banks" },
        { label = "Cold", text = "Set the fridge to coldest" },
        { label = "Access", text = "Move the can opener to the kit" },
        { label = "Go-bag", text = "Stage one bag by the door" }
    },
    footer_note = "Rotate stock every 6 months · Store cool, dark and dry · Planning aid only — follow your local emergency guidance"
}
