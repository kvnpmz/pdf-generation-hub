#!/usr/bin/env lua

print("before")

--local status = os.execute("./lua_modules/bin/tl gen main.tl")
local status1 = os.execute("./lua_modules/bin/tl gen document.tl")
local status2 = os.execute("./lua_modules/bin/tl gen checklist_data.tl")

if status1 and status2 then
    print("after")
else
    print("An error occurred during execution.")
    os.exit(1)
end
