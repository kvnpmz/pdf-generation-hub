return {
    id = "kidney_grocery_list",
    output_name = "kidney_friendly_grocery_list",
    template = "kidney_grocery_list",
    header = "Kidney-Friendly Grocery List",
    badge = "Aisle by aisle",
    subtitle = "A single-page supermarket plan with portion notes and easy swaps for low potassium, low phosphorus, and low sodium shopping",
    completed_date = "2026-11-09",
    aisles = {
        {
            name = "Produce",
            tag = "Enjoy freely",
            items = {
                { name = "Cucumber", note = "½ cup", level = "go" },
                { name = "Bell pepper", note = "1 cup", level = "go" },
                { name = "Cabbage", note = "1 cup", level = "go" },
                { name = "Green beans", note = "½ cup", level = "go" },
                { name = "Zucchini", note = "½ cup", level = "go" },
                { name = "Radishes", note = "½ cup", level = "go" },
                { name = "Blueberries", note = "½ cup", level = "go" }
            }
        },
        {
            name = "Produce",
            tag = "Half portions",
            items = {
                { name = "Potatoes", note = "leach, ½ cup", level = "care" },
                { name = "Bananas", note = "½ small", level = "care" },
                { name = "Spinach", note = "½ cup cooked", level = "care" },
                { name = "Tomatoes", note = "½ cup", level = "care" },
                { name = "Avocado", note = "2 tbsp", level = "care" },
                { name = "Oranges", note = "½ fruit", level = "care" },
                { name = "Dried fruit", note = "skip", level = "no" }
            }
        },
        {
            name = "Meat & Fish",
            tag = "Low phosphorus",
            items = {
                { name = "Skinless chicken", note = "3 oz", level = "go" },
                { name = "Turkey breast", note = "3 oz", level = "go" },
                { name = "Cod", note = "3 oz", level = "go" },
                { name = "Tilapia", note = "3 oz", level = "go" },
                { name = "Egg whites", note = "2 whites", level = "go" },
                { name = "Canned tuna", note = "rinse, 2 oz", level = "care" },
                { name = "Bacon", note = "high sodium", level = "no" }
            }
        },
        {
            name = "Dairy",
            tag = "Check additives",
            items = {
                { name = "Rice milk", note = "unsweetened", level = "go" },
                { name = "Almond milk", note = "unsweetened", level = "go" },
                { name = "Coconut milk", note = "carton", level = "go" },
                { name = "Unsalted butter", note = "small pat", level = "go" },
                { name = "Cream cheese", note = "1 tbsp", level = "care" },
                { name = "Milk", note = "½ cup", level = "care" },
                { name = "Aged cheese", note = "high phosphorus", level = "no" }
            }
        },
        {
            name = "Grains",
            tag = "Half-cup cooked",
            items = {
                { name = "White rice", note = "½ cup", level = "go" },
                { name = "Pasta", note = "½ cup", level = "go" },
                { name = "Couscous", note = "½ cup", level = "go" },
                { name = "Corn tortillas", note = "2 small", level = "go" },
                { name = "Rice cakes", note = "unsalted", level = "go" },
                { name = "Sourdough bread", note = "1 slice", level = "care" },
                { name = "Bran cereal", note = "high potassium", level = "no" }
            }
        },
        {
            name = "Pantry",
            tag = "Rinse well",
            items = {
                { name = "Canned beans", note = "rinse, ½ cup", level = "care" },
                { name = "Canned corn", note = "rinse, ½ cup", level = "care" },
                { name = "Canned peaches", note = "rinse, ½ cup", level = "care" },
                { name = "Canned tomatoes", note = "high potassium", level = "no" },
                { name = "Canned soup", note = "high sodium", level = "no" },
                { name = "Pickles", note = "high sodium", level = "no" },
                { name = "Instant noodles", note = "phosphate", level = "no" }
            }
        },
        {
            name = "Spices & Sauces",
            tag = "No salt subs",
            items = {
                { name = "Garlic powder", note = "no salt", level = "go" },
                { name = "Onion powder", note = "no salt", level = "go" },
                { name = "Dried oregano", note = "", level = "go" },
                { name = "Cinnamon", note = "", level = "go" },
                { name = "Lemon juice", note = "", level = "go" },
                { name = "Olive oil", note = "", level = "go" },
                { name = "Salt", note = "try lemon", level = "no" },
                { name = "Soy sauce", note = "high sodium", level = "no" }
            }
        },
        {
            name = "Drinks",
            tag = "Hydration first",
            items = {
                { name = "Water", note = "", level = "go" },
                { name = "Black coffee", note = "1 cup", level = "go" },
                { name = "Green tea", note = "", level = "go" },
                { name = "Peppermint tea", note = "", level = "go" },
                { name = "Rooibos tea", note = "", level = "go" },
                { name = "Coconut water", note = "high potassium", level = "no" },
                { name = "Cola", note = "phosphate", level = "no" },
                { name = "Sports drink", note = "high sodium", level = "no" }
            }
        },
        {
            name = "Snacks",
            tag = "Read the label",
            items = {
                { name = "Frozen vegetables", note = "plain", level = "go" },
                { name = "Frozen berries", note = "unsweetened", level = "go" },
                { name = "Frozen white fish", note = "plain", level = "go" },
                { name = "Unsalted popcorn", note = "3 cups", level = "go" },
                { name = "Ice cream", note = "½ cup", level = "care" },
                { name = "Frozen fries", note = "added phosphate", level = "no" },
                { name = "Frozen dinner", note = "high sodium", level = "no" },
                { name = "Breaded fish", note = "high sodium", level = "no" }
            }
        }
    },
    footer = "Drain and rinse canned beans and vegetables to cut sodium by up to 40%, and choose products without potassium chloride or phosphate additives. Confirm your daily limits with your renal dietitian."
}
