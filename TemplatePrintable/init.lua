local tl = require('tl')
tl.loader()

render = require("templates.base.render")
local context = require("context")

local style_provider = require("style_provider")
ApplyStyling = style_provider.Apply
SetupDocument = context.Execute
