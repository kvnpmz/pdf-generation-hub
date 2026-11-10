return {
    id = "gout_food_list",
    output_name = "gout_food_list",
    template = "gout_food_list",
    header = "Gout Food Guide",
    subtitle = "What to eat freely, what to limit, and what to skip — with purines per 100 g",
    completed_date = "2026-11-10",

    targets = {
        { label = "Daily purines", value = "under 500 mg" },
        { label = "Urate target", value = "under 6 mg/dL" },
        { label = "Water", value = "8 cups a day" },
        { label = "In a flare", value = "no alcohol" }
    },

    columns = {
        {
            title = "Eat Freely",
            caption = "under 100 mg per 100 g",
            items = {
                { name = "Eggs", mg = "0 mg" },
                { name = "Milk", mg = "0 mg" },
                { name = "Greek yogurt", mg = "1 mg" },
                { name = "Cottage cheese", mg = "6 mg" },
                { name = "Cheddar cheese", mg = "6 mg" },
                { name = "Apples", mg = "1 mg" },
                { name = "Oranges", mg = "2 mg" },
                { name = "Bananas", mg = "3 mg" },
                { name = "Grapes", mg = "4 mg" },
                { name = "Cherries", mg = "2 mg" },
                { name = "Strawberries", mg = "2 mg" },
                { name = "Blueberries", mg = "2 mg" },
                { name = "Watermelon", mg = "3 mg" },
                { name = "Potatoes", mg = "16 mg" },
                { name = "Sweet potato", mg = "15 mg" },
                { name = "White rice", mg = "25 mg" },
                { name = "Pasta", mg = "20 mg" },
                { name = "White bread", mg = "14 mg" },
                { name = "Rolled oats", mg = "94 mg" },
                { name = "Corn", mg = "52 mg" },
                { name = "Carrots", mg = "17 mg" },
                { name = "Tomatoes", mg = "11 mg" },
                { name = "Cucumber", mg = "7 mg" },
                { name = "Lettuce", mg = "9 mg" },
                { name = "Cabbage", mg = "10 mg" },
                { name = "Cauliflower", mg = "51 mg" },
                { name = "Mushrooms", mg = "58 mg" },
                { name = "Spinach", mg = "57 mg" },
                { name = "Asparagus", mg = "23 mg" },
                { name = "Green beans", mg = "37 mg" },
                { name = "Green peas", mg = "84 mg" },
                { name = "Tofu", mg = "68 mg" },
                { name = "Chickpeas", mg = "56 mg" },
                { name = "Black beans", mg = "75 mg" },
                { name = "Almonds", mg = "31 mg" },
                { name = "Walnuts", mg = "25 mg" },
                { name = "Peanut butter", mg = "79 mg" },
                { name = "Olive oil", mg = "0 mg" },
                { name = "Coffee", mg = "0 mg" },
                { name = "Herbal tea", mg = "0 mg" }
            }
        },
        {
            title = "Limit",
            caption = "100–200 mg per 100 g",
            items = {
                { name = "Chicken breast", mg = "150 mg" },
                { name = "Chicken thigh", mg = "180 mg" },
                { name = "Turkey breast", mg = "150 mg" },
                { name = "Duck", mg = "138 mg" },
                { name = "Ground beef", mg = "110 mg" },
                { name = "Beef steak", mg = "133 mg" },
                { name = "Pork loin", mg = "122 mg" },
                { name = "Pork chop", mg = "145 mg" },
                { name = "Bacon", mg = "145 mg" },
                { name = "Ham", mg = "131 mg" },
                { name = "Lamb", mg = "182 mg" },
                { name = "Venison", mg = "138 mg" },
                { name = "Rabbit", mg = "132 mg" },
                { name = "Salmon", mg = "170 mg" },
                { name = "Tuna", mg = "142 mg" },
                { name = "Cod", mg = "109 mg" },
                { name = "Tilapia", mg = "110 mg" },
                { name = "Shrimp", mg = "147 mg" },
                { name = "Scallops", mg = "136 mg" },
                { name = "Squid", mg = "135 mg" },
                { name = "Crab", mg = "100 mg" },
                { name = "Lentils", mg = "127 mg" },
                { name = "Kidney beans", mg = "128 mg" },
                { name = "Soybeans", mg = "190 mg" },
                { name = "Sausage", mg = "160 mg" },
                { name = "Caviar", mg = "144 mg" }
            }
        },
        {
            title = "Avoid",
            caption = "over 200 mg per 100 g",
            items = {
                { name = "Beef liver", mg = "300 mg" },
                { name = "Chicken liver", mg = "243 mg" },
                { name = "Pork liver", mg = "289 mg" },
                { name = "Kidney", mg = "300 mg" },
                { name = "Heart", mg = "350 mg" },
                { name = "Sweetbreads", mg = "400 mg" },
                { name = "Tripe", mg = "300 mg" },
                { name = "Liver pâté", mg = "300 mg" },
                { name = "Foie gras", mg = "300 mg" },
                { name = "Sardines", mg = "345 mg" },
                { name = "Anchovies", mg = "239 mg" },
                { name = "Mackerel", mg = "250 mg" },
                { name = "Herring", mg = "210 mg" },
                { name = "Mussels", mg = "241 mg" },
                { name = "Dried fish", mg = "500 mg" },
                { name = "Anchovy paste", mg = "300 mg" },
                { name = "Shrimp paste", mg = "300 mg" },
                { name = "Fish sauce", mg = "400 mg" },
                { name = "Meat extract", mg = "500 mg" },
                { name = "Yeast extract", mg = "500 mg" },
                { name = "Bouillon cube", mg = "250 mg" },
                { name = "Gravy granules", mg = "300 mg" }
            }
        }
    },

    swaps = {
        { from = "Sardines", to = "Cod" },
        { from = "Liver", to = "Eggs" },
        { from = "Mussels", to = "Shrimp" },
        { from = "Beer", to = "Sparkling water" }
    },

    tally = {
        label = "Daily purine tally",
        cells = { "Breakfast", "Lunch", "Dinner", "Snacks" },
        target = "/ 500 mg"
    },

    note = "Purine values are approximate averages per 100 g and vary by cut, curing, and cooking method. Alcohol, sugary drinks, and dehydration can raise urate even when purines are low."
}
