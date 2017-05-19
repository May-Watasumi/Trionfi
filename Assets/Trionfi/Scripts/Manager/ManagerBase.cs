using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Audio活動を管理する
namespace NovelEx
{
    public class FlipObject<T,Y> : SingletonMonoBehaviour<FlipObject<T,Y>>
    {
        public GameObject objectPrefab;

        public Y currentInstance;
        [SerializeField]
        public Y primaryInstance;
        [SerializeField]
        public Y secondaryInstance;

        public void Flip()
        {
            if (currentInstance.Equals(primaryInstance))
                currentInstance = secondaryInstance;
            else
                currentInstance = primaryInstance;
        }

        public List<T> tagedObject = new List<T>();

        public List<T> GetImageByTag(string tag)
        {
            tagedObject.Clear();
            foreach (KeyValuePair<string, T> abs in dicObject)
            {
                GameObject g = abs.Value as GameObject;
                if(g != null && g.tag == "tag") 
                    tagedObject.Add(abs.Value);
            }
            return tagedObject;
        }

        public Dictionary<string, T> dicObject = new Dictionary<string, T>();

        public virtual T Create(string name, TRObjectType type = TRObjectType.None)
        {
            if(!dicObject.ContainsKey(name))
                return GameObject.Instantiate(objectPrefab).GetComponent<T>();
            else
                return dicObject[name];
        }
/*
        public static void AddObject(T instance)
        {
            dicObject[instance.GetParam("name")] = instance;
        }
*/
        public T Find(string key)
        {
            return dicObject[key];
        }

        public void Remove(string key)
        {
            GameObject.Destroy((dicObject[key] as GameObject).gameObject);
            dicObject.Remove(key);
        }

        public void DestroyAll()
        {
            foreach (KeyValuePair<string, T> dat in dicObject)
            {
                GameObject.Destroy((dat.Value as GameObject).gameObject);
            }
            dicObject.Clear();
        }

        public void OnDestroy()
        {
            DestroyAll();
        }
    }
}