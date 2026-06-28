local tl = require('tl')
tl.loader() 

local render = require("templates.base.render")
local context = require("context")
local style_provider = require("style_provider")

global GenerateBase = render.GenerateDocument
global ApplyStyling = style_provider.Apply
global SetupDocument = context.Execute
