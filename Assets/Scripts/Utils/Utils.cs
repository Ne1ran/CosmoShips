using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Utils
{
    [PublicAPI]
    public static class Utils
    {
        #region Base

        public static object? Get(this IReadOnlyDictionary<string, object> dictionary, string key)
        {
            dictionary.TryGetValue(key, out object? result);
            return result;
        }

        public static object Require(this IReadOnlyDictionary<string, object> dictionary, string key)
        {
            object? result = Get(dictionary, key);

            if (result == null) {
                throw new NullReferenceException($"Entry with key '{key}' not found in dictionary");
            }

            return result;
        }

        public static T Require<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            try {
                return collection.First(predicate);
            } catch (InvalidOperationException) {
                throw new NullReferenceException($"Trying to require an item from collection, but it doesn't exist!");
            }
        }

        public static T Require<T>(this IReadOnlyDictionary<string, T> dictionary, string key)
        {
            if (!dictionary.TryGetValue(key, out T? result)) {
                throw new NullReferenceException($"Entry with key {key} not found in dictionary");
            }

            return result;
        }

        public static TValue Require<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.TryGetValue(key, out TValue? result)) {
                throw new NullReferenceException($"Entry with key {key} not found in dictionary");
            }

            return result!;
        }
        
        #endregion

        #region GameObject

        public static T RequireComponentInChild<T>(this GameObject gameObject, string childName)
                where T : Component
        {
            return gameObject.transform.RequireComponentInChild<T>(childName);
        }

        public static T? GetComponentInChildrenOnly<T>(this GameObject gameObject)
                where T : Component
        {
            T? component = null;
            foreach (Transform child in gameObject.transform) {
                component = child.GetComponent<T>();
                if (component != null) {
                    break;
                } else {
                    component = GetComponentInChildrenOnly<T>(child.gameObject);
                    break;
                }
            }
            return component != null ? component : null;
        }

        public static T RequireComponentInChildrenOnly<T>(this GameObject gameObject)
                where T : Component
        {
            foreach (Transform child in gameObject.transform) {
                T found = child.GetComponentInChildren<T>();

                if (found) {
                    return found!;
                }
            }

            throw new NullReferenceException($"Component {typeof(T).Name} not found in children of {gameObject.name}");
        }

        public static T RequireComponentInChildren<T>(this GameObject gameObject, bool includeInactive = true)
        {
            return gameObject.GetComponentInChildren<T>(includeInactive)
                   ?? throw new NullReferenceException($"Component {typeof(T).Name} not found in children of object {gameObject.name}");
        }

        public static Component RequireComponent(this GameObject gameObject, Type componentType)
        {
            return gameObject.GetComponent(componentType)
                   ?? throw new NullReferenceException($"Component {componentType.Name} not found in object {gameObject.name}");
        }

        public static T RequireComponent<T>(this GameObject gameObject)
                where T : class
        {
            return (RequireComponent(gameObject, typeof(T)) as T)!;
        }

        public static void DestroyObject(this GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }

        #endregion

        #region Transform

        public static Transform? GetChildRecursive(this Transform transform, string name)
        {
            foreach (Transform child in transform) {
                if (child.name == name) {
                    return child;
                }

                Transform? found = GetChildRecursive(child, name);
                if (found != null) {
                    return found;
                }
            }

            return null;
        }

        public static Transform RequireChildRecursive(this Transform transform, string name)
        {
            return GetChildRecursive(transform, name) ?? throw new NullReferenceException();
        }

        public static T? GetComponentInChild<T>(this Transform transform, string childName, bool includeInactive = true)
                where T : Component
        {
            foreach (Transform child in transform) {
                if (!includeInactive && !child.gameObject.activeSelf) {
                    continue;
                }

                if (child.name == childName) {
                    return child.GetComponent<T>();
                }

                T found = GetComponentInChild<T>(child, childName, includeInactive);
                if (found) {
                    return found;
                }
            }

            return null;
        }

        public static T? GetComponentInChildrenNotRecursive<T>(this Transform transform)
                where T : Component
        {
            foreach (Transform child in transform) {
                T component = child.GetComponent<T>();

                if (component) {
                    return component;
                }
            }

            return null;
        }

        public static T? GetComponentInChildrenNotRecursive<T>(this Transform transform, string childName)
                where T : Component
        {
            foreach (Transform child in transform) {
                if (child.name == childName) {
                    return child.GetComponent<T>();
                }
            }

            return null;
        }

        public static List<T> GetComponentsInChildrenNotRecursive<T>(this Transform transform)
                where T : Component
        {
            List<T> result = new();
            foreach (Transform child in transform) {
                T component = child.GetComponent<T>();

                if (component) {
                    result.Add(component);
                }
            }

            return result;
        }

        public static RectTransform ToRect(this Transform transform)
        {
            return (RectTransform) transform;
        }

        public static void AddChild(this Transform transform, Transform child)
        {
            child.SetParent(transform, false);
        }

        public static void AddChild(this Transform transform, Component child)
        {
            child.transform.SetParent(transform, false);
        }

        public static void AddChild(this Transform transform, GameObject child)
        {
            child.transform.SetParent(transform, false);
        }

        public static void RemoveChild(this Transform transform, Component child)
        {
            child.transform.SetParent(null, false);
        }

        public static void ClearChildren(this Transform transform)
        {
            foreach (Transform child in transform) {
                Object.Destroy(child.gameObject);
            }
        }

        #endregion

        #region Component

        public static T? GetComponentInChildrenOnly<T>(this Component component)
                where T : Component
        {
            return GetComponentInChildrenOnly<T>(component.gameObject);
        }

        public static T RequireComponentInChildrenOnly<T>(this Component component)
                where T : Component
        {
            return RequireComponentInChildrenOnly<T>(component.gameObject);
        }

        public static T RequireComponentInChild<T>(this Component component, string childName)
                where T : Component
        {
            return GetComponentInChild<T>(component, childName)
                   ?? throw new NullReferenceException($"Component {typeof(T).Name} not found on child {childName} of object {component.name}");
        }

        public static T? GetComponentInChild<T>(this Component component, string childName)
                where T : Component
        {
            return GetComponentInChild<T>(component.transform, childName);
        }

        public static List<T> GetComponentsInAllChild<T>(this Component component, string childName)
                where T : Component
        {
            List<T> components = component.GetComponentsInChildren<T>().ToList();
            return components.FindAll(comp => comp.name == childName);
        }

        public static T? GetComponentInSiblings<T>(this Component component)
                where T : Component
        {
            if (component.transform.parent == null) {
                return null;
            }

            foreach (Transform child in component.transform.parent) {
                if (ReferenceEquals(child, component.transform)) {
                    continue;
                }

                T found = child.GetComponent<T>();

                if (found != null) {
                    return found;
                }
            }

            return null;
        }

        public static T RequireComponentInSiblings<T>(this Component component)
                where T : Component
        {
            return GetComponentInSiblings<T>(component)
                   ?? throw new NullReferenceException($"Component {typeof(T).Name} not found in siblings of object {component.name}");
        }

        public static T GetOrAddComponent<T>(this Component component)
                where T : Component
        {
            return component.GetComponent<T>() ?? component.gameObject.AddComponent<T>();
        }

        public static T RequireComponent<T>(this Component component)
                where T : class
        {
            return RequireComponent<T>(component.gameObject);
        }

        public static T RequireComponentInChildren<T>(this Component component, bool includeInactive = true)
        {
            return component.GetComponentInChildren<T>(includeInactive)
                   ?? throw new NullReferenceException($"Component {typeof(T).Name} not found in children of object {component.name}");
        }

        public static T RequireComponentInChildren<T>(this Component component, string name, bool includeInactive = true)
                where T : Component
        {
            T[] children = component.GetComponentsInChildren<T>(includeInactive);
            for (int i = 0; i < children.Length; i++) {
                T c = children[i];
                if (c.name != name) {
                    continue;
                }

                return c;
            }

            throw new NullReferenceException($"Object not found in children with name ={component.name}");
        }

        public static T RequireComponentInParent<T>(this Component component)
                where T : class
        {
            Transform? parent = component.transform.parent;

            if (!parent) {
                throw new NullReferenceException($"Object {component.name} don't has parent");
            }

            return parent.RequireComponent<T>();
        }

        public static Transform RequireChildRecursive(this Component component, string name)
        {
            return component.transform.GetChildRecursive(name)
                   ?? throw new NullReferenceException($"Child '{name}' not found in object '{component.name}'");
        }

        public static RectTransform RectTransform(this Component component)
        {
            return component.transform.ToRect();
        }

        public static void Toggle(this Component component)
        {
            component.SetActive(!component.gameObject.activeSelf);
        }

        public static void SetActive(this Component component, bool active)
        {
            component.gameObject.SetActive(active);
        }

        public static void DestroyObject(this Component component)
        {
            Object.Destroy(component.gameObject);
        }

        public static void CheckSingleComponent<T>(this T component)
                where T : MonoBehaviour
        {
            T[]? components = component.GetComponents<T>();

            if (components.Length > 1) {
                throw new($"There are more than 1 {typeof(T).Name} on {component.name}");
            }
        }

        public static IEnumerable<Transform> EnumerateAllChildren(this Transform transform)
        {
            foreach (Transform child in transform) {
                yield return child;

                foreach (Transform? subchild in child.EnumerateAllChildren()) {
                    yield return subchild;
                }
            }
        }

        #endregion
    }
}