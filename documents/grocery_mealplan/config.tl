return {
    id = "grocery_mealplan",
    output_name = "grocery_mealplan",
    template = "grocery_mealplan",
    header = "Meal Plan & Grocery List",
    completed_date = "2026-11-11",
    week_label = "Week of",
    servings_label = "Servings",
    legend_tick = "tick as you shop",
    plan_title = "This Week's Meals",
    plan_note = "Plan the week, then shop the list",
    list_title = "Grocery List",
    list_note = "One line per item · qty on the right",
    days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" },
    meals = { "Breakfast", "Lunch", "Dinner" },
    columns = {
        { -- Column 1: Produce
            {
                title = "Produce",
                items = { "apples", "avocados", "bananas", "bell peppers", "broccoli", "carrots", "celery", "cucumbers", "garlic", "lemons", "lettuce", "onions", "potatoes", "spinach", "tomatoes" }
            }
        },
        { -- Column 2: Meat and seafood
            {
                title = "Meat & Seafood",
                items = { "bacon", "chicken breast", "chicken thighs", "cod", "ground beef", "ground turkey", "ham", "pork chops", "salmon", "sausage", "shrimp", "steak", "tuna", "turkey slices", "lamb" }
            }
        },
        { -- Column 3: Dairy, eggs and bakery
            {
                title = "Dairy & Bakery",
                items = { "butter", "cheddar cheese", "cream cheese", "eggs", "english muffins", "half & half", "milk", "mozzarella", "parmesan", "sour cream", "tortillas", "yogurt", "bread", "burger buns", "dinner rolls" }
            }
        },
        { -- Column 4: Pantry and freezer
            {
                title = "Pantry & Freezer",
                items = { "black beans", "broth", "canned tomatoes", "chickpeas", "coconut milk", "flour", "frozen berries", "frozen peas", "frozen spinach", "honey", "olive oil", "pasta", "peanut butter", "rice", "salsa" }
            }
        }
    },
    footer_budget = "Est. budget",
    footer_spent = "Total spent",
    footer_notes = "Notes"
}
