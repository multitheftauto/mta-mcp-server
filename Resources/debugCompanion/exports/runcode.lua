function httpRun(code)
	if not verifyApiKey() then
		return "Unauthorised"
	end

	local notReturned
	local commandFunction, errorMsg = loadstring("return " .. code)
	if errorMsg then
		notReturned = true
		commandFunction, errorMsg = loadstring(code)
	end

	if errorMsg then
		return "Error: " .. errorMsg
	end

	local results = { pcall(commandFunction) }
	if not results[1] then
		return "Error: " .. results[2]
	end

	if not notReturned then
		local resultsString = ""
		local first = true
		for i = 2, #results do
			if first then
				first = false
			else
				resultsString = resultsString .. ", "
			end
			local resultType = type(results[i])
			if isElement(results[i]) then
				resultType = "element:" .. getElementType(results[i])
			end
			resultsString = resultsString .. tostring(results[i]) .. " [" .. resultType .. "]"
		end
		
		return "Command results: " .. resultsString
	end

	return "Command executed!"
end