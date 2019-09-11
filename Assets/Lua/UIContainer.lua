local UIContainer = class("UIContainer")
local M = UIContainer
local this = M
this.updateFunc = nil
this.createFunc = nil
this.disposeFunc = nil
this.restoreFunc = nil
this.onItems = nil
this.offItems = nil
this.onItemDic = nil

function M:ctor( createFunc , updateFunc , restoreFunc , disposeFunc)
    self.createFunc = createFunc
    self.updateFunc = updateFunc
    self.restoreFunc = restoreFunc
    self.disposeFunc = disposeFunc
    self.onItems = {}
    self.offItems = {}
    self.onItemDic = {}
end

function M:Update(dataList)
    for i, v in ipairs(self.onItems) do
        self.restoreFunc(v)
        table.insert(self.offItems , v)
    end
    self.onItems = {}
    self.onItemDic = {}
    for i = 1, #dataList do
        local item
        if #self.offItems > 0 then
            item = self.offItems[1]
            table.remove(self.offItems , 1)
        else
            item = self.createFunc()
            if not item then
                log.fatal("self.createFun return nil ")
            end
        end
        self.updateFunc(dataList[i] , item)
        table.insert(self.onItems , item)
        self.onItemDic[dataList[i]] = item
    end
end

function M:Clear()
    if self.disposeFunc then
        for i, v in pairs(self.onItems) do
            self.disposeFunc(v)
        end
        for i, v in pairs(self.offItems) do
            self.disposeFunc(v)
        end
    end
    self.onItems = {}
    self.offItems = {}
end

function M:Dispose()
    if self.disposeFunc then
        for i, v in pairs(self.onItems) do
            self.disposeFunc(v)
        end
        for i, v in pairs(self.offItems) do
            self.disposeFunc(v)
        end
    end
    self.onItems = {}
    self.offItems = {}
    self.disposeFunc = nil
    self.restoreFunc = nil
    self.createFunc = nil
    self.updateFunc = nil
end

function M:GetItem(data)
    return self.onItemDic[data]
end

return UIContainer