return {
    id = "emergency_stock_log",
    template = "emergency_stock_log",
    output_name = "emergency_food_stock_log",
    completed_date = "2026-11-09",

    kicker = "Pantry readiness",
    header_line1 = "Emergency Food",
    header_line2 = "Stock & Rotation Log",
    subtitle = "Set a 30-day target, count what you own, and rotate before anything expires",
    plan_card_title = "My stock",
    plan_card_fields = { "People", "Days of food", "Date counted", "Next review" },

    stock_title = "Stock check",
    stock_aside = "30-day target · 3 meals a day · 2,000 calories",
    households = {
        { people = 1, label = "1" },
        { people = 2, label = "2" },
        { people = 4, label = "4" }
    },
    write_columns = { "On hand", "Short", "Buy" },
    groups = {
        { name = "Drinking water", unit = "gal", per_person = 30, life = "1 year" },
        { name = "Grains & cereals", unit = "lb", per_person = 15, life = "2 years" },
        { name = "Canned protein", unit = "cans", per_person = 30, life = "2 years" },
        { name = "Canned vegetables", unit = "cans", per_person = 20, life = "2 years" },
        { name = "Fruit & applesauce", unit = "cans", per_person = 15, life = "18 months" },
        { name = "Beans & dry goods", unit = "lb", per_person = 8, life = "3 years" },
        { name = "Powdered milk", unit = "lb", per_person = 2, life = "2 years" },
        { name = "Nut butter & fats", unit = "jars", per_person = 3, life = "1 year" },
        { name = "Snacks & bars", unit = "bars", per_person = 30, life = "6 months" },
        { name = "Shelf-stable milk", unit = "cans", per_person = 8, life = "1 year" }
    },
    stock_notes = {
        "Start with water: one gallon per person per day covers drinking, cooking and basic hygiene.",
        "Buy the food you already eat — an unfamiliar brand is the first thing to go to waste.",
        "Aim for 2,000 calories per person per day and keep a manual can opener beside the stock."
    },

    rotation_title = "Expiry rotation log",
    rotation_aside = "Use first, replace as you go",
    rotation_columns = { "Best by", "Pulled", "Replaced" },
    rotation_rows = {
        { name = "Bottled water", life = "1 year" },
        { name = "Canned tuna & chicken", life = "2 years" },
        { name = "Canned vegetables", life = "2 years" },
        { name = "Canned fruit", life = "18 months" },
        { name = "White rice & pasta", life = "4 years" },
        { name = "Rolled oats", life = "2 years" },
        { name = "Dried beans & lentils", life = "3 years" },
        { name = "Powdered milk", life = "2 years" },
        { name = "Granola & energy bars", life = "6 months" }
    },

    routine_title = "Monthly rotation routine",
    routine_aside = "Ten minutes, once a month",
    routine_steps = {
        { label = "Check water", text = "Confirm a full gallon per person per day is still on the shelf." },
        { label = "Pull oldest", text = "Move the nearest best-by dates to the front of the cupboard." },
        { label = "Cook it in", text = "Eat one pantry meal this week so the stock is not a museum." },
        { label = "Replace used", text = "Add what you ate to the shopping list and log the new date." },
        { label = "Log review", text = "Write next month's review date on the card at the top of the page." }
    },
    footer_note = "Rotate every six months · store cool, dark and dry · planning aid only — follow local guidance"
}
