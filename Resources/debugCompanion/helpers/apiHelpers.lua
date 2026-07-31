local apiKey

function verifyApiKey()
    if not apiKey or not requestHeaders then
        return false
    end

    local header = requestHeaders["api-key"]

    return header == apiKey
end

function loadApiKey()
    local setting = get("@apiKey")
    
    if setting then
        apiKey = setting

        if apiKey == "default" then
            outputServerLog("Default API key detected, you can change it with /generatekey")
            addCommandHandler("generatekey", generateApiKey, false, false)
        end
    end
end

function generateApiKey()
    local setting = get("@apiKey")
    
    if setting == "default" then
        local key = generateRandomString(32)
        set("@apiKey", key)
        apiKey = key

        outputServerLog("A new key was generated: " .. key)
    end
end

function generateRandomString(length)
    local charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    local randomString = ""
    for i = 1, length do
        local randomIndex = math.random(1, #charset)
        randomString = randomString .. charset:sub(randomIndex, randomIndex)
    end
    return randomString
end

loadApiKey()
