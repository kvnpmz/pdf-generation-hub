local tl = require('tl')
tl.loader() 

global render = require("templates.base.render")
local context = require("context")

local style_provider = require("style_provider")
global ApplyStyling = style_provider.Apply
global SetupDocument = context.Execute
