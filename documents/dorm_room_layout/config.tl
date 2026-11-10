return {
    id = "dorm_room_layout",
    output_name = "dorm_room_layout",
    template = "dorm_room_layout",
    completed_date = "2026-11-10",

    kicker = "College move-in · room planning",
    header = "Dorm Room Layout Planner",
    subtitle = "Measure the room, place the furniture, then buy only what fits",
    plan_card_title = "My room",
    plan_card_fields = { "Building / room", "Move-in date" },

    measure_title = "Measure the room",
    measure_aside = "Feet & inches · measure twice",
    measure_fields = {
        { label = "Room width", unit = "ft" },
        { label = "Room length", unit = "ft" },
        { label = "Ceiling height", unit = "ft" },
        { label = "Door width", unit = "in" },
        { label = "Window width", unit = "in" }
    },

    plan_title = "Plan to scale",
    plan_aside = "1 square = 1 foot",

    furniture_title = "Furniture footprints",
    furniture_note = "Draw the bed first — a twin XL is 3.3 × 6.7 squares. Everything else fits around it.",
    furniture = {
        { name = "Twin XL bed", size = "39 × 80 in", squares = "3.3 × 6.7" },
        { name = "Desk", size = "42 × 24 in", squares = "3.5 × 2" },
        { name = "Desk chair", size = "24 × 24 in", squares = "2 × 2" },
        { name = "Dresser", size = "30 × 16 in", squares = "2.5 × 1.3" },
        { name = "Mini fridge", size = "20 × 20 in", squares = "1.7 × 1.7" },
        { name = "Bookshelf", size = "24 × 12 in", squares = "2 × 1" },
        { name = "Nightstand", size = "18 × 18 in", squares = "1.5 × 1.5" },
        { name = "Storage bin", size = "24 × 18 in", squares = "2 × 1.5" },
        { name = "Area rug", size = "60 × 84 in", squares = "5 × 7" }
    },

    features_title = "Fixed features",
    features = { "Door swing", "Closet", "Outlets", "Window", "Radiator / AC", "Lofted bed" },

    clearance_title = "Clearance rules",
    clearances = {
        { size = "36 in", text = "clear at the foot of the bed" },
        { size = "30 in", text = "between pieces you walk between" },
        { size = "24 in", text = "in front of the closet" }
    },

    buy_title = "Buy only what fits",
    buy_items = {
        "Bed risers",
        "Under-bed bins",
        "Over-door rack",
        "Power strip, 6 ft",
        "Clip-on desk lamp",
        "Storage cart",
        "Rug, 5 × 7",
        "Desk fan"
    },

    notes_title = "Wall notes",
    fit_title = "Fit checks",
    fit_aside = "Tick before move-in",
    fit_checks = {
        "Bed and desk fit without blocking the closet",
        "Desk chair pulls out without hitting the bed",
        "Mini fridge has 2 in of air on every side",
        "Bed risers fit under the frame",
        "Power strip reaches the nearest outlet",
        "Rug clears the door swing",
        "Walkway to the door stays clear",
        "Your side has room for laundry and storage",
        "Storage fits under the bed",
        "Closet door clears the dresser"
    },
    fit_note = "Mark doors, windows and outlets on the grid before you place anything.",

    split_title = "Shared buys",
    split_aside = "Who brings what",
    split_first_column = "Shared item",
    split_columns = { "You", "Roommate" },
    split_rows = {
        "Mini fridge",
        "Microwave",
        "Area rug",
        "TV / speaker",
        "Cleaning supplies",
        "Snacks and dishes"
    },

    recheck_title = "Re-check at move-in",
    recheck_aside = "Compare the plan to the room",
    recheck_items = {
        "Room width",
        "Door swing",
        "Outlets",
        "Bed height",
        "Closet depth"
    },
    recheck_note = "Adjust the grid before you unpack."
}
