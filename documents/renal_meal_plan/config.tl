return {
    id = "renal_meal_plan",
    output_name = "renal_meal_plan",
    template = "renal_meal_plan",
    completed_date = "2026-11-09",

    kicker = "Renal nutrition",
    header_line1 = "Renal Week",
    header_line2 = "7-Day Meal Plan",
    subtitle = "Low potassium, low sodium and low phosphorus menus with a matching shopping list",
    plan_card_title = "My week",
    plan_card_fields = { "Week of", "People", "Fluid goal" },

    targets = {
        { label = "Potassium", value = "2,000–3,000 mg" },
        { label = "Sodium", value = "under 2,000 mg" },
        { label = "Phosphorus", value = "800–1,000 mg" },
        { label = "Fluid", value = "32–40 oz" }
    },

    menu_title = "The week at a glance",
    menu_note = "Portions: 1 cup = 240 ml · protein 3 oz cooked",
    day_note = "Estimates for the portions shown — confirm your mineral and fluid limits with your renal dietitian",
    days = {
        { name = "Mon", breakfast = "Oatmeal, blueberries, rice milk", lunch = "Chicken salad on white bread", dinner = "Cod, white rice, green beans", k = "2,650", na = "1,480", phos = "840", fluid = "40" },
        { name = "Tue", breakfast = "Egg white omelet, white toast", lunch = "Turkey wrap, lettuce, cranberries", dinner = "Shrimp, rice noodles, zucchini", k = "2,380", na = "1,320", phos = "780", fluid = "36" },
        { name = "Wed", breakfast = "Cream of wheat, strawberries", lunch = "Tuna sandwich, cucumber sticks", dinner = "Chicken, leached potato, cabbage", k = "2,720", na = "1,560", phos = "860", fluid = "40" },
        { name = "Thu", breakfast = "Rice cakes, cream cheese, pear", lunch = "Egg salad, white bread, peppers", dinner = "Pork loin, couscous, summer squash", k = "2,450", na = "1,390", phos = "800", fluid = "32" },
        { name = "Fri", breakfast = "Pancakes, maple syrup, apple slices", lunch = "Chicken soup, white roll, green beans", dinner = "White fish, rice, steamed cabbage", k = "2,810", na = "1,610", phos = "890", fluid = "36" },
        { name = "Sat", breakfast = "Cornflakes, rice milk, half banana", lunch = "Grilled cheese, cucumber salad", dinner = "Turkey burger, white bun, lettuce", k = "2,520", na = "1,450", phos = "820", fluid = "32" },
        { name = "Sun", breakfast = "Oatmeal, dried cranberries, almond milk", lunch = "Veggie wrap, hummus, grapes", dinner = "Roast chicken, mashed potato, wax beans", k = "2,640", na = "1,510", phos = "850", fluid = "40" }
    },

    shopping_title = "Shopping list",
    shopping_note = "One person, one week · check labels for added phosphate",
    aisles = {
        {
            name = "Produce",
            items = {
                { name = "Blueberries", qty = "1 pt" },
                { name = "Strawberries", qty = "1 pt" },
                { name = "Apples", qty = "4" },
                { name = "Pears", qty = "2" },
                { name = "Green & wax beans", qty = "1 lb" },
                { name = "Cucumbers", qty = "2" },
                { name = "Lettuce", qty = "1 head" }
            }
        },
        {
            name = "Bakery & grains",
            items = {
                { name = "White bread", qty = "1 loaf" },
                { name = "Oatmeal", qty = "1 box" },
                { name = "Cream of wheat", qty = "1 box" },
                { name = "White rice", qty = "2 lb" },
                { name = "Couscous", qty = "1 box" },
                { name = "Rice cakes", qty = "1 pack" },
                { name = "Cornflakes", qty = "1 box" }
            }
        },
        {
            name = "Protein",
            items = {
                { name = "Cod fillets", qty = "2" },
                { name = "Chicken breast", qty = "2 lb" },
                { name = "Turkey mince", qty = "1 lb" },
                { name = "Shrimp", qty = "1/2 lb" },
                { name = "Eggs", qty = "6" },
                { name = "Egg whites", qty = "1 carton" },
                { name = "Tuna", qty = "2 cans" }
            }
        },
        {
            name = "Dairy & alternatives",
            items = {
                { name = "Rice milk", qty = "1 carton" },
                { name = "Almond milk", qty = "1 carton" },
                { name = "Cream cheese", qty = "1 tub" },
                { name = "Unsalted butter", qty = "1" },
                { name = "Cheddar", qty = "4 oz" }
            }
        },
        {
            name = "Pantry",
            items = {
                { name = "Olive oil", qty = "1 bottle" },
                { name = "Honey", qty = "1 jar" },
                { name = "Cranberry sauce", qty = "1 jar" },
                { name = "Hummus", qty = "1 tub" },
                { name = "Low-sodium broth", qty = "2 cans" },
                { name = "Maple syrup", qty = "1 bottle" }
            }
        },
        {
            name = "Frozen & extras",
            items = {
                { name = "Frozen blueberries", qty = "1 bag" },
                { name = "Frozen green beans", qty = "1 bag" },
                { name = "White rolls", qty = "4" },
                { name = "Turkey burgers", qty = "2" },
                { name = "Rice noodles", qty = "1 pack" },
                { name = "Unsalted crackers", qty = "1 box" }
            }
        }
    },

    rules = {
        { label = "Protein", text = "Palm-size, 3 oz cooked, twice a day" },
        { label = "Starch", text = "1/2 cup rice, pasta or leached potato" },
        { label = "Vegetables", text = "1/2 cup low-potassium pick, twice a day" },
        { label = "Fluids", text = "Measure drinks, ice and soup in one jug" }
    }
}
