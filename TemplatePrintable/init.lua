local render = require("templates.base.render")
local context = require("context")
local style_provider = require("style_provider")

return {
   GenerateBase = render.GenerateDocument,
   ApplyStyling = style_provider.Apply,
   SetupDocument = context.Execute,
}
