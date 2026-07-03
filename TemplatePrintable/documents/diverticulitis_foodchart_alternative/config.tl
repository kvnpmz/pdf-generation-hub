return {
    id = "diverticulitis_foodchart_alternative",
    template = "diverticulitis_foodchart",
    output_name = "diverticulitis_foodchart",
    overrides = "alternative_style",
    header = "Diverticulitis Food Chart",
    completed_date = "",
    columns = {
        { -- Column 1
            column_name = "Foods to Eat",
            sections = {
                {
                    title = "High-Fiber Foods",
                    type = "nested",
                    items = {
                        { title = "Whole Grains:", items = { "Whole wheat bread", "Quinoa", "Brown rice", "Oats" } },
                        { title = "Fruits And Vegetables:", items = { "Apples", "Spinach", "Berries (strawberries, blueberries)", "Bananas", "Broccoli" } },
                        { title = "Legumes (Beans, Lentils):", items = { "Black beans", "Chickpeas", "Lentils" } },
                        { title = "Nuts And Seeds:", items = { "Almonds", "Chia seeds" } }
                    }
                },
                {
                    title = "Lean Proteins",
                    type = "nested",
                    items = {
                        { title = "Skinless Poultry:", items = { "Chicken breast", "Turkey" } },
                        { title = "Fish:", items = { "Salmon", "Tuna" } },
                        { title = "Eggs", items = {"Eggs prepared without added fat or spice"} }
                    }
                },
                {
                    title = "Healthy Fats",
                    type = "simple",
                    items = { "Avocados", "Flaxseeds", "Olive oil", "Walnuts" }
                },
                {
                    title = "Low-Fat Dairy Products",
                    type = "simple",
                    items = { "Yogurt (Greek yogurt)", "Cottage cheese", "Skim milk", "Low-fat cheese" }
                },
                {
                    title = "Fluids",
                    type = "simple",
                    items = { "Water", "Broth" }
                },
                {
                    title = "Fluids To Consume",
                    type = "detailed",
                    items = {
                        { name = "1. Water:", description = "Adequate hydration is crucial for digestive health and overall well-being." },
                        { name = "2. Herbal Teas:", description = "Non-caffeinated herbal teas can be soothing and hydrating." },
                        { name = "3. Clear Broths:", description = "Broths made from vegetables or lean meats can provide hydration and some nutrients." },
                        { name = "4. Coconut Water:", description = "Provides hydration and electrolytes." },
                        { name = "5. Vegetable Juices:", description = "Low-sodium vegetable juices can contribute to fluid intake and provide some vitamins and minerals." }
                    }
                }
            }
        },
        { -- Column 2
            column_name = "Foods to Avoid",
            sections = {
                {
                    title = "Low-Fiber Foods",
                    type = "simple",
                    items = { "Processed grains (white bread, white rice)", "Cereals high in sugar and low in fiber" }
                },
                {
                    title = "Canned Fruits And Vegetables",
                    type = "simple",
                    items = { "Canned fruits and vegetables in syrup or with added salt." }
                },
                {
                    title = "Fruit Juices",
                    type = "simple",
                    items = { "Fruit juices without pulp" }
                },
                {
                    title = "Refined Carbohydrates",
                    type = "simple",
                    items = { "Pastries", "White pasta", "Instant noodles" }
                },
                {
                    title = "Nuts And Seeds",
                    type = "simple",
                    items = { "Peanuts", "Cashews", "Pistachios" }
                },
                {
                    title = "Fatty Meats",
                    type = "simple",
                    items = { "Bacon", "Sausages", "Processed meats (hot dogs, deli meats)" }
                },
                {
                    title = "High-Fat Foods",
                    type = "simple",
                    items = { "Fried foods", "Potato chips", "Fast food", "Butter" }
                },
                {
                    title = "Spicy Or Highly Seasoned Foods",
                    type = "simple",
                    items = { "Chili peppers", "Spicy sauces" }
                },
                {
                    title = "Liquids To Avoid",
                    type = "detailed",
                    items = {
                        { name = "1. Carbonated Beverages:", description = "High sugar content and gas-producing."},
                        { name = "2. Alcohol:", description = "Can irritate the digestive system." }
                    }
                }
            }
        }
    }
}
