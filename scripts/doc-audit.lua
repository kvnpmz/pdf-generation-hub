-- doc-audit.lua - structure audit for one document config.
--
-- Why: data edits (moving items between sections, cloning a variant, renaming
-- fields) fail silently when an entry is duplicated, emptied or orphaned. This
-- walks the merged config table and reports those mechanically, with no
-- knowledge of any particular template.
--
-- Called by scripts/doc-audit.sh with compiled Lua tables:
--   lua doc-audit.lua <tmpdir> <slug> <root> <base> ... <slug>
--
-- Rules:
--   - id / template / output_name describe the render; missing or malformed
--     required fields fail, inherited values are reported as INFO.
--   - duplicate identifier inside one array is a FAIL
--   - identifier reused across different arrays is INFO (often legitimate)
--   - duplicate scalar inside one array is a WARN
--   - empty string is a WARN; an intentionally empty list is INFO
--   - counts are printed for every array of records with an identifier
--
-- Exit code: 1 if any FAIL, 0 otherwise (WARN does not fail).

local tmpdir = assert(arg[1], "missing tmpdir")
local slug = assert(arg[2], "missing slug")
local root = assert(arg[3], "missing root")

local failures = {}
local warnings = {}
local infos = {}
local counts = {}

local function fail(message) table.insert(failures, message) end
local function warn(message) table.insert(warnings, message) end
local function info(message) table.insert(infos, message) end

local function join(a, b)
    if a == "" then return b end
    return a .. "." .. b
end

local function indexPath(path, index)
    return string.format("%s[%d]", path, index)
end

local function trim(value)
    return (value:gsub("^%s+", ""):gsub("%s+$", ""))
end

local function normalise(value)
    return trim(value):lower():gsub("%s+", " ")
end

-- An array is a table whose keys are exactly 1..n.
local function arrayLength(value)
    local count = 0
    for key in pairs(value) do
        if type(key) ~= "number" or key < 1 or key % 1 ~= 0 then
            return nil
        end
        count = count + 1
    end
    for index = 1, count do
        if value[index] == nil then return nil end
    end
    return count
end

local function fileExists(path)
    local handle = io.open(path, "r")
    if not handle then return false end
    handle:close()
    return true
end

-- Load and shallow-merge the extends chain: base first, target last.
local function loadMerged(chain)
    local merged = {}
    for _, name in ipairs(chain) do
        local chunk = assert(loadfile(tmpdir .. "/" .. name .. ".lua"))
        local config = chunk()
        if type(config) ~= "table" then
            fail(string.format("%s/config.tl did not return a table", name))
        else
            for key, value in pairs(config) do
                merged[key] = value
            end
        end
    end
    return merged
end

local chain = {}
for index = 4, #arg do table.insert(chain, arg[index]) end
if #chain == 0 then chain = { slug } end

local config = loadMerged(chain)
local target = assert(loadfile(tmpdir .. "/" .. slug .. ".lua"))()

-- ---------- identity ----------

local inheritedFields = {}

if type(config.id) ~= "string" or trim(config.id) == "" then
    fail("id is missing")
elseif config.id ~= slug then
    if type(target.extends) == "string" then
        inheritedFields[#inheritedFields + 1] = string.format("id %q inherited from %s", config.id, target.extends)
    else
        warn(string.format("id %q does not match folder %q", config.id, slug))
    end
end

if type(config.template) ~= "string" or trim(config.template) == "" then
    fail("template is missing")
else
    local directory = root .. "/templates/" .. config.template
    if not fileExists(directory .. "/render.tl") and not fileExists(directory .. "/render.cs") then
        fail(string.format("template %q has neither render.tl nor render.cs", config.template))
    end
    local style = type(config.style) == "string" and config.style or "style"
    if not fileExists(directory .. "/" .. style .. ".css") then
        warn(string.format("stylesheet %s.css is missing for template %q", style, config.template))
    end
    if type(config.overrides) == "string" and trim(config.overrides) ~= "" then
        if not fileExists(directory .. "/" .. config.overrides .. ".css") then
            fail(string.format("overrides %s.css is missing for template %q", config.overrides, config.template))
        end
    end
end

if type(config.output_name) ~= "string" or trim(config.output_name) == "" then
    warn("output_name is missing; the renderer will fall back to id")
elseif config.output_name:find("[^%w_%-]") then
    fail(string.format("output_name %q contains characters that are unsafe in a file name", config.output_name))
end

if config.completed_date == nil then
    info("completed_date is missing")
elseif type(config.completed_date) ~= "string" or not config.completed_date:match("^%d%d%d%d%-%d%d%-%d%d$") then
    fail(string.format("completed_date %q is not YYYY-MM-DD", tostring(config.completed_date)))
end

for _, message in ipairs(inheritedFields) do info(message) end

-- ---------- walk ----------

local identifierFields = { "name", "title", "label", "key", "id" }
-- Only these fields are identities: repeated title/label is often a layout
-- spacer or a reused section heading, so it is reported but does not fail.
local strictFields = { name = true, key = true, id = true }
local seenTables = {}
local seenNames = {}
local listCount = 0
local entryCount = 0

local function pickIdentifier(rows, length)
    for _, field in ipairs(identifierFields) do
        local ok = true
        for index = 1, length do
            if type(rows[index][field]) ~= "string" then
                ok = false
                break
            end
        end
        if ok then return field end
    end
    return nil
end

local function auditNameList(rows, length, path, field)
    listCount = listCount + 1
    entryCount = entryCount + length
    counts[#counts + 1] = { path = path, length = length, field = field }

    local seen = {}
    for index = 1, length do
        local name = rows[index][field]
        local key = normalise(name)

        if key ~= "" then
            if seen[key] then
                local message = string.format(
                    'duplicate %s "%s" in %s (entries %d and %d)',
                    field, name, path, seen[key], index
                )
                if strictFields[field] then
                    fail(message)
                else
                    info(message .. " — labels may repeat")
                end
            else
                seen[key] = index
            end

            if strictFields[field] then
                local registryKey = field .. ":" .. key
                seenNames[registryKey] = seenNames[registryKey] or { name = name, lists = {} }
                seenNames[registryKey].lists[path] = true
            end
        end
    end
end

local function auditScalarList(rows, length, path)
    local seen = {}
    for index = 1, length do
        local value = rows[index]
        if type(value) == "string" then
            local key = trim(value)
            if seen[key] then
                warn(string.format(
                    'repeated value "%s" in %s (entries %d and %d)',
                    value, path, seen[key], index
                ))
            else
                seen[key] = index
            end
        end
    end
end

local walk

walk = function(node, path)
    if type(node) ~= "table" then
        if type(node) == "string" and trim(node) == "" then
            warn("empty string at " .. (path == "" and "<root>" or path))
        end
        return
    end

    if seenTables[node] then return end
    seenTables[node] = true

    local length = arrayLength(node)
    if length ~= nil then
        if length == 0 then
            info("empty list at " .. (path == "" and "<root>" or path))
            return
        end

        local tables = 0
        for index = 1, length do
            if type(node[index]) == "table" then tables = tables + 1 end
        end

        if tables == length then
            local field = pickIdentifier(node, length)
            if field then
                auditNameList(node, length, path, field)
            end
        elseif tables == 0 then
            auditScalarList(node, length, path)
        end

        for index = 1, length do
            walk(node[index], indexPath(path, index))
        end
        return
    end

    local keys = {}
    for key in pairs(node) do keys[#keys + 1] = key end
    table.sort(keys, function(a, b) return tostring(a) < tostring(b) end)

    for _, key in ipairs(keys) do
        walk(node[key], join(path, tostring(key)))
    end
end

walk(config, "")

-- ---------- cross-list reuse ----------

local reused = 0
local registryKeys = {}
for key in pairs(seenNames) do registryKeys[#registryKeys + 1] = key end
table.sort(registryKeys)

for _, key in ipairs(registryKeys) do
    local registry = seenNames[key]
    local paths = {}
    for path in pairs(registry.lists) do paths[#paths + 1] = path end
    table.sort(paths)
    if #paths > 1 then
        reused = reused + 1
        if reused <= 8 then
            info(string.format(
                '"%s" appears in %d lists: %s',
                registry.name, #paths, table.concat(paths, ", ")
            ))
        end
    end
end
if reused > 8 then
    info(string.format("%d more names reused across lists not shown", reused - 8))
end

-- ---------- report ----------

print("doc-audit: " .. slug)
print("  chain   " .. table.concat(chain, " -> "))

if listCount > 0 then
    print(string.format("  lists   %d lists / %d entries", listCount, entryCount))
    local shown = {}
    for index = 1, math.min(#counts, 8) do
        shown[#shown + 1] = string.format("%s %d", counts[index].path, counts[index].length)
    end
    local line = table.concat(shown, ", ")
    if #counts > 8 then
        line = line .. string.format(", +%d more", #counts - 8)
    end
    print("  counts  " .. line)
end

for _, message in ipairs(infos) do print("  INFO    " .. message) end
for _, message in ipairs(warnings) do print("  WARN    " .. message) end
for _, message in ipairs(failures) do print("  FAIL    " .. message) end

local result = "PASS"
if #failures > 0 then
    result = "FAIL"
elseif #warnings > 0 then
    result = "WARN"
end

print("  result  " .. result)
os.exit(result == "FAIL" and 1 or 0)
