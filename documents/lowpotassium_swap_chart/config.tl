return {
    id = "lowpotassium_swap_chart",
    template = "lowpotassium_swap_chart",
    output_name = "lowpotassium_swap_chart",
    completed_date = "2026-11-10",

    kicker = "CKD & dialysis · renal nutrition",
    header = "Low Potassium",
    header_second_line = "Food Swap Chart",
    subtitle = "Trade the higher-potassium food for the lower one — and keep the serving small",
    card_title = "My targets",
    card_fields = { "Potassium target", "Fluid limit", "Date" },

    columns = { "Instead of — higher potassium", "→", "Eat instead — lower potassium" },
    legend_high = "= potassium in the serving shown",
    legend_low = "lower-potassium swap to eat instead",

    groups = {
        {
            name = "Vegetables",
            note = "Cook, drain and leach where you can",
            rows = {
                { food = "Baked potato, with skin", serving = "1 medium", potassium = "926 mg", swap = "Cauliflower mash, 1/2 cup" },
                { food = "Sweet potato", serving = "1 medium", potassium = "542 mg", swap = "Green beans, 1/2 cup" },
                { food = "Spinach, cooked", serving = "1/2 cup", potassium = "419 mg", swap = "Iceberg lettuce, 1 cup" },
                { food = "Beet, cooked", serving = "1/2 cup", potassium = "259 mg", swap = "Cabbage, 1/2 cup" },
            }
        },
        {
            name = "Fruits",
            note = "Keep fruit to two or three small portions a day",
            rows = {
                { food = "Avocado", serving = "1/2 fruit", potassium = "487 mg", swap = "Strawberries, 1/2 cup" },
                { food = "Banana", serving = "1 medium", potassium = "422 mg", swap = "Apple, 1 medium" },
                { food = "Cantaloupe", serving = "1 cup", potassium = "427 mg", swap = "Grapes, 1/2 cup" },
                { food = "Raisins", serving = "1/4 cup", potassium = "272 mg", swap = "Dried cranberries, 1/4 cup" },
            }
        },
        {
            name = "Proteins",
            note = "Portion meat to 3 oz cooked",
            rows = {
                { food = "Lentils, cooked", serving = "1/2 cup", potassium = "365 mg", swap = "White rice, 1/2 cup" },
                { food = "Pork chop", serving = "3 oz", potassium = "380 mg", swap = "Chicken breast, 3 oz" },
                { food = "Salmon, cooked", serving = "3 oz", potassium = "326 mg", swap = "Egg, 1 large" },
                { food = "Black beans, cooked", serving = "1/2 cup", potassium = "305 mg", swap = "Rice noodles, 1/2 cup" },
            }
        },
        {
            name = "Grains & starches",
            note = "Refined white grains run lower than whole",
            rows = {
                { food = "Bran cereal", serving = "1/2 cup", potassium = "350 mg", swap = "Cornflakes, 3/4 cup" },
                { food = "Granola", serving = "1/2 cup", potassium = "260 mg", swap = "Rice Krispies, 3/4 cup" },
                { food = "Brown rice, cooked", serving = "1 cup", potassium = "154 mg", swap = "White rice, 1 cup" },
            }
        },
        {
            name = "Dairy & drinks",
            note = "Watch milk, juice and coconut water",
            rows = {
                { food = "Coconut water", serving = "1 cup", potassium = "600 mg", swap = "Water with lemon" },
                { food = "Hot chocolate", serving = "1 cup", potassium = "380 mg", swap = "Herbal tea, 1 cup" },
                { food = "Milk, 2%", serving = "1 cup", potassium = "366 mg", swap = "Rice milk, 1 cup" },
                { food = "Yogurt, plain", serving = "6 oz", potassium = "260 mg", swap = "Cottage cheese, 1/4 cup" },
                { food = "Orange juice", serving = "1/2 cup", potassium = "248 mg", swap = "Apple juice, 1/2 cup" },
            }
        },
        {
            name = "Snacks & extras",
            note = "Salt substitutes are the biggest trap",
            rows = {
                { food = "Salt substitute", serving = "1/4 tsp", potassium = "650 mg", swap = "Lemon juice and herbs" },
                { food = "Potato chips", serving = "1 oz", potassium = "360 mg", swap = "Rice cakes, 1" },
                { food = "Molasses", serving = "1 tbsp", potassium = "293 mg", swap = "Honey, 1 tbsp" },
                { food = "Chocolate, dark", serving = "1 oz", potassium = "210 mg", swap = "Vanilla wafers, 2" },
                { food = "Almonds", serving = "1 oz", potassium = "208 mg", swap = "Popcorn, 3 cups" },
            }
        },
    },

    rules = {
        { label = "Portion first", text = "1/2 cup of a higher-potassium food can still fit your daily limit." },
        { label = "Leach roots", text = "Peel, dice and soak potatoes for 2 hours, then boil and drain." },
        { label = "Read labels", text = "Skip salt substitutes and anything with potassium chloride added." },
        { label = "Check your numbers", text = "Values are averages per serving — confirm limits with your dietitian." },
    }
}
