return {
    id = "grocery_master_list",
    output_name = "grocery_master_list",
    template = "grocery_master_list",
    header = "Master Grocery List",
    store_label = "Store",
    date_label = "Date",
    legend_tick = "tick as you shop",
    legend_star = "pantry staple",
    completed_date = "2026-11-09",
    columns = {
        { -- Column 1
            {
                title = "Fruits",
                items = { "apples", "apricots", "avocados", "bananas", "berries", "cherries", "grapefruit", "grapes", "kiwi", "lemons", "limes", "mango", "melons", "nectarines", "oranges", "papaya", "peaches", "pears", "plantains", "plums", "pomegranate", "watermelon" },
                staples = { "lemons" }
            },
            {
                title = "Vegetables",
                items = { "artichokes", "arugula", "asparagus", "beets", "broccoli", "cabbage", "cauliflower", "carrots", "celery", "chilies", "chives", "cilantro", "corn", "cucumbers", "eggplant", "garlic cloves", "green onions", "kale", "lettuce", "onions", "peppers", "potatoes", "salad greens", "spinach", "sprouts", "squash", "tomatoes", "zucchini", "brussels sprouts", "ginger" },
                staples = { "garlic cloves", "onions", "potatoes" }
            },
            {
                title = "Breakfast",
                items = { "breakfast shake", "cereal", "english muffins", "grits", "hash browns", "instant coffee", "maple syrup", "oatmeal", "pancake mix", "protein powder", "sausage patties" },
                staples = { "instant coffee", "oatmeal" }
            }
        },
        { -- Column 2
            {
                title = "Meat",
                items = { "bacon", "chicken", "deli meat", "ground beef", "ground turkey", "ham", "hot dogs", "pork", "rotisserie chicken", "sausage", "steak", "turkey" },
                staples = { "chicken" }
            },
            {
                title = "Seafood",
                items = { "catfish", "cod", "crab", "halibut", "lobster", "oysters", "salmon", "shrimp", "smoked salmon", "tilapia", "tuna" }
            },
            {
                title = "Frozen",
                items = { "chicken bites", "desserts", "edamame", "fish sticks", "fruit", "ice", "ice cream", "ice pops", "juice", "meat", "mixed vegetables", "pie shells", "pizza", "pot pies", "potatoes", "TV dinners", "vegetables", "veggie burger", "waffles" },
                staples = { "vegetables" }
            },
            {
                title = "Baby",
                items = { "baby cereal", "baby food", "baby shampoo", "diapers", "diaper cream", "formula", "wipes" }
            },
            {
                title = "Pets",
                items = { "cat food", "cat litter", "dog food", "pet shampoo", "treats", "flea treatment" }
            }
        },
        { -- Column 3
            {
                title = "Baking",
                items = { "baking powder", "baking soda", "bread crumbs", "cake decor", "cake mix", "canned milk", "chocolate chips", "cocoa", "cornmeal", "cornstarch", "flour", "food coloring", "frosting", "muffin mix", "pie crust", "shortening", "sugar (brown)", "sugar (powdered)", "sugar", "yeast" },
                staples = { "baking powder", "baking soda", "flour", "sugar" }
            },
            {
                title = "Snacks",
                items = { "candy", "cookies", "crackers", "dried fruit", "fruit snacks", "gelatin", "graham crackers", "granola bars", "gum", "nuts", "popcorn", "potato chips", "pretzels", "protein bars", "pudding", "raisins", "rice cakes", "seeds", "tortilla chips", "trail mix" },
                staples = { "nuts" }
            },
            {
                title = "Bakery",
                items = { "bagels", "bread", "croissants", "cookies", "croutons", "donuts", "dinner rolls", "hamburger buns", "hot dog buns", "muffins", "pastries", "pie", "pita bread", "tortillas (corn)", "tortillas (flour)" },
                staples = { "bread" }
            }
        },
        { -- Column 4
            {
                title = "Pasta/Rice",
                items = { "brown rice", "burger helper", "couscous", "egg noodles", "elbow macaroni", "lasagna", "mac & cheese", "noodle mix", "orzo", "quinoa", "rice mix", "rice noodles", "spaghetti", "white rice" },
                staples = { "spaghetti", "white rice" }
            },
            {
                title = "Cans & Jars",
                items = { "applesauce", "baked beans", "black beans", "broth", "boullion cubes", "canned fruit", "canned vegetables", "carrots", "chili", "corn", "creamed corn", "jam / jelly", "mushrooms", "olives (green)", "olives (black)", "canned pasta", "pasta sauce", "peanut butter", "pickles", "pie filling", "soup", "chickpeas", "coconut milk", "tomato paste", "refried beans" },
                staples = { "black beans", "broth", "canned vegetables", "pasta sauce", "peanut butter" }
            },
            {
                title = "Refrigerated",
                items = { "biscuits", "butter", "cheddar cheese", "cream / creamer ", "cream cheese", "dip", "eggs", "egg substitute", "feta cheese", "half & half", "jack cheese", "milk", "mozzarella", "processed cheese", "salsa", "sour cream", "swiss cheese", "whipped cream", "yogurt", "oat milk", "hummus", "greek yogurt" },
                staples = { "butter", "cheddar cheese", "eggs", "milk" }
            }
        },
        { -- Column 5
            {
                title = "Seasoning",
                items = { "basil", "bay leaves", "BBQ seasoning", "chili flakes", "cinnamon", "cloves", "cumin", "curry", "dill", "garlic powder", "garlic salt", "gravy mix", "Italian seasoning", "marinade", "meat tenderizer", "onion powder", "oregano", "paprika", "pepper", "poppy seed", "red pepper", "sage", "salt", "seasoned salt", "soup mix", "vanilla extract" },
                staples = { "chili flakes", "onion powder" }
            },
            {
                title = "Sauces & Condiments",
                items = { "BBQ sauce", "cocktail sauce", "cooking spray", "honey", "horseradish", "hot sauce", "ketchup", "lemon juice", "mayonnaise", "mustard", "olive oil", "pesto", "relish", "salad dressing", "salsa", "soy sauce", "sriracha", "steak sauce", "sweet & sour", "teriyaki", "vegetable oil", "vinegar" },
                staples = { "ketchup", "mayonnaise", "mustard", "olive oil" }
            },
            {
                title = "Drinks",
                items = { "beer", "champagne", "club soda", "coffee", "diet soft drinks", "energy drinks", "juice", "liquor", "soft drinks", "tea", "wine" },
                staples = { "coffee", "tea" }
            }
        },
        { -- Column 6
            {
                title = "Paper Products",
                items = { "aluminum foil", "coffee filters", "cups", "food storage bags", "garbage bags", "napkins", "paper plates", "paper towels", "plastic bags", "plastic cutlery", "plastic wrap", "straws", "waxed paper" },
                staples = { "garbage bags", "paper towels" }
            },
            {
                title = "Cleaning",
                items = { "air freshener", "bleach", "dish soap", "dishwasher pods", "floor cleaner", "glass spray", "laundry detergent", "fabric softener", "polish", "sponges", "vacuum bags", "disinfecting wipes" },
                staples = { "bleach", "dish soap", "laundry detergent" }
            },
            {
                title = "Personal Care",
                items = { "bath soap", "bug repellant", "family planning", "feminine products", "conditioner", "cotton swabs", "dental floss", "deodorant", "facial tissue", "hair spray", "hand soap", "lip care", "lotion", "makeup", "mouthwash", "razors/blades", "shampoo", "shaving cream", "sunscreen", "toilet tissue", "toothbrush", "toothpaste", "hand sanitizer" },
                staples = { "deodorant", "shampoo", "toilet tissue", "toothpaste" }
            },
            {
                title = "Misc. Items",
                items = { "bandages", "batteries", "charcoal", "first aid kit", "greeting cards", "light bulbs", "pain reliever" },
                staples = { "batteries" }
            }
        }
    }
}
