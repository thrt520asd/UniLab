using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaSerializerTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic["a"] = "aaaa";
        dic["b"] = "bbbb";
        dic["c"] = "cccc";
        string lua = LuaSerializer.Serialize(dic);
        Debug.Log(lua);
        List<ushort> sList = new List<ushort>(){1 , 2,  0,3 , 4};
        string sLua = LuaSerializer.Serialize(sList);
        Debug.Log(sLua);
        MyClass myClass = new MyClass("myclass", 20);
        Debug.Log(LuaSerializer.Serialize(myClass));
    }

    class MyClass
    {
        public string name;
        public ulong age;
        [SerializeField] private MyClass2 class2;

        public MyClass(string name , ulong age)
        {
            this.name = name;
            this.age = age;
//            class2 = new MyClass2();
        }

    }

    class MyClass2
    {
        public string name;
        public ulong age;
    }
	
}
