return {
    id = "carryon_packing",
    output_name = "carryon_packing_list",
    header = "Carry-On Packing List",
    subtitle = "One bag, no checked luggage · pack by trip length",
    template = "carryon_packing",
    completed_date = "2026-11-10",
    guide = {
        title = "Pack by trip length",
        limits = {
        { value = "22 × 14 × 9 in", label = "Carry-on size" },
        { value = "7 kg / 15 lb", label = "Weight limit" },
        { value = "1 quart bag", label = "Liquids" },
    },
    columns = { "Trip", "Tops", "Bottoms", "Underwear + socks", "Laundry" },
        rows = {
            { length = "3 days", tops = "3", bottoms = "2", socks = "3 + 1 spare", laundry = "—" },
            { length = "5 days", tops = "5", bottoms = "3", socks = "5 + 1 spare", laundry = "—" },
            { length = "7 days", tops = "5", bottoms = "3", socks = "7", laundry = "1 load" },
        },
        note = "One top per day, bottoms twice before a wash, and a single laundry load on the seven-day trip.",
    },
    limits = {
        { value = "22 × 14 × 9 in", label = "Carry-on size" },
        { value = "7 kg / 15 lb", label = "Weight limit" },
        { value = "1 quart bag", label = "Liquids" },
    },
    columns = {
        { -- Column 1: personal item
            {
                title = "Documents & money",
                items = { "Passport / ID", "Mobile boarding pass", "Wallet & cards", "Travel insurance", "Hotel address", "Emergency contact card" }
            },
            {
                title = "Medication & health",
                items = { "Prescriptions", "Pain reliever", "Allergy tablets", "Motion sickness pills", "Hand sanitizer", "Spare mask" }
            },
            {
                title = "Tech",
                items = { "Phone + charger", "Power bank", "Earbuds", "Plug adapter", "E-reader", "Camera" }
            },
            {
                title = "Comfort",
                items = { "Neck pillow", "Eye mask", "Refillable bottle", "Snacks", "Lip balm", "Pen" }
            }
        },
        { -- Column 2: carry-on clothing
            {
                title = "Clothing capsule",
                groups = {
                    { sub = "Tops", items = { "T-shirts", "Long-sleeve tee", "Button-down shirt", "Light sweater" } },
                    { sub = "Bottoms", items = { "Jeans", "Chinos / trousers", "Shorts", "Skirt" } },
                    { sub = "Underwear & socks", items = { "Underwear", "Socks", "Bras / base layer" } },
                    { sub = "Sleep & swim", items = { "Pajamas", "Swimsuit", "Flip flops" } },
                    { sub = "Shoes", items = { "Walking shoes", "Sandals", "Flats" } }
                }
            },
            {
                title = "Accessories & laundry",
                items = { "Belt", "Sunglasses", "Hat", "Scarf", "Jewelry roll", "Detergent sheets", "Dry bag" }
            }
        },
        { -- Column 3: toiletries, extras, what stays on you
            {
                title = "Toiletries (3-1-1)",
                items = { "Shampoo bar", "Conditioner bar", "Body wash", "Deodorant", "Toothbrush + paste", "Razor", "Moisturizer", "Sunscreen", "Hair ties" }
            },
            {
                title = "Extras",
                items = { "Packing cubes", "Laundry bag", "Umbrella", "Foldable tote", "Travel lock", "Mini first-aid kit" }
            },
            {
                title = "Worn on travel day",
                items = { "Jacket", "Walking shoes", "Jeans", "Heaviest layer", "Neck wallet" }
            },
            {
                title = "Before you zip",
                items = { "Bag within 22 × 14 × 9 in", "Liquids in one quart bag", "Chargers at the top", "One outfit at the top" }
            }
        }
    }
}
