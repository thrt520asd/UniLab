---@class ObjectPool
ObjectPool = {}
ObjectPool.__index=ObjectPool

---@return ObjectPool
function ObjectPool.New(createFunc , keyfunc,restoreFunc,disposeFunc)
    local o =
    {
        CreateFunc = createFunc,
        DisposeFunc = disposeFunc,
        RestoreFunc = restoreFunc,
        KeyFunc = keyfunc,

        AvailableObjects = {}
    }
    setmetatable(o,ObjectPool)
    return o
end

function ObjectPool:Create(...)
    local list
    if self.KeyFunc then
        local key = self.KeyFunc(...)
        list = self.AvailableObjects[key]
        if not list then
            list = {}
            self.AvailableObjects[key] = list
        end
    else
        list = self.AvailableObjects
    end
    --local list = self.KeyFunc and  self.AvailableObjects[self.KeyFunc(...)] or self.AvailableObjects
    if #list > 0 then
        local item = list[1]
        table.remove(list , 1)
        return  item
    end
    return self.CreateFunc(...)
end


function ObjectPool:Return(obj , ...)
    if self.RestoreFunc then
        self.RestoreFunc(obj)
    end
    if self.KeyFunc then
        local key = self.KeyFunc(...)
        local list = self.KeyFunc and self.AvailableObjects[self.KeyFunc(...)] or {}
        table.insert(list , obj)
        self.AvailableObjects[key] = list
    else
        table.insert(self.AvailableObjects , obj)
    end

end

---销毁对象池中现有的所有空闲对象
function ObjectPool:Clear()
    if self.DisposeFunc then
        local disposeFunc=self.DisposeFunc
        for k,list in pairs(self.AvailableObjects) do
            for i, v in ipairs(list) do
                disposeFunc(v)
            end
        end
    end
    self.AvailableObjects = {}
end

