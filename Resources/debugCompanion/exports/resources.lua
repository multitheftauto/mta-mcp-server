function httpStartResource(name)
	if not verifyApiKey() then
		return "Unauthorised"
	end

	return toJSON({ result = startResource(getResourceFromName(name)) })
end

function httpRestartResource(name)
	if not verifyApiKey() then
		return "Unauthorised"
	end

	return toJSON({ result = restartResource(getResourceFromName(name)) })
end

function httpStopResource(name)
	if not verifyApiKey() then
		return "Unauthorised"
	end
    
	return toJSON({ result = stopResource(getResourceFromName(name)) })
end
