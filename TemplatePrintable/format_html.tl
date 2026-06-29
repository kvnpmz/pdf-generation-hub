local tl = require("tl")
tl.loader()

local function read_file(path: string): string
    local f = assert(io.open(path, "r"))
    local content = f:read("*a")
    f:close()
    return content
end

local function format_html(html: string): string
    local out: {string} = {}
    local indent: integer = 0

    local function tabs(): string
        return string.rep("    ", indent)
    end

    local i: integer = 1
    local len: integer = #html

    while i <= len do
        local tag_start = html:find("<[^>]*>", i)

        if tag_start == i then
            local tag = html:match("^<[^>]*>", i)

            -- safeguard (Teal needs this because match can be nil)
            if tag == nil then
                break
            end

            if tag:match("^</") then
                indent = math.max(indent - 1, 0)
                table.insert(out, tabs() .. tag)
            else
                table.insert(out, tabs() .. tag)

                if not tag:match("/>$")
                    and not tag:match("^<br")
                    and not tag:match("^<hr")
                    and not tag:match("^<img")
                    and not tag:match("^<input")
                    and not tag:match("^<meta")
                    and not tag:match("^<link")
                    and not tag:match("^<!") then
                    indent = indent + 1
                end
            end

            i = i + #tag
        else
            local next_tag = html:find("<", i)
            local text = html:sub(i, (next_tag and next_tag - 1) or len)

            if text:match("^%s*$") == nil then
                table.insert(out, tabs() .. text)
            end

            i = next_tag or (len + 1)
        end
    end

    return table.concat(out, "\n")
end

-- read file
local input_path: string = "preview.html"
local html: string = read_file(input_path)

-- format
local formatted: string = format_html(html)

print(formatted)
