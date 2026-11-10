# Keyword families shared by the sales / coverage reports.
#
# First match wins. Order matters: a title that mentions several families
# belongs to the most specific one (glp-1 before heart, cleaning before
# grocery/budget, etc). Add new families here once and both reports agree.

import re

RULES = [
    ("kidney / renal", ["kidney", "renal", "ckd", "potassium", "dialysis"]),
    ("diverticulitis", ["diverticul"]),
    ("fatty liver", ["fatty liver", "fattyliver", "nafld", "nash", "steatosis", "liver"]),
    ("gallbladder", ["gallbladder", "gallstone", "cholecystitis"]),
    ("low carb / keto", ["low carb", "lowcarb", "keto", "atkins", "carnivore"]),
    ("folic acid", ["folic"]),
    ("blood sugar", ["blood sugar", "bloodsugar", "glucose", "diabet"]),
    ("mediterranean", ["mediterranean"]),
    ("glp-1", ["glp-1", "glp1", "glp 1"]),
    ("kids / adhd", ["adhd", "brain food"]),
    ("baby", ["baby", "newborn", "infant", "nursery", "first foods"]),
    ("wedding", ["wedding", "bridal", "bride"]),
    ("heart health", ["heart"]),
    ("high protein", ["protein"]),
    ("emergency / survival", ["emergency", "survival", "72-hour", "72 hour", "72hour"]),
    ("cleaning / upkeep", ["clean", "maintenance", "repair"]),
    ("grocery / meal", ["grocery", "grocer", "meal plan", "pantry", "food list", "foodlist", "stocktake"]),
    ("budget / finance", ["budget", "finance", "expense", "paycheck", "bill", "saving", "investment", "debt", "monthly"]),
    ("home / move", ["apartment", "dorm", "college", "moving", "move-in", "move in", "movein", "new home", "first home", "household", "house"]),
    ("travel", ["travel", "packing", "vacation"]),
    ("planner / productivity", ["planner", "schedule", "time block", "productivity", "weekly plan"]),
]

# Dev fixtures that are not products.
EXCLUDED_DOCS = {"example_checklist", "grid_demo", "layout_probe"}


def family_of(text: str) -> str:
    haystack = text.lower().replace("_", " ")
    haystack = re.sub(r"\s+", " ", haystack)
    for family, keywords in RULES:
        for keyword in keywords:
            if keyword in haystack:
                return family
    return "other"
