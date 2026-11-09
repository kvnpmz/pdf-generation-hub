return {
    id = "dialysis_food_fluid",
    template = "dialysis_food_fluid",
    output_name = "dialysis_food_fluid_guide",
    completed_date = "2026-11-09",

    header = "Dialysis Food & Fluid Guide",
    subtitle = "What to enjoy, limit and avoid — plus a 7-day fluid log",

    targets = {
        { label = "Potassium", value = "2,000–3,000 mg" },
        { label = "Phosphorus", value = "800–1,000 mg" },
        { label = "Sodium", value = "under 2,000 mg" },
        { label = "Fluid", value = "1–1.5 L" }
    },
    target_note = "Your care team sets your personal numbers — 1 cup = 240 ml · 3 oz cooked protein",

    columns = {
        {
            title = "Enjoy",
            tone = "enjoy",
            items = {
                "Egg whites",
                "Chicken breast",
                "Turkey breast",
                "White fish",
                "Lean beef",
                "White rice",
                "White pasta",
                "White bread",
                "Unsalted rice cakes",
                "Olive oil",
                "Green beans",
                "Cabbage",
                "Cauliflower",
                "Cucumber",
                "Red bell pepper",
                "Apple",
                "Blueberries"
            }
        },
        {
            title = "Limit",
            tone = "limit",
            items = {
                "Potatoes, leached",
                "Sweet potato",
                "Banana, half",
                "Orange",
                "Milk",
                "Yogurt",
                "Cheese",
                "Whole grains",
                "Brown rice",
                "Beans & lentils",
                "Nuts & seeds",
                "Avocado",
                "Tomatoes",
                "Watermelon",
                "Dried fruit",
                "Chocolate",
                "Salted butter"
            }
        },
        {
            title = "Avoid",
            tone = "avoid",
            items = {
                "Salt substitutes",
                "Bacon & ham",
                "Sausage & hot dogs",
                "Deli meats",
                "Canned meat",
                "Canned soup",
                "Instant noodles",
                "Processed cheese",
                "Pickles & olives",
                "Soy sauce",
                "Cola & dark soda",
                "Potato chips",
                "Salted crackers",
                "Fast food",
                "Energy drinks",
                "Sports drinks",
                "Herbal supplements"
            }
        }
    },

    rules_title = "Dialysis-day plate",
    rules = {
        { label = "Protein first", text = "Eggs, chicken, fish or lean beef at every meal" },
        { label = "Fresh, not canned", text = "Rinse canned food and skip added salt" },
        { label = "No salt substitutes", text = "They are usually potassium chloride" }
    },

    fluid_title = "7-day fluid log",
    fluid_note = "Target ___ cups · mark each cup",
    days = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" },
    cups_per_day = 6,

    footer_note = "Follow your care team's numbers — dialysis changes potassium, phosphorus, sodium and fluid needs."
}
