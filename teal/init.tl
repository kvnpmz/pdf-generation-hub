local generator = require("generator")
local contracts = require("contracts")

local M = {}

function M.Render(config: contracts.Config): contracts.RenderResult
    return generator.Render(config)
end

return M
