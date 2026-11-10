return {
    id = "carryon_packing",
    output_name = "carryon_packing_list",
    header = "Carry-On Packing List",
    subtitle = "One bag, no checked luggage · pack by trip length",
    template = "carryon_packing",
    completed_date = "2026-11-10",
    guide = {
        title = "Pack by trip length",
        columns = { "Trip", "Tops", "Bottoms", "Underwear + socks", "Shoes" },
        rows = {
            { length = "3 days", tops = "3", bottoms = "2", socks = "4", shoes = "1" },
            { length = "5 days", tops = "5", bottoms = "3", socks = "6", shoes = "2" },
            { length = "7 days", tops = "5", bottoms = "3", socks = "7", shoes = "2" },
        },
    },
    columns = {
        { -- Column 1: personal item
            {
                title = "Documents & money",
                items = { "Passport / ID", "Boarding pass", "Wallet & cards", "Travel insurance", "Hotel address", "Emergency contact" }
            },
            {
                title = "Medicine",
                items = { "Prescriptions", "Pain reliever", "Allergy tablets", "Motion sickness pills", "Hand sanitizer" }
            },
            {
                title = "Tech",
                items = { "Phone + charger", "Power bank", "Earbuds", "Plug adapter", "E-reader", "Camera" }
            },
            {
                title = "Comfort",
                items = { "Neck pillow", "Eye mask", "Refillable bottle", "Snacks", "Lip balm" }
            }
        },
        { -- Column 2: carry-on clothing
            {
                title = "Clothing",
                groups = {
                    { sub = "Tops", items = { "T-shirts", "Long-sleeve tee", "Button-down shirt", "Light sweater" } },
                    { sub = "Bottoms", items = { "Jeans", "Chinos / trousers", "Shorts", "Skirt" } },
                    { sub = "Underwear & socks", items = { "Underwear", "Socks", "Bras / base layer" } },
                    { sub = "Sleep & swim", items = { "Pajamas", "Swimsuit" } },
                    { sub = "Shoes", items = { "Walking shoes", "Sandals" } }
                }
            },
            {
                title = "Accessories",
                items = { "Belt", "Sunglasses", "Hat", "Scarf", "Umbrella", "Foldable tote" }
            }
        },
        { -- Column 3: toiletries, extras, what stays on you
            {
                title = "Toiletries (3-1-1)",
                items = { "Shampoo bar", "Conditioner bar", "Body wash", "Deodorant", "Toothbrush + paste", "Razor", "Moisturizer", "Sunscreen" }
            },
            {
                title = "Extras",
                items = { "Packing cubes", "Laundry bag", "Travel lock", "Mini first-aid kit", "Detergent sheets", "Dry bag" }
            },
            {
                title = "Wear on plane",
                items = { "Jacket", "Walking shoes", "Jeans", "Heaviest layer" }
            },
            {
                title = "Before you zip",
                items = { "Bag 22 × 14 × 9 in", "Liquids in 1 qt bag", "Chargers at the top", "One outfit at the top" }
            }
        }
    }
}
