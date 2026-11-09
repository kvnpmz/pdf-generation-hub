return {
    id = "layout_probe",
    output_name = "layout_probe",
    template = "layout_probe",
    header = "Kitchen Stocktake",
    subtitle = "Pantry, fridge and freezer inventory",
    footer = "Layout probe — generated from a brand new template",
    completed_date = "2026-11-06",
    columns = {
        {
            title = "Have",
            kicker = "Stocked",
            sections = {
                {
                    title = "Fridge",
                    items = { "Milk", "Butter", "Eggs", "Cheddar", "Greek yogurt", "Sour cream", "Hummus", "Spinach", "Bell peppers", "Cucumber" }
                },
                {
                    title = "Freezer",
                    items = { "Peas", "Sweetcorn", "Berries", "Chicken thighs", "Salmon fillets", "Puff pastry", "Ice cream", "Bread rolls" }
                },
                {
                    title = "Pantry",
                    items = { "Rice", "Pasta", "Oats", "Flour", "Sugar", "Olive oil", "Tinned tomatoes", "Chickpeas", "Stock cubes", "Tea" }
                }
            }
        },
        {
            title = "Running Low",
            kicker = "Restock soon",
            sections = {
                {
                    title = "Dairy & chilled",
                    items = { "Whole milk", "Parmesan", "Cream cheese", "Fresh pasta", "Orange juice" }
                },
                {
                    title = "Fruit & veg",
                    items = { "Bananas", "Apples", "Lemons", "Garlic", "Onions", "Carrots", "Potatoes", "Broccoli", "Mushrooms" }
                },
                {
                    title = "Cupboard",
                    items = { "Coffee", "Honey", "Peanut butter", "Soy sauce", "Vinegar", "Baking powder", "Salt", "Black pepper", "Curry powder", "Paprika", "Bay leaves", "Cinnamon" }
                },
                {
                    title = "Household",
                    items = { "Dish soap", "Sponges", "Kitchen roll", "Bin bags", "Laundry detergent" }
                }
            }
        },
        {
            title = "Out",
            kicker = "Add to list",
            sections = {
                {
                    title = "Bakery",
                    items = { "Sourdough loaf", "Tortillas", "Bagels", "Croissants" }
                },
                {
                    title = "Snacks",
                    items = { "Crackers", "Dark chocolate", "Popcorn", "Nuts", "Dried apricots", "Rice cakes", "Granola bars", "Pretzels" }
                },
                {
                    title = "Drinks",
                    items = { "Sparkling water", "Orange juice", "Oat milk", "Green tea", "Coffee beans" }
                },
                {
                    title = "Extras",
                    items = { "Kitchen foil", "Parchment paper", "Cling film", "Freezer bags", "Matches", "Batteries", "Light bulbs", "Greeting card" }
                }
            }
        }
    }
}
