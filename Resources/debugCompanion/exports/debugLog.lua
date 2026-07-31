local debugLogCache = {}
local MAX_DEBUG_LOG = 100

local function addToDebugCache(entry)
	table.insert(debugLogCache, entry)
	if #debugLogCache > MAX_DEBUG_LOG then
		table.remove(debugLogCache, 1)
	end
end

local function onDebugMessageHandler(debugMessage, debugLevel, debugFile, debugLine, debugRed, debugGreen, debugBlue)
	local entry = {
		message = tostring(debugMessage),
		level = tonumber(debugLevel) or 0,
		file = debugFile or false,
		line = debugLine or false,
		color = { debugRed or 255, debugGreen or 255, debugBlue or 255 },
		time = os.time()
	}

	addToDebugCache(entry)

	return false
end

addEventHandler("onDebugMessage", root, onDebugMessageHandler)

function httpGetDebugLog()
	if not verifyApiKey() then
		return "Unauthorised"
	end
	
	return toJSON(debugLogCache)
end

