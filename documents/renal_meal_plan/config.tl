return {
    id = "renal_meal_plan",
    output_name = "renal_meal_plan",
    template = "renal_meal_plan",
    completed_date = "2026-11-09",

    kicker = "Non-dialysis CKD · sample week",
    header = "7-Day Renal Meal Plan",
    subtitle = "A sample week of lower-potassium, lower-sodium meals for CKD stage 3–4",
    plan_card_title = "My week",
    plan_card_fields = { "Week of", "Fluid target" },

    targets = {
        { label = "Potassium", value = "2,000–3,000 mg" },
        { label = "Sodium", value = "under 2,300 mg" },
        { label = "Phosphorus", value = "800–1,000 mg" },
        { label = "Fluid", value = "as advised" }
    },

    menu_title = "The week at a glance",
    menu_note = "2 oz cooked protein per main meal · 1 cup = 240 ml",
    day_note = "Portions are for one adult — confirm your protein, mineral and fluid targets with your renal dietitian",
    days = {
        { name = "Mon", breakfast = "Oatmeal, blueberries, rice milk", lunch = "Chicken salad on white bread", dinner = "Cod, white rice, green beans", watch = "Berries: 1/2 cup max" },
        { name = "Tue", breakfast = "Egg white omelet, white toast", lunch = "Turkey wrap, lettuce, cranberries", dinner = "Shrimp, rice noodles, zucchini", watch = "Cranberries: 2 tbsp max" },
        { name = "Wed", breakfast = "Cream of wheat, strawberries", lunch = "Tuna sandwich, cucumber sticks", dinner = "Chicken, leached potato, cabbage", watch = "Leach the potato first" },
        { name = "Thu", breakfast = "Rice cakes, cream cheese, pear", lunch = "Egg salad, white bread, peppers", dinner = "Pork loin, couscous, summer squash", watch = "Cream cheese: 1 tbsp" },
        { name = "Fri", breakfast = "Pancakes, maple syrup, apple slices", lunch = "Chicken soup, white roll, green beans", dinner = "White fish, rice, steamed cabbage", watch = "Syrup: 1 tbsp · low-salt soup" },
        { name = "Sat", breakfast = "Cornflakes, rice milk, half banana", lunch = "Grilled cheese, cucumber salad", dinner = "Turkey burger, white bun, lettuce", watch = "Half banana only" },
        { name = "Sun", breakfast = "Oatmeal, fresh cranberries, rice milk", lunch = "Veggie wrap, hummus, grapes", dinner = "Roast chicken, leached mash, wax beans", watch = "Hummus: 2 tbsp max" }
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
                { name = "Chicken breast", qty = "1 lb" },
                { name = "Turkey mince", qty = "1/2 lb" },
                { name = "Shrimp", qty = "1/4 lb" },
                { name = "Eggs", qty = "4" },
                { name = "Egg whites", qty = "1 carton" },
                { name = "Tuna", qty = "1 can" }
            }
        },
        {
            name = "Dairy & alternatives",
            items = {
                { name = "Rice milk", qty = "2 cartons" },
                { name = "Cream cheese", qty = "1 tub" },
                { name = "Unsalted butter", qty = "1" },
                { name = "Cheddar", qty = "2 oz" }
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
                { name = "Cranberries, frozen", qty = "1 bag" },
                { name = "Frozen green beans", qty = "1 bag" },
                { name = "White rolls", qty = "4" },
                { name = "Turkey burgers", qty = "2" },
                { name = "Rice noodles", qty = "1 pack" },
                { name = "Unsalted crackers", qty = "1 box" }
            }
        }
    },

    rules = {
        { label = "Protein", text = "2 oz cooked protein at lunch and dinner" },
        { label = "Starch", text = "1/2 cup rice, pasta or leached potato" },
        { label = "Vegetables", text = "1/2 cup low-potassium pick, twice a day" },
        { label = "Fluids", text = "Follow the target your renal team sets" }
    }
}
